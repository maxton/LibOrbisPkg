using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace PkgEditor
{
  public partial class LogWindow : Form
  {
    public LogWindow()
    {
      InitializeComponent();
      writer = new LogWriter(logBox);
    }
    private TextWriter writer;

    private class LogWriter : TextWriter
    {
      private TextBox box;
      public LogWriter(TextBox b) { box = b; }
      public override void Write(char value)
      {
        box.Text += value;
      }
      public override void Write(string value)
      {
        box.Text += value;
      }
      public override Encoding Encoding
      {
        get { return Encoding.ASCII; }
      }
    }

    public TextWriter GetWriter() => writer;
  }

}
