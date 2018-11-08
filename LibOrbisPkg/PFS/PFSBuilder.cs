using LibOrbisPkg.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Linq;

namespace LibOrbisPkg.PFS
{
  /// <summary>
  /// Contains the functionality to construct a PFS disk image.
  /// </summary>
  public class PfsBuilder
  {
    static int CeilDiv(int a, int b) => a / b + (a % b == 0 ? 0 : 1);
    static long CeilDiv(long a, long b) => a / b + (a % b == 0 ? 0 : 1);

    private PfsHeader hdr;
    private List<inode> inodes;
    private List<PfsDirent> super_root_dirents;

    private inode super_root_ino, fpt_ino;

    private FSDir root;

    private List<FSDir> allDirs;
    private List<FSFile> allFiles;
    private List<FSNode> allNodes;

    private FlatPathTable fpt;

    private PfsProperties properties;

    private struct BlockSigInfo
    {
      public int Block;
      public long SigOffset;
      public BlockSigInfo(int block, long offset)
      {
        Block = block;
        SigOffset = offset;
      }
    }
    private Stack<BlockSigInfo> sig_order = new Stack<BlockSigInfo>();

    Action<string> logger;
    private void Log(string s) => logger?.Invoke(s);

    public PfsBuilder(PfsProperties p, Action<string> logger = null)
    {
      this.logger = logger;
      properties = p;
      Setup();
    }

    public long CalculatePfsSize()
    {
      return hdr.Ndblock * hdr.BlockSize;
    }

    void Setup()
    {
      // TODO: Combine the superroot-specific stuff with the rest of the data block writing.
      // I think this is as simple as adding superroot and flat_path_table to allNodes

      hdr = new PfsHeader {
        BlockSize = properties.BlockSize,
        ReadOnly = 1,
        Mode = (properties.Sign ? PfsMode.Signed : 0) 
             | (properties.Encrypt ? PfsMode.Encrypted : 0)
             | PfsMode.UnknownFlagAlwaysSet,
        Seed = properties.Seed
      };
      inodes = new List<inode>();

      Log("Setting up root structure...");
      SetupRootStructure();
      allDirs = root.GetAllChildrenDirs();
      allFiles = root.GetAllChildrenFiles();
      allNodes = new List<FSNode>(allDirs);
      allNodes.AddRange(allFiles);

      Log(string.Format("Creating directory inodes ({0})...", allDirs.Count));
      addDirInodes();

      Log(string.Format("Creating file inodes ({0})...", allFiles.Count));
      addFileInodes();

      Log("Creating flat_path_table...");
      fpt = new FlatPathTable(allNodes);


      Log("Calculating data block layout...");
      allNodes.Insert(0, root);
      CalculateDataBlockLayout();
    }

    public void WriteImage(Stream stream)
    {
      Log("Writing header...");
      hdr.WriteToStream(stream);
      Log("Writing inodes...");
      WriteInodes(stream);
      Log("Writing superroot dirents");
      WriteSuperrootDirents(stream);

      Log("Writing flat_path_table");
      stream.Position = fpt_ino.DirectBlocks[0] * hdr.BlockSize;
      fpt.WriteToStream(stream);

      Log("Writing data blocks...");
      for (var x = 0; x < allNodes.Count; x++)
      {
        var f = allNodes[x];
        stream.Position = f.ino.DirectBlocks[0] * hdr.BlockSize;
        WriteFSNode(stream, f);
      }
      stream.SetLength(hdr.Ndblock * hdr.BlockSize);

      if (properties.Sign)
      {
        Log("Signing...");
        var signKey = Crypto.PfsGenSignKey(properties.EKPFS, properties.Seed);
        foreach (var sig in sig_order)
        {
          var sig_buffer = new byte[properties.BlockSize];
          stream.Position = sig.Block * properties.BlockSize;
          stream.Read(sig_buffer, 0, (int)properties.BlockSize);
          stream.Position = sig.SigOffset;
          stream.Write(Crypto.HmacSha256(signKey, sig_buffer), 0, 32);
        }
      }

      if (properties.Encrypt)
      {
        Log("Encrypting...");
        var encKey = Crypto.PfsGenEncKey(properties.EKPFS, properties.Seed);
        var dataKey = new byte[16];
        var tweakKey = new byte[16];
        Buffer.BlockCopy(encKey, 0, tweakKey, 0, 16);
        Buffer.BlockCopy(encKey, 16, dataKey, 0, 16);
        stream.Position = hdr.BlockSize;
        var transformer = new XtsBlockTransform(dataKey, tweakKey);
        const int sectorSize = 0x1000;
        long xtsSector = 16;
        long totalSectors = (stream.Length + 0xFFF) / sectorSize;
        byte[] sectorBuffer = new byte[sectorSize];
        while (xtsSector < totalSectors)
        {
          stream.Position = xtsSector * sectorSize;
          stream.Read(sectorBuffer, 0, sectorSize);
          transformer.EncryptSector(sectorBuffer, (ulong)xtsSector);
          stream.Position = xtsSector * sectorSize;
          stream.Write(sectorBuffer, 0, sectorSize);
          xtsSector += 1;
        }
      }
    }

