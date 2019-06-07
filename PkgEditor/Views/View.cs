using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PkgEditor.Views
{
  public class View : UserControl
  {
    /// <summary>
    /// A reference to the main window owning this view control.
    /// </summary>
    public MainWin mainWin;

    /// <summary>
    /// The Main window should subscribe to this event to get notified when the document is modified.
    /// </summary>
    public event EventHandler SaveStatusChanged;

    /// <summary>
    /// This method should be called by an overloading class when the document has been modified, so the UI can update the Save/As buttons.
    /// </summary>
    protected void OnSaveStatusChanged() => SaveStatusChanged?.Invoke(this, new EventArgs());

    /// <summary>
    /// This should return true if the current document can be File->saved with Ctrl-S.
    /// </summary>
    public virtual bool CanSave { get; }

    /// <summary>
    /// This should return true if the current document can be File->save-as'd with Ctrl-Shift-S.
    /// </summary>
    public virtual bool CanSaveAs { get; }

    /// <summary>
    /// This method is called when the user presses Ctrl-S or clicks File->Save
    /// </summary>
    public virtual void Save() { }

    /// <summary>
    /// This method is called when the user presses Ctrl-Shift-S or clicks File->Save As...
    /// </summary>
    public virtual void SaveAs() { }

    /// <summary>
    /// This method is called when the user presse Ctrl-W or clicks File->Close
    /// </summary>
    public virtual void Close() { }
  }
}
