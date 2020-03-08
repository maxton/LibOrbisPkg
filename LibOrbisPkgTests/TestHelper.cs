using LibOrbisPkg.GP4;
using LibOrbisPkg.PFS;
using LibOrbisPkg.PKG;
using LibOrbisPkg.SFO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibOrbisPkgTests
{
  static class TestHelper
  {
    private readonly static DateTime PkgDate = new DateTime(2013, 03, 15, 12, 00, 00, CultureInfo.InvariantCulture.Calendar);
    public static PkgProperties MakeProperties(
        string ContentId = "UP0000-TEST00000_00-0000000000000000",
        DateTime? CreationDate = null,
        VolumeType VolumeType = VolumeType.pkg_ps4_ac_data,
        string EntitlementKey = "00000000000000000000000000000000",
        string Passcode = "00000000000000000000000000000000",
        DateTime? TimeStamp = null,
        bool UseCreationTime = true,
        FSDir RootDir = null,
        FSFile[] sc0Files = null)
    {
      return new PkgProperties()
      {
        ContentId = ContentId,
        CreationDate = CreationDate ?? PkgDate,
        VolumeType = VolumeType,
        EntitlementKey = EntitlementKey,
        Passcode = Passcode,
        TimeStamp = TimeStamp ?? PkgDate,
        UseCreationTime = UseCreationTime,
        RootDir = RootDir ?? MakeRoot(sc0Files: sc0Files),
      };
    }

    public static FSDir MakeRoot(ParamSfo sfo = null, FSFile[] sc0Files = null)
    {
      if (sfo == null)
        sfo = ParamSfo.DefaultAC;
      var root = new FSDir();
      var sysDir = new FSDir()
      {
        name = "sce_sys",
        Parent = root,
      };
      sysDir.Files.Add(new FSFile(s => sfo.Write(s), "param.sfo", sfo.FileSize)
      {
        Parent = sysDir
      });
      if (sc0Files != null)
      {
        foreach (var f in sc0Files)
        {
          sysDir.Files.Add(f);
          f.Parent = sysDir;
        }
      }
      root.Dirs.Add(sysDir);
      return root;
    }

    // Helper for checking the internal files of a PKG
    public static void OpenPkgFilesystem(string pkgPath, Action<PfsReader> innerPfsAction)
    {
      Pkg pkg;
      using (var mmf = MemoryMappedFile.CreateFromFile(pkgPath))
      {
        using (var s = mmf.CreateViewStream(0, 0, MemoryMappedFileAccess.Read))
        {
          pkg = new PkgReader(s).ReadPkg();
        }
        var ekpfs = LibOrbisPkg.Util.Crypto.ComputeKeys(pkg.Header.content_id, "00000000000000000000000000000000", 1);
        var outerPfsOffset = (long)pkg.Header.pfs_image_offset;
        using (var acc = mmf.CreateViewAccessor(outerPfsOffset, (long)pkg.Header.pfs_image_size, MemoryMappedFileAccess.Read))
        {
          var outerPfs = new PfsReader(acc, pkg.Header.pfs_flags, ekpfs);
          var inner = new PfsReader(new PFSCReader(outerPfs.GetFile("pfs_image.dat").GetView()));
          // Check that the sce_sys directory exists
          innerPfsAction(inner);
        }
      }
    }

    public static void OpenPkgFilesystem(System.IO.Stream pkgStream, Action<PfsReader> innerPfsAction)
    {
      Pkg pkg;
      pkg = new PkgReader(pkgStream).ReadPkg();
      var ekpfs = LibOrbisPkg.Util.Crypto.ComputeKeys(pkg.Header.content_id, "00000000000000000000000000000000", 1);
      var outerPfsOffset = (long)pkg.Header.pfs_image_offset;
      using (var acc = new LibOrbisPkg.Util.StreamReader(pkgStream, outerPfsOffset))
      {
        var outerPfs = new PfsReader(acc, pkg.Header.pfs_flags, ekpfs);
        var inner = new PfsReader(new PFSCReader(outerPfs.GetFile("pfs_image.dat").GetView()));
        // Check that the sce_sys directory exists
        innerPfsAction(inner);
      }
    }

    public class ArrayMemoryReader : LibOrbisPkg.Util.IMemoryReader
    {
      private byte[] array;
      public ArrayMemoryReader(byte[] backing)
      {
        array = backing;
      }
      public void Dispose()
      { }

      public void Read(long pos, byte[] buf, int offset, int count)
      {
        if (pos > int.MaxValue) throw new Exception("Don't think there's a way to block-copy beyond the int32 limit :(");
        Buffer.BlockCopy(array, (int)pos, buf, offset, count);
      }
    }
  }
}
