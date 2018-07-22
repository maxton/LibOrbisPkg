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
        OpenTab(new Views.GP4View(proj), Path.GetFileName(filename));
      }
    }

    private void openPkg(string filename)
    {
      using (var fs = File.OpenRead(filename))
      {
        var pkg = new LibOrbisPkg.PKG.PkgReader(fs).ReadHeader();
        OpenTab(new Views.ObjectView(pkg), Path.GetFileName(filename));
      }
    }

    public void OpenTab(UserControl c, string name)
    {
      var x = new TabPage(name);
      c.Name = name;
      x.Controls.Add(c);
      //c.SetBrowser(this);
      c.Dock = DockStyle.Fill;
      tabs.TabPages.Add(x);
      tabs.SelectedTab = x;
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
    }
  }
}
