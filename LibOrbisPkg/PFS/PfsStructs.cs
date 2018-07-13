using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibOrbisPkg.Util;

namespace LibOrbisPkg.PFS
{
  public enum PfsMode : ushort
  {
    Decrypted = 0x8,
    CompressedAndEncrypted = 0xD,
    None = 0
  }
  public class PfsHeader
  {
    public long Version = 1; // 1
    public long Magic = 20130315; // 20130315 (march 15 2013???)
    public long Id = 0;
    public byte Fmode = 0;
    public byte Clean = 0;
    public byte ReadOnly = 0;
    public byte Rsv = 0;
    public ushort Mode = 0;
    public ushort Unk1 = 0;
    public uint BlockSize = 0x10000;
    public uint NBackup = 0;
    /// <summary>
    /// This is always 1 for some reason.
    /// </summary>
    public long NBlock = 1;
    public long DinodeCount = 0;
    public long Ndblock = 0;
    public long DinodeBlockCount = 0;
    public long SuperrootInode = 0;

    public void WriteToStream(System.IO.Stream s)
    {
      s.WriteInt64LE(Version);
      s.WriteInt64LE(Magic);
      s.WriteInt64LE(Id);
      s.WriteByte(Fmode);
      s.WriteByte(Clean);
      s.WriteByte(ReadOnly);
      s.WriteByte(Rsv);
      s.WriteUInt16LE(Mode);
      s.WriteUInt16LE(Unk1);
      s.WriteUInt32LE(BlockSize);
      s.WriteUInt32LE(NBackup);
      s.WriteInt64LE(NBlock);
      s.WriteInt64LE(DinodeCount);
      s.WriteInt64LE(Ndblock);
      s.WriteInt64LE(DinodeBlockCount);
      s.WriteInt64LE(SuperrootInode);
    }

    public static PfsHeader ReadFromStream(System.IO.Stream s)
    {
      return new PfsHeader
      {
        Version = s.ReadInt64LE(),
        Magic = s.ReadInt64LE(),
        Id = s.ReadInt64LE(),
        Fmode = s.ReadUInt8(),
        Clean = s.ReadUInt8(),
        ReadOnly = s.ReadUInt8(),
        Rsv = s.ReadUInt8(),
        Mode = s.ReadUInt16LE(),
        Unk1 = s.ReadUInt16LE(),
        BlockSize = s.ReadUInt32LE(),
        NBackup = s.ReadUInt32LE(),
        NBlock = s.ReadInt64LE(),
        DinodeCount = s.ReadInt64LE(),
        Ndblock = s.ReadInt64LE(),
        DinodeBlockCount = s.ReadInt64LE(),
        SuperrootInode = s.ReadInt64LE()
      };
    }
  }

  [Flags]
  public enum InodeMode : ushort
  {
    o_read = 1,
    o_write = 2,
    o_execute = 4,
    g_read = 8,
    g_write = 16,
    g_execute = 32,
    u_read = 64,
    u_write = 128,
    u_execute = 256,
    dir = 16384,
    file = 32768,
    rx_only = 0x16D,
    rwx = 0x1FF
  }

  [Flags]
  public enum InodeFlags : uint
  {
    compressed = 0x1,
    unk1 = 0x2,
    unk2 = 0x4,
    unk3 = 0x8,
    @readonly = 0x10,
    unk4 = 0x20,
    unk5 = 0x40,
    unk6 = 0x80,
    unk7 = 0x100,
    unk8 = 0x200,
    unk9 = 0x400,
    unk10 = 0x800,
    unk11 = 0x1000,
    unk12 = 0x2000,
    unk13 = 0x4000,
    unk14 = 0x8000,
    unk15 = 0x10000,
    @internal = 0x20000
  }

