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
using PfsDir = LibOrbisPkg.PFS.PfsReader.Dir;
using PfsFile = LibOrbisPkg.PFS.PfsReader.File;
using PfsNode = LibOrbisPkg.PFS.PfsReader.Node;
using System.IO;

namespace PkgEditor.Views
{
  public partial class FileView : UserControl
  {
    public FileView()
    {
      InitializeComponent();
    }

    public void AddRoot(PfsReader p, string name)
    {
      var superroot = p.GetSuperRoot();
      var root = new TreeNode(name) { Tag = superroot };
      directoryTreeView.Nodes.Add(root);
      root.Nodes.Add("Loading", "Loading...", 0);
      ExpandNode(root);
      directoryTreeView.SelectedNode = root.Nodes[0];
    }

    private void LoadDirectory(PfsDir directory)
    {
      currentFolderListView.Items.Clear();
      foreach(var child in directory.children)
      {
        currentFolderListView.Items.Add(
          new ListViewItem(new[] {
            child.name,
            child is PfsDir ? "" : HumanReadableFileSize(child.compressed_size),
            child is PfsDir ? "" : HumanReadableFileSize(child.size),
          },
          child is PfsDir ? 1 : 0)
          {
            Tag = child
          });
      }
    }
    private void ExpandNode(TreeNode node)
    {
      if (node.Nodes.Count == 0 || node.Nodes[0].Tag == null)
      {
        node.Nodes.Clear();
        foreach (var dir in (node.Tag as PfsDir).children)
        {
          if (dir is PfsDir d)
          {
            var newNode = new TreeNode(d.name) { Tag = d };
            if (d.children.Any(x => x is PfsDir))
              newNode.Nodes.Add("Loading", "Loading...", 0);
            node.Nodes.Add(newNode);
          }
        }
      }
    }
    private void DirectoryTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
    {
      ExpandNode(e.Node);
    }

    private void DirectoryTreeView_AfterSelect(object sender, TreeViewEventArgs e)
    {
      LoadDirectory(e.Node.Tag as PfsDir);
    }

    public static string HumanReadableFileSize(long size)
    {
      if (size > (1024 * 1024 * 1024))
      {
        return (size / (double)(1024 * 1024 * 1024)).ToString("0.#") + " GiB";
      }
      else if (size > (1024 * 1024))
      {
        return (size / (double)(1024 * 1024)).ToString("0.#") + " MiB";
      }
      else if (size > 1024)
      {
        return (size / 1024.0).ToString("0.#") + " KiB";
      }
      else
      {
        return size.ToString() + " B";
      }
    }

    private void Extract(PfsNode n, bool compressed = false)
    {
      var sfd = new SaveFileDialog()
      {
        FileName = n.name,
      };
      if (sfd.ShowDialog() == DialogResult.OK)
      {
        if (n is PfsDir d)
        {
          Directory.CreateDirectory(sfd.FileName);
          SaveTo(d.children, sfd.FileName);
        }
        else if (n is PfsFile f)
        {
          f.Save(sfd.FileName, !compressed);
        }
      }
    }

    private void ExtractMultiple(IEnumerable<PfsNode> n)
    {
      var sfd = new SaveFileDialog() { FileName = "Save this dummy file to the target directory"};
      if (sfd.ShowDialog() == DialogResult.OK)
      {
        var directory = Path.GetDirectoryName(sfd.FileName);
        SaveTo(n, directory);
      }
    }

    // TODO: Parallelize this
    private void SaveTo(IEnumerable<PfsNode> nodes, string path)
    {
      foreach(var n in nodes)
      {
        if(n is PfsFile f)
        {
          f.Save(Path.Combine(path, n.name));
        }
        else if(n is PfsDir d)
        {
          var newPath = Path.Combine(path, d.name);
          Directory.CreateDirectory(newPath);
          SaveTo(d.children, newPath);
        }
      }
    }

    private void ExtractToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if(directoryTreeView.Focused || (currentFolderListView.Focused && currentFolderListView.SelectedItems.Count == 0))
      {
        if(directoryTreeView.SelectedNode?.Tag is PfsDir d)
        {
          Extract(d);
        }
      }
      else if(currentFolderListView.Focused)
      {
        if (currentFolderListView.SelectedItems.Count == 1)
        {
          Extract(currentFolderListView.SelectedItems[0].Tag as PfsNode);
        }
        else
        {
          var stuff = new List<PfsNode>();
          foreach (var i in currentFolderListView.SelectedItems)
            stuff.Add((i as ListViewItem)?.Tag as PfsNode);
          ExtractMultiple(stuff);
        }
      }
    }

    private void extractCompressedPFSCToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (currentFolderListView.Focused && currentFolderListView.SelectedItems.Count == 1 && currentFolderListView.SelectedItems[0].Tag is PfsNode n)
      {
        Extract(n, compressed: true);
      }
    }

    private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
    {
      extractCompressedPFSCToolStripMenuItem.Enabled = false;
      if (currentFolderListView.Focused && currentFolderListView.SelectedItems.Count == 1 && currentFolderListView.SelectedItems[0].Tag is PfsNode n)
      {
        extractCompressedPFSCToolStripMenuItem.Enabled = n.compressed_size != n.size;
      }
    }
  }
}
