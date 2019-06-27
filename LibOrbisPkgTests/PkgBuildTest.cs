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

    /// <summary>
    /// Tests that a PKG passes all validation checks.
    /// </summary>
    [TestMethod]
    public void Validate()
    {
      using (var pkgFile = new TempFile())
      {
        new PkgBuilder(TestHelper.MakeProperties()).Write(pkgFile.Path, s => { });

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
