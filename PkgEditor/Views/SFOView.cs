using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using LibOrbisPkg.SFO;
using LibOrbisPkg.Util;

namespace PkgEditor.Views
{
  public partial class SFOView : View
  {
    private ParamSfo proj;
    /// <summary>
    /// The full path to the current active project
    /// </summary>
    private string path;
    private bool loaded = false;
    private bool @readonly = false;
    private bool modified = false;
    public bool Modified
    {
      get => modified;
      set {
        if (value != modified)
        {
          modified = value;
          var pathTxt = path != null ? Path.GetFileName(path) : "New file";
          if (modified)
            Parent.Text = "*" + pathTxt;
          else
            Parent.Text = pathTxt;
          OnSaveStatusChanged();
        }
      }
    }

    public SFOView() : this(null) { }

    public SFOView(ParamSfo proj, bool @readonly = false, string path = null) : base()
    {
      loaded = false;
      InitializeComponent();
      this.path = path;
      this.proj = proj;
      this.@readonly = @readonly;
      Reset();
      PopulateInput(null);
    }

    public override bool CanSave => Modified && !@readonly;
    public override bool CanSaveAs => true;
    public override void Save()
    {
      if (!CanSave) return;
      if (path == null)
      {
        SaveAs();
        return;
      }
      using (var f = File.Open(path, FileMode.Create))
      {
        var sfoBytes = proj.Serialize();
        f.Write(sfoBytes, 0, sfoBytes.Length);
        Modified = false;
      }
    }
    public override void SaveAs()
    {
      if (!CanSaveAs) return;
      using (var sfd = new SaveFileDialog() { Filter = "System File Object (*.sfo)|*.sfo", FileName = path ?? "param.sfo" })
      {
        if (sfd.ShowDialog() == DialogResult.OK)
        {
          path = sfd.FileName;
          using (var f = File.Open(path, FileMode.Create))
          {
            var sfoBytes = proj.Serialize();
            f.Write(sfoBytes, 0, sfoBytes.Length);
            @readonly = false;
            Modified = false;
          }
        }
      }
    }

    private void Reset()
    {
      if (proj == null) return;
      maxLengthInput.ReadOnly = valueTextBox.ReadOnly = nameTextBox.ReadOnly = @readonly;
      typeDropDown.Enabled = toolStrip1.Enabled = maxLengthInput.Enabled = !@readonly;
      listView1.Items.Clear();
      foreach (var param in proj.Values)
      {
        switch (param)
        {
          case Utf8Value v:
            listView1.Items.Add(new ListViewItem(new[] { v.Name, "string", v.Value })
            {
              Tag = param
            });
            break;
          case Utf8SpecialValue v:
            listView1.Items.Add(new ListViewItem(new[] { v.Name, "bytes", v.Value.Select(x => string.Format("{0:X2}", x)).DefaultIfEmpty("").Aggregate((s1, s2) => s1 + s2) })
            {
              Tag = param
            });
            break;
          case IntegerValue v:
            listView1.Items.Add(new ListViewItem(new[] { v.Name, "integer", v.Value.ToString("X8") })
            {
              Tag = param
            });
            break;
        }
      }
      loaded = true;
    }

    Value selectedValue;
    private void PopulateInput(Value v, object sender = null)
    {
      loaded = false;
      selectedValue = v;
      if (v == null)
      {
        typeDropDown.Enabled = false;
        maxLengthInput.Enabled = false;
        nameTextBox.Text = "";
        nameTextBox.Enabled = false;
        valueTextBox.Text = "";
        valueTextBox.Enabled = false;
        sizeLabel.Text = "";
      }
      else
      {
        typeDropDown.Enabled = !@readonly;
        typeDropDown.SelectedIndex = v.Type == SfoEntryType.Integer ? 1 : v.Type == SfoEntryType.Utf8 ? 0 : 2;

        nameTextBox.Enabled = true;
        if (sender != nameTextBox)
          nameTextBox.Text = v.Name;

        valueTextBox.Enabled = true;
        if (sender != valueTextBox)
          valueTextBox.Text = v.ToString();

        maxLengthInput.Enabled = !@readonly && v.Type != SfoEntryType.Integer;
        if (sender != maxLengthInput)
          maxLengthInput.Value = v.MaxLength;

        sizeLabel.Text = "Size: " + v.Length;
        if (v.Length > v.MaxLength)
        {
          sizeLabel.ForeColor = Color.DarkRed;
          sizeLabel.Font = new Font(sizeLabel.Font, FontStyle.Bold);
        }
        else
        {
          sizeLabel.ForeColor = Color.Black;
          sizeLabel.Font = new Font(sizeLabel.Font, 0);
        }
      }
      loaded = true;
    }

