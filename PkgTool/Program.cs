using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LibOrbisPkg.GP4;
using LibOrbisPkg.PFS;
using LibOrbisPkg.PKG;

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
            new PfsBuilder().BuildPfs(props);
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
      }
    }
  }
}
