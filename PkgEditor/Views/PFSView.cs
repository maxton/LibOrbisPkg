using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibOrbisPkg.PFS;
using System.IO.MemoryMappedFiles;

namespace PkgEditor.Views
{
  public partial class PFSView : View
  {
    private MemoryMappedFile pfsFile;
    private MemoryMappedViewAccessor va;
    private PfsReader reader;
    public PFSView(string filename)
    {
      InitializeComponent();
      pfsFile = MemoryMappedFile.CreateFromFile(filename, System.IO.FileMode.Open, mapName: null, 0, MemoryMappedFileAccess.Read);
      va = pfsFile.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);
      va.Read(0, out int val);
      if (val == PFSCReader.Magic)
        reader = new PfsReader(new PFSCReader(va));
      else
        reader = new PfsReader(va);
      fileView1.AddRoot(reader, filename);
    }

    public override void Close()
    {
      va.Dispose();
      pfsFile.Dispose();
      base.Close();
    }
  }
}
