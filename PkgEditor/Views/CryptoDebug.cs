using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.MemoryMappedFiles;
using LibOrbisPkg.Util;
using LibOrbisPkg.PFS;

namespace PkgEditor.Views
{
  public partial class CryptoDebug : View
  {
    public CryptoDebug()
    {
      InitializeComponent();
      pfsSeed.TextChanged += propagate_key;
      ekpfsInput.TextChanged += propagate_key;
      indexInput.TextChanged += propagate_key;

      dataKey.TextChanged += invalidateSector;
      tweakKey.TextChanged += invalidateSector;
      xtsSectorSize.TextChanged += invalidateSector;
      xtsStartSector.TextChanged += invalidateSector;
    }


    private void propagate_key(object sender, EventArgs e)
    {
      uint index = 1;
      uint.TryParse(indexInput.Text, out index);
      var keys = Crypto.PfsGenCryptoKey(ekpfsInput.Text.FromHexCompact(), pfsSeed.Text.FromHexCompact(), index);
      var data = new byte[16];
      var tweak = new byte[16];
      Buffer.BlockCopy(keys, 0, tweak, 0, 16);
      Buffer.BlockCopy(keys, 16, data, 0, 16);
      dataKey.Text = data.ToHexCompact();
      tweakKey.Text = tweak.ToHexCompact();
    }

    private MemoryMappedFile pfs;
    private IMemoryAccessor accessor;
    private IMemoryReader xtsReader;
    private PfsHeader header;
    void ClosePfs()
    {
      accessor?.Dispose();
      pfs?.Dispose();
      pfs = null;
      accessor = null;
    }

    void LoadPfs(string filename)
    {
      pfs = MemoryMappedFile.CreateFromFile(filename, System.IO.FileMode.Open, mapName: null, 0, MemoryMappedFileAccess.Read);
      accessor = new MemoryMappedViewAccessor_(pfs.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read), true);
      using (var s = pfs.CreateViewStream(0, 0x5A0, MemoryMappedFileAccess.Read))
      {
        header = PfsHeader.ReadFromStream(s);
        objectView1.ObjectPreview(header);
        pfsSeed.Text = header.Seed.ToHexCompact();
      }
      xtsReader = accessor;
      UpdateHexView();
    }

    void UpdateHexView()
    {
      if (xtsReader == null) return;
      var buf = new byte[header.BlockSize];
      xtsReader.Read(header.BlockSize, buf, 0, buf.Length);
      var sb = new StringBuilder();
      for (int i = 0; i < header.BlockSize; i++)
      {
        if (i != 0 && i % 16 == 0)
        {
          sb.AppendLine();
        }
        sb.AppendFormat("{0:X2} ", buf[i]);
      }
      sectorPreview.Text = sb.ToString();
    }

    void RedoXts()
    {
      if (accessor == null) return;
      dataKey.Text = dataKey.Text.PadRight(32, '0');
      tweakKey.Text = tweakKey.Text.PadRight(32, '0');
      var data = dataKey.Text.FromHexCompact();
      var tweak = tweakKey.Text.FromHexCompact();
      uint start, sectorSize;
      uint.TryParse(xtsStartSector.Text, out start);
      uint.TryParse(xtsSectorSize.Text, out sectorSize);
      xtsReader = new XtsDecryptReader(accessor, data, tweak, start, sectorSize);
      UpdateHexView();
    }

    private void button1_Click(object sender, EventArgs e)
    {
      ClosePfs();
      using (var ofd = new OpenFileDialog() { Title = "Open a PFS image" })
      {
        if (ofd.ShowDialog() == DialogResult.OK)
        {
          LoadPfs(ofd.FileName);
        }
      }
    }

    private void button2_Click(object sender, EventArgs e)
    {
      RedoXts();
      reloadButton.Enabled = false;
    }

    private void invalidateSector(object sender, EventArgs e)
    {
      reloadButton.Enabled = true;
    }
  }
}
