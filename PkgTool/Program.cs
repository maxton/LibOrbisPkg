using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibOrbisPkg.GP4;
using LibOrbisPkg.PFS;
using LibOrbisPkg.PKG;
using LibOrbisPkg.SFO;
using LibOrbisPkg.Util;

namespace PkgTool
{
  class Program
  {
    static void Main(string[] args)
    {
      Verb.Run(Verbs, args, AppDomain.CurrentDomain.FriendlyName);
    }

    public static Verb[] Verbs = new[]
    {
      Verb.Create(
        "pfs_buildinner",
        "Builds an inner PFS image from the given GP4 project.",
        ArgDef.Required("input_project.gp4", "output_pfs.dat"),
        args =>
        {
          var proj = args[1];
          var output = args[2];
          var outFile = File.Create(output);
          var props = PfsProperties.MakeInnerPFSProps(PkgProperties.FromGp4(
              Gp4Project.ReadFrom(File.OpenRead(proj)),
              Path.GetDirectoryName(proj)));
          new PfsBuilder(props, Console.WriteLine).WriteImage(outFile);
        }),
      Verb.Create(
        "pfs_buildouter",
        "Builds an outer PFS image, optionally encrypted, from the given GP4 project.",
        ArgDef.Multi(ArgDef.Bool("encrypt"), "input_project.gp4", "output_pfs.dat"),
        (switches, args) =>
        {
          var proj = args[1];
          var output = args[2];
          var outFile = File.Open(output, FileMode.Create);
          var project = Gp4Project.ReadFrom(File.OpenRead(proj));
          var projectDir = Path.GetDirectoryName(proj);
          var pkgProps = PkgProperties.FromGp4(project, projectDir);
          var EKPFS = Crypto.ComputeKeys(project.volume.Package.ContentId, project.volume.Package.Passcode, 1);
          Console.WriteLine("Preparing inner PFS...");
          var innerPfs = new PfsBuilder(PfsProperties.MakeInnerPFSProps(pkgProps), x => Console.WriteLine($"[innerpfs] {x}"));
          Console.WriteLine("Preparing outer PFS...");
          var outerPfs = new PfsBuilder(PfsProperties.MakeOuterPFSProps(pkgProps, innerPfs, EKPFS, switches["encrypt"]), x => Console.WriteLine($"[outerpfs] {x}"));
          outerPfs.WriteImage(outFile);
        }),
      Verb.Create(
        "pkg_build",
        "Builds a fake PKG from the given GP4 project in the given output directory.",
        ArgDef.Required("input_project.gp4", "output_directory"),
        args =>
        {
          var proj = args[1];
          var project = Gp4Project.ReadFrom(File.OpenRead(proj));
          var validation = Gp4Validator.ValidateProject(project, Path.GetDirectoryName(proj));
          bool ok = true;
          if (validation.Count > 0)
          {
            Console.WriteLine("Found {0} issue(s) with GP4 project:", validation.Count);
          }
          foreach (ValidateResult v in validation)
          {
            if (v.Type == ValidateResult.ResultType.Fatal)
            {
              ok = false;
            }
            Console.WriteLine("{0}: {1}",
              v.Type == ValidateResult.ResultType.Fatal ? "FATAL ERROR" : "WARNING    ",
              v.Message);
          }
          if (!ok)
          {
            Console.WriteLine("Cannot build PKG due to fatal errors.");
            Environment.Exit(1);
            return;
          }
          var props = PkgProperties.FromGp4(project, Path.GetDirectoryName(proj));
          var outputPath = args[2];
          new PkgBuilder(props).Write(Path.Combine(
            outputPath,
            $"{project.volume.Package.ContentId}.pkg"));
        }),
      Verb.Create(
        "pkg_makegp4",
        "Extracts all content from the PKG and creates a GP4 project in the output directory",
        ArgDef.Multi(ArgDef.Option("passcode"), "input.pkg", "output_dir"),
        (_, optionals, args) => 
        {
          var passcode = optionals.ContainsKey("passcode") ? optionals["passcode"] : null;
          Gp4Creator.CreateProjectFromPKG(args[2], args[1], passcode);
        }),
      Verb.Create(
        "pkg_extract",
        "Extracts all the files from a PKG to the given output directory. Use the verbose flag to print filenames as they are extracted.",
        ArgDef.Multi(ArgDef.Bool("verbose"), ArgDef.Option("passcode", "xts_tweak", "xts_data"), "input.pkg", "output_directory"),
        (flags, optionals, args) =>
        {
          var pkgPath = args[1];
          var outPath = args[2];
          var passcode = optionals["passcode"];
          Pkg pkg;

          var mmf = MemoryMappedFile.CreateFromFile(pkgPath);
          using (var s = mmf.CreateViewStream(0, 0, MemoryMappedFileAccess.Read))
          {
            pkg = new PkgReader(s).ReadPkg();
          }
          var ekpfs = EkPfsFromPasscode(pkg, passcode);
          var outerPfsOffset = (long)pkg.Header.pfs_image_offset;
          using(var acc = mmf.CreateViewAccessor(outerPfsOffset, (long)pkg.Header.pfs_image_size, MemoryMappedFileAccess.Read))
          {
            var outerPfs = new PfsReader(acc, pkg.Header.pfs_flags, ekpfs, optionals["xts_tweak"]?.FromHexCompact(), optionals["xts_data"]?.FromHexCompact());
            var inner = new PfsReader(new PFSCReader(outerPfs.GetFile("pfs_image.dat").GetView()));
            ExtractInParallel(inner, outPath, flags["verbose"]);
          }
        }),
      Verb.Create(
        "pfs_extract",
        "Extracts all the files from a PFS image to the given output directory. Use the verbose flag to print filenames as they are extracted.",
        ArgDef.Multi(ArgDef.Bool("verbose"), "input.dat", "output_directory"),
        (flags, args) =>
        {
          var pfsPath = args[1];
          var outPath = args[2];
          using(var mmf = MemoryMappedFile.CreateFromFile(pfsPath))
          using(var acc = mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read))
          {
            var pfs = new PfsReader(acc);
            ExtractInParallel(pfs, outPath, flags["verbose"]);
          }
        }),
      Verb.Create(
        "pkg_extractinnerpfs",
        "Extracts the inner PFS image from a PKG file.",
        ArgDef.Multi(ArgDef.Bool("compressed"), ArgDef.Option("passcode", "xts_tweak", "xts_data"), "input.pkg", "output_pfs.dat"),
        (switches, optionals, args) =>
        {
          var pkgPath = args[1];
          var outPath = args[2];
          var passcode = optionals["passcode"];

          Pkg pkg;
          var mmf = MemoryMappedFile.CreateFromFile(pkgPath);
          using (var s = mmf.CreateViewStream(0, 0, MemoryMappedFileAccess.Read))
          {
            pkg = new PkgReader(s).ReadPkg();
          }
          var ekpfs = EkPfsFromPasscode(pkg, passcode);
          var outerPfsOffset = (long)pkg.Header.pfs_image_offset;
          using(var acc = mmf.CreateViewAccessor(outerPfsOffset, (long)pkg.Header.pfs_image_size, MemoryMappedFileAccess.Read))
          {
            var outerPfs = new PfsReader(acc, pkg.Header.pfs_flags, ekpfs, optionals["xts_tweak"]?.FromHexCompact(), optionals["xts_data"]?.FromHexCompact());
            var inner = outerPfs.GetFile("pfs_image.dat");
            using(var v = inner.GetView())
            using(var d = switches["compressed"] ? v : new PFSCReader(v))
            using(var f = File.OpenWrite(outPath))
            {
              var buf = new byte[8 * 1024 * 1024];
              long wrote = 0;
              var size = switches["compressed"] ? inner.size : inner.compressed_size;
              while (size - wrote > buf.Length)
              {
                const int parallelSlice = 0x100000;
                Parallel.For(0, buf.Length / parallelSlice - 1, idx => {
                  int offset = idx * parallelSlice;
                  d.Read(wrote + offset, buf, offset, parallelSlice);
                });
                f.Write(buf, 0, buf.Length);
                wrote += buf.Length;
              }
              while(wrote < size)
              {
                int toWrite = (int)Math.Min(size - wrote, buf.Length);
                if(toWrite <= 0) break;
                d.Read(wrote, buf, 0, toWrite);
                f.Write(buf, 0, toWrite);
                wrote += toWrite;
              }
            }
          }
        }),
      Verb.Create(
        "pkg_extractouterpfs",
        "Extracts and decrypts the outer PFS image from a PKG file. Use the --encrypted flag to leave the image encrypted.",
        ArgDef.Multi(ArgDef.Bool("encrypted"), ArgDef.Option("passcode", "xts_tweak", "xts_data"), "input.pkg", "pfs_image.dat"),
        (switches, optionals, args) =>
        {
          var pkgPath = args[1];
          var outPath = args[2];
          var passcode = optionals["passcode"];
          Pkg pkg;
          using (var s = File.OpenRead(pkgPath))
          {
            pkg = new PkgReader(s).ReadPkg();
            var outer_pfs = new OffsetStream(s, (long)pkg.Header.pfs_image_offset);
            if(switches["encrypted"])
            {
              using (var o = File.Create(outPath))
              {
                outer_pfs.Position = 0;
                outer_pfs.CopyTo(o);
              }
            }
            else
            {
              var pfs_seed = new byte[16];
              outer_pfs.Position = 0x370;
              outer_pfs.Read(pfs_seed, 0, 16);
              var outerpfs_size = outer_pfs.Length;
              byte[] tweakKey, dataKey;
              if (passcode != null)
              {
                var ekpfs = EkPfsFromPasscode(pkg, passcode);
                (tweakKey, dataKey) = Crypto.PfsGenEncKey(ekpfs, pfs_seed);
              } 
              else
              {
                tweakKey = optionals["xts_tweak"]?.FromHexCompact();
                dataKey = optionals["xts_data"]?.FromHexCompact();
              }
              s.Close();
              using (var pkgMM = MemoryMappedFile.CreateFromFile(pkgPath, FileMode.Open))
              using (var o = MemoryMappedFile.CreateFromFile(outPath, capacity: outerpfs_size, mapName: null, mode: FileMode.Create))
              using (var outputView = o.CreateViewAccessor(0, 0, MemoryMappedFileAccess.ReadWrite))
              using (var outerPfs = new MemoryMappedViewAccessor_(
                  pkgMM.CreateViewAccessor((long)pkg.Header.pfs_image_offset, (long)pkg.Header.pfs_image_size, MemoryMappedFileAccess.Read),
                  true))
              {
                var transform = new XtsDecryptReader(outerPfs, dataKey, tweakKey);
                long length = (long)pkg.Header.pfs_image_size;
                const int copySize = 1024 * 1024;
                var buf = new byte[copySize];
                for(long i = 0; i < length; i += copySize)
                {
                  int count = (int)Math.Min(copySize, length - i);
                  transform.Read(i, buf, 0, count);
                  outputView.WriteArray(i, buf, 0, count);
                }
                // Unset "encrypted" flag
                outputView.Write(0x1C, outputView.ReadByte(0x1C) & ~4);
              }
            }
          }
        }),
      Verb.Create(
        "pkg_listentries",
        "Lists the entries in a PKG file.",
        ArgDef.Required("input.pkg"),
        args =>
        {
          var pkgPath = args[1];
          using(var file = File.OpenRead(pkgPath))
          {
            var pkg = new PkgReader(file).ReadPkg();
            Console.WriteLine("Offset      Size      Flags      Index Enc? Name");
            var i = 0;
            foreach(var meta in pkg.Metas.Metas)
            {
              Console.WriteLine($"0x{meta.DataOffset,-10:X2}0x{meta.DataSize,-8:X}{meta.Flags1,-11:X8}{i++,-6}{(meta.KeyIndex == 0 ? "" : meta.KeyIndex.ToString()),-5}{meta.id}");
            }
          }
        }),
      Verb.Create(
        "pkg_extractentry",
        "Extracts the selected entry from the given PKG file.",
        ArgDef.Multi(ArgDef.Option("passcode"), "input.pkg", "entry_id", "output.bin"),
        (flags, optionals, args) =>
        {
          var pkgPath = args[1];
          var idx = int.Parse(args[2]);
          var outPath = args[3];
          var passcode = optionals["passcode"];

          using (var pkgFile = File.OpenRead(pkgPath))
          {
            var pkg = new PkgReader(pkgFile).ReadPkg();
            if (idx < 0 || idx >= pkg.Metas.Metas.Count)
            {
              Console.WriteLine("Error: entry number out of range");
              return;
            }
            using (var outFile = File.Create(outPath))
            {
              var meta = pkg.Metas.Metas[idx];
              outFile.SetLength(meta.DataSize);
              var totalEntrySize = meta.Encrypted ? (meta.DataSize + 15) & ~15 : meta.DataSize;
              if(meta.Encrypted)
              {
                if(passcode == null && meta.KeyIndex != 3)
                {
                  Console.WriteLine("Warning: Entry is encrypted but no passcode was provided! Saving encrypted bytes.");
                }
                else
                {
                  var entry = new SubStream(pkgFile, meta.DataOffset, totalEntrySize);
                  var tmp = new byte[totalEntrySize];
                  entry.Read(tmp, 0, tmp.Length);
                  tmp = meta.KeyIndex == 3 ? Entry.Decrypt(tmp, pkg, meta) : Entry.Decrypt(tmp, pkg.Header.content_id, passcode, meta);
                  outFile.Write(tmp, 0, (int)meta.DataSize);
                  return;
                }
              }
              new SubStream(pkgFile, meta.DataOffset, totalEntrySize).CopyTo(outFile);
            }
          }
          return;
        }),
      Verb.Create(
        "pkg_validate",
        "Checks the hashes and signatures of a PKG.",
        ArgDef.Multi(ArgDef.Bool("verbose"), "input.pkg"),
        (option, args) =>
        {
          using(var fs = File.OpenRead(args[1]))
          {
            var pkg = new PkgReader(fs).ReadPkg();
            var validator = new PkgValidator(pkg);
            foreach(var validation in validator.Validations(fs))
            {
              switch(validation.Validate())
              {
                case PkgValidator.ValidationResult.Fail:
                  Console.WriteLine("[ERROR] {2} invalid at 0x{0:X}: {1}", validation.Location, validation.Name, validation.Type);
                  break;
                case PkgValidator.ValidationResult.NoKey:
                  Console.WriteLine("[WARN]  {2} cannot be validated without secret keys at 0x{0:X}: {1}", validation.Location, validation.Name, validation.Type);
                  break;
                case PkgValidator.ValidationResult.Ok:
                  Console.WriteLine("[OK]    ({2}) {0} @ 0x{1:X}", validation.Name, validation.Location, validation.Type);
                  break;
              }
              if(option["verbose"])
              {
                Console.WriteLine("          " + validation.Description);
              }
            }

          }
        }),
      Verb.Create(
        "sfo_listentries",
        "Lists the entries in an SFO file.",
        ArgDef.Required("param.sfo"),
        args =>
        {
          var sfoFilename = args[1];
          using(var f = File.OpenRead(sfoFilename))
          {
            var sfo = ParamSfo.FromStream(f);
            Console.WriteLine($"Entry Name : Entry Type(Size / Max Size) = Entry Value");
            foreach(var x in sfo.Values)
            {
              Console.WriteLine($"{x.Name} : {x.Type}({x.Length}/{x.MaxLength}) = {x.ToString()}");
            }
          }
        }),
      Verb.Create(
        "sfo_deleteentry",
        "Deletes the named entry from the SFO file.",
        ArgDef.Required("param.sfo", "entry_name"),
        args =>
        {
          var sfoFilename = args[1];
          var entryName = args[2];
          using(var f = File.Open(sfoFilename, FileMode.Open))
          {
            var sfo = ParamSfo.FromStream(f);
            if(sfo.Values.Exists(x => x.Name == entryName))
            {
              f.SetLength(0);
              sfo.Values.RemoveAt(sfo.Values.FindIndex(x => x.Name == entryName));
              f.Position = 0;
              sfo.Write(f);
            }
            else
            {
              Console.WriteLine($"Error: No entry with the name {entryName} exists in {sfoFilename}");
            }
          }
        }),
      Verb.Create(
        "sfo_setentry",
        "Creates or modifies the named entry in the given SFO file.",
        ArgDef.Multi(ArgDef.Option("value", "type", "maxsize", "name"), "param.sfo", "entry_name"),
        (flags, optionals, args) =>
        {
          var newValue = optionals["value"];
          var newType = optionals["type"];
          var newMaxSize = int.Parse(optionals["maxsize"] ?? "-1");
          var newName = optionals["name"] ?? args[2];
          var sfoFilename = args[1];
          var entryName = args[2];
          using(var f = File.Open(sfoFilename, FileMode.Open))
          {
            var sfo = ParamSfo.FromStream(f);
            var value = sfo[entryName];
            if(value != null)
            {
              sfo.Values.Remove(value);
              SfoEntryType type = value.Type;
              newValue = newValue ?? value.ToString();
              switch (newType?.ToLowerInvariant())
              {
                case null:
                  if(newMaxSize == -1)
                  {
                    newMaxSize = value.MaxLength;
                  }
                  break;
                case "integer":
                case "int":
                case "int32":
                  type = SfoEntryType.Integer;
                  newMaxSize = 4;
                  break;
                case "string":
                case "utf8":
                  type = SfoEntryType.Utf8;
                  if(newMaxSize == -1)
                  {
                    newMaxSize = value.MaxLength;
                  }
                  if(newMaxSize <= Encoding.UTF8.GetByteCount(newValue))
                  {
                    Console.WriteLine($"Error: value \"{newValue}\" does not fit in the maximum size given ({newMaxSize})");
                    return;
                  }
                  break;
                default:
                  Console.WriteLine("Error: unknown entry type. Supported types are: integer, utf8");
                  return;
              }
              sfo[newName] = Value.Create(newName, type, newValue, newMaxSize);
            }
            else
            {
              if(newValue == null)
              {
                Console.WriteLine("Error: no value specified for new SFO entry");
                return;
              }
              switch (newType?.ToLowerInvariant())
              {
                case "integer":
                case "int":
                case "int32":
                  sfo[newName] = Value.Create(newName, SfoEntryType.Integer, newValue);
                  break;
                case "string":
                case "utf8":
                  if(newMaxSize == -1)
                  {
                    Console.WriteLine("Error: no maximum size specified for string");
                    return;
                  }
                  if(newMaxSize <= Encoding.UTF8.GetByteCount(newValue))
                  {
                    Console.WriteLine("Error: value does not fit in the maximum size given");
                    return;
                  }
                  sfo[newName] = Value.Create(newName, SfoEntryType.Utf8, newValue, newMaxSize);
                  break;
                case null:
                default:
                  Console.WriteLine("Error: invalid type specified for new SFO entry. Available types: integer, utf8");
                  return;
              }
            }
            f.SetLength(0);
            f.Position = 0;
            sfo.Write(f);
            var x = sfo[newName];
            Console.WriteLine($"{x.Name} : {x.Type}({x.Length}/{x.MaxLength}) = {x.ToString()}");
          }
        }),
      Verb.Create(
        "sfo_new",
        "Creates a new empty SFO file at the given path.",
        ArgDef.Required("param.sfo"),
        args =>
        {
          using(var f = File.Open(args[1], FileMode.Create))
          {
            new ParamSfo().Write(f);
          }
        }),
      Verb.Create(
        "version",
        "Print the version and exit.",
        new List<ArgDef>(),
        _ => {
          var assembly = System.Reflection.Assembly.GetExecutingAssembly();
          var version = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
          var libAssembly = System.Reflection.Assembly.GetAssembly(typeof(Pkg));
          var libVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(libAssembly.Location).FileVersion;
          Console.WriteLine("PkgTool (c) 2020 Maxton");
          Console.WriteLine("LibOrbisPkg version " + libVersion);
          Console.WriteLine("PkgTool version " + version);
        }),
    };

