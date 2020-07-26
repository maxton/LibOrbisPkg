using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using LibOrbisPkg;
using System.Windows.Forms;
using System.Diagnostics;
using LibOrbisPkg.PKG;
using LibOrbisPkg.PFS;

namespace PkgEditor
{
  public partial class MainWin : Form
  {
    public MainWin()
    {
      InitializeComponent();
      var args = Environment.GetCommandLineArgs();
      if(args.Length > 1 && File.Exists(args[1]))
      {
        openFile(args[1]);
      }
    }

    private void ShowOpenFileDialog(string title, string filetypes, Action<string> successCb)
    {
      var ofd = new OpenFileDialog();
      ofd.Title = title;
      ofd.Filter = filetypes;
      if(ofd.ShowDialog() == DialogResult.OK)
      {
        successCb(ofd.FileName);
      }
    }

    private void openGp4(string filename)
    {
      using (var fs = File.OpenRead(filename))
      {
        var proj = LibOrbisPkg.GP4.Gp4Project.ReadFrom(fs);
        OpenTab(new Views.GP4View(proj, filename), Path.GetFileName(filename));
      }
    }

    private void openPkg(string filename)
    {
      OpenTab(new Views.PkgView(filename), Path.GetFileName(filename));
    }

    private void openSfo(string filename)
    {
      using (var fs = File.OpenRead(filename))
      {
        var sfo = LibOrbisPkg.SFO.ParamSfo.FromStream(fs);
        OpenTab(new Views.SFOView(sfo, false, filename), Path.GetFileName(filename));
      }
    }

    private void openPFS(string filename)
    {
      OpenTab(new Views.PFSView(filename), Path.GetFileName(filename));
    }

    public void OpenTab(Views.View c, string name)
    {
      var x = new TabPage(name);
      c.Name = name;
      c.mainWin = this;
      x.Controls.Add(c);
      //c.SetBrowser(this);
      c.SaveStatusChanged += UpdateSaveButtons;
      c.Dock = DockStyle.Fill;
      tabs.TabPages.Add(x);
      tabs.SelectedTab = x;
      UpdateSaveButtons();
      closeToolStripMenuItem.Enabled = true;
    }
    
    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Application.Exit();
    }

    private void openFile(string file)
    {
      switch (file.Split('.').Last().ToLowerInvariant())
      {
        case "gp4":
          openGp4(file);
          break;
        case "pkg":
          openPkg(file);
          break;
        case "sfo":
          openSfo(file);
          break;
        case "pfs":
        case "dat":
          openPFS(file);
          break;
      }
    }

    private void openToolStripMenuItem_Click(object sender, EventArgs e)
    {
      ShowOpenFileDialog(
        "Open a GP4, PKG, SFO",
        "All Supported|*.gp4;*.pkg;*.sfo;*.pfs;*.dat"
        + "|GP4 Projects (*.gp4)|*.gp4"
        + "|PKG Files (*.pkg)|*.pkg"
        + "|SFO Files (*.sfo)|*.sfo"
        + "|PFS Images (*.pfs, *.dat)|*.pfs;*.dat",
        f =>
        {
          openFile(f);
        });
    }

    private void closeToolStripMenuItem_Click(object sender, EventArgs e)
    {
      CurrentView?.Close();
      tabs.TabPages.Remove(tabs.SelectedTab);
      UpdateSaveButtons();
      if (tabs.TabPages.Count == 0)
      {
        closeToolStripMenuItem.Enabled = false;
      }
    }

    private Views.View CurrentView => tabs.SelectedTab?.Controls[0] as Views.View;
    private void UpdateSaveButtons(object sender = null, EventArgs e = null)
    {
      var v = CurrentView;
      saveToolStripMenuItem.Enabled = v != null && v.CanSave;
      saveAsToolStripMenuItem.Enabled = v != null && v.CanSaveAs;
    }

    private void saveToolStripMenuItem_Click(object sender, EventArgs e)
    {
      CurrentView?.Save();
    }

