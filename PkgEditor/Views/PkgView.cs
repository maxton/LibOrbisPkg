using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

using GameArchives;
using LibArchiveExplorer;
using LibOrbisPkg.PKG;
using LibOrbisPkg.Util;

namespace PkgEditor.Views
{
  public partial class PkgView : View
  {
    public override bool CanSave => false;
    public override bool CanSaveAs => false;

    private Pkg pkg;
    private IFile pkgFile;

    public PkgView(IFile pkgFile)
    {
      InitializeComponent();
      this.pkgFile = pkgFile;
      using (var s = pkgFile.GetStream())
        ObjectPreview(new LibOrbisPkg.PKG.PkgReader(s).ReadHeader());
      using (var s = pkgFile.GetStream())
        pkg = new PkgReader(s).ReadPkg();
      try
      {
        var dk3 = Crypto.RSA2048Decrypt(pkg.EntryKeys.Keys[3].key, RSAKeyset.PkgDerivedKey3Keyset);
        var iv_key = Crypto.Sha256(
          pkg.ImageKey.meta.GetBytes()
          .Concat(dk3)
          .ToArray());
        var imageKeyDecrypted = pkg.ImageKey.FileData.Clone() as byte[];
        Crypto.AesCbcCfb128Decrypt(
          imageKeyDecrypted,
          imageKeyDecrypted,
          imageKeyDecrypted.Length,
          iv_key.Skip(16).Take(16).ToArray(),
          iv_key.Take(16).ToArray());
        var ekpfs = Crypto.RSA2048Decrypt(imageKeyDecrypted, RSAKeyset.FakeKeyset);
        var package = PackageReader.ReadPackageFromFile(pkgFile, new string(ekpfs.Select(b => (char)b).ToArray()));
        var innerPfs = PackageReader.ReadPackageFromFile(package.GetFile("/pfs_image.dat"));
        var view = new PackageView(innerPfs, PackageManager.GetInstance());
        view.Dock = DockStyle.Fill;
        filesTab.Controls.Clear();
        filesTab.Controls.Add(view);
      } catch (Exception) {
      }

      foreach(var e in pkg.Metas.Metas)
      {
        var lvi = new ListViewItem(new[] {
          e.id.ToString(),
          string.Format("0x{0:X}", e.DataSize),
          string.Format("0x{0:X}", e.DataOffset),
          e.Encrypted ? "Yes" : "No",
          e.KeyIndex.ToString(),
        });
        lvi.Tag = e;
        entriesListView.Items.Add(lvi);
      }
    }

    object previewObj;
    void ObjectPreview(object obj)
    {
      treeView1.Nodes.Clear();
      AddObjectNodes(obj, treeView1.Nodes);
      previewObj = obj;
    }

    string toString(object obj)
    {
      switch (obj)
      {
        case Byte _:
        case UInt16 _:
        case UInt32 _:
        case UInt64 _:
        case Int16 _:
        case Int32 _:
        case Int64 _:
          return string.Format("0x{0:X}", obj);
        default:
          return obj.ToString();
      }
    }

    /// <summary>
    /// Adds the given object's public fields to the given TreeNodeCollection.
    /// </summary>
    void AddObjectNodes(object obj, TreeNodeCollection nodes)
    {
      if (obj == null) return;
      var fields = obj.GetType().GetFields();
      foreach (var f in fields)
      {
        if (f.IsLiteral) continue;
        var val = f.GetValue(obj);
        if (val is byte[] b)
        {
          nodes.Add(f.Name + " = " + LibOrbisPkg.Util.Crypto.AsHexCompact(b));
        }
        else if (f.FieldType.IsPrimitive || f.FieldType == typeof(string) || f.FieldType.IsEnum)
        {
          if (val != null)
          {
            nodes.Add(f.Name + " = " + toString(val));
          }
        }
        else if (f.FieldType.IsArray)
        {
          AddArrayNodes(f.GetValue(obj) as Array, f.Name, nodes);
        }
        else if (f.FieldType.IsGenericType && f.FieldType.GetGenericTypeDefinition() == typeof(List<>))
        {
          var internalType = f.FieldType.GetGenericArguments()[0];
          AddArrayNodes((f.GetValue(obj) as IList).Cast<object>().ToArray(), f.Name, nodes);
        }
        else
        {
          var node = new TreeNode(f.Name);
          AddObjectNodes(f.GetValue(obj), node.Nodes);
          nodes.Add(node);
        }
      }
    }

    /// <summary>
    /// Adds the given array to the given TreeNodeCollection.
    /// </summary>
    void AddArrayNodes(Array arr, string name, TreeNodeCollection nodes)
    {
      var node = new TreeNode($"{name} ({arr.Length})");
      var eType = arr.GetType().GetElementType();
      if (eType.IsPrimitive || eType == typeof(string) || eType.IsEnum)
        for (var i = 0; i < arr.Length; i++)
        {
          var n = new TreeNode($"{name}[{i}] = {toString(arr.GetValue(i))}");
          node.Nodes.Add(n);
        }
      else for (var i = 0; i < arr.Length; i++)
        {
          var myName = $"{name}[{i}]";
          if (eType.IsArray)
            AddArrayNodes(arr.GetValue(i) as Array, myName, node.Nodes);
          else
          {
            var obj = arr.GetValue(i);

            System.Reflection.FieldInfo nameField;
            if (null != (nameField = obj.GetType().GetField("Name")))
            {
              myName += $" (Name: {nameField.GetValue(obj)})";
            }
            var n = new TreeNode(myName);
            var item = arr.GetValue(i);
            AddObjectNodes(item, n.Nodes);
            node.Nodes.Add(n);
          }
        }
      nodes.Add(node);
    }

    private string passcode;
    AbstractPackage package, innerPfs;
    private bool pkg_loaded;
    private void CloseFileView()
    {

    }

    private void ReopenFileView()
    {

    }

    private void button1_Click(object sender, EventArgs e)
    {
      try
      {
        var ekpfs = new string(Crypto.ComputeKeys(pkg.Header.content_id, passcodeTextBox.Text, 1)
          .Select(b => (char)b).ToArray());
        package = PackageReader.ReadPackageFromFile(pkgFile, ekpfs);
        innerPfs = PackageReader.ReadPackageFromFile(package.GetFile("/pfs_image.dat"));
        var view = new PackageView(innerPfs, PackageManager.GetInstance());
        view.Dock = DockStyle.Fill;
        filesTab.Controls.Clear();
        filesTab.Controls.Add(view);
        passcode = passcodeTextBox.Text;
      }
      catch (Exception)
      {
        MessageBox.Show("Invalid passcode!");
      }
    }

    private void entriesListView_DoubleClick(object sender, EventArgs e)
    {
      if(entriesListView.SelectedItems[0].SubItems[0].Text == "PARAM_SFO")
      {
        mainWin.OpenTab(new SFOView(pkg.ParamSfo.ParamSfo), "param.sfo");
      }
    }

    private void button2_Click(object sender, EventArgs e)
    {
      using (var sfd = new SaveFileDialog())
      {
        sfd.Filter = "PFS Image (*.dat)|*.dat";
        if(sfd.ShowDialog() == DialogResult.OK)
        {
          using (var fs = System.IO.File.OpenWrite(sfd.FileName))
          {
            pkgFile.GetStream();
          }
        }
      }
    }
  }
}
