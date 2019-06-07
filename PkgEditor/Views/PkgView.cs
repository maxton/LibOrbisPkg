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

    public override void Close()
    {
      CloseFileView();
    }

    private Pkg pkg;
    private IFile pkgFile;

    public PkgView(IFile pkgFile)
    {
      InitializeComponent();
      if (pkgFile == null) return;
      this.pkgFile = pkgFile;
      using (var s = pkgFile.GetStream())
        ObjectPreview(new PkgReader(s).ReadHeader());
      using (var s = pkgFile.GetStream())
        pkg = new PkgReader(s).ReadPkg();
      var sfoEditor = new SFOView(pkg.ParamSfo.ParamSfo, true);
      sfoEditor.Dock = DockStyle.Fill;
      tabPage1.Controls.Add(sfoEditor);

      ekpfs = pkg.GetEkpfs();
      ReopenFileView();

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

    private byte[] ekpfs;
    Action closePkg;
    private void CloseFileView()
    {
      closePkg?.Invoke();
      closePkg = null;
    }

    private void ReopenFileView()
    {
      if (!pkg.CheckEkpfs(ekpfs))
        return;

      var package = PackageReader.ReadPackageFromFile(pkgFile, new string(ekpfs.Select(b => (char)b).ToArray()));
      closePkg = () => package.Dispose();
      var innerPfs = PackageReader.ReadPackageFromFile(package.GetFile("/pfs_image.dat"));
      closePkg = () => { innerPfs.Dispose(); package.Dispose(); };
      var view = new PackageView(innerPfs, PackageManager.GetInstance());
      view.Dock = DockStyle.Fill;
      filesTab.Controls.Clear();
      filesTab.Controls.Add(view);
    }

    private void button1_Click(object sender, EventArgs e)
    {
      if(pkg.CheckPasscode(passcodeTextBox.Text))
      {
        ekpfs = Crypto.ComputeKeys(pkg.Header.content_id, passcodeTextBox.Text, 1);
        ReopenFileView();
      }
      else
      {
        MessageBox.Show("Invalid passcode!");
      }
    }

    private void entriesListView_DoubleClick(object sender, EventArgs e)
    {
      if(entriesListView.SelectedItems[0].SubItems[0].Text == "PARAM_SFO")
      {
        mainWin.OpenTab(new SFOView(pkg.ParamSfo.ParamSfo, true), "param.sfo [Read-Only]");
      }
    }

    private void checkDigests_Click(object sender, EventArgs _)
    {
      validateResult.Text = "Checking PKG digests...";
      Action<string> errorHandler = e => validateResult.Text += Environment.NewLine + $"Digest mismatch: {e}";
      var validator = new PkgValidator(pkg, errorHandler);
      CloseFileView();
      using (var s = pkgFile.GetStream())
      {
        var errors = validator.Validate(s);
        if (errors.Count == 0)
        {
          validateResult.Text += Environment.NewLine + "PKG Successfully validated";
        }
        else
        {
          validateResult.Text += Environment.NewLine + $"PKG Validation Failed: {errors.Count} hashes did not match";
        }
      }
      ReopenFileView();
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