    /// <summary>
    /// Adds inodes for each dir.
    /// </summary>
    void addDirInodes()
    {
      inodes.Add(root.ino);
      foreach (var dir in allDirs)
      {
        var ino = MakeInode(
          Mode: InodeMode.dir | InodeMode.rwx,
          Number: (uint)inodes.Count,
          Blocks: 1,
          Size: 65536
        );
        dir.ino = ino;
        dir.Dirents.Add(new PfsDirent { Name = ".", InodeNumber = ino.Number, Type = DirentType.Dot });
        dir.Dirents.Add(new PfsDirent { Name = "..", InodeNumber = dir.Parent.ino.Number, Type = DirentType.DotDot });

        var dirent = new PfsDirent { Name = dir.name, InodeNumber = (uint)inodes.Count, Type = DirentType.Directory };
        dir.Parent.Dirents.Add(dirent);
        dir.Parent.ino.Nlink++;
        inodes.Add(ino);
      }
    }

    /// <summary>
    /// Adds inodes for each file.
    /// </summary>
    void addFileInodes()
    {
      foreach (var file in allFiles)
      {
        var ino = MakeInode(
          Mode: InodeMode.file | InodeMode.rwx,
          Size: file.Size,
          SizeCompressed: file.Size,
          Number: (uint)inodes.Count,
          Blocks: (uint)CeilDiv(file.Size, hdr.BlockSize)
        );
        file.ino = ino;
        var dirent = new PfsDirent { Name = file.name, Type = DirentType.File, InodeNumber = (uint)inodes.Count };
        file.Parent.Dirents.Add(dirent);
        inodes.Add(ino);
      }
    }

    long dirSizeToSize(long size) => CeilDiv(size, hdr.BlockSize) * hdr.BlockSize;

    /// <summary>
    /// Sets the data blocks. Also updates header for total number of data blocks.
    /// </summary>
    void CalculateDataBlockLayout()
    {
      // Include the header block in the total count
      hdr.Ndblock = 1;
      var inodesPerBlock = hdr.BlockSize / (properties.Sign ? DinodeS32.SizeOf : DinodeD32.SizeOf);
      hdr.DinodeCount = inodes.Count;
      hdr.DinodeBlockCount = CeilDiv(inodes.Count, inodesPerBlock);
      hdr.Ndblock += hdr.DinodeBlockCount;
      super_root_ino.SetDirectBlock(0, (int)(hdr.DinodeBlockCount + 1));
      hdr.Ndblock += super_root_ino.Blocks;

      // flat path table
      fpt_ino.SetDirectBlock(0, super_root_ino.DirectBlocks[0] + 1);
      fpt_ino.Size = fpt.Size;
      fpt_ino.SizeCompressed = fpt.Size;
      fpt_ino.Blocks = (uint)CeilDiv(fpt.Size, hdr.BlockSize);
      // DATs I've found include an empty block after the FPT
      hdr.Ndblock += fpt_ino.Blocks + 1;

      for (int i = 1; i < fpt_ino.Blocks && i < 12; i++)
        fpt_ino.SetDirectBlock(i, -1);

      // All fs entries.
      var currentBlock = fpt_ino.DirectBlocks[0] + fpt_ino.Blocks + 1;
      // Calculate length of all dirent blocks
      foreach (var n in allNodes)
      {
        var blocks = CeilDiv(n.Size, hdr.BlockSize);
        n.ino.SetDirectBlock(0, (int)currentBlock);
        n.ino.Blocks = (uint)blocks;
        n.ino.Size = n is FSDir ? dirSizeToSize(n.Size) : n.Size;
        n.ino.SizeCompressed = n.ino.Size;
        for (int i = 1; i < blocks && i < 12; i++)
        {
          n.ino.SetDirectBlock(i, -1);
        }
        currentBlock += blocks;
        hdr.Ndblock += blocks;
      }
    }