    private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      CurrentView?.SaveAs();
    }

    private void newGp4ProjectMenuItem_Click(object sender, EventArgs e)
    {
      var proj = LibOrbisPkg.GP4.Gp4Project.Create(LibOrbisPkg.GP4.VolumeType.pkg_ps4_ac_data);
      var sfd = new SaveFileDialog()
      {
        Title = "Choose project location...",
        Filter = "GP4 Projects|*.gp4",
      };
      if (sfd.ShowDialog() == DialogResult.OK)
      {
        var view = new Views.GP4View(proj, sfd.FileName);
        OpenTab(view, "*" + Path.GetFileName(sfd.FileName));
        view.Modified = true;
      }
    }

    private void sfoFileToolStripMenuItem_Click(object sender, EventArgs e)
    {
      var sfo = new LibOrbisPkg.SFO.ParamSfo();
      var view = new Views.SFOView(sfo);
      OpenTab(view, "New file");
    }

    private void MainWin_Drop(object sender, DragEventArgs e)
    {
      if (e.Data.GetDataPresent(DataFormats.FileDrop))
      {
        string[] files = (string[])(e.Data.GetData(DataFormats.FileDrop, false));
        foreach (string file in files)
        {
          openFile(Path.GetFullPath(file).ToString());
        }
      }
    }

    private void MainWin_DragEnter(object sender, DragEventArgs e)
    {
      e.Effect = DragDropEffects.Copy;
    }

    private async void CombinePKGPartsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      using (var ofd = new OpenFileDialog() { Title = "Select part 0", Filter = "PKG Files (*_0.pkg)|*_0.pkg" })
      {
        if (ofd.ShowDialog() != DialogResult.OK)
          return;

        var filenames = new List<string>();
        filenames.Add(ofd.FileName);
        ulong pkgSize = 0;
        long remainingSize = 0;
        using (var s = File.OpenRead(filenames[0]))
        {
          var hdr = new LibOrbisPkg.PKG.PkgReader(s).ReadHeader();
          pkgSize = hdr.package_size;
          remainingSize = (long)hdr.package_size - s.Length;
        }
        if(remainingSize <= 0)
        {
          MessageBox.Show("Error: reported package size was less than part file size");
          return;
        }

        // remove the _0.pkg (6 characters) from the filename
        var baseFilename = filenames[0].Substring(0, filenames[0].Length - 6);
        var targetFilename = baseFilename + ".pkg";
        using (var sfd = new SaveFileDialog()
        {
          Title = "Select output file",
          Filter = "PKG Files (*.pkg)|*.pkg",
          FileName = targetFilename
        })
        {
          if (sfd.ShowDialog() != DialogResult.OK)
            return;
          targetFilename = sfd.FileName;
        }
        var i = 0;
        while (remainingSize > 0)
        {
          var newFile = $"{baseFilename}_{++i}.pkg";
          if (File.Exists(newFile))
          {
            filenames.Add(newFile);
            remainingSize -= new FileInfo(newFile).Length;
          }
          else
          {
            MessageBox.Show($"Error: missing part {i}, should be {remainingSize} bytes.");
            return;
          }
        }

        using (var logWindow = new LogWindow())
        using (var fo = File.Create(targetFilename))
        {
          fo.SetLength((long)pkgSize);
          logWindow.StartPosition = this.StartPosition;
          logWindow.Show(this);
          logWindow.GetWriter().WriteLine($"Merging files to {targetFilename}...");
          foreach (var fn in filenames)
          {
            using (var fi = File.OpenRead(fn))
            {
              logWindow.GetWriter().WriteLine($"Copying {fn}");
              await fi.CopyToAsync(fo);
            }
          }
          logWindow.GetWriter().WriteLine("Done. Saved to "+targetFilename);
        }
      }
    }

    private void cryptoDebuggerToolStripMenuItem_Click(object sender, EventArgs e)
    {
      OpenTab(new Views.CryptoDebug(), "Crypto Debugger");
    }

    private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
      var assembly = System.Reflection.Assembly.GetExecutingAssembly();
      var version = FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
      var libAssembly = System.Reflection.Assembly.GetAssembly(typeof(LibOrbisPkg.PKG.Pkg));
      var libVersion = FileVersionInfo.GetVersionInfo(libAssembly.Location).FileVersion;
      MessageBox.Show(this, 
        "PkgEditor (c) 2020 Maxton" + Environment.NewLine +
        "LibOrbisPkg version "+ libVersion + Environment.NewLine +
        "PkgEditor version " + version,
        "About PkgEditor");
    }

    private void visitGitHubRepoToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Process.Start("https://github.com/maxton/LibOrbisPkg");
    }

    private void setPKGPFSFileMetadataToolStripMenuItem_Click(object sender, EventArgs e)
    {
      using (var ofd = new OpenFileDialog() {
        Title = "Select files",
        Filter = "PKG/PFS Files (*.pkg, *.dat)|*.pkg;*.dat",
        Multiselect = true
      })
      {
        if (ofd.ShowDialog() != DialogResult.OK)
          return;
        foreach(var filename in ofd.FileNames)
        {
          DateTime creationTime = File.GetCreationTimeUtc(filename);
          DateTime modifiedTime = File.GetLastWriteTime(filename);
          using (var f = File.OpenRead(filename))
          {
            if (filename.ToLowerInvariant().EndsWith(".dat"))
            {
              var header = PfsHeader.ReadFromStream(f);
              creationTime = modifiedTime =
                new DateTime(1970, 1, 1)
                .AddSeconds(header.InodeBlockSig.Time1_sec);
            }
            else if (filename.ToLowerInvariant().EndsWith(".pkg"))
            {
              var pkgHeader = new PkgReader(f).ReadHeader();
              f.Position = (long)pkgHeader.pfs_image_offset;
              var header = PfsHeader.ReadFromStream(f);
              creationTime = modifiedTime =
                new DateTime(1970, 1, 1)
                .AddSeconds(header.InodeBlockSig.Time1_sec);
            }
          }
          File.SetCreationTimeUtc(filename, creationTime);
          File.SetLastWriteTimeUtc(filename, modifiedTime);
        }
      }
    }
  }
}
