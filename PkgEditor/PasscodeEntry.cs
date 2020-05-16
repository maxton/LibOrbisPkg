using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PkgEditor
{
  public partial class PasscodeEntry : Form
  {
    public PasscodeEntry(string prompt = "Please enter the package's passcode.", int length = 32)
    {
      InitializeComponent();
      label1.Text = prompt;
      textBox1.MaxLength = length;
    }
    public string Passcode => textBox1.Text;
  }
}
