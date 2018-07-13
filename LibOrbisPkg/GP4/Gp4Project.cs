using System;
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
    [XmlElement(ElementName = "files")]
    public Files files;
    [XmlElement(ElementName = "rootdir")]
    public Dir[] RootDir;

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
    public VolumeType Type;
    [XmlElement(ElementName = "volume_id")]
    public string Id;
    [XmlElement(ElementName = "volume_ts")]
    public DateTime TimeStamp;
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

  public class Files : List<File>
  {
    [XmlAttribute("img_no")]
    public int ImageNum;
  }

  public class File
  {
    [XmlAttribute("targ_path")]
    public string TargetPath;
    [XmlAttribute("orig_path")]
    public string OrigPath;
  }

  public class Dir : List<Dir>
  {
    [XmlAttribute("targ_name")]
    public string TargetName;
  }

}
