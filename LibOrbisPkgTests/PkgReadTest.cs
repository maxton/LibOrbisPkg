using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using LibOrbisPkg.GP4;
using LibOrbisPkg.PKG;
using LibOrbisPkg.PFS;
using LibOrbisPkg.SFO;
using LibOrbisPkg.Util;

namespace LibOrbisPkgTests
{
  /// <summary>
  /// Tests functionality of reading PKGs
  /// </summary>
  [TestClass]
  public class PkgReadTest
  {
    private static byte[] TestFile = new byte[] { 1, 2, 3, 4, 255, 255, 0, 0 };
    private TempFile pkgFile;
    private PkgProperties props;

    [TestInitialize]
    public void Initialize()
    {
      pkgFile = new TempFile();
      props = TestHelper.MakeProperties();
      props.EntitlementKey = "000102030405060708090A0B0C0D0E0F";
      props.RootDir.Files.Add(new FSFile(s => s.Write(TestFile, 0, 8), "test.file", 8)
      {
        Parent = props.RootDir
      });
      new PkgBuilder(props).Write(pkgFile.Path, s => { });
    }

    [TestCleanup]
    public void Cleanup()
    {
      pkgFile.Dispose();
    }

    /// <summary>
    /// Tests that the debug rif is read back when the PKG is opened
    /// </summary>
    [TestMethod]
    public void TestReadBackLicenseDat()
    {
      using (var s = File.OpenRead(pkgFile.Path))
      {
        var pkg = new PkgReader(s).ReadPkg();
        Assert.IsNotNull(pkg.LicenseDat);
      }
    }

    /// <summary>
    /// Tests that files extracted from the inner pfs filesystem are OK
    /// </summary>
    [TestMethod]
    public void TestExtractFile()
    {
      using (var extractedFile = new TempFile())
      {
        TestHelper.OpenPkgFilesystem(pkgFile.Path, inner =>
        {
          inner.GetFile("test.file").Save(extractedFile.Path);
          Assert.IsTrue(File.Exists(extractedFile.Path));
          var file = File.ReadAllBytes(extractedFile.Path);
          CollectionAssert.AreEqual(TestFile, file);
        });
      }
    }

    [TestMethod]
    public void TestCreateGP4()
    {
      using (var extractDir = new TempDir())
      {
        Gp4Creator.CreateProjectFromPKG(extractDir.Path, pkgFile.Path);
        using (var f = File.OpenRead(Path.Combine(extractDir.Path, "Project.gp4")))
        {
          var project = Gp4Project.ReadFrom(f);
          Assert.AreEqual(props.EntitlementKey, project.volume.Package.EntitlementKey);
          Assert.AreEqual(props.VolumeType, project.volume.Type);
          // TODO: Figure out timezone problems
          //Assert.AreEqual(props.TimeStamp, project.volume.TimeStamp);


          var file = File.ReadAllBytes(Path.Combine(extractDir.Path, "test.file"));
          CollectionAssert.AreEqual(TestFile, file);
        }
      }
    }
  }
}