  public class PfsDinode32
  {
    public PfsDinode32()
    {
      SetTime((long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
    }

    public const long SizeOf = 0xA8;
    public uint Number;

    /// <summary>
    /// Default is 555 octal.
    /// </summary>
    public InodeMode Mode = (InodeMode)0x16D;

    /// <summary>
    /// Number of links to this file in the filesystem.
    /// 1 for regular files, 1 + 1 for every subdirectory for dirs.
    /// </summary>
    public ushort Nlink;
    public InodeFlags Flags;
    public long Size;
    public long SizeCompressed;
    public long Time1_sec;
    public long Time2_sec;
    public long Time3_sec;
    public long Time4_sec;
    public uint Time1_nsec;
    public uint Time2_nsec;
    public uint Time3_nsec;
    public uint Time4_nsec;
    public uint Uid;
    public uint Gid;
    public ulong Unk1;
    public ulong Unk2;
    public uint Blocks;
    public int[] db = new int[12];
    public int[] ib = new int[5];

    public PfsDinode32 SetTime(long time)
    {
      Time1_sec = time;
      Time2_sec = time;
      Time3_sec = time;
      Time4_sec = time;
      return this;
    }

    public void WriteToStream(Stream s)
    {
      s.WriteLE((ushort)Mode);
      s.WriteLE(Nlink);
      s.WriteLE((uint)Flags);
      s.WriteLE(Size);
      s.WriteLE(SizeCompressed);
      s.WriteLE(Time1_sec);
      s.WriteLE(Time2_sec);
      s.WriteLE(Time3_sec);
      s.WriteLE(Time4_sec);
      s.WriteLE(Time1_nsec);
      s.WriteLE(Time2_nsec);
      s.WriteLE(Time3_nsec);
      s.WriteLE(Time4_nsec);
      s.WriteLE(Uid);
      s.WriteLE(Gid);
      s.WriteLE(Unk1);
      s.WriteLE(Unk2);
      s.WriteLE(Blocks);
      foreach (var x in db) s.WriteLE(x);
      foreach (var x in ib) s.WriteLE(x);
    }

    public static PfsDinode32 ReadFromStream(Stream s)
    {
      var di = new PfsDinode32
      {
        Mode = (InodeMode)s.ReadUInt16LE(),
        Nlink = s.ReadUInt16LE(),
        Flags = (InodeFlags)s.ReadUInt32LE(),
        Size = s.ReadInt64LE(),
        SizeCompressed = s.ReadInt64LE(),
        Time1_sec = s.ReadInt64LE(),
        Time2_sec = s.ReadInt64LE(),
        Time3_sec = s.ReadInt64LE(),
        Time4_sec = s.ReadInt64LE(),
        Time1_nsec = s.ReadUInt32LE(),
        Time2_nsec = s.ReadUInt32LE(),
        Time3_nsec = s.ReadUInt32LE(),
        Time4_nsec = s.ReadUInt32LE(),
        Uid = s.ReadUInt32LE(),
        Gid = s.ReadUInt32LE(),
        Unk1 = s.ReadUInt64LE(),
        Unk2 = s.ReadUInt64LE(),
        Blocks = s.ReadUInt32LE()
      };
      for (var i = 0; i < 12; i++) di.db[i] = s.ReadInt32LE();
      for (var i = 0; i < 5; i++) di.ib[i] = s.ReadInt32LE();
      return di;
    }
  }

  public class PfsDirent
  {
    public static int MaxSize = 280;

    public uint InodeNumber;
    public int Type;
    public int NameLength;
    public int EntSize;

    public string Name
    {
      get { return name; }
      set
      {
        name = value;
        NameLength = name.Length;
        EntSize = NameLength + 17;
        if (EntSize % 8 != 0)
          EntSize += 8 - (EntSize % 8);
      }
    }

    private string name;

    public void WriteToStream(Stream s)
    {
      var pos = s.Position;
      s.WriteLE(InodeNumber);
      s.WriteLE(Type);
      s.WriteLE(NameLength);
      s.WriteLE(EntSize);
      s.Write(Encoding.ASCII.GetBytes(Name), 0, NameLength);
      var remaining = (int)(EntSize - (s.Position - pos));
      s.Write(new byte[remaining], 0, remaining);
    }

    public PfsDirent ReadFromStream(Stream s)
    {
      var pos = s.Position;
      var d = new PfsDirent
      {
        InodeNumber = s.ReadUInt32LE(),
        Type = s.ReadInt32LE(),
        NameLength = s.ReadInt32LE(),
        EntSize = s.ReadInt32LE(),
        name = s.ReadASCIINullTerminated(NameLength)
      };
      s.Position = pos + d.EntSize;
      return d;
    }
  }
}
