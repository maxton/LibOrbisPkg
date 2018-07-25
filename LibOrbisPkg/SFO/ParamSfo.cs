using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LibOrbisPkg.SFO
{
  public class ParamSfo
  {
    public static byte[] Update(Stream s)
    {
      // TODO: Add pubtools info to SFO
      var ret = new byte[s.Length + 572];
      s.Read(ret, 0, (int)s.Length);
      return ret;
    }
  }
}
