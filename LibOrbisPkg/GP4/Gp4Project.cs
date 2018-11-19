using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LibOrbisPkg.GP4
{

  [XmlRoot(ElementName = "psproject")]
  public class Gp4Project
  {
    [XmlAttribute("fmt")]
    public string Format;
    [XmlAttribute("version")]
    public int version;
    [XmlElement(ElementName = "volume")]
    public Volume volume;
    [XmlArrayItem(Type = typeof(Gp4File), ElementName = "file")]
    [XmlArray(ElementName = "files")]
    public Files files;
    [XmlArrayItem(Type = typeof(Dir), ElementName = "dir")]
    [XmlArray(ElementName = "rootdir")]
    public List<Dir> RootDir = new List<Dir>();

    public static void WriteTo(Gp4Project proj, System.IO.Stream s)
    {
      XmlSerializer mySerializer = new XmlSerializer(typeof(Gp4Project));
      mySerializer.Serialize(s, proj);
    }

    public static Gp4Project ReadFrom(System.IO.Stream s)
    {
      XmlSerializer mySerializer = new XmlSerializer(typeof(Gp4Project));
      var proj = (Gp4Project)mySerializer.Deserialize(s);
      // Fixup dir tree
      void setParent(List<Dir> dirs, Dir parent)
      {
        foreach(var dir in dirs)
        {
          dir.Parent = parent;
          setParent(dir.Children, dir);
        }
      }
      setParent(proj.RootDir, null);
      foreach(var file in proj.files)
      {
        if(file.OrigPath == null)
        {
          file.OrigPath = file.TargetPath.Replace('/','\\');
        }
      }
      return proj;
    }

    public void RenameFile(Gp4File f, string newName)
    {
      f.TargetPath = f.DirName + newName;
    }

    public void RenameDir(Dir d, string newName)
    {
      var origPath = d.Path;
      d.TargetName = newName;
      var newPath = d.Path;
      foreach (var file in files)
      {
        if (file.TargetPath.StartsWith(origPath))
        {
          file.TargetPath = newPath + file.FileName;
        }
      }
    }

    /// <summary>
    /// Deletes the given file from this project.
    /// </summary>
    public void DeleteFile(Gp4File f)
    {
      files.Remove(f);
    }

    /// <summary>
    /// Deletes the directory and all files and subdirectories.
    /// </summary>
    public void DeleteDir(Dir d)
    {
      var path = d.Path;
      var deleteQueue = new List<Gp4File>();
      // This covers all children files, too.
      foreach (var f in files)
      {
        if (f.TargetPath.StartsWith(path))
          deleteQueue.Add(f);
      }
      foreach (var f in deleteQueue)
      {
        files.Remove(f);
      }
      RootDir.Remove(d);
      DeleteDirs(d);
    }

    public Dir AddDir(Dir parent, string name)
    {
      var newDir = new Dir
      {
        TargetName = name,
        Parent = parent,
        Children = new List<Dir>(),
      };
      (parent?.Children ?? RootDir).Add(newDir);
      return newDir;
    }

    /// <summary>
    /// Unlinks all directories in the given directory's subtree.
    /// </summary>
    private static void DeleteDirs(Dir dir)
    {
      dir.Parent?.Children.Remove(dir);
      dir.Parent = null;
      // Work on a copy of the children so we can modify the original list
      foreach (var d2 in dir.Children.ToList())
        DeleteDirs(d2);
      dir.Children.Clear();
      dir.Children = null;
    }
  }

  public class Volume
  {
    [XmlElement(ElementName = "volume_type")]
    public string Type;
    [XmlElement(ElementName = "volume_id")]
    public string Id;
    [XmlElement(ElementName = "volume_ts")]
    public string TimeStamp;
    [XmlElement(ElementName = "package")]
    public PackageInfo Package;
  }

  public enum VolumeType
  {
    bd25,
    bd50,
    bd50_50,
    bd50_25,
    bd25_50,
    exfat,
    pfs_plain,
    pfs_signed,
    pfs_nested,
    pkg_ps4_app,
    pkg_ps4_patch,
    pkg_ps4_remaster,
    pkg_ps4_ac_data,
    pkg_ps4_ac_nodata,
    pkg_ps4_sf_theme,
    pkg_ps4_theme,
  }

  public static class VolumeTypeUtil
  {
    public static VolumeType OfString(string s)
    {
      switch (s)
      {
        case "bd25": return VolumeType.bd25;
        case "bd50": return VolumeType.bd50;
        case "bd50_50": return VolumeType.bd50_50;
        case "bd50_25": return VolumeType.bd50_25;
        case "bd25_50": return VolumeType.bd25_50;
        case "exfat": return VolumeType.exfat;
        case "pfs_plain": return VolumeType.pfs_plain;
        case "pfs_signed": return VolumeType.pfs_signed;
        case "pfs_nested": return VolumeType.pfs_nested;
        case "pkg_ps4_app": return VolumeType.pkg_ps4_app;
        case "pkg_ps4_patch": return VolumeType.pkg_ps4_patch;
        case "pkg_ps4_remaster": return VolumeType.pkg_ps4_remaster;
        case "pkg_ps4_ac_data": return VolumeType.pkg_ps4_ac_data;
        case "pkg_ps4_ac_nodata": return VolumeType.pkg_ps4_ac_nodata;
        case "pkg_ps4_sf_theme": return VolumeType.pkg_ps4_sf_theme;
        case "pkg_ps4_theme": return VolumeType.pkg_ps4_theme;
        default: throw new Exception("Unknown Volume Type: " + s);
      }
    }
  }

  public class PackageInfo
  {
    [XmlAttribute("content_id")]
    public string ContentId;
    [XmlAttribute("passcode")]
    public string Passcode;
    [XmlAttribute("entitlement_key")]
    public string EntitlementKey;
    [XmlAttribute("c_date")]
    public string CreationDate;
  }

  public class Files : ICollection<Gp4File>
  {
    [XmlAttribute("img_no")]
    public int ImageNum;
    [XmlIgnore]
    public List<Gp4File> Items = new List<Gp4File>();
    [XmlIgnore]
    public int Count => Items.Count;
    [XmlIgnore]
    public bool IsReadOnly => false;
    public void Add(Gp4File item) => Items.Add(item);
    public void Clear() => Items.Clear();
    public bool Contains(Gp4File item) => Items.Contains(item);
    public void CopyTo(Gp4File[] array, int arrayIndex) => Items.CopyTo(array, arrayIndex);
    public bool Remove(Gp4File item) => Items.Remove(item);
    IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();
    public IEnumerator<Gp4File> GetEnumerator()
    {
      foreach (var item in Items) yield return item;
    }

  }

  public class Gp4File
  {
    [XmlAttribute("targ_path")]
    public string TargetPath;
    [XmlAttribute("orig_path")]
    public string OrigPath;
    public string FileName => TargetPath.Substring(TargetPath.LastIndexOf('/') + 1);
    public string DirName => TargetPath.Substring(0, TargetPath.LastIndexOf('/') + 1);
  }

  public class Dir
  {
    [XmlAttribute("targ_name")]
    public string TargetName;
    [XmlElement(ElementName = "dir")]
    public List<Dir> Children = new List<Dir>();
    [XmlIgnore]
    public Dir Parent;
    public string Path
    {
      get
      {
        var prefix = "";
        var dir = this;
        while (dir != null)
        {
          prefix = dir.TargetName + "/" + prefix;
          dir = dir.Parent;
        }
        return prefix;
      }
    }
  }

}
