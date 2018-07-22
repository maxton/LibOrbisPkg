using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PkgEditor.Views
{
  public class View : UserControl
  {
    public MainWin mainWin;
    public event EventHandler SaveStatusChanged;
    protected void OnSaveStatusChanged() => SaveStatusChanged(this, new EventArgs());
    public virtual bool CanSave { get; }
    public virtual bool CanSaveAs { get; }
    public virtual void Save() { }
    public virtual void SaveAs() { }
  }
}
