using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LibOrbisPkg.Util
{
  public static class Crypto
  {
    /// <summary>
    /// From FPKG code:
    /// a common function to generate a final key for PFS
    /// </summary>
    public static byte[] PfsGenCryptoKey(byte[] ekpfs, byte[] seed, uint index)
    {
      byte[] d = new byte[4 + seed.Length];
      Array.Copy(BitConverter.GetBytes(index), d, 4);
      Array.Copy(seed, 0, d, 4, seed.Length);
      using (var hmac = new HMACSHA256(ekpfs))
      {
        return hmac.ComputeHash(d);
      }
    }

    /// <summary>
    /// From FPKG code:
    /// an encryption key generator based on EKPFS and PFS header seed
    /// </summary>
    public static byte[] PfsGenEncKey(byte[] ekpfs, byte[] seed)
    {
      return PfsGenCryptoKey(ekpfs, seed, 1);
    }

    /// <summary>
    /// From FPKG code:
    /// asigning key generator based on EKPFS and PFS header seed
    /// </summary>
    public static byte[] PfsGenSignKey(byte[] ekpfs, byte[] seed)
    {
      return PfsGenCryptoKey(ekpfs, seed, 2);
    }

    /// <summary>
    /// From FPKG Code (sceSblPfsSetKeys): Turns the EEKPfs to an EKPfs
    /// </summary>
    public static byte[] DecryptEEKPfs(byte[] eekpfs, RSAKeyset keyset)
    {
      var @params = new RSAParameters
      {
        D = keyset.PrivateExponent,
        DP = keyset.Exponent1,
        DQ = keyset.Exponent2,
        Exponent = keyset.PublicExponent,
        InverseQ = keyset.Coefficient,
        Modulus = keyset.Modulus,
        P = keyset.Prime1,
        Q = keyset.Prime2
      };
      using(var rsa = RSA.Create())
      {
        rsa.KeySize = 2048;
        rsa.ImportParameters(@params);
        return rsa.DecryptValue(eekpfs);
      }
    }

    // TODO
    public static int AesCbcCfb128Encrypt(byte[] @out, byte[] @in, int size, byte[] key, int key_size, byte[] iv)
    {

      return 0;
    }
    public static int AesCbcCfb128Decrypt(byte[] @out, byte[] @in, int size, byte[] key, int key_size, byte[] iv)
    {

      return 0;
    }

    /// <summary>
    /// Computes the SHA256 hash of the given data.
    /// </summary>
    public static byte[] Sha256(byte[] data) => SHA256.Create().ComputeHash(data);
    public static byte[] Sha256(Stream data)
    {
      data.Position = 0;
      return SHA256.Create().ComputeHash(data);
    }
    /// <summary>
    /// Computes the SHA256 hash of the data in the stream between (start) and (start+length)
    /// </summary>
    public static byte[] Sha256(Stream data, long start, long length)
    {
      var sha = SHA256.Create();
      using (var s = new SubStream(data, start, length))
      {
        s.Position = 0;
        return sha.ComputeHash(s);
      }
    }

    /// <summary>
    /// Computes keys for the package.
    /// The key is the result of a SHA256 hash of the concatenation of:
    ///  - The SHA256 hash of the index (4 bytes big-endian)
    ///  - The SHA256 hash of the Contend ID (36 bytes padded to 48 with nulls)
    ///  - The passcode
    /// The EKPFS is Index 1. 
    /// </summary>
    public static byte[] ComputeKeys(string ContentId, string Passcode, uint Index)
    {
      if (ContentId.Length != 36)
        throw new Exception("Content ID must be 36 characters long");
      if (Passcode.Length != 32)
        throw new Exception("Passcode must be 32 characters long");
            
      byte[] data = new byte[96];
      Buffer.BlockCopy(Sha256(BitConverter.GetBytes(Index).Reverse().ToArray()), 0, data, 0, 32);
      Buffer.BlockCopy(Sha256(Encoding.ASCII.GetBytes(ContentId.PadRight(48, '\0'))), 0, data, 32, 32);
      Buffer.BlockCopy(Encoding.ASCII.GetBytes(Passcode), 0, data, 64, 32);

      return Sha256(data);
    }

    public static string AsHexCompact(this byte[] k)
    {
      StringBuilder sb = new StringBuilder(k.Length * 2);
      foreach(var b in k)
      {
        sb.AppendFormat("{0:X2}", b);
      }
      return sb.ToString();
    }
  }
}
