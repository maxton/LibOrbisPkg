using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LibOrbisPkg.PKG;
using LibOrbisPkg.Util;

namespace LibOrbisPkg.Rif
{
  class LicenseDat
  {
    public short Version = 1;
    public ulong PsnAccountId = 0;
    public long StartTime;
    public long EndTime;
    public string ContentId;
    public short LicenseType = 0x200;
    public DrmType DrmType = DrmType.PS4;
    public ContentType ContentType = ContentType.AC;
    public short SkuFlag = 0;
    public int Flags = 0;
    public int Unk_5C = 0;
    public int Unk_60 = 0;
    public int Unk_64 = 1;
    public int Unk_Flag = 0;
    public byte[] DiscKey = new byte[32];
    public byte[] SecretIv;
    public byte[] Secret;
    public byte[] Signature;
  }

  class LicenseDatWriter : WriterBase
  {
    public LicenseDatWriter(Stream stream) : base(true, stream) { }

    public void Write(LicenseDat dat)
    {
      Write(0x52494600); // "RIF\0";
      Write(dat.Version);
      Write((short)-1);
      Write(dat.PsnAccountId);
      Write(dat.StartTime);
      Write(dat.EndTime);
      Write(Encoding.ASCII.GetBytes(dat.ContentId));
      Write(new byte[12]);
      Write(dat.LicenseType);
      Write((short)dat.DrmType);
      Write((short)dat.ContentType);
      Write(dat.SkuFlag);
      Write(dat.Flags);
      Write(dat.Unk_5C);
      Write(dat.Unk_60);
      Write(dat.Unk_64);
      Write(dat.Unk_Flag);
      s.Position += 468;
      Write(dat.DiscKey);
      Write(dat.SecretIv);
      Write(dat.Secret);
      Write(dat.Signature);
    }
  }
}
