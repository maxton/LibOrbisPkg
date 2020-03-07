namespace PkgEditor.Views
{
  partial class PFSView
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.fileView1 = new PkgEditor.Views.FileView();
      this.SuspendLayout();
      // 
      // fileView1
      // 
      this.fileView1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.fileView1.Location = new System.Drawing.Point(0, 0);
      this.fileView1.Name = "fileView1";
      this.fileView1.Size = new System.Drawing.Size(557, 354);
      this.fileView1.TabIndex = 0;
      // 
      // PFSView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.fileView1);
      this.Name = "PFSView";
      this.Size = new System.Drawing.Size(557, 354);
      this.ResumeLayout(false);

    }

    #endregion

    private FileView fileView1;
  }
}