    inode MakeInode(InodeMode Mode, uint Blocks, long Size=0, long SizeCompressed=0, ushort Nlink=2, uint Number=0, InodeFlags Flags=0)
    {
      if (properties.Sign)
      {
        return new DinodeS32()
        {
          Mode = Mode,
          Blocks = Blocks,
          Size = Size,
          SizeCompressed = SizeCompressed,
          Nlink = Nlink,
          Number = Number,
          Flags = Flags
        };
      }
      else
      {
        return new DinodeD32()
        {
          Mode = Mode,
          Blocks = Blocks,
          Size = Size,
          SizeCompressed = SizeCompressed,
          Nlink = Nlink,
          Number = Number,
          Flags = Flags
        };
      }
    }

    /// <summary>
    /// Creates inodes and dirents for superroot, flat_path_table, and uroot.
    /// Also, creates the root node for the FS tree.
    /// </summary>
    void SetupRootStructure()
    {
      inodes.Add(super_root_ino = MakeInode(
        Mode: InodeMode.dir | InodeMode.rx_only,
        Blocks: 1,
        Size: 65536,
        SizeCompressed: 65536,
        Nlink: 1,
        Number: 0,
        Flags: InodeFlags.@internal
      ));
      inodes.Add(fpt_ino = MakeInode(
        Mode: InodeMode.file | InodeMode.rwx,
        Blocks: 1,
        Number: 1,
        Flags: InodeFlags.@internal
      ));
      var uroot_ino = MakeInode(Mode: InodeMode.dir | InodeMode.rwx, Number: 2, Size: 65536, SizeCompressed: 65536, Blocks: 1);

      super_root_dirents = new List<PfsDirent>
      {
        new PfsDirent { InodeNumber = 1, Name = "flat_path_table", Type = DirentType.File },
        new PfsDirent { InodeNumber = 2, Name = "uroot", Type = DirentType.Directory }
      };

      root = properties.root;
      root.name = "uroot";
      root.ino = uroot_ino;
      root.Dirents = new List<PfsDirent>
      {
        new PfsDirent { Name = ".", Type = DirentType.Dot, InodeNumber = 2 },
        new PfsDirent { Name = "..", Type = DirentType.DotDot, InodeNumber = 2 }
      };
    }

    /// <summary>
    /// Writes all the inodes to the image file. 
    /// </summary>
    /// <param name="s"></param>
    void WriteInodes(Stream s)
    {
      s.Position = hdr.BlockSize;
      foreach (var di in inodes)
      {
        di.WriteToStream(s);
        if (s.Position % hdr.BlockSize > hdr.BlockSize - (properties.Sign ? DinodeS32.SizeOf : DinodeD32.SizeOf))
        {
          s.Position += hdr.BlockSize - (s.Position % hdr.BlockSize);
        }
      }
    }

    /// <summary>
    /// Writes the dirents for the superroot, which precede the flat_path_table.
    /// </summary>
    /// <param name="stream"></param>
    void WriteSuperrootDirents(Stream stream)
    {
      stream.Position = hdr.BlockSize * (hdr.DinodeBlockCount + 1);
      foreach (var d in super_root_dirents)
      {
        d.WriteToStream(stream);
      }
    }

    /// <summary>
    /// Writes all the data blocks.
    /// </summary>
    /// <param name="s"></param>
    void WriteFSNode(Stream s, FSNode f)
    {
      if (f is FSDir)
      {
        var dir = (FSDir)f;
        var startBlock = f.ino.DirectBlocks[0];
        foreach (var d in dir.Dirents)
        {
          d.WriteToStream(s);
          if (s.Position % hdr.BlockSize > hdr.BlockSize - PfsDirent.MaxSize)
          {
            s.Position = (++startBlock * hdr.BlockSize);
          }
        }
      }
      else if (f is FSFile)
      {
        var file = (FSFile)f;
        file.Write(s);
      }
    }
  }
}