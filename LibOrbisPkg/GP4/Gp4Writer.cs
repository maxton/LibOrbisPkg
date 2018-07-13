using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace LibOrbisPkg.GP4
{
  class Gp4Writer
  {
    private Stream s;
    public Gp4Writer(Stream s)
    {
      this.s = s;
    }

    public void Write(Gp4Project proj)
    {
      XmlSerializer mySerializer = new XmlSerializer(typeof(Gp4Project));
      mySerializer.Serialize(s, proj);
    }
  }
}
