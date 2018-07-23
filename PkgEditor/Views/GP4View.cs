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
        contentIdTextBox.Text = proj.volume.Package.ContentId;
        passcodeTextBox.Text = proj.volume.Package.Passcode;
        PopulateDirs(proj.RootDir);
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
      PopulateFiles(prefix, e.Node.Tag == proj.RootDir ? proj.RootDir : (e.Node.Tag as Dir).Items);
    }

    private void propertyChanged(object sender, EventArgs e)
    {
      if (loaded)
      {
        Modified = true;
        proj.volume.Package.ContentId = contentIdTextBox.Text;
        proj.volume.Package.Passcode = passcodeTextBox.Text;
      }
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

    private void filesListView_AfterLabelEdit(object sender, LabelEditEventArgs e)
    {
      if (e.Label == "" || e.Label == null)
      {
        e.CancelEdit = true;
        return;
      }
      var item = filesListView.Items[e.Item];
      if(item.Tag is LibOrbisPkg.GP4.File)
      {
        var f = item.Tag as LibOrbisPkg.GP4.File;
        var newName = f.DirName + e.Label;
        if(newName != f.TargetPath)
        {
          Modified = true;
          f.TargetPath = newName;
        }
        else
        {
          e.CancelEdit = true;
        }
      }
      else
      {
        // TODO: dirs
        e.CancelEdit = true;
      }
    }


    private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
    {
      foreach(var i in filesListView.SelectedItems)
      {
        var item = i as ListViewItem;
        if (item.Tag is LibOrbisPkg.GP4.File)
        {
          var f = item.Tag as LibOrbisPkg.GP4.File;
          proj.files.Remove(f);
          filesListView.Items.Remove(item);
          Modified = true;
        }
        else
        {
          // TODO: dirs
        }
      }
    }

    private void renameToolStripMenuItem_Click(object sender, EventArgs e)
    {
      filesListView.SelectedItems[0].BeginEdit();
    }

    private void fileViewContextMenu_Opening(object sender, CancelEventArgs e)
    {
      renameToolStripMenuItem.Enabled = filesListView.SelectedItems.Count == 1;
      deleteToolStripMenuItem.Enabled = filesListView.SelectedItems.Count > 0;
    }
  }
}
