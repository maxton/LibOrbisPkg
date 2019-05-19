using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Threading.Tasks;
using LibOrbisPkg.GP4;
using LibOrbisPkg.PFS;
using LibOrbisPkg.PKG;
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
        "makepfs",
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
        "makeouterpfs",
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
        "makepkg",
        ArgDef.Required("input_project.gp4", "output_directory"),
        args =>
        {
          var proj = args[1];
          var project = Gp4Project.ReadFrom(File.OpenRead(proj));
          var props = PkgProperties.FromGp4(project, Path.GetDirectoryName(proj));
          var outputPath = args[2];
          new PkgBuilder(props).Write(Path.Combine(
            outputPath,
            $"{project.volume.Package.ContentId}.pkg"));
        }),
      Verb.Create(
        "extractpkg",
        ArgDef.Multi(ArgDef.Bool("verbose"), ArgDef.Option("passcode"), "input.pkg", "output_directory"),
        (flags, optionals, args) =>
        {
          var pkgPath = args[1];
          var outPath = args[2];
          var passcode = optionals["passcode"];
          Pkg pkg;

          var mmf = MemoryMappedFile.CreateFromFile(pkgPath);
          using (var s = mmf.CreateViewStream())
          {
            pkg = new PkgReader(s).ReadPkg();
          }
          var ekpfs = EkPfsFromPasscode(pkg, passcode);
          var outerPfsOffset = (long)pkg.Header.pfs_image_offset;
          using(var acc = mmf.CreateViewAccessor(outerPfsOffset, (long)pkg.Header.pfs_image_size))
          {
            var outerPfs = new PfsReader(acc, ekpfs);
            var inner = new PfsReader(new PFSCReader(outerPfs.GetFile("pfs_image.dat").GetView()));
            ExtractInParallel(inner, outPath, flags["verbose"]);
          }
        }),
      Verb.Create(
        "extractpfs",
        ArgDef.Multi(ArgDef.Bool("verbose"), "input.dat", "output_directory"),
        (flags, args) =>
        {
          var pfsPath = args[1];
          var outPath = args[2];
          using(var mmf = MemoryMappedFile.CreateFromFile(pfsPath))
          using(var acc = mmf.CreateViewAccessor())
          {
            var pfs = new PfsReader(acc);
            ExtractInParallel(pfs, outPath, flags["verbose"]);
          }
        }),
      Verb.Create(
        "extractinnerpfs",
        ArgDef.Multi(ArgDef.Option("passcode"), "input.pkg", "output_pfs.dat"),
        (switches, optionals, args) =>
        {
          var pkgPath = args[1];
          var outPath = args[2];
          var passcode = optionals["passcode"];

          Pkg pkg;
          var mmf = MemoryMappedFile.CreateFromFile(pkgPath);
          using (var s = mmf.CreateViewStream())
          {
            pkg = new PkgReader(s).ReadPkg();
          }
          var ekpfs = EkPfsFromPasscode(pkg, passcode);
          var outerPfsOffset = (long)pkg.Header.pfs_image_offset;
          using(var acc = mmf.CreateViewAccessor(outerPfsOffset, (long)pkg.Header.pfs_image_size))
          {
            var outerPfs = new PfsReader(acc, ekpfs);
            var inner = outerPfs.GetFile("pfs_image.dat");
            using(var v = inner.GetView())
            using(var f = File.OpenWrite(outPath))
            {
              var buf = new byte[1024 * 1024];
              long wrote = 0;
              while(wrote < inner.size)
              {
                int toWrite = (int)Math.Min(inner.size - wrote, buf.Length);
                if(toWrite <= 0) break;
                v.Read(wrote, buf, 0, toWrite);
                f.Write(buf, 0, toWrite);
                wrote += toWrite;
              }
            }
          }
        }),
      Verb.Create(
        "extractouterpfs",
        ArgDef.Multi(ArgDef.Bool("encrypted"), ArgDef.Option("passcode"), "input.pkg", "pfs_image.dat"),
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
              var ekpfs = EkPfsFromPasscode(pkg, passcode);
              var pfs_seed = new byte[16];
              outer_pfs.Position = 0x370;
              outer_pfs.Read(pfs_seed, 0, 16);
              var outerpfs_size = outer_pfs.Length;
              var (tweakKey, dataKey) = Crypto.PfsGenEncKey(ekpfs, pfs_seed);
              s.Close();
              using (var pkgMM = MemoryMappedFile.CreateFromFile(pkgPath, FileMode.Open))
              using (var o = MemoryMappedFile.CreateFromFile(outPath, capacity: outerpfs_size, mapName: "output_outerpfs", mode: FileMode.Create))
              using (var outputView = o.CreateViewAccessor())
              using (var outerPfs = new MemoryMappedViewAccessor_(
                  pkgMM.CreateViewAccessor((long)pkg.Header.pfs_image_offset, (long)pkg.Header.pfs_image_size),
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
        "listentries",
        ArgDef.Required("input.pkg"),
        args =>
        {
          var pkgPath = args[1];
          using(var file = File.OpenRead(pkgPath))
          {
            var pkg = new PkgReader(file).ReadPkg();
            Console.WriteLine("Offset\tSize\tFlags\t\t#\tName");
            var i = 0;
            foreach(var meta in pkg.Metas.Metas)
            {
              Console.WriteLine($"0x{meta.DataOffset:X2}\t0x{meta.DataSize:X}\t{meta.Flags1:X8}\t{i++}\t{meta.id}");
            }
          }
        }),
      Verb.Create(
        "extractentry",
        ArgDef.Required("input.pkg", "entry_id", "output.bin"),
        args =>
        {
          var pkgPath = args[1];
          var idx = int.Parse(args[2]);
          var outPath = args[3];
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
              var entry = new SubStream(pkgFile, meta.DataOffset, meta.DataSize);
              outFile.SetLength(entry.Length);
              entry.CopyTo(outFile);
            }
          }
          return;
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
          var pos = 0;
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
    public static Verb Create(string name, List<ArgDef> args, Action<string[]> action)
    {
      return new Verb { Name = name, Args = args, Body = (_, _2, a) => action(a) };
    }

    /// <summary>
    /// Creates a verb that uses boolean switches and positional arguments.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="args"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static Verb Create(string name, List<ArgDef> args, Action<Dictionary<string, bool>, string[]> action)
    {
      return new Verb { Name = name, Args = args, Body = (b, _, n) => action(b, n) };
    }


    /// <summary>
    /// Creates a verb that uses boolean switches, optional parameters, and positional arguments.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="args"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static Verb Create(string name, List<ArgDef> args, Action<Dictionary<string,bool>, Dictionary<string,string>, string[]> action)
    {
      return new Verb { Name = name, Args = args, Body = action };
    }
    public override string ToString()
    {
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
      foreach (var verb in verbs)
      {
        Console.WriteLine($"  {verb}");
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
