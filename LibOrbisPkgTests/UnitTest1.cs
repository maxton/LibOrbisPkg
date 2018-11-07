using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LibOrbisPkg.Util;
using System.Linq;

namespace LibOrbisPkgTests
{
  [TestClass]
  public class XtsTest
  {
    [TestMethod]
    public void TestRoundTrip()
    {
      byte[] dataKey = new byte[16];
      byte[] tweakKey = new byte[16];
      var transformer = new XtsBlockTransform(dataKey, tweakKey);
      byte[] data = new byte[]
      {
        0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 0xE, 0xF,
        0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F,
      };
      byte[] expected = data.ToArray();
      transformer.EncryptSector(data, 0);
      CollectionAssert.AreNotEqual(expected, data);
      transformer.DecryptSector(data, 0);
      CollectionAssert.AreEqual(expected, data);


      transformer.EncryptSector(data, 1);
      CollectionAssert.AreNotEqual(expected, data);
      transformer.DecryptSector(data, 1);
      CollectionAssert.AreEqual(expected, data);
    }
  }
}
