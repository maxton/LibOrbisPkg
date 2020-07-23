namespace PkgEditor
{
  partial class ValidationDialog
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
      this.label1 = new System.Windows.Forms.Label();
      this.fatalLabel = new System.Windows.Forms.Label();
      this.ignoreButton = new System.Windows.Forms.Button();
      this.cancelButton = new System.Windows.Forms.Button();
      this.validateListBox1 = new PkgEditor.ValidateListBox();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.label1.Location = new System.Drawing.Point(0, 0);
      this.label1.Name = "label1";
      this.label1.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
      this.label1.Size = new System.Drawing.Size(594, 39);
      this.label1.TabIndex = 1;
      this.label1.Text = "The GP4 project has failed validation.\r\nThis means there could be problems when b" +
    "uilding or running the target PKG file.";
      this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      // 
      // fatalLabel
      // 
      this.fatalLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.fatalLabel.Location = new System.Drawing.Point(0, 313);
      this.fatalLabel.Name = "fatalLabel";
      this.fatalLabel.Size = new System.Drawing.Size(594, 24);
      this.fatalLabel.TabIndex = 2;
      this.fatalLabel.Text = "Please resolve all fatal errors before building the PKG.";
      this.fatalLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      // 
      // ignoreButton
      // 
      this.ignoreButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.ignoreButton.Location = new System.Drawing.Point(297, 287);
      this.ignoreButton.Name = "ignoreButton";
      this.ignoreButton.Size = new System.Drawing.Size(174, 23);
      this.ignoreButton.TabIndex = 3;
      this.ignoreButton.Text = "&Ignore and Continue";
      this.ignoreButton.UseVisualStyleBackColor = true;
      this.ignoreButton.Click += new System.EventHandler(this.ignoreButton_Click);
      // 
      // cancelButton
      // 
      this.cancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancelButton.Location = new System.Drawing.Point(117, 287);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(174, 23);
      this.cancelButton.TabIndex = 4;
      this.cancelButton.Text = "&Cancel Build";
      this.cancelButton.UseVisualStyleBackColor = true;
      // 
      // validateListBox1
      // 
      this.validateListBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.validateListBox1.FormattingEnabled = true;
      this.validateListBox1.IntegralHeight = false;
      this.validateListBox1.Location = new System.Drawing.Point(3, 42);
      this.validateListBox1.Name = "validateListBox1";
      this.validateListBox1.ScrollAlwaysVisible = true;
      this.validateListBox1.Size = new System.Drawing.Size(591, 239);
      this.validateListBox1.TabIndex = 5;
      // 
      // ValidationDialog
      // 
      this.AcceptButton = this.ignoreButton;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.cancelButton;
      this.ClientSize = new System.Drawing.Size(594, 337);
      this.Controls.Add(this.validateListBox1);
      this.Controls.Add(this.cancelButton);
      this.Controls.Add(this.ignoreButton);
      this.Controls.Add(this.fatalLabel);
      this.Controls.Add(this.label1);
      this.Name = "ValidationDialog";
      this.Text = "PKG Build Issues";
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label fatalLabel;
    private System.Windows.Forms.Button ignoreButton;
    private System.Windows.Forms.Button cancelButton;
    private ValidateListBox validateListBox1;
  }
}