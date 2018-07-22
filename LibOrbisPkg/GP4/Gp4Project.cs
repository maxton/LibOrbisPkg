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
    [XmlArrayItem(Type = typeof(File), ElementName = "file")]
    [XmlArray(ElementName = "files")]
    public Files files;
    [XmlElement(ElementName = "rootdir")]
    public List<Dir> RootDir = new List<Dir>();

    public static void WriteTo(Gp4Project proj, System.IO.Stream s)
    {
      XmlSerializer mySerializer = new XmlSerializer(typeof(Gp4Project));
      mySerializer.Serialize(s, proj);
    }

    public static Gp4Project ReadFrom(System.IO.Stream s)
    {
      XmlSerializer mySerializer = new XmlSerializer(typeof(Gp4Project));
      return (Gp4Project)mySerializer.Deserialize(s);
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
    pkg_ps4_ac_data,
  }

  public class PackageInfo
  {
    [XmlAttribute("content_id")]
    public string ContentId;
    [XmlAttribute("passcode")]
    public string Passcode;
  }

  public class Files : ICollection<File>
  {
    [XmlAttribute("img_no")]
    public int ImageNum;
    [XmlIgnore]
    public List<File> Items = new List<File>();
    [XmlIgnore]
    public int Count => Items.Count;
    [XmlIgnore]
    public bool IsReadOnly => false;
    public void Add(File item) => Items.Add(item);
    public void Clear() => Items.Clear();
    public bool Contains(File item) => Items.Contains(item);
    public void CopyTo(File[] array, int arrayIndex) => Items.CopyTo(array, arrayIndex);
    public bool Remove(File item) => Items.Remove(item);
    IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();
    public IEnumerator<File> GetEnumerator()
    {
      foreach (var item in Items) yield return item;
    }

  }

  public class File
  {
    [XmlAttribute("targ_path")]
    public string TargetPath;
    [XmlAttribute("orig_path")]
    public string OrigPath;
    public string FileName => TargetPath.Substring(TargetPath.LastIndexOf('/') + 1);
  }

  public class Dir
  {
    [XmlAttribute("targ_name")]
    public string TargetName;
    [XmlElement(ElementName = "dir")]
    public List<Dir> Items = new List<Dir>();
  }

}
