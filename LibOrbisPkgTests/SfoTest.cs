using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LibOrbisPkg.SFO;
using System.Linq;
using System.IO;

namespace LibOrbisPkgTests
{
  [TestClass]
  public class SfoTest
  {
    [TestMethod]
    public void TestValueCreate()
    {
      var value = Value.Create("KEY", SfoEntryType.Integer, "12345");
      Assert.IsInstanceOfType(value, typeof(IntegerValue));
      Assert.AreEqual(12345, (value as IntegerValue).Value);

      value = Value.Create("KEY2", SfoEntryType.Integer, "0x12345");
      Assert.IsInstanceOfType(value, typeof(IntegerValue));
      Assert.AreEqual(0x12345, (value as IntegerValue).Value);

      value = Value.Create("KEY3", SfoEntryType.Integer, "-12345");
      Assert.IsInstanceOfType(value, typeof(IntegerValue));
      Assert.AreEqual(-12345, (value as IntegerValue).Value);

      value = Value.Create("KEY4", SfoEntryType.Integer, "0xFFFF1234");
      Assert.IsInstanceOfType(value, typeof(IntegerValue));
      Assert.AreEqual(-60876, (value as IntegerValue).Value);

      value = Value.Create("KEY5", SfoEntryType.Utf8, "");
      Assert.IsInstanceOfType(value, typeof(Utf8Value));
      Assert.AreEqual("", (value as Utf8Value).Value);

      value = Value.Create("KEY6", SfoEntryType.Utf8, "ABCDEFG");
      Assert.IsInstanceOfType(value, typeof(Utf8Value));
      Assert.AreEqual("ABCDEFG", (value as Utf8Value).Value);
    }

    [TestMethod]
    public void TestSfoValueAddRemove()
    {
      var sfo = new ParamSfo();
      sfo.SetValue("TestKey", SfoEntryType.Integer, "1234");

      Assert.IsNotNull(sfo["TestKey"]);

      sfo["TestKey"] = null;
      Assert.IsNull(sfo["TestKey"]);
      Assert.AreEqual(0, sfo.Values.Count);
    }


    [TestMethod]
    public void TestSfoWriteRead()
    {
      var sfo = new ParamSfo();
      sfo.SetValue("KEY1", SfoEntryType.Integer, "1234");
      sfo.SetValue("KEY2", SfoEntryType.Utf8, "This is the title", 32);
      sfo.SetValue("KEY3", SfoEntryType.Utf8Special, "This is a special string", 32);
      sfo.SetValue("KEY4", SfoEntryType.Integer, "0x1234");

      using (var ms = new MemoryStream())
      {
        sfo.Write(ms);
        ms.Seek(0, SeekOrigin.Begin);
        var sfo2 = ParamSfo.FromStream(ms);
        Assert.AreEqual((sfo2["KEY1"] as IntegerValue).Value, 1234);
        Assert.AreEqual((sfo2["KEY2"] as Utf8Value).Value, "This is the title");
        Assert.AreEqual((sfo2["KEY3"] as Utf8SpecialValue).Value, "This is a special string");
        Assert.AreEqual((sfo2["KEY4"] as IntegerValue).Value, 0x1234);
      }
    }
  }
}
