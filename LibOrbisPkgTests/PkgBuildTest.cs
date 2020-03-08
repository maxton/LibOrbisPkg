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
using LibOrbisPkg.Util;
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
    /// <summary>
    /// Tests that PkgBuilder.Write(Stream) and PkgBuilder.Write(string) make the same result.
    /// </summary>
    [TestMethod]
    public void StreamAndMMFileSameResult()
    {
      using (var pkg1 = new TempFile())
      using (var pkg2 = new TempFile())
      {
        new PkgBuilder(TestHelper.MakeProperties()).Write(pkg1.Path, s => { });
        using (var f = File.Open(pkg2.Path, FileMode.Create))
        {
          new PkgBuilder(TestHelper.MakeProperties()).Write(f, s => { });
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
          new PkgBuilder(TestHelper.MakeProperties()).Write(f, s => { });
        }

        // This PKG only has an sce_sys directory and nothing else
        TestHelper.OpenPkgFilesystem(pkgFile.Path, innerPfs =>
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
        new PkgBuilder(TestHelper.MakeProperties()).Write(pkgFile.Path, s => { });
        TestHelper.OpenPkgFilesystem(pkgFile.Path, innerPfs =>
          Assert.AreEqual("sce_sys", innerPfs.GetURoot().children[0].name));
      }
    }

    [TestMethod]
    public void ManyFilePkg()
    {
      const int NumFiles = 5000;
      using (var pkgFile = new MemoryStream())
      {
        var buf = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        Action<Stream> fileWriter = s => s.Write(buf, 0, 16);
        var rootDir = TestHelper.MakeRoot();
        var filesDir = new FSDir() { Parent = rootDir, name = "files" };
        rootDir.Dirs.Add(filesDir);
        for(int i = 0; i < NumFiles; i++)
        {
          filesDir.Files.Add(new FSFile(fileWriter, $"file_{i}", 16) { Parent = filesDir });
        }
        new PkgBuilder(TestHelper.MakeProperties(RootDir: rootDir)).Write(pkgFile, Console.WriteLine);
        int foundFiles = 0;
        TestHelper.OpenPkgFilesystem(pkgFile, innerPfs =>
          {
            var testBuf = new byte[16];
            foreach(var f in innerPfs.GetURoot().GetAllFiles())
            {
              foundFiles++;
              using (var view = f.GetView())
              {
                view.Read(0, testBuf, 0, 16);
                CollectionAssert.AreEqual(buf, testBuf, $"Expected {buf.AsHexCompact()}, got {testBuf.AsHexCompact()}");
              }
            }
          });

        Assert.AreEqual(NumFiles, foundFiles);
      }
    }

    /// <summary>
    /// Tests that a PKG passes all validation checks.
    /// </summary>
    [TestMethod]
    public void ValidateAC()
    {
      using (var pkgFile = new MemoryStream())
      {
        new PkgBuilder(TestHelper.MakeProperties()).Write(pkgFile, s => { });

        var pkg = new PkgReader(pkgFile).ReadPkg();
        foreach(var v in new PkgValidator(pkg).Validate(pkgFile))
        {
          Assert.IsTrue(v.Item2 == PkgValidator.ValidationResult.Ok, v.Item1.Name);
        }
      }
    }

    /// <summary>
    /// Tests that a PKG passes all validation checks.
    /// </summary>
    [TestMethod]
    public void ValidateGD()
    {
      using (var pkgFile = new MemoryStream())
      {
        new PkgBuilder(TestHelper.MakeProperties(VolumeType: VolumeType.pkg_ps4_app)).Write(pkgFile, s => { });

        var pkg = new PkgReader(pkgFile).ReadPkg();
        foreach (var v in new PkgValidator(pkg).Validate(pkgFile))
        {
          Assert.IsTrue(v.Item2 == PkgValidator.ValidationResult.Ok, v.Item1.Name);
        }
      }
    }

    /// <summary>
    /// Tests that a PKG passes all validation checks.
    /// </summary>
    [TestMethod]
    public void ValidateGD_LargeSc0()
    {
      using (var pkgFile = new MemoryStream())
      {
        const int size = 0x1023456;
        var properties = TestHelper.MakeProperties(
          VolumeType: VolumeType.pkg_ps4_app,
          sc0Files: new[] {  new FSFile(s => s.Position += size, "save_data.png", size) });
        new PkgBuilder(properties).Write(pkgFile, s => { });

        var pkg = new PkgReader(pkgFile).ReadPkg();
        foreach (var v in new PkgValidator(pkg).Validate(pkgFile))
        {
          Assert.IsTrue(v.Item2 == PkgValidator.ValidationResult.Ok, v.Item1.Name);
        }
      }
    }
  }
}
