using System;
using System.IO;
using System.Runtime.InteropServices;

namespace LibOrbisPkg.Util
{
  public class ReaderBase
  {
    [StructLayout(LayoutKind.Explicit, Size = 8)]
    private struct storage
    {
      [FieldOffset(0)]
      public byte u8;
      [FieldOffset(0)]
      public sbyte s8;
      [FieldOffset(0)]
      public ushort u16;
      [FieldOffset(0)]
      public short s16;
      [FieldOffset(0)]
      public uint u32;
      [FieldOffset(0)]
      public int s32;
      [FieldOffset(0)]
      public ulong u64;
      [FieldOffset(0)]
      public long s64;
      [FieldOffset(0)]
      public float f32;
      [FieldOffset(0)]
      public double f64;
      [FieldOffset(0)]
      public byte[] buf;
    }

    private storage buffer;
    protected Stream s;
    protected bool flipEndian;
    protected ReaderBase(bool flipEndian, Stream stream)
    {
      s = stream;
      this.flipEndian = flipEndian;
      buffer.buf = new byte[8];
    }
    private ref storage ReadEndian(int count)
    {
      s.Read(buffer.buf, 0, count);
      if (flipEndian)
        Array.Reverse(buffer.buf, 0, count);
      return ref buffer;
    }
    protected byte Byte() => ReadEndian(1).u8;
    protected sbyte SByte() => ReadEndian(1).s8;
    protected ushort UShort() => ReadEndian(2).u16;
    protected short Short() => ReadEndian(2).s16;
    protected uint UInt() => ReadEndian(4).u32;
    protected int Int() => ReadEndian(4).s32;
    protected ulong ULong() => ReadEndian(8).u64;
    protected long Long() => ReadEndian(8).s64;
    //protected unsafe void ReadBytes(byte* arr, int count)
    //{
    //  for (var i = 0; i < count; i++)
    //  {
    //    arr[i] = (byte)s.ReadByte();
    //  }
    //}
    protected byte[] ReadBytes(int count)
    {
      var ret = new byte[count];
      s.Read(ret, 0, count);
      return ret;
    }
  }
}
