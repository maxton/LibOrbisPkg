using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LibOrbisPkg.PFS
{
  abstract class FSNode
  {
    public FSDir Parent = null;
    public string name;
    public PfsDinode32 ino;
    public virtual long Size { get; set; }

    public string FullPath(string suffix = "")
    {
      if (Parent == null) return suffix;
      return Parent.FullPath("/" + name + suffix);
    }
  }

  class FSDir : FSNode
  {
    public List<FSDir> Dirs = new List<FSDir>();
    public List<FSFile> Files = new List<FSFile>();


    public List<PfsDirent> Dirents = new List<PfsDirent>();

    public override long Size
    {
      get { return Dirents.Sum(d => d.EntSize); }
    }

    public List<FSNode> GetAllChildren()
    {
      var ret = new List<FSNode>(GetAllChildrenDirs());
      ret.AddRange(GetAllChildrenFiles());
      return ret;
    }

    public List<FSDir> GetAllChildrenDirs()
    {
      var ret = new List<FSDir>(Dirs);
      foreach (var dir in Dirs)
        foreach (var child in dir.GetAllChildrenDirs())
          ret.Add(child);
      return ret;
    }

    public List<FSFile> GetAllChildrenFiles()
    {
      var ret = new List<FSFile>(Files);
      foreach (var dir in GetAllChildrenDirs())
        foreach (var f in dir.Files)
          ret.Add(f);
      return ret;
    }
  }

  class FSFile : FSNode
  {
    public string OrigFileName;
  }
}
