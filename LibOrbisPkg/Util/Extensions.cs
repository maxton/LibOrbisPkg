using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibOrbisPkg.Util
{
  public static class DictionaryExtensions
  {
    public static V GetOrDefault<K,V>(this Dictionary<K,V> d, K key, V def = default(V))
    {
      if (d.ContainsKey(key)) return d[key];
      return def;
    }
  }
}