    private void ClearInput()
    {
      PopulateInput(null);
    }

    private void AddKey(SfoEntryType type)
    {
      var name = "NEW_KEY";
      while (proj.GetValueByName(name) != null)
      {
        name += "_";
      }
      Value val;
      switch (type)
      {
        case SfoEntryType.Utf8Special:
          val = new Utf8SpecialValue(name, new byte[] { 00 }, 1);
          break;
        case SfoEntryType.Utf8:
          val = new Utf8Value(name, "", 1);
          break;
        case SfoEntryType.Integer:
          val = new IntegerValue(name, 0);
          break;
        default:
          return;
      }
      proj.Values.Add(val);
      Reset();
      PopulateInput(val);
      Modified = true;
    }

    private void DeleteSelected()
    {
      if (@readonly) return;
      if (listView1.SelectedItems.Count > 0)
      {
        foreach (var item in listView1.SelectedItems)
        {
          if ((item as ListViewItem).Tag is Value v)
          {
            proj.Values.Remove(v);
          }
        }
        Reset();
        PopulateInput(null);
        Modified = true;
      }
    }

    private void UpdateValue(object sender, EventArgs e)
    {
      if (!loaded) return;
      Value newValue = null;
      switch (typeDropDown.SelectedIndex)
      {
        case 0:
          newValue = new Utf8Value(nameTextBox.Text, valueTextBox.Text, (int)maxLengthInput.Value);
          break;
        case 1:
          int newNumber;
          if (valueTextBox.Text.StartsWith("0x"))
          {
            int.TryParse(valueTextBox.Text.Substring(2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out newNumber);
          }
          else
          {
            int.TryParse(valueTextBox.Text, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out newNumber);
          }
          newValue = new IntegerValue(nameTextBox.Text, newNumber);
          break;
        case 2:
          newValue = new Utf8SpecialValue(nameTextBox.Text, valueTextBox.Text.FromHexCompact(), (int)maxLengthInput.Value);
          break;
      }
      if (newValue != null)
        selectedValue = proj[selectedValue.Name] = newValue;
      Modified = true;
      Reset();
      PopulateInput(selectedValue, sender);
    }

    private void loadACDefaultsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      proj = new ParamSfo() { Values = ParamSfo.DefaultAC.Values.ToList() };
      Reset();
      Modified = true;
    }

    private void loadGPDefaultsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      proj = new ParamSfo() { Values = ParamSfo.DefaultGD.Values.ToList() };
      Reset();
      Modified = true;
    }

    private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
    {
      if (listView1.SelectedItems.Count == 1)
      {
        PopulateInput(listView1.SelectedItems[0].Tag as Value);
      }
      else
      {
        ClearInput();
      }
    }

    private void listView1_KeyUp(object sender, KeyEventArgs e)
    {
      if(e.KeyCode == System.Windows.Forms.Keys.Delete)
      {
        DeleteSelected();
      }
    }

    private void stringToolStripMenuItem_Click(object sender, EventArgs e) => AddKey(SfoEntryType.Utf8);
    private void intToolStripMenuItem_Click(object sender, EventArgs e) => AddKey(SfoEntryType.Integer);
    private void bytesUtf8SpecialToolStripMenuItem_Click(object sender, EventArgs e) => AddKey(SfoEntryType.Utf8Special);
  }
}
