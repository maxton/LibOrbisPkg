using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
              proj = LibOrbisPkg.GP4.Gp4Project.ReadFrom(File.OpenRead(proj)),
              projDir = Path.GetDirectoryName(proj)
            };
            new LibOrbisPkg.PFS.PfsBuilder().BuildPfs(props);
            break;
          }
      }
    }
  }
}
