using System;
using System.Collections.Generic;
using System.IO;
using LibOrbisPkg.Util;

namespace LibOrbisPkg.PFS
{

  class FlatPathTable
  {
    private SortedDictionary<uint, uint> hashMap;

    public int Size => hashMap.Count * 8;

    /// <summary>
    /// Construct a flat_path_table out of the given filesystem nodes.
    /// </summary>
    /// <param name="nodes"></param>
    public FlatPathTable(List<FSNode> nodes)
    {
      hashMap = new SortedDictionary<uint, uint>();
      foreach (var n in nodes)
      {
        var hash = HashFunction(n.FullPath());
        if (hashMap.ContainsKey(hash))
          Console.WriteLine("Warning: hash collisions not yet handled.");
        hashMap[hash] = n.ino.Number;
      }
    }

    /// <summary>
    /// Write this file to the stream.
    /// </summary>
    /// <param name="s"></param>
    public void WriteToStream(Stream s)
    {
      foreach (var hash in hashMap.Keys)
      {
        s.WriteUInt32LE(hash);
        s.WriteUInt32LE(hashMap[hash]);
      }
    }

    /// <summary>
    /// Hashes the given name for the table.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private static uint HashFunction(string name)
    {
      uint hash = 0;
      foreach (var c in name)
        hash += char.ToUpper(c) + 31 * hash;
      return hash;
    }
  }
}