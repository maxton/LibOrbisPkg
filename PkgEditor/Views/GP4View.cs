using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using LibOrbisPkg.GP4;
using System.Windows.Forms;

namespace PkgEditor.Views
{
  public partial class GP4View : UserControl
  {
    private Gp4Project proj;
    public GP4View(Gp4Project proj)
    {
      InitializeComponent();
      if(proj != null)
      {
        this.proj = proj;
        textBoxContentId.Text = proj.volume.Package.ContentId;
        textBoxPasscode.Text = proj.volume.Package.Passcode;
        PopulateDirs(proj.RootDir[0].Items);
      }
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

    private void PopulateFiles(string dir)
    {
      var files = proj.files.Where(f => f.TargetPath.LastIndexOf('/') < dir.Length && f.TargetPath.StartsWith(dir));
      filesListView.Items.Clear();
      foreach (var f in files)
      {
        filesListView.Items.Add(new ListViewItem(f.FileName));
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
      PopulateFiles(prefix);
    }
  }
}
