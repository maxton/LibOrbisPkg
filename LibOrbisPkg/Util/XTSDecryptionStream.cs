using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace LibOrbisPkg
{
  public static class Bytes
  {
    public static byte[] Key1 = new byte[] { 0x21, 0x98, 0xee, 0xd2, 0xe7, 0x9c, 0x70, 0x56, 0xff, 0xb0, 0x0f, 0xfa, 0x52, 0x1e, 0x73, 0xcb };
    public static byte[] Key2 = new byte[] { 0xd8, 0x07, 0x0b, 0x72, 0x30, 0xc7, 0xd7, 0x4a, 0x45, 0x4c, 0xf7, 0x4e, 0x4c, 0x01, 0x14, 0xf8 };
    public static byte[] TestSector = new byte[] { 0xB7, 0x1F, 0x16, 0xBA, 0x0A, 0xA7, 0xFF, 0x21, 0xB8, 0x78, 0x63, 0xF0, 0x69, 0x7E, 0x3B, 0xCE, };
    public static void PrintHex(this byte[] b)
    {
      for(var x = 0; x < b.Length; x++)
      {
        Console.Write("{0:X2} ", b[x]);
        if ((x + 1) % 16 == 0) Console.WriteLine();
      }
    }
  }
}

namespace LibOrbisPkg.Util
{
  public class XtsTest
  {
    // Used on the plaintext XORed with the encrypted sector number
    private SymmetricAlgorithm cipher;
    // Used to encrypt the tweak
    private SymmetricAlgorithm tweakCipher;

    private byte[] tweak = new byte[16];
    private byte[] xor = new byte[16];
    private byte[] tmp = new byte[16];

    public XtsTest(byte[] key1, byte[] key2)
    {
      cipher = new AesManaged
      {
        Mode = CipherMode.ECB,
        KeySize = 128,
        Key = key1,
        Padding = PaddingMode.None,
        BlockSize = 128,
      };
      tweakCipher = new AesManaged
      {
        Mode = CipherMode.ECB,
        KeySize = 128,
        Key = key2,
        Padding = PaddingMode.None,
        BlockSize = 128,
      };
    }

    public byte[] DecryptSector(byte[] sector, ulong sectorNum)
    {
      // Reset tweak to sector number
      unsafe
      {
        fixed(byte* t = &tweak[0])
        {
          *(ulong*)t = sectorNum;
          *(ulong*)(t + 8) = 0UL;
        }
      }
      byte[] ret = new byte[16];
      using (var tweakEncryptor = tweakCipher.CreateEncryptor())
      using (var decryptor = cipher.CreateDecryptor())
      {
        tweak.PrintHex();
        // TODO: Implement Galois field multiplication of 2^128 so we can decrypt the whole sector
        int plaintextOffset = 0;
        tweakEncryptor.TransformBlock(tweak, 0, 16, tmp, 0);
        tmp.PrintHex();
        sector.PrintHex();
        for (var x = 0; x < 16; x++)
        {
          ret[x] = (byte)(sector[x] ^ tmp[x]);
        }
        ret.PrintHex();
        decryptor.TransformBlock(sector, plaintextOffset, 16, xor, 0);
        xor.PrintHex();
        for (var x = 0; x < 16; x++)
        {
          ret[x + plaintextOffset] ^= xor[x];
        }
      }
      return ret;
    }
  }
  class XTSDecryptionStream : Stream
  {
    public override bool CanRead => throw new NotImplementedException();

    public override bool CanSeek => throw new NotImplementedException();

    public override bool CanWrite => throw new NotImplementedException();

    public override long Length => throw new NotImplementedException();

    public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override void Flush()
    {
      throw new NotImplementedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      throw new NotImplementedException();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      throw new NotImplementedException();
    }

    public override void SetLength(long value)
    {
      throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      throw new NotImplementedException();
    }
  }
}