    private static void ExtractInParallel(PfsReader inner, string outPath, bool verbose)
    {
      Console.WriteLine("Extracting in parallel...");
      Parallel.ForEach(
        inner.GetAllFiles(),
        () => new byte[0x10000],
        (f, _, buf) =>
        {
          var size = f.size;
          long pos = 0;
          var view = f.GetView();
          var fullName = f.FullName;
          var path = Path.Combine(outPath, fullName.Replace('/', Path.DirectorySeparatorChar).Substring(1));
          var dir = path.Substring(0, path.LastIndexOf(Path.DirectorySeparatorChar));
          if (verbose)
          {
            Console.WriteLine($"{fullName} -> {path}");
          }
          Directory.CreateDirectory(dir);
          using (var file = File.OpenWrite(path))
          {
            file.SetLength(size);
            while (size > 0)
            {
              var toRead = (int)Math.Min(size, buf.Length);
              view.Read(pos, buf, 0, toRead);
              file.Write(buf, 0, toRead);
              pos += toRead;
              size -= toRead;
            }
          }
          return buf;
        },
        x => { });
    }
    
    private static string SafeName(string name)
    {
      name = name.Replace("\\", "").Replace("/", "").Replace(":", "").Replace("*", "")
        .Replace("?", "").Replace("\"", "").Replace("<", "").Replace(">", "").Replace("|", "");
      if (name == ".." || name == ".")
      {
        name = "(" + name + ")";
      }
      return name;
    }

