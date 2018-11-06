using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LibOrbisPkg.Util
{
  /// <summary>
  /// A stream that transparently encrypts and decrypts using XEX-mode AES.
  /// This stream is buffered, so be sure to Flush() after you write
  /// </summary>
  public class XtsCryptStream : Stream
  {
    // Used on the plaintext XORed with the encrypted sector number
    private SymmetricAlgorithm cipher;
    // Used to encrypt the tweak
    private SymmetricAlgorithm tweakCipher;

    private byte[] tweak = new byte[16];
    private byte[] xor = new byte[16];
    private byte[] xor2 = new byte[16];
    private byte[] encryptedTweak = new byte[16];
    /// <summary>
    /// Size of each XEX sector
    /// </summary>
    private uint sectorSize;
    /// <summary>
    /// Offset within the sector for Read()s
    /// Should always be == position % sectorSize
    /// </summary>
    private int offsetIntoSector;
    /// <summary>
    /// Active sector number
    /// </summary>
    private ulong activeSector;
    /// <summary>
    /// Sector at and after which the encryption is active
    /// </summary>
    private uint cryptStartSector;
    /// <summary>
    /// Position within logical stream.
    /// </summary>
    private long position;
    /// <summary>
    /// Temporary location for the decrypted sector
    /// </summary>
    private byte[] sectorBuf;
    private byte[] writeBuf;
    private Stream stream;
    private bool dirty;

    /// <summary>
    /// Creates an AES-XTS-128 stream.
    /// Reads from the stream will decrypt data. Writes to the stream will encrypt data.
    /// </summary>
    public XtsCryptStream(Stream s, byte[] dataKey, byte[] tweakKey, uint startSector = 16, uint sectorSize = 0x1000)
    {
      cipher = new AesManaged
      {
        Mode = CipherMode.ECB,
        KeySize = 128,
        Key = dataKey,
        Padding = PaddingMode.None,
        BlockSize = 128,
      };
      tweakCipher = new AesManaged
      {
        Mode = CipherMode.ECB,
        KeySize = 128,
        Key = tweakKey,
        Padding = PaddingMode.None,
        BlockSize = 128,
      };
      cryptStartSector = startSector;
      this.sectorSize = sectorSize;
      sectorBuf = new byte[sectorSize];
      writeBuf = new byte[sectorSize];
      stream = s;
      stream.Position = 0;
      position = 0;
      offsetIntoSector = 0;
      ReadSectorBuffer(0);
    }

    public override bool CanRead => stream.CanRead;

    public override bool CanSeek => stream.CanSeek;

    public override bool CanWrite => stream.CanWrite;

    public override long Length => stream.Length;

    public override long Position
    {
      get => position;
      set
      {
        ReadSectorBuffer((ulong)(value / sectorSize));
        offsetIntoSector = (int)(value - position);
        position = value;
      }
    }

    /// <summary>
    /// Encrypts or decrypts the given sector with XEX.
    /// </summary>
    /// <param name="sector">Sector plain/ciphertext</param>
    /// <param name="sectorNum">Sector index number</param>
    /// <param name="encrypt">If this is set to true, encrypt the sector</param>
    public void CryptSector(byte[] sector, ulong sectorNum, bool encrypt = false)
    {
      // Reset tweak to sector number
      Buffer.BlockCopy(BitConverter.GetBytes(sectorNum), 0, tweak, 0, 8);
      for (int x = 8; x < 16; x++)
        tweak[x] = 0;
      using (var tweakEncryptor = tweakCipher.CreateEncryptor())
      using (var cryptor = encrypt ? cipher.CreateEncryptor() : cipher.CreateDecryptor())
      {
        tweakEncryptor.TransformBlock(tweak, 0, 16, encryptedTweak, 0);
        for (int destOffset = 0; destOffset < sector.Length; destOffset += 16)
        {
          for (var x = 0; x < 16; x++)
          {
            xor[x] = (byte)(sector[x + destOffset] ^ encryptedTweak[x]);
          }
          cryptor.TransformBlock(xor, 0, 16, xor, 0);
          for (var x = 0; x < 16; x++)
          {
            sector[x + destOffset] = (byte)(xor[x] ^ encryptedTweak[x]);
          }
          // GF-Multiply Tweak
          int feedback = 0;
          for (int k = 0; k < 16; k++)
          {
            byte tmp = encryptedTweak[k];
            encryptedTweak[k] = (byte)(2 * encryptedTweak[k] | feedback);
            feedback = (tmp & 0x80) >> 7;
          }
          if (feedback != 0)
            encryptedTweak[0] ^= 0x87;
        }
      }
    }

    public override void Flush()
    {
      if (!dirty) return;
      byte[] sector = sectorBuf;
      if (activeSector >= cryptStartSector)
      {
        Buffer.BlockCopy(sectorBuf, 0, writeBuf, 0, sectorBuf.Length);
        CryptSector(writeBuf, activeSector, true);
        sector = writeBuf;
      }
      stream.Position = (long)activeSector * sectorSize;
      stream.Write(sector, 0, sector.Length);
      stream.Flush();
      dirty = false;
    }

    /// <summary>
    /// Postconditions:
    /// - Stream is flushed
    /// - activeSector is set to the sector number
    /// - offsetIntoSector is reset to 0
    /// - sectorBuf[] is filled with decrypted sector
    /// - position is updated
    /// </summary>
    private void ReadSectorBuffer(ulong newActiveSector)
    {
      Flush();
      position = sectorSize * (long)newActiveSector;
      stream.Position = position;
      stream.Read(sectorBuf, 0, (int)sectorSize);
      if (newActiveSector >= cryptStartSector)
        CryptSector(sectorBuf, newActiveSector);
      activeSector = newActiveSector;
      offsetIntoSector = 0;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      int totalRead = 0;
      while (count > 0 && position < stream.Length)
      {
        if (offsetIntoSector >= sectorSize)
        {
          ReadSectorBuffer(activeSector + 1);
        }
        int bufferedRead = Math.Min((int)sectorSize - offsetIntoSector, count);
        Buffer.BlockCopy(sectorBuf, offsetIntoSector, buffer, offset, bufferedRead);
        count -= bufferedRead;
        offset += bufferedRead;
        totalRead += bufferedRead;
        offsetIntoSector += bufferedRead;
        position += bufferedRead;
      }
      return totalRead;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      switch (origin)
      {
        case SeekOrigin.Begin:
          Position = offset;
          break;
        case SeekOrigin.Current:
          Position += offset;
          break;
        case SeekOrigin.End:
          Position = Length + offset;
          break;
      }
      return position;
    }

    public override void SetLength(long value) => stream.SetLength(value);

    public override void Write(byte[] buffer, int offset, int count)
    {
      if (count <= 0)
        return;
      int totalWritten = 0;
      if (stream.Position + count > stream.Length)
      {
        var newLength = stream.Length + count;
        newLength = newLength + sectorSize - (newLength % sectorSize);
        stream.SetLength(newLength);
      }

      while (count > 0 && position < stream.Length)
      {
        if (offsetIntoSector >= sectorSize)
        {
          ReadSectorBuffer(activeSector + 1);
        }
        int bufferedWrite = Math.Min((int)sectorSize - offsetIntoSector, count);
        Buffer.BlockCopy(buffer, 0, sectorBuf, offsetIntoSector, bufferedWrite);
        dirty = true;
        count -= bufferedWrite;
        offset += bufferedWrite;
        totalWritten += bufferedWrite;
        offsetIntoSector += bufferedWrite;
        position += bufferedWrite;
      }
    }
  }
}
