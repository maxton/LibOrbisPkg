using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GameArchives;
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
      if(!Verb.Run(Verbs, args, "PkgTool.exe"))
      {
        Console.WriteLine("PkgTool.exe <verb> [options ...]");
        Console.WriteLine("");
        Console.WriteLine("Verbs:");
        foreach (var verb in Verbs)
        {
          Console.WriteLine($"  {verb}");
        }
        Console.WriteLine();
        Console.WriteLine("Use passcode \"fake\" to decrypt a FAKE PKG without knowing the actual passcode.");
      }
    }

    public static Verb[] Verbs = new[]
    {
      Verb.Create(
        "makepfs", 
        ArgDef.List("input_project.gp4", "output_pfs.dat"),
        args =>
        {
          var proj = args[1];
          var output = args[2];
          var outFile = File.OpenWrite(output);
          var props = PfsProperties.MakeInnerPFSProps(
              Gp4Project.ReadFrom(File.OpenRead(proj)),
              Path.GetDirectoryName(proj));
          new PfsBuilder(props, Console.WriteLine).WriteImage(outFile);
        }),
      Verb.Create(
        "makeouterpfs",
        ArgDef.List(ArgDef.Switches("encrypt"), "input_project.gp4", "output_pfs.dat"),
        (switches, args) =>
        {
          var proj = args[1];
          var output = args[2];
          var outFile = File.Open(output, FileMode.Create);
          var project = Gp4Project.ReadFrom(File.OpenRead(proj));
          var projectDir = Path.GetDirectoryName(proj);
          var EKPFS = Crypto.ComputeKeys(project.volume.Package.ContentId, project.volume.Package.Passcode, 1);
          Console.WriteLine("Preparing inner PFS...");
          var innerPfs = new PfsBuilder(PfsProperties.MakeInnerPFSProps(project, projectDir), x => Console.WriteLine($"[innerpfs] {x}"));
          Console.WriteLine("Preparing outer PFS...");
          var outerPfs = new PfsBuilder(PfsProperties.MakeOuterPFSProps(project, innerPfs, EKPFS, switches["encrypt"]), x => Console.WriteLine($"[outerpfs] {x}"));
          outerPfs.WriteImage(outFile);
        }),
      Verb.Create(
        "makepkg",
        ArgDef.List("input_project.gp4", "output_directory"),
        args =>
        {
          var proj = args[1];
          var project = Gp4Project.ReadFrom(File.OpenRead(proj));
          var outputPath = args[2];

          using (var outFile = File.Open(
            Path.Combine(
              outputPath,
              $"{project.volume.Package.ContentId}.pkg"),
              FileMode.Create))
          {
            new PkgBuilder(project, Path.GetDirectoryName(proj)).Write(outFile);
          }
        }),
      Verb.Create(
        "extractpkg",
        ArgDef.List("input.pkg", "passcode", "output_directory"),
        args =>
        {
          var pkgPath = args[1];
          var passcode = args[2];
          var outPath = args[3];
          var pkgFile = Util.LocalFile(pkgPath);
          Pkg pkg;
          using (var s = pkgFile.GetStream())
          {
            pkg = new PkgReader(s).ReadPkg();
          }
          var keyString = new string(EkPfsFromPasscode(pkg, passcode).Select(b => (char)b).ToArray());
          var package = PackageReader.ReadPackageFromFile(pkgFile, keyString);
          var innerPfs = PackageReader.ReadPackageFromFile(package.GetFile("/pfs_image.dat"));
          void ExtractDir(IDirectory dir, string path)
          {
            foreach (IFile f in dir.Files)
            {
              Console.WriteLine(f.Name);
              f.ExtractTo(Path.Combine(path, SafeName(f.Name)));
            }
            foreach (IDirectory d in dir.Dirs)
            {
              string newPath = Path.Combine(path, SafeName(d.Name));
              Directory.CreateDirectory(newPath);
              ExtractDir(d, newPath);
            }
          }
          ExtractDir(innerPfs.RootDirectory, outPath);
        }),
      Verb.Create(
        "extractinnerpfs",
        ArgDef.List("input.pkg", "passcode", "output_pfs.dat"),
        args =>
        {
          var pkgPath = args[1];
          var passcode = args[2];
          var outPath = args[3];
          var pkgFile = Util.LocalFile(pkgPath);
          Pkg pkg;
          using (var s = pkgFile.GetStream())
          {
            pkg = new PkgReader(s).ReadPkg();
          }
          var keyString = new string(EkPfsFromPasscode(pkg, passcode).Select(b => (char)b).ToArray());
          var package = PackageReader.ReadPackageFromFile(pkgFile, keyString);
          var innerPfs = package.GetFile("/pfs_image.dat");
          using (var ipfs = innerPfs.GetStream())
          using (var o = File.OpenWrite(outPath))
          using (var ipfs_d = new GameArchives.PFS.PFSCDecompressStream(ipfs))
          {
            ipfs_d.CopyTo(o);
          }
        }),
      Verb.Create(
        "extractouterpfs_e",
        ArgDef.List("input.pkg", "output_pfs_encrypted.dat"),
        args =>
        {
          var pkgPath = args[1];
          var outPath = args[2];
          var pkgFile = Util.LocalFile(pkgPath);
          Pkg pkg;
          using (var s = pkgFile.GetStream())
          {
            pkg = new PkgReader(s).ReadPkg();
            var outer_pfs = new OffsetStream(s, (long)pkg.Header.pfs_image_offset);
            using (var o = File.OpenWrite(outPath))
            {
              outer_pfs.Position = 0;
              outer_pfs.CopyTo(o);
            }
          }
        }),
      Verb.Create(
        "extractouterpfs",
        ArgDef.List("input.pkg", "passcode", "pfs_image.dat"),
        args =>
        {
          var pkgPath = args[1];
          var passcode = args[2];
          var outPath = args[3];
          var pkgFile = Util.LocalFile(pkgPath);
          Pkg pkg;
          using (var s = pkgFile.GetStream())
          {
            pkg = new PkgReader(s).ReadPkg();
            var outer_pfs = new OffsetStream(s, (long)pkg.Header.pfs_image_offset);
            var ekpfs = EkPfsFromPasscode(pkg, passcode);
            var pfs_seed = new byte[16];
            outer_pfs.Position = 0x370;
            outer_pfs.Read(pfs_seed, 0, 16);
            var enc_key = Crypto.PfsGenEncKey(ekpfs, pfs_seed);
            var data_key = new byte[16];
            var tweak_key = new byte[16];
            Buffer.BlockCopy(enc_key, 0, tweak_key, 0, 16);
            Buffer.BlockCopy(enc_key, 16, data_key, 0, 16);
            var decrypt_stream = new GameArchives.PFS.XtsCryptStream(outer_pfs, data_key, tweak_key, 16, 0x1000);
            using (var o = File.OpenWrite(outPath))
            {
              decrypt_stream.CopyTo(o);
              // Unset "encrypted" flag
              o.Position = decrypt_stream.Position = 0x1C;
              var b = (byte)decrypt_stream.ReadByte();
              b = (byte)(b & ~4);
              o.WriteByte(b);
            }
          }
        }),
      Verb.Create(
        "listentries",
        ArgDef.List("input.pkg"),
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
        ArgDef.List("input.pkg", "entry_id", "output.bin"),
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
            using (var outFile = File.OpenWrite(outPath))
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
      if (passcode == "fake")
      {
        var dk3 = Crypto.RSA2048Decrypt(pkg.EntryKeys.Keys[3].key, RSAKeyset.PkgDerivedKey3Keyset);
        var iv_key = Crypto.Sha256(
          pkg.ImageKey.meta.GetBytes()
          .Concat(dk3)
          .ToArray());
        var imageKeyDecrypted = pkg.ImageKey.FileData.Clone() as byte[];
        Crypto.AesCbcCfb128Decrypt(
          imageKeyDecrypted,
          imageKeyDecrypted,
          imageKeyDecrypted.Length,
          iv_key.Skip(16).Take(16).ToArray(),
          iv_key.Take(16).ToArray());
        return Crypto.RSA2048Decrypt(imageKeyDecrypted, RSAKeyset.FakeKeyset);
      }
      else
      {
        return Crypto.ComputeKeys(pkg.Header.content_id, passcode, 1);
      }
    }

  }

  public class Verb
  {
    public string Name;
    public ArgDef[] Args;
    public Action<Dictionary<string, bool>, string[]> Action;
    public static Verb Create(string name, ArgDef[] args, Action<string[]> action)
    {
      return new Verb { Name = name, Args = args, Action = (ignore, a) => action(a) };
    }
    public static Verb Create(string name, ArgDef[] args, Action<Dictionary<string, bool>, string[]> action)
    {
      return new Verb { Name = name, Args = args, Action = action };
    }
    public override string ToString()
    {
      var options = Args.Select(x => x.Switch ? $"[--{x.Name}]" : $"<{x.Name}>").Aggregate((x, y) => $"{x} {y}");
      return Name + " " + options;
    }

    public static bool Run(Verb[] verbs, string[] args, string name)
    {
      if (args.Length > 0 && verbs.Where(x => x.Name == args[0]).FirstOrDefault() is Verb v)
      {
        var switches = new Dictionary<string, bool>();
        int numSwitches = 0;
        foreach(var x in v.Args.Where(x => x.Switch))
        {
          var s = args.Contains("--"+x.Name);
          if (s) numSwitches++;
          switches[x.Name] = s;
        }
        args = args.Skip(numSwitches).ToArray();
        if (args.Length == (v.Args.Where(a => !a.Switch).Count() + 1))
        {
          v.Action(switches, args);
        }
        else
        {
          Console.WriteLine($"Usage: {name} {v}");
        }
        return true;
      }
      return false;
    }
  }

  public class ArgDef
  {
    public string Name;
    public bool Switch = false;
    public static ArgDef[] List(ArgDef[] optionalArgs, params string[] names)
    {
      return optionalArgs.Concat(List(names)).ToArray();
    }
    public static ArgDef[] List(params string[] names)
    {
      var ret = new ArgDef[names.Length];
      for(var i = 0; i < ret.Length; i++)
      {
        ret[i] = new ArgDef { Name = names[i] };
      }
      return ret;
    }
    public static ArgDef[] Switches(params string[] names)
    {
      var ret = new ArgDef[names.Length];
      for (var i = 0; i < ret.Length; i++)
      {
        ret[i] = new ArgDef { Name = names[i], Switch = true };
      }
      return ret;
    }
  }
}
