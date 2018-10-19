using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using LibOrbisPkg.GP4;
using LibOrbisPkg.PFS;
using LibOrbisPkg.PKG;
using System.IO;
using System.Windows.Forms;

namespace PkgEditor.Views
{
  public partial class GP4View : View
  {
    private Gp4Project proj;
    /// <summary>
    /// The full path to the current active project
    /// </summary>
    private string path;
    private bool loaded = false;
    private bool modified = false;
    public bool Modified
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
    /// <summary>
    /// The current path that's being displayed in the file listview
    /// </summary>
    private string listViewPath = "";
    private Dir currentDir = null;

    public GP4View(Gp4Project proj, string path) : base()
    {
      InitializeComponent();
      if(proj != null)
      {
        this.proj = proj;
        this.path = path;
        contentIdTextBox.Text = proj.volume.Package.ContentId;
        passcodeTextBox.Text = proj.volume.Package.Passcode;
        currentDir = null;
        PopulateDirs();
        loaded = true;
      }
    }

    public override bool CanSave => Modified;
    public override bool CanSaveAs => true;
    public override void Save()
    {
      using (var fs = File.OpenWrite(path))
      {
        fs.SetLength(0);
        Gp4Project.WriteTo(proj, fs);
      }
      Modified = false;
    }
    public override void SaveAs()
    {
      var ofd = new SaveFileDialog
      {
        Filter = "GP4 Projects|*.gp4",
      };
      if(ofd.ShowDialog() == DialogResult.OK)
      {
        path = ofd.FileName;
        Save();
      }
    }

    private void PopulateDirs()
    {
      void AddDir(Dir d, TreeNode n)
      {
        var node = n.Nodes.Add(d.TargetName);
        node.Tag = d;
        if(d == currentDir)
        {
          dirsTreeView.SelectedNode = node;
        }
        foreach (var dir in d.Children)
          AddDir(dir, node);
      }

      dirsTreeView.Nodes.Clear();
      var root = dirsTreeView.Nodes.Add("Image0");
      root.Tag = proj.RootDir;
      foreach (var d in proj.RootDir)
        AddDir(d, root);
      dirsTreeView.ExpandAll();
      if(currentDir == null)
      {
        dirsTreeView.SelectedNode = root;
      }
    }

    private void PopulateFiles()
    {
      filesListView.Items.Clear();
      foreach (var dir in currentDir?.Children ?? proj.RootDir)
      {
        var item = filesListView.Items.Add(new ListViewItem(dir.TargetName));
        item.ImageIndex = 0;
        item.Tag = dir;
      }
      var files = proj.files.Where(f => f.TargetPath.LastIndexOf('/') < listViewPath.Length && f.TargetPath.StartsWith(listViewPath));
      foreach (var f in files)
      {
        var item = filesListView.Items.Add(
          new ListViewItem(new string[] { f.FileName, f.OrigPath }));
        item.ImageIndex = 1;
        item.Tag = f;
      }
    }
    
