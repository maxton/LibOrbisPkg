using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using LibOrbisPkg.Util;

namespace LibOrbisPkg.PFS
{
  // Class allowing parallel readonly access to a PFS archive
  public class PfsReader
  {
    public abstract class Node
    {
      public Node parent;
      public string name;
      public long offset;
      public long size;
      public uint ino;
      public string FullName => parent != null ? parent.FullName + "/" + name : name;
    }
    public class Dir : Node
    {
      public List<Node> children = new List<Node>();
      public Node Get(string name)
        => children.Where(x => x.name == name).FirstOrDefault();
      public Node GetPath(string name)
      {
        var breadcrumbs = name.Split('/');
        Node n = this;
        var bc = 0;
        while(n != null && bc < breadcrumbs.Length)
        {
          n = (n as Dir)?.Get(breadcrumbs[bc]);
          bc++;
        }
        if(bc < breadcrumbs.Length)
        {
          return null;
        }
        return n;
      }
      public IEnumerable<File> GetAllFiles()
      {
        foreach(var n in children)
        {
          if (n is File f) yield return f;
          if (n is Dir d)
            foreach (var x in d.GetAllFiles())
              yield return x;
        }
      }
    }
    public class File : Node
    {
      private IMemoryReader reader;
      public File(IMemoryReader r) { reader = r; }
      public IMemoryReader GetView()
      {
        return new MemoryAccessor(reader, offset);
      }
    }
    private IMemoryReader reader;
    private PfsHeader hdr;
    private inode[] dinodes;
    private Dir root;
    private Dir uroot;
    private byte[] sectorBuf;
    private Stream sectorStream;

    public PfsReader(MemoryMappedViewAccessor r, byte[] ekpfs = null)
    : this(new MemoryMappedViewAccessor_(r), ekpfs)
    { }
    public PfsReader(IMemoryReader r, byte[] ekpfs = null)
    {
      reader = r;
      var buf = new byte[0x400];
      reader.Read(0, buf, 0, 0x400);

      using (var ms = new MemoryStream(buf))
      {
        hdr = PfsHeader.ReadFromStream(ms);
      }
      int dinodeSize;
      Func<Stream, inode> dinodeReader;
      if (hdr.Mode.HasFlag(PfsMode.Signed))
      {
        dinodes = new DinodeS32[hdr.DinodeCount];
        dinodeReader = DinodeS32.ReadFromStream;
        dinodeSize = 0x2C8;
      }
      else
      {
        dinodes = new DinodeD32[hdr.DinodeCount];
        dinodeReader = DinodeD32.ReadFromStream;
        dinodeSize = 0xA8;
      }
      if (hdr.Mode.HasFlag(PfsMode.Encrypted))
      {
        if (ekpfs == null)
          throw new ArgumentException("PFS image is encrypted but no EKPFS was provided");
        var (tweakKey, dataKey) = Crypto.PfsGenEncKey(ekpfs, hdr.Seed);
        reader = new XtsDecryptReader(reader, dataKey, tweakKey, 16, 0x1000);
      }
      var total = 0;

      var maxPerSector = hdr.BlockSize / dinodeSize;
      sectorBuf = new byte[hdr.BlockSize];
      sectorStream = new MemoryStream(sectorBuf);
      for (var i = 0; i < hdr.DinodeBlockCount; i++)
      {
        var position = hdr.BlockSize + hdr.BlockSize * i;
        reader.Read(position, sectorBuf, 0, sectorBuf.Length);
        sectorStream.Position = 0;
        for (var j = 0; j < maxPerSector && total < hdr.DinodeCount; j++)
          dinodes[total++] = dinodeReader(sectorStream);
      }
      root = LoadDir(0, null, "");
      uroot = root.Get("uroot") as Dir;
      if (uroot == null)
        throw new Exception("Invalid PFS image (no uroot)");
      uroot.parent = null;
      uroot.name = "";
    }

    public File GetFile(string fullPath)
    {
      return uroot.GetPath(fullPath) as File;
    }

    public IEnumerable<File> GetAllFiles()
    {
      return uroot.GetAllFiles();
    }

    public Dir GetURoot()
    {
      return uroot;
    }

    private Dir LoadDir(uint dinode, Node parent, string name)
    {
      var ret = new Dir() { name = name, parent = parent };
      var ino = dinodes[dinode];
      var postLoad = new List<Func<Dir>>();
      foreach (var x in Enumerable.Range(ino.StartBlock, (int)ino.Blocks))
      {
        var position = hdr.BlockSize * x;
        reader.Read(position, sectorBuf, 0, sectorBuf.Length);
        sectorStream.Position = 0;
        while (position < hdr.BlockSize * (x + 1))
        {
          var dirent = PfsDirent.ReadFromStream(sectorStream);
          if (dirent.EntSize == 0) break;
          switch (dirent.Type)
          {
            case DirentType.File:
              ret.children.Add(LoadFile(dirent.InodeNumber, ret, dirent.Name));
              break;
            case DirentType.Directory:
              postLoad.Add(() => LoadDir(dirent.InodeNumber, ret, dirent.Name));
              break;
            case DirentType.Dot:
              break;
            case DirentType.DotDot:
              break;
            default:
              break;
          }
          position += dirent.EntSize;
        }
      }
      foreach (var p in postLoad)
      {
        ret.children.Add(p());
      }
      return ret;
    }

    private File LoadFile(uint dinode, Dir parent, string name)
    {
      return new File(reader)
      {
        name = name,
        parent = parent,
        offset = dinodes[dinode].StartBlock * hdr.BlockSize,
        size = dinodes[dinode].Size,
        ino = dinode
      };
    }
  }
}
