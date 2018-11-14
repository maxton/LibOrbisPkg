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

    public SFOView(ParamSfo proj) : base()
    {
      InitializeComponent();
      if(proj != null)
      {
        foreach(var param in proj.Values)
        {
          switch(param)
          {
            case Utf8Value v:
              listView1.Items.Add(new ListViewItem(new[] { v.Name, "string", v.Value }));
              break;
            case Utf8SpecialValue v:
              listView1.Items.Add(new ListViewItem(new[] { v.Name, "bytes", v.Value.Select(x => string.Format("{0:X2}", x)).Aggregate((s1, s2) => s1 + s2) }));
              break;
            case IntegerValue v:
              listView1.Items.Add(new ListViewItem(new[] { v.Name, "integer", v.Value.ToString("X8") }));
              break;
          }
        }
      }
    }

    public override bool CanSave => Modified;
    public override bool CanSaveAs => true;
    public override void Save()
    {
    }
    public override void SaveAs()
    {
    }
  }
}