    private static byte[] EkPfsFromPasscode(Pkg pkg, string passcode)
    {
      if (passcode == null || passcode == "" || passcode == "fake")
        return pkg.GetEkpfs();
      else
        return Crypto.ComputeKeys(pkg.Header.content_id, passcode, 1);
    }
  }

  public class Verb
  {
    public string Name;
    public string HelpText;
    public List<ArgDef> Args;

    /// <summary>
    /// The body of the verb. The first param is a map of switch name -> switch present,
    /// the second is a map of optional value name -> value,
    /// the third is a list of positional arguments.
    /// </summary>
    public Action<Dictionary<string, bool>, Dictionary<string,string>, string[]> Body;

    /// <summary>
    /// Creates a verb that uses only positional arguments.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="args"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static Verb Create(string name, string helpText, List<ArgDef> args, Action<string[]> action)
    {
      return new Verb { Name = name, HelpText = helpText, Args = args, Body = (_, _2, a) => action(a) };
    }

    /// <summary>
    /// Creates a verb that uses boolean switches and positional arguments.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="args"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static Verb Create(string name, string helpText, List<ArgDef> args, Action<Dictionary<string, bool>, string[]> action)
    {
      return new Verb { Name = name, HelpText = helpText, Args = args, Body = (b, _, n) => action(b, n) };
    }


    /// <summary>
    /// Creates a verb that uses boolean switches, optional parameters, and positional arguments.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="args"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static Verb Create(string name, string helpText, List<ArgDef> args, Action<Dictionary<string,bool>, Dictionary<string,string>, string[]> action)
    {
      return new Verb { Name = name, HelpText = helpText, Args = args, Body = action };
    }
    public override string ToString()
    {
      if (Args == null || Args.Count == 0)
        return Name;
      var options = Args
        .Select(x => 
          x.Type == ArgType.Boolean ? $"[--{x.Name}]" :
          x.Type == ArgType.Optional ? $"[--{x.Name} <...>]" :
          /* ArgType.Positional */ $"<{x.Name}>")
        .Aggregate((x, y) => $"{x} {y}");
      return Name + " " + options;
    }

    public static bool Run(Verb[] verbs, string[] args, string name)
    {
      if (args.Length > 0 && verbs.Where(x => x.Name == args[0]).FirstOrDefault() is Verb v)
      {
        // Parse the command line into separate containers of switches, optional arguments, and positional arguments
        var switches = new Dictionary<string, bool>();
        var optionals = new Dictionary<string, string>();
        var positionalArgs = new List<string>() { args[0] };

        // Keep track of the arguments that still need to be matched in the command line
        var remainingArgs = v.Args.ToList();

        for (int i = 1; i < args.Length; i++)
        {
          if (args[i].StartsWith("--"))
          {
            if (remainingArgs.FirstOrDefault(x => x.Type == ArgType.Boolean && x.Name == args[i].Substring(2)) is ArgDef boolArg)
            {
              remainingArgs.Remove(boolArg);
              switches[boolArg.Name] = true;
            }
            else if (remainingArgs.FirstOrDefault(x => x.Type == ArgType.Optional && x.Name == args[i].Substring(2)) is ArgDef optArg)
            {
              remainingArgs.Remove(optArg);
              ++i;
              if (i >= args.Length)
              {
                Console.WriteLine($"Command line error: No value provided for optional param {args[i - 1]}");
                Console.WriteLine($"Usage: {name} {v}");
                return true;
              }
              optionals[optArg.Name] = args[i];
            }
            else
            {
              Console.WriteLine($"Command line error: Unknown optional parameter \"{args[i]}\"");
              Console.WriteLine($"Usage: {name} {v}");
              return true;
            }
          }
          else // arg doesn't start with --
          {
            if (remainingArgs.FirstOrDefault(x => x.Type == ArgType.Positional) is ArgDef posArg)
            {
              positionalArgs.Add(args[i]);
              remainingArgs.Remove(posArg);
            }
            else
            {
              Console.WriteLine($"Command line error: Too many arguments");
              Console.WriteLine($"Usage: {name} {v}");
              return true;
            }
          }
        }

        // Fill out the unset optional args with the default values, and catch missing required arguments.
        foreach(var arg in remainingArgs)
        {
          switch (arg.Type)
          {
            case ArgType.Boolean:
              switches[arg.Name] = false;
              break;
            case ArgType.Optional:
              optionals[arg.Name] = null;
              break;
            case ArgType.Positional:
              Console.WriteLine("Command line error: not enough arguments");
              Console.WriteLine($"Usage: {name} {v}");
              return true;
          }
        }

        // At this point it is safe to call the body.
        v.Body(switches, optionals, positionalArgs.ToArray());
        return true;
      }

      // In this case, the verb wasn't found, so show the full usage and list of verbs.
      Console.WriteLine($"Usage: {name} <verb> [options ...]");
      Console.WriteLine("");
      Console.WriteLine("Verbs:");
      var verb_list = (args.Length > 0
          && verbs.Where(verb => verb.Name.StartsWith(args[0])).ToArray() is Verb[] prefixList 
          && prefixList.Length > 0) ? prefixList : verbs;
      foreach (var verb in verb_list.OrderBy(z => z.Name))
      {
        Console.WriteLine($"  {verb}");
        Console.WriteLine($"    {verb.HelpText}");
        Console.WriteLine();
      }
      return false;
    }
  }

  public enum ArgType
  {
    Boolean, Optional, Positional
  }
  public class ArgDef
  {
    public string Name;
    public ArgType Type = ArgType.Positional;
    public static List<ArgDef> Multi(List<ArgDef> optionalArgs, params string[] names)
    {
      return optionalArgs.Concat(Required(names)).ToList();
    }
    public static List<ArgDef> Multi(List<ArgDef> optionalArgs, List<ArgDef> moreOptionalArgs, params string[] names)
    {
      return optionalArgs.Concat(moreOptionalArgs).Concat(Required(names)).ToList();
    }
    public static List<ArgDef> Required(params string[] names)
    {
      return names.Select(x => new ArgDef { Name = x }).ToList();
    }
    public static List<ArgDef> Bool(params string[] names)
    {
      return names.Select(x => new ArgDef { Name = x, Type = ArgType.Boolean }).ToList();
    }
    public static List<ArgDef> Option(params string[] names)
    {
      return names.Select(x => new ArgDef { Name = x, Type = ArgType.Optional }).ToList();
    }
  }
}
