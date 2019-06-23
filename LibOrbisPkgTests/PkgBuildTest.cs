using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibOrbisPkg.GP4;
using LibOrbisPkg.PKG;
using LibOrbisPkg.PFS;
using LibOrbisPkg.SFO;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace LibOrbisPkgTests
{
  /// <summary>
  /// Tests functionality of building PKGs
  /// </summary>
  [TestClass]
  public class PkgBuildTest
  {
    private readonly static DateTime PkgDate = new DateTime(2013, 03, 15, 12, 00, 00, CultureInfo.InvariantCulture.Calendar);
    private static PkgProperties MakeProperties(
        string ContentId = "UP0000-TEST00000_00-0000000000000000",
        DateTime? CreationDate = null,
        VolumeType VolumeType = VolumeType.pkg_ps4_ac_data,
        string EntitlementKey = "00000000000000000000000000000000",
        string Passcode = "00000000000000000000000000000000",
        DateTime? TimeStamp = null,
        bool UseCreationTime = true,
        FSDir RootDir = null)
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
        RootDir = RootDir ?? MakeRoot(),
      };
    }

    private static FSDir MakeRoot(ParamSfo sfo = null)
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
      root.Dirs.Add(sysDir);
      return root;
    }

    // Helper for checking the internal files of a PKG
    private static void OpenPkgFilesystem(string pkgPath, Action<PfsReader> innerPfsAction)
    {
      Pkg pkg;
      using (var mmf = MemoryMappedFile.CreateFromFile(pkgPath))
      {
        using (var s = mmf.CreateViewStream())
        {
          pkg = new PkgReader(s).ReadPkg();
        }
        var ekpfs = LibOrbisPkg.Util.Crypto.ComputeKeys(pkg.Header.content_id, "00000000000000000000000000000000", 1);
        var outerPfsOffset = (long)pkg.Header.pfs_image_offset;
        using (var acc = mmf.CreateViewAccessor(outerPfsOffset, (long)pkg.Header.pfs_image_size))
        {
          var outerPfs = new PfsReader(acc, ekpfs);
          var inner = new PfsReader(new PFSCReader(outerPfs.GetFile("pfs_image.dat").GetView()));
          // Check that the sce_sys directory exists
          innerPfsAction(inner);
        }
      }
    }

    /// <summary>
    /// Tests that PkgBuilder.Write(Stream) and PkgBuilder.Write(string) make the same result.
    /// </summary>
    [TestMethod]
    public void StreamAndMMFileSameResult()
    {
      using (var pkg1 = new TempFile())
      using (var pkg2 = new TempFile())
      {
        new PkgBuilder(MakeProperties()).Write(pkg1.Path, s => { });
        using (var f = File.Open(pkg2.Path, FileMode.Create))
        {
          new PkgBuilder(MakeProperties()).Write(f, s => { });
        }

        var pkg1Bytes = File.ReadAllBytes(pkg1.Path);
        var pkg2Bytes = File.ReadAllBytes(pkg2.Path);
        CollectionAssert.AreEqual(pkg1Bytes, pkg2Bytes);
      }
    }

    /// <summary>
    /// Tests that a PKG created using a file stream can be read back.
    /// </summary>
    [TestMethod]
    public void EmptyPkgStreamed()
    {
      using(var pkgFile = new TempFile())
      {
        using (var f = File.Open(pkgFile.Path, FileMode.Create))
        {
          new PkgBuilder(MakeProperties()).Write(f, s => { });
        }

        // This PKG only has an sce_sys directory and nothing else
        OpenPkgFilesystem(pkgFile.Path, innerPfs =>
          Assert.AreEqual("sce_sys", innerPfs.GetURoot().children[0].name));
      }
    }

    /// <summary>
    /// Tests that a PKG created using a memory-mapped file can be read back.
    /// </summary>
    [TestMethod]
    public void EmptyPkgMMFile()
    {
      using (var pkgFile = new TempFile())
      {
        new PkgBuilder(MakeProperties()).Write(pkgFile.Path, s => { });
        OpenPkgFilesystem(pkgFile.Path, innerPfs =>
          Assert.AreEqual("sce_sys", innerPfs.GetURoot().children[0].name));
      }
    }

    /// <summary>
    /// Tests that a PKG passes all validation checks.
    /// </summary>
    [TestMethod]
    public void Validate()
    {
      using (var pkgFile = new TempFile())
      {
        new PkgBuilder(MakeProperties()).Write(pkgFile.Path, s => { });

        using (var pkgStream = File.OpenRead(pkgFile.Path))
        {
          var pkg = new PkgReader(pkgStream).ReadPkg();
          foreach(var v in new PkgValidator(pkg).Validate(pkgStream))
          {
            Assert.IsTrue(v.Item2 == PkgValidator.ValidationResult.Ok, v.Item1.Name);
          }
        }
      }
    }
  }
}
