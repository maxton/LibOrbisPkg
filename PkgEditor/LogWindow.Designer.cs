namespace PkgEditor
{
  partial class LogWindow
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.logBox = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // logBox
      // 
      this.logBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.logBox.Location = new System.Drawing.Point(0, 0);
      this.logBox.Multiline = true;
      this.logBox.Name = "logBox";
      this.logBox.ReadOnly = true;
      this.logBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.logBox.Size = new System.Drawing.Size(575, 257);
      this.logBox.TabIndex = 0;
      // 
      // LogWindow
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(575, 257);
      this.Controls.Add(this.logBox);
      this.Name = "LogWindow";
      this.ShowIcon = false;
      this.Text = "LogWindow";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    public System.Windows.Forms.TextBox logBox;
  }
}