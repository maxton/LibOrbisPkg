using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
  public class PfsReaderTests
  {
    [TestMethod]
    public void TestMissingKeyException()
    {
      var pfs = new PfsHeader() { Seed = new byte[32], Mode = PfsMode.Encrypted | PfsMode.Signed | PfsMode.UnknownFlagAlwaysSet };
      using (var s = new MemoryStream())
      {
        pfs.WriteToStream(s);
        s.Position = 0;
        var reader = new TestHelper.ArrayMemoryReader(s.GetBuffer());
        Assert.ThrowsException<ArgumentException>(() => { new PfsReader(reader, 0, null, null, null); });
        Assert.ThrowsException<ArgumentException>(() => { new PfsReader(reader, 0, null, new byte[32], null); });
        Assert.ThrowsException<ArgumentException>(() => { new PfsReader(reader, 0, null, null, new byte[32]); });
      }
    }



    [TestMethod]
    public void TestTweakDataKeys()
    {
      var pfsProperties = new PfsProperties
      {
        BlockSize = 0x10000,
        EKPFS = new byte[32],
        Encrypt = true,
        FileTime = 1,
        root = TestHelper.MakeRoot(),
        Seed = new byte[16],
        Sign = true,
      };
      using (var pfsImg = new MemoryStream())
      {
        new PfsBuilder(pfsProperties).WriteImage(pfsImg);
        var streamReader = new LibOrbisPkg.Util.StreamReader(pfsImg);
        var reader1 = new PfsReader(streamReader, 0, ekpfs: new byte[32]);
        var (tweak, data) = Crypto.PfsGenEncKey(new byte[32], new byte[16]);
        var reader2 = new PfsReader(streamReader, 0, data: data, tweak: tweak);
      }
    }
  }
}
