using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LibOrbisPkg.PKG;
using LibOrbisPkg.Util;

namespace LibOrbisPkg.Rif
{
  class LicenseInfo
  {
    public LicenseInfo(string contentId, ContentType type)
    {
      ContentId = contentId;
      ContentType = type;
      Unknown_40 = ContentType == ContentType.AL ? 1 : 0;
      Unknown_48 = 0;
      Unknown_4C = 1;
    }
    public string ContentId;
    public byte[] Unknown_30 = new byte[16];
    public int Unknown_40;
    public ContentType ContentType = ContentType.AC;
    public int Unknown_48;
    public int Unknown_4C;
  }

  class LicenseInfoWriter : WriterBase
  {
    public LicenseInfoWriter(Stream stream) : base(true, stream) { }

    public void Write(LicenseInfo dat)
    {
      Write(Encoding.ASCII.GetBytes(dat.ContentId));
      Write(new byte[12]);
      Write(dat.Unknown_30);
      Write(dat.Unknown_40);
      Write((int)dat.ContentType);
      Write(dat.Unknown_48);
      Write(dat.Unknown_4C);
    }
  }
}
