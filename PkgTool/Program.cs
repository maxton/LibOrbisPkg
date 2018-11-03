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
      switch (args[0])
      {
        case "makepfs":
          {
            var proj = args[1];
            var output = args[2];
            var outFile = File.OpenWrite(output);
            var props = new LibOrbisPkg.PFS.PfsProperties
            {
              BlockSize = 65536,
              output = outFile,
              proj = Gp4Project.ReadFrom(File.OpenRead(proj)),
              projDir = Path.GetDirectoryName(proj)
            };
            new PfsBuilder(props, Console.WriteLine).BuildPfs();
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
            var keyString = new string(Crypto.ComputeKeys(pkg.Header.content_id, passcode, 1).Select(b => (char)b).ToArray());
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
        default:
          Console.WriteLine("PkgTool.exe <verb> <input> <output>");
          Console.WriteLine("");
          Console.WriteLine("Verbs:");
          Console.WriteLine("  makepfs <input_project.gp4> <output_pfs.dat>");
          Console.WriteLine("  makepkg <input_project.gp4> <output_directory>");
          Console.WriteLine("  extractpkg <input.pkg> <passcode> <output_directory>");
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
  }
}