    private void dirsTreeView_AfterSelect(object sender, TreeViewEventArgs e)
    {
      if (e.Node.Tag is Dir dir)
      {
        listViewPath = dir.Path;
        currentDir = dir;
      }
      else
      {
        listViewPath = "";
        currentDir = null;
      }
      PopulateFiles();
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

    private void buildPfs_Click(object sender, EventArgs e)
    {
      var ofd = new SaveFileDialog();
      ofd.Filter = "PFS Image|*.dat";
      ofd.Title = "Choose output path for PFS";
      if(ofd.ShowDialog() == DialogResult.OK)
      {
        var logBox = new LogWindow();
        Console.SetOut(logBox.GetWriter());
        logBox.Show();
        using (var fs = File.OpenWrite(ofd.FileName))
        {
          new PfsBuilder(new PfsProperties
          {
            BlockSize = 65536,
            output = fs,
            proj = proj,
            projDir = Path.GetDirectoryName(path)
          }, Console.WriteLine).BuildPfs();
          Console.WriteLine("Done! Saved to {0}", ofd.FileName);
        }
      }
    }

    private void buildPkg_Click(object sender, EventArgs e)
    {
      var ofd = new SaveFileDialog();
      ofd.Filter = "PKG Image|*.pkg";
      ofd.Title = "Choose output path for PKG";
      ofd.FileName = proj.volume.Package.ContentId + ".pkg";
      if (ofd.ShowDialog() == DialogResult.OK)
      {
        var logBox = new LogWindow();
        Console.SetOut(logBox.GetWriter());
        logBox.Show();
        using (var fs = File.Open(ofd.FileName, FileMode.Create))
        {
          new PkgBuilder(proj, Path.GetDirectoryName(path)).Write(fs);
          Console.WriteLine("Done! Saved to {0}", ofd.FileName);
        }
      }
    }

    private void filesListView_AfterLabelEdit(object sender, LabelEditEventArgs e)
    {
      var item = filesListView.Items[e.Item];
      if (e.Label == "" || e.Label == null || e.Label == item.Text)
        goto cancel;
      // Ensure you don't rename something to an existing name
      foreach (ListViewItem i in filesListView.Items)
      {
        if (i.Text == e.Label)
        {
          item.BeginEdit();
          goto cancel;
        }
      }
      if (item.Tag is Gp4File f)
      {
        if (e.Label == f.FileName) goto cancel;
        proj.RenameFile(f, e.Label);
        Modified = true;
      }
      else if(item.Tag is Dir d)
      {
        if (d.TargetName == e.Label) goto cancel;
        proj.RenameDir(d, e.Label);
        Modified = true;
        PopulateDirs();
      }
      return;
      cancel:
        e.CancelEdit = true;
    }


    private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if(filesListView.SelectedItems.Count == 0)
      {
        return;
      }

      foreach (var i in filesListView.SelectedItems)
      {
        var item = i as ListViewItem;
        if (item.Tag is Gp4File f)
        {
          proj.DeleteFile(f);
        }
        else if (item.Tag is Dir d)
        {
          proj.DeleteDir(d);
        }
      }
      Modified = true;
      PopulateDirs();
      PopulateFiles();
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

    private void filesListView_DragEnter(object sender, DragEventArgs e)
    {
      if (e.Data.GetDataPresent(DataFormats.FileDrop))
      {
        e.Effect = DragDropEffects.Copy;
      }
    }

    private void AddFile(string targetPath, string origPath)
    {
      var f = origPath;
      var projDir = Path.GetDirectoryName(path);
      if (origPath.StartsWith(projDir))
      {
        f = origPath.Substring(projDir.Length + 1);
      }
      var fileEntry = new Gp4File
      {
        OrigPath = f,
        TargetPath = targetPath + Path.GetFileName(origPath)
      };
      proj.files.Add(fileEntry);
    }

    private void AddFileTree(Dir parent, string path)
    {
      var newDir = proj.AddDir(parent, Path.GetFileName(path));
      foreach (var d in Directory.EnumerateDirectories(path))
      {
        AddFileTree(newDir, d);
      }
      var targetPath = newDir.Path;
      foreach(var f in Directory.EnumerateFiles(path))
      {
        AddFile(targetPath, f);
      }
    }

    private void filesListView_DragDrop(object sender, DragEventArgs e)
    {
      var files = ((string[])e.Data.GetData(DataFormats.FileDrop));
      foreach(var file in files)
      {
        if (File.Exists(file))
        {
          AddFile(listViewPath, file);
        }
        else if (Directory.Exists(file))
        {
          AddFileTree(currentDir, file);
          PopulateDirs();
        }
      }
      PopulateFiles();
    }

    private void newFolderToolStripMenuItem_Click(object sender, EventArgs e)
    {
      var dir = proj.AddDir(currentDir, "New Folder");
      PopulateFiles();
      foreach(ListViewItem item in filesListView.Items)
      {
        if(item.Tag == dir)
        {
          item.BeginEdit();
        }
      }
    }

    private void filesListView_DoubleClick(object sender, EventArgs e)
    {
      if(filesListView.SelectedItems.Count == 1)
      {
        if(filesListView.SelectedItems[0].Tag is Dir d)
        {
          currentDir = d;
          listViewPath = d.Path;
          PopulateDirs();
          PopulateFiles();
        }
      }
    }
  }
}
