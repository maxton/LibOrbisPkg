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
      switch (args.Length > 0 ? args[0] : null)
      {
        case "makepfs":
          {
            var proj = args[1];
            var output = args[2];
            var outFile = File.OpenWrite(output);
            var props = PfsProperties.MakeInnerPFSProps(
                Gp4Project.ReadFrom(File.OpenRead(proj)),
                Path.GetDirectoryName(proj));
            new PfsBuilder(props, Console.WriteLine).WriteImage(outFile);
            break;
          }
        case "makepkg":
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
            break;
          }
        case "extractpkg":
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
            break;
          }
        case "extractinnerpfs":
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
            break;
          }
        case "extractouterpfs_e":
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
            break;
          }
        case "extractouterpfs":
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
            break;
          }
        case "listentries":
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
            break;
          }
        case "extractentry":
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
                break;
              }
              using (var outFile = File.OpenWrite(outPath))
              {
                var meta = pkg.Metas.Metas[idx];
                var entry = new SubStream(pkgFile, meta.DataOffset, meta.DataSize);
                outFile.SetLength(entry.Length);
                entry.CopyTo(outFile);
              }
            }
            break;
          }
        default:
          Console.WriteLine("PkgTool.exe <verb> <input> <output>");
          Console.WriteLine("");
          Console.WriteLine("Verbs:");
          Console.WriteLine("  makepfs <input_project.gp4> <output_pfs.dat>");
          Console.WriteLine("  makepkg <input_project.gp4> <output_directory>");
          Console.WriteLine("  extractpkg <input.pkg> <passcode> <output_directory>");
          Console.WriteLine("  extractouterpfs <input.pkg> <passcode> <output_pfs.dat>");
          Console.WriteLine("  extractouterpfs_e <input.pkg> <output_pfs_encrypted.dat>");
          Console.WriteLine("  extractinnerpfs <input.pkg> <passcode> <pfs_image.dat>");
          Console.WriteLine("  listentries <input.pkg>");
          Console.WriteLine("  extractentry <input.pkg> <entry_id> <output.bin>");
          Console.WriteLine();
          Console.WriteLine("Use passcode \"fake\" to decrypt a FAKE PKG without knowing the actual passcode.");
          break;
      }
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
      if(passcode == "fake")
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
}
