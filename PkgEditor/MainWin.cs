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

    public void OpenTab(Views.View c, string name)
    {
      var x = new TabPage(name);
      c.Name = name;
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

    private void openGP4ToolStripMenuItem_Click(object sender, EventArgs e)
    {
      ShowOpenFileDialog("Open GP4 Project", "GP4 Projects|*.gp4", openGp4);
    }

    private void openPKGToolStripMenuItem_Click(object sender, EventArgs e)
    {
      ShowOpenFileDialog("Open PKG Package", "PKG|*.pkg", openPkg);
    }

    private void closeToolStripMenuItem_Click(object sender, EventArgs e)
    {
      tabs.TabPages.Remove(tabs.SelectedTab);
      if(tabs.TabPages.Count == 0)
      {
        closeToolStripMenuItem.Enabled = false;
      }
    }

    private Views.View CurrentView => tabs.SelectedTab?.Controls[0] as Views.View;
    private void UpdateSaveButtons(object sender = null, EventArgs e = null)
    {
      var v = CurrentView;
      if (v == null) return;
      saveToolStripMenuItem.Enabled = v.CanSave;
      saveAsToolStripMenuItem.Enabled = v.CanSaveAs;
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
      var proj = new LibOrbisPkg.GP4.Gp4Project
      {
        files = new LibOrbisPkg.GP4.Files(),
        Format = "gp4",
        RootDir = new List<LibOrbisPkg.GP4.Dir>(),
        version = 1000,
        volume = new LibOrbisPkg.GP4.Volume
        {
          Type = "pkg_ps4_ac_data",
          TimeStamp = DateTime.UtcNow.ToString("s").Replace('T', ' '),
          Package = new LibOrbisPkg.GP4.PackageInfo
          {
            ContentId = "XXXXXX-CUSA00000_00-ZZZZZZZZZZZZZZZZ",
            Passcode = "00000000000000000000000000000000"
          }
        }
      };
      var sfd = new SaveFileDialog()
      {
        Title = "Choose project location...",
        Filter = "GP4 Projects|*.gp4",
      };
      if(sfd.ShowDialog() == DialogResult.OK)
      {
        var view = new Views.GP4View(proj, sfd.FileName);
        OpenTab(view, "*" + Path.GetFileName(sfd.FileName));
        view.Modified = true;
      }
    }
  }
}
