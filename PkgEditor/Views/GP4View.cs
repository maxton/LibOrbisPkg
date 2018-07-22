using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using LibOrbisPkg.GP4;
using System.IO;
using System.Windows.Forms;

namespace PkgEditor.Views
{
  public partial class GP4View : View
  {
    private Gp4Project proj;
    private string path;
    private bool loaded = false;
    private bool modified = false;
    private bool Modified
    {
      get => modified;
      set {
        modified = value;
        if (modified)
          Parent.Text = "*" + Path.GetFileName(path);
        else
          Parent.Text = Path.GetFileName(path);
        OnSaveStatusChanged();
      }
    }

    public GP4View(Gp4Project proj, string path) : base()
    {
      InitializeComponent();
      if(proj != null)
      {
        this.proj = proj;
        this.path = path;
        textBoxContentId.Text = proj.volume.Package.ContentId;
        textBoxPasscode.Text = proj.volume.Package.Passcode;
        PopulateDirs(proj.RootDir[0].Items);
      }
      loaded = true;
    }

    public override bool CanSave => Modified;
    public override bool CanSaveAs => true;
    public override void Save()
    {
      Modified = false;
    }

    private void PopulateDirs(List<Dir> dirs)
    {
      void AddDir(Dir d, TreeNode n)
      {
        var node = n.Nodes.Add(d.TargetName);
        node.Tag = d;
        foreach (var dir in d.Items)
          AddDir(dir, node);
      }

      dirsTreeView.Nodes.Clear();
      var root = dirsTreeView.Nodes.Add("Image0");
      root.Tag = proj.RootDir;
      foreach (var d in dirs)
        AddDir(d, root);
    }

    private void PopulateFiles(string prefix, List<Dir> d)
    {
      filesListView.Items.Clear();
      foreach (var dir in d)
      {
        var item = filesListView.Items.Add(new ListViewItem(dir.TargetName));
        item.ImageIndex = 0;
        item.Tag = dir;
      }
      var files = proj.files.Where(f => f.TargetPath.LastIndexOf('/') < prefix.Length && f.TargetPath.StartsWith(prefix));
      foreach (var f in files)
      {
        var item = filesListView.Items.Add(new ListViewItem(f.FileName));
        item.ImageIndex = 1;
        item.Tag = f;
      }
    }
    
    private void dirsTreeView_AfterSelect(object sender, TreeViewEventArgs e)
    {
      var prefix = "";
      var node = e.Node;
      while(node.Tag != proj.RootDir)
      {
        prefix = node.Text + "/" + prefix;
        node = node.Parent;
      }
      PopulateFiles(prefix, e.Node.Tag == proj.RootDir ? proj.RootDir[0].Items : (e.Node.Tag as Dir).Items);
    }

    private void propertyChanged(object sender, EventArgs e)
    {
      if(loaded)
        Modified = true;
    }

    private void button1_Click(object sender, EventArgs e)
    {
      var ofd = new SaveFileDialog();
      ofd.Filter = "PFS Image|*.dat";
      ofd.Title = "Choose output path for PFS";
      if(ofd.ShowDialog() == DialogResult.OK)
      {
        var logBox = new LogWindow();
        Console.SetOut(logBox.GetWriter());
        logBox.Show();
        using (var fs = System.IO.File.OpenWrite(ofd.FileName))
        {
          new LibOrbisPkg.PFS.PfsBuilder().BuildPfs(new LibOrbisPkg.PFS.PfsProperties
          {
            BlockSize = 65536,
            output = fs,
            proj = proj,
            projDir = Path.GetDirectoryName(path)
          });
          Console.WriteLine("Done! Saved to {0}", ofd.FileName);
        }
      }
    }
  }
}
