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

namespace PkgEditor
{
  public partial class MainWin : Form
  {
    public MainWin()
    {
      InitializeComponent();
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
      var pkg = GameArchives.Util.LocalFile(filename);
      OpenTab(new Views.PkgView(pkg), Path.GetFileName(filename));
    }

    private void openSfo(string filename)
    {
      using (var fs = File.OpenRead(filename))
      {
        var sfo = LibOrbisPkg.SFO.ParamSfo.FromStream(fs);
        OpenTab(new Views.SFOView(sfo, false, filename), Path.GetFileName(filename));
      }
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

    private void openToolStripMenuItem_Click(object sender, EventArgs e)
    {
      ShowOpenFileDialog("Open a GP4, PKG, SFO", "All Supported|*.gp4;*.pkg;*.sfo|GP4 Projects (*.gp4)|*.gp4|PKG Files (*.pkg)|*.pkg|SFO Files (*.sfo)|*.sfo", f =>
      {
        switch (f.Split('.').Last().ToLowerInvariant())
        {
          case "gp4":
            openGp4(f);
            break;
          case "pkg":
            openPkg(f);
            break;
          case "sfo":
            openSfo(f);
            break;
        }
      });
    }

    private void closeToolStripMenuItem_Click(object sender, EventArgs e)
    {
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
  }
}
