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
    private int attributeFlags = 0;
    private int attribute2Flags = 0;
    private Value selectedValue = null;

    public bool Modified
    {
      get => modified;
      set
      {
        if (value != modified)
        {
          modified = value;
          var pathTxt = path != null ? Path.GetFileName(path) : "New file";
          if (modified)
            Parent.Text = "*" + pathTxt;
          else
            Parent.Text = pathTxt + (@readonly ? " [Read-Only]" : "");
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
      TableEditor_Init();
      TableEditor_PopulateInput(null);
      GuidedEditor_Init();
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
            Parent.Text = Path.GetFileName(path);
            @readonly = false;
            Modified = false;
            TableEditor_Init();
            GuidedEditor_Init();
            TableEditor_PopulateInput(selectedValue);
          }
        }
      }
    }

    private void ProjectWasModified(object sender = null)
    {
      Modified = true;
      TableEditor_Init();
      TableEditor_PopulateInput(selectedValue, sender);
      GuidedEditor_Reload(sender);
    }

    private void TableEditor_Init()
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
            listView1.Items.Add(new ListViewItem(new[] { v.Name, "utf8", v.Length.ToString(), v.MaxLength.ToString(), v.Value })
            {
              Tag = param
            });
            break;
          case Utf8SpecialValue v:
            listView1.Items.Add(new ListViewItem(new[] { v.Name, "utf8_special", v.Length.ToString(), v.MaxLength.ToString(), v.Value.Select(x => string.Format("{0:X2}", x)).DefaultIfEmpty("").Aggregate((s1, s2) => s1 + s2) })
            {
              Tag = param
            });
            break;
          case IntegerValue v:
            listView1.Items.Add(new ListViewItem(new[] { v.Name, "integer", "4", "4", v.Value.ToString("X8") })
            {
              Tag = param
            });
            break;
        }
      }
      loaded = true;
    }

    /// <summary>
    /// Populates the input of the table editor to the given value.
    /// While the input is being updated, the loaded variable is set to false.
    /// </summary>
    /// <param name="v">The value to edit. Use null to clear the editor.</param>
    /// <param name="sender">The given control will not have its value updated.</param>
    private void TableEditor_PopulateInput(Value v, object sender = null)
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

    private void TableEditor_ClearInput()
    {
      TableEditor_PopulateInput(null);
    }

    private void TableEditor_AddKey(SfoEntryType type)
    {
      if (@readonly) return;
      var name = "NEW_KEY";
      while (proj.GetValueByName(name) != null)
      {
        name += "_";
      }
      Value val;
      switch (type)
      {
        case SfoEntryType.Utf8Special:
          val = new Utf8SpecialValue(name, "", 0);
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
      ProjectWasModified();
    }

    private void TableEditor_DeleteSelected()
    {
      if (@readonly) return;
      if (listView1.SelectedItems.Count > 0)
      {
        foreach (var item in listView1.SelectedItems)
        {
          if ((item as ListViewItem).Tag is Value v)
          {
            proj.Values.Remove(v);
            if(v == selectedValue)
            {
              // This is required to clear the value editor
              selectedValue = null;
            }
          }
        }
        ProjectWasModified();
      }
    }

    private void TableEditor_UpdateValue(object sender, EventArgs e)
    {
      if (!loaded) return;
      Value newValue = null;
      switch (typeDropDown.SelectedIndex)
      {
        case 0:
          newValue = Value.Create(nameTextBox.Text, SfoEntryType.Utf8, valueTextBox.Text, (int)maxLengthInput.Value);
          break;
        case 1:
          newValue = Value.Create(nameTextBox.Text, SfoEntryType.Integer, valueTextBox.Text);
          break;
        case 2:
          newValue = new Utf8SpecialValue(nameTextBox.Text, valueTextBox.Text, (int)maxLengthInput.Value);
          break;
      }
      if (newValue != null)
      {
        proj[selectedValue.Name] = null;
        selectedValue = proj[newValue.Name] = newValue;

      }
      ProjectWasModified(sender);
    }

    private void GuidedEditor_Init()
    {
      sfoTypeCombobox.Enabled = !@readonly;
      contentIdTextBox.ReadOnly = titleTextBox.ReadOnly = appVersionTextBox.ReadOnly = versionTextBox.ReadOnly = @readonly;
      attributes2Enable.Enabled = !@readonly;
      attributesListBox.Items.Clear();
      for (var i = 0; i < 32; i++)
      {
        attributesListBox.Items.Add(SfoData.AttributeNames[i], false);
        attributes2ListBox.Items.Add(SfoData.Attribute2Names[i], false);
      }
      sfoTypeCombobox.Items.Clear();
      foreach (var t in SfoData.SfoTypes)
      {
        sfoTypeCombobox.Items.Add(t);
      }
      appTypeComboBox.Items.Clear();
      foreach (var t in SfoData.AppTypes)
      {
        appTypeComboBox.Items.Add(t);
      }
      downloadSizeComboBox.Items.Clear();
      foreach (var d in SfoData.DownloadSizes)
      {
        downloadSizeComboBox.Items.Add(d);
      }
      GuidedEditor_Reload();
    }

    private void GuidedEditor_Reload(object sender = null)
    {
      loaded = false;
      if (sender != attributesListBox)
      {
        attributeFlags = proj["ATTRIBUTE"] is IntegerValue a ? a.Value : 0;
        for (int i = 0; i < 32; i++)
        {
          attributesListBox.SetItemChecked(i, (attributeFlags & (1 << i)) == (1 << i));
        }
      }
      if (sender != attributes2ListBox)
      {
        attribute2Flags = proj["ATTRIBUTE2"] is IntegerValue a ? a.Value : 0;
        for (int i = 0; i < 32; i++)
        {
          attributes2ListBox.SetItemChecked(i, (attribute2Flags & (1 << i)) == (1 << i));
        }
      }
      if (sender != contentIdTextBox)
      {
        contentIdTextBox.Text = proj["CONTENT_ID"] is Utf8Value contentId ? contentId.Value : "";
      }
      if (sender != sfoTypeCombobox)
      {
        sfoTypeCombobox.SelectedItem = SfoData.SfoTypes.FirstOrDefault(x => x.Category == proj["CATEGORY"]?.ToString());
      }
      if (sender != titleTextBox)
      {
        titleTextBox.Text = proj["TITLE"] is Utf8Value t ? t.Value : "";
      }
      downloadSizeComboBox.Enabled
        = appTypeComboBox.Enabled 
        = appVersionTextBox.Enabled 
        = proj["CATEGORY"] is Utf8Value c ? c.Value.StartsWith("g") : false;
      if (sender != appVersionTextBox)
      {
        appVersionTextBox.Text = proj["APP_VER"] is Utf8Value a ? a.Value : "";
      }
      if (sender != versionTextBox)
      {
        versionTextBox.Text = proj["VERSION"] is Utf8Value v ? v.Value : "";
      }
      if (sender != appTypeComboBox)
      {
        appTypeComboBox.SelectedIndex = proj["APP_TYPE"] is IntegerValue a ? a.Value : 0;
      }
      if (sender != downloadSizeComboBox)
      {
        var index = proj["DOWNLOAD_DATA_SIZE"] is IntegerValue d && d.Value > 63 ? (int)Math.Log(d.Value / 64, 2) + 1 : 0;
        downloadSizeComboBox.SelectedIndex = index >= 0 && index <= 5 ? index : 0;
      }
      attributes2Enable.Checked = proj["ATTRIBUTE2"] != null;
      loaded = true;
    }

    private void GuidedEditor_Changed(object sender, EventArgs e)
    {
      if (!loaded || @readonly) return;
      if (contentIdTextBox.Text.Length == 36)
      {
        proj.SetValue("CONTENT_ID", SfoEntryType.Utf8, contentIdTextBox.Text, 48);
        proj.SetValue("TITLE_ID", SfoEntryType.Utf8, contentIdTextBox.Text.Substring(7, 9), 12);
      }
      proj.SetValue("TITLE", SfoEntryType.Utf8, titleTextBox.Text, 128);
      if (sfoTypeCombobox.SelectedItem is SfoType st)
      {
        proj.SetValue("CATEGORY", SfoEntryType.Utf8, st.Category, 4);
        appTypeComboBox.Enabled = appVersionTextBox.Enabled = st.Category.StartsWith("g");
      }
      proj.SetValue("ATTRIBUTE", SfoEntryType.Integer, attributeFlags.ToString());
      if (attributes2Enable.Checked)
        proj.SetValue("ATTRIBUTE2", SfoEntryType.Integer, attribute2Flags.ToString());
      else
        proj["ATTRIBUTE2"] = null;
      if (appVersionTextBox.Enabled)
        proj.SetValue("APP_VER", SfoEntryType.Utf8, appVersionTextBox.Text, 8);
      else
        proj["APP_VER"] = null;
      if (appTypeComboBox.Enabled)
        proj.SetValue("APP_TYPE", SfoEntryType.Integer, appTypeComboBox.SelectedIndex.ToString());
      else
        proj["APP_TYPE"] = null;
      if (downloadSizeComboBox.Enabled)
        proj.SetValue("DOWNLOAD_DATA_SIZE", SfoEntryType.Integer, $"{64 * (1 << (downloadSizeComboBox.SelectedIndex - 1))}");
      proj.SetValue("VERSION", SfoEntryType.Utf8, versionTextBox.Text, 8);
      ProjectWasModified(sender);
    }

    private void AttributesListBox_ItemCheck(object sender, ItemCheckEventArgs e)
    {
      if (!loaded) return;
      // Disable edits in readonly mode. 
      // We can't simply use the Enabled property because that breaks scrolling.
      if (@readonly)
      {
        e.NewValue = e.CurrentValue;
        return;
      }
      var flag = 1 << e.Index;
      int attributes = sender == attributesListBox ? attributeFlags : attribute2Flags;
      if (e.NewValue == CheckState.Checked)
        // Set flag
        attributes |= flag;
      else
        // Unset flag
        attributes &= ~flag;

      if (sender == attributesListBox)
        attributeFlags = attributes;
      else
        attribute2Flags = attributes;
      GuidedEditor_Changed(sender, null);
    }


    private void loadACDefaultsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      proj = new ParamSfo() { Values = ParamSfo.DefaultAC.Values.ToList() };
      ProjectWasModified();
    }

    private void loadGPDefaultsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      proj = new ParamSfo() { Values = ParamSfo.DefaultGD.Values.ToList() };
      ProjectWasModified();
    }

    private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
    {
      if (listView1.SelectedItems.Count == 1)
      {
        TableEditor_PopulateInput(listView1.SelectedItems[0].Tag as Value);
      }
      else
      {
        TableEditor_ClearInput();
      }
    }

    private void listView1_KeyUp(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == System.Windows.Forms.Keys.Delete)
      {
        TableEditor_DeleteSelected();
      }
    }

    private void stringToolStripMenuItem_Click(object sender, EventArgs e) => TableEditor_AddKey(SfoEntryType.Utf8);
    private void intToolStripMenuItem_Click(object sender, EventArgs e) => TableEditor_AddKey(SfoEntryType.Integer);
    private void bytesUtf8SpecialToolStripMenuItem_Click(object sender, EventArgs e) => TableEditor_AddKey(SfoEntryType.Utf8Special);

    private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
    {
      // Move the table editor panel to the current tab
      if (tabControl1.SelectedIndex == 0)
      {
        tableEditorPage.Controls.Add(editorPanel);
      }
      else
      {
        guidedEditorBottomPanel.Controls.Add(editorPanel);
      }
    }

    private void DeleteToolStripMenuItem_Click(object sender, EventArgs e) => TableEditor_DeleteSelected();
  }
}
