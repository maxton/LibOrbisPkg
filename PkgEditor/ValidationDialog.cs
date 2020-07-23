using LibOrbisPkg.GP4;
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
  public partial class ValidationDialog : Form
  {
    public ValidationDialog(List<ValidateResult> results = null)
    {
      InitializeComponent();
      if (results != null)
      {
        fatalLabel.Visible = false;
        foreach (var error in results)
        {
          validateListBox1.Items.Add(error);
          if (error.Type == ValidateResult.ResultType.Fatal)
          {
            ignoreButton.Enabled = false;
            fatalLabel.Visible = true;
          }
        }
      }
    }

    private void ignoreButton_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.Ignore;
      Close();
    }
  }

  public class ValidateListBox : ListBox
  {
    public ValidateListBox()
    {
      DrawMode = DrawMode.OwnerDrawFixed;
      ItemHeight = 54;
    }
    protected override void OnDrawItem(DrawItemEventArgs e)
    {
      const TextFormatFlags flags = TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.WordBreak;

      e.DrawBackground();
      if (Items.Count > e.Index && Items[e.Index] is ValidateResult v)
      {
        var penColor = v.Type == ValidateResult.ResultType.Fatal ? Brushes.Red : Brushes.Yellow;
        e.Graphics.FillRectangle(penColor, 2, e.Bounds.Y + 2, width: ItemHeight / 2 - 4, height: ItemHeight - 4);
        var textRect = e.Bounds;
        textRect.X += ItemHeight / 2;
        textRect.Width -= ItemHeight / 2;
        textRect.Height = ItemHeight / 3;
        string itemText = v.Type.ToString();
        TextRenderer.DrawText(e.Graphics, itemText, new Font(e.Font, FontStyle.Bold), textRect, e.ForeColor, flags);

        textRect.Y += ItemHeight / 3;
        textRect.Height = 2 * ItemHeight / 3;
        itemText = DesignMode ? "Error Description" : v.Message;
        TextRenderer.DrawText(e.Graphics, itemText, e.Font, textRect, e.ForeColor, flags);
      }

      e.DrawFocusRectangle();
    }
  }
}
