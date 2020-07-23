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
using System.Threading.Tasks;

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
        if(value != modified)
        {
          modified = value;
          if (modified)
            Parent.Text = "*" + Path.GetFileName(path);
          else
            Parent.Text = Path.GetFileName(path);
          OnSaveStatusChanged();
        }
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
        currentDir = null;
        ReloadView();
      }
    }

    private void ReloadView()
    {
      loaded = false;
      switch (proj.volume.Type)
      {
        case VolumeType.pkg_ps4_app:
          pkgTypeDropdown.SelectedIndex = 0;
          entitlementKeyTextbox.Enabled = false;
          break;
        case VolumeType.pkg_ps4_ac_data:
          pkgTypeDropdown.SelectedIndex = 1;
          entitlementKeyTextbox.Enabled = true;
          break;
        case VolumeType.pkg_ps4_ac_nodata:
          pkgTypeDropdown.SelectedIndex = 2;
          entitlementKeyTextbox.Enabled = true;
          break;
        default:
          break;
      }
      volumeTimestampPicker.Value = proj.volume.TimeStamp.ToLocalTime();
      if (proj.volume.Package.CreationDate != null && proj.volume.Package.CreationDate != "actual_datetime")
      {
        creationTimePicker.Value = proj.volume.Package.CreationTimeStamp.ToLocalTime();
        includeTimeCheckBox.Checked = proj.volume.Package.CreationDate.Contains(" ");
        creationTimePicker.Enabled = true;
        useTimeOfBuildCheckBox.Checked = false;
      }
      else
      {
        creationTimePicker.Value = proj.volume.TimeStamp.ToLocalTime();
        creationTimePicker.Enabled = false;
        useTimeOfBuildCheckBox.Checked = true;
        includeTimeCheckBox.Checked = proj.volume.Package.CreationDate == "actual_datetime";
      }
      if (includeTimeCheckBox.Checked)
        creationTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
      else
        creationTimePicker.CustomFormat = "yyyy-MM-dd";
      contentIdTextBox.Text = proj.volume.Package.ContentId;
      passcodeTextBox.Text = proj.volume.Package.Passcode;
      entitlementKeyTextbox.Text = proj.volume.Package.EntitlementKey;
      PopulateDirs();
      loaded = true;
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
        Parent.Text = Path.GetFileName(path);
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
      var files = proj.files.Items.Where(f => f.TargetPath.LastIndexOf('/') < listViewPath.Length && f.TargetPath.StartsWith(listViewPath));
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
        proj.volume.Package.EntitlementKey = entitlementKeyTextbox.Text;
        if(entitlementKeyTextbox.Text == "")
        {
          proj.volume.Package.EntitlementKey = null;
        }
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
        var writer = logBox.GetWriter();
        logBox.Show();
        using (var fs = File.OpenWrite(ofd.FileName))
        {
          try
          { 
            new PfsBuilder(PfsProperties.MakeInnerPFSProps(PkgProperties.FromGp4(proj, Path.GetDirectoryName(path))), writer.WriteLine).WriteImage(fs);
            writer.WriteLine("Done! Saved to {0}", ofd.FileName);
          }
          catch (Exception ex)
          {
            writer.WriteLine("Error: " + ex);
            writer.WriteLine(ex.StackTrace);
          }
        }
      }
    }

    private async void buildPkg_Click(object sender, EventArgs e)
    {
      var validateResults = Gp4Validator.ValidateProject(proj, Path.GetDirectoryName(path));
      if (validateResults.Count != 0)
      {
        var validateDialog = new ValidationDialog(validateResults);
        if (validateDialog.ShowDialog() == DialogResult.Cancel)
        {
          return;
        }
      }
      var ofd = new SaveFileDialog();
      ofd.Filter = "PKG Image|*.pkg";
      ofd.Title = "Choose output path for PKG";
      ofd.FileName = proj.volume.Package.ContentId + ".pkg";
      if (ofd.ShowDialog() == DialogResult.OK)
      {
        var logBox = new LogWindow();
        var writer = logBox.GetWriter();
        logBox.Show();
        Action<string> LogLine = x => logBox.BeginInvoke(new Action(() => writer.WriteLine(x)));
        try
        {
          await Task.Run(() => {
            new PkgBuilder(PkgProperties.FromGp4(proj, Path.GetDirectoryName(path))).Write(ofd.FileName, LogLine);
            LogLine($"Saved to {ofd.FileName}");
          });
        }
        catch(Exception ex)
        {
          writer.WriteLine("Error: " + ex);
          writer.WriteLine(ex.StackTrace);
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

    /// <summary>
    /// Deletes the currently selected files/folders in the fileview from the project and redraws the view.
    /// </summary>
    private void DeleteSelectedFiles()
    {
      if (filesListView.SelectedItems.Count == 0)
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

    private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
    {
      DeleteSelectedFiles();
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
      proj.files.Items.Add(fileEntry);
      Modified = true;
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
      Modified = true;
    }

    private void AddFiles(string[] filenames)
    {
      foreach (var file in filenames)
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

    private void filesListView_DragDrop(object sender, DragEventArgs e)
    {
      var files = ((string[])e.Data.GetData(DataFormats.FileDrop));
      AddFiles(files);
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

    private void pkgTypeDropdown_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (!loaded) return;
      switch(pkgTypeDropdown.SelectedIndex)
      {
        case 0: // GP
          proj.SetType(VolumeType.pkg_ps4_app);
          ReloadView();
          break;
        case 1: // AC
          proj.SetType(VolumeType.pkg_ps4_ac_data);
          ReloadView();
          break;
        case 2: // AL
          proj.SetType(VolumeType.pkg_ps4_ac_nodata);
          ReloadView();
          break;
        default:
          return;
      }
      Modified = true;
    }

    private void volumeTimestampPicker_ValueChanged(object sender, EventArgs e)
    {
      if (!loaded) return;
      proj.volume.TimeStamp = volumeTimestampPicker.Value.ToUniversalTime();
      UpdateCreationDate();
      Modified = true;
    }

    private void filesListView_KeyUp(object sender, KeyEventArgs e)
    {
      if(e.KeyCode == Keys.Delete)
      {
        DeleteSelectedFiles();
      }
    }

    private void UpdateCreationDate()
    {
      Modified = true;
      if(useTimeOfBuildCheckBox.Checked)
      {
        proj.volume.Package.CreationDate = includeTimeCheckBox.Checked ? "actual_datetime" : null;
      }
      else
      {
        var str = creationTimePicker.Value.ToUniversalTime().ToString("s");
        if (includeTimeCheckBox.Checked)
          proj.volume.Package.CreationDate = str.Replace('T', ' ');
        else
          proj.volume.Package.CreationDate = str.Substring(0, 10);
      }
    }

    private void creationDateCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      if (!loaded) return;
      UpdateCreationDate();
      ReloadView();
    }

    private void includeTimeCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      if (!loaded) return;
      UpdateCreationDate();
      ReloadView();
    }

    private void creationTimePicker_ValueChanged(object sender, EventArgs e)
    {
      if (!loaded) return;
      UpdateCreationDate();
    }

    private void addFilesToolStripMenuItem_Click(object sender, EventArgs e)
    {
      using(var fsd = new OpenFileDialog() { Multiselect = true, Title = "Select file(s)..."})
      {
        if(fsd.ShowDialog(this) == DialogResult.OK)
        {
          AddFiles(fsd.FileNames);
        }
      }
    }
  }
}
