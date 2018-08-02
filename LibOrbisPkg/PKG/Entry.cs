using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LibOrbisPkg.Util;

namespace LibOrbisPkg.PKG
{
  // Represents the data of an entry
  public abstract class Entry
  {
    public abstract EntryId Id { get; }
    public abstract uint Length { get; }
    public abstract string Name { get; }
    public abstract void Write(Stream s);
  }

  /// <summary>
  /// The representation of an entry in the PKG entry table.
  /// </summary>
  public class MetaEntry
  {
    public EntryId id;
    public uint NameTableOffset;
    public uint Flags1;
    public uint Flags2;
    public uint DataOffset;
    public uint DataSize;
    // public ulong Pad; // zero-pad

    public void Write(Stream s)
    {
      s.WriteUInt32BE((uint)id);
      s.WriteUInt32BE(NameTableOffset);
      s.WriteUInt32BE(Flags1);
      s.WriteUInt32BE(Flags2);
      s.WriteUInt32BE(DataOffset);
      s.WriteUInt32BE(DataSize);
      s.Position += 8; // pad
    }
    public static MetaEntry Read(Stream s)
    {
      var ret = new MetaEntry();
      ret.id = (EntryId)s.ReadUInt32BE();
      ret.NameTableOffset = s.ReadUInt32BE();
      ret.Flags1 = s.ReadUInt32BE();
      ret.Flags2 = s.ReadUInt32BE();
      ret.DataOffset = s.ReadUInt32BE();
      ret.DataSize = s.ReadUInt32BE();
      s.Position += 8;
      return ret;
    }
    public uint KeyIndex => (Flags2 & 0xF000) >> 12;
    public bool Encrypted => (Flags1 & 0x80000000) != 0;
  }

  public class GenericEntry : Entry
  {
    public GenericEntry(EntryId id, string name = null)
    {
      Id = id;
      Name = name;
    }

    public byte[] FileData;
    public override EntryId Id { get; }
    public override string Name { get; }
    public override uint Length => (uint)FileData.Length;
    public override void Write(Stream s)
    {
      s.Write(FileData, 0, FileData.Length);
    }
  }

  public class PkgEntryKey
  {
    public const int Size = 32;
    public byte[] key = new byte[16];
    public byte[] iv = new byte[16];
  }

  public class KeysEntry : Entry
  {
    public KeysEntry()
    {
      Keys = new PkgEntryKey[2048 / PkgEntryKey.Size];
      for(var i = 0; i < Keys.Length; i++)
      {
        Keys[i] = new PkgEntryKey();
      }
    }
    public PkgEntryKey[] Keys;
    public override EntryId Id => EntryId.ENTRY_KEYS;
    public override string Name => null;
    public override uint Length => 2048;
    public override void Write(Stream s)
    {
      foreach(var keyset in Keys)
      {
        s.Write(keyset.key, 0, keyset.key.Length);
        s.Write(keyset.iv, 0, keyset.iv.Length);
      }
    }
  }

  public class NameTableEntry : Entry
  {
    /// <summary>
    /// Default constructor, intended for a new PKG
    /// </summary>
    public NameTableEntry() { }
    /// <summary>
    /// Constructor intended to be used when reading from a PKG
    /// </summary>
    public NameTableEntry(List<string> names)
    {
      int len = 0;
      Names = new Dictionary<string, int>();
      nameList = names;
      foreach(var n in names)
      {
        Names.Add(n, len);
        len += n.Length + 1;
      }
    }
    private int length = 1;
    private Dictionary<string, int> Names = new Dictionary<string, int> { { "", 0 } };
    private List<string> nameList = new List<string> { "" };
    /// <summary>
    /// Gets the offset of a name, adding it to the table if it's not here already.
    /// </summary>
    public uint GetOffset(string name)
    {
      if (name == null || name == "") return 0;
      if(!Names.ContainsKey(name))
      {
        nameList.Add(name);
        Names[name] = length;
        length += name.Length + 1;
      }
      return (uint)Names[name];
    }
    public override EntryId Id => EntryId.ENTRY_NAMES;
    public override string Name => null;
    public override uint Length => (uint)length;
    public override void Write(Stream s)
    {
      foreach(var k in nameList)
      {
        var bytes = Encoding.ASCII.GetBytes(k);
        s.Write(bytes, 0, bytes.Length);
        s.WriteByte(0);
      }
    }
  }

  public class GeneralDigestsEntry : Entry
  {
    public byte[] UnknownDigest;
    public byte[] ContentDigest;
    public byte[] GameDigest;
    public byte[] HeaderDigest;
    public byte[] UnknownDigest2;
    public byte[] MajorParamDigest;
    public byte[] ParamDigest;
    //public byte[] Unk; // 0xA0 of zeroes?

    public override EntryId Id => EntryId.GENERAL_DIGESTS;
    public override uint Length => 0x180;
    public override string Name => null;

    public override void Write(Stream s)
    {
      s.Write(UnknownDigest, 0, 32); 
      s.Write(ContentDigest, 0, 32); 
      s.Write(GameDigest, 0, 32); 
      s.Write(HeaderDigest, 0, 32); 
      s.Write(UnknownDigest2, 0, 32); 
      s.Write(MajorParamDigest, 0, 32); 
      s.Write(ParamDigest, 0, 32);
      s.Position += 0xA0;
    }
  }

  public class MetasEntry : Entry
  {
    public List<MetaEntry> Metas = new List<MetaEntry>();
    public override EntryId Id => EntryId.METAS;
    public override uint Length => (uint) Metas.Count * 32;
    public override string Name => null;
    public override void Write(Stream s)
    {
      foreach(var entry in Metas)
      {
        entry.Write(s);
      }
    }
  }

}
