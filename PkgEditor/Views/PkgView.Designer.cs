namespace PkgEditor.Views
{
  partial class PkgView
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
      this.tabControl1 = new System.Windows.Forms.TabControl();
      this.headerTab = new System.Windows.Forms.TabPage();
      this.treeView1 = new System.Windows.Forms.TreeView();
      this.entriesTab = new System.Windows.Forms.TabPage();
      this.entriesListView = new System.Windows.Forms.ListView();
      this.columnHeaderId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.columnHeaderSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.columnHeaderOffset = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.columnHeaderEnc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.filesTab = new System.Windows.Forms.TabPage();
      this.label1 = new System.Windows.Forms.Label();
      this.passcodeTextBox = new System.Windows.Forms.TextBox();
      this.button1 = new System.Windows.Forms.Button();
      this.tabPage1 = new System.Windows.Forms.TabPage();
      this.button4 = new System.Windows.Forms.Button();
      this.button3 = new System.Windows.Forms.Button();
      this.button2 = new System.Windows.Forms.Button();
      this.tabControl1.SuspendLayout();
      this.headerTab.SuspendLayout();
      this.entriesTab.SuspendLayout();
      this.filesTab.SuspendLayout();
      this.tabPage1.SuspendLayout();
      this.SuspendLayout();
      // 
      // tabControl1
      // 
      this.tabControl1.Controls.Add(this.headerTab);
      this.tabControl1.Controls.Add(this.entriesTab);
      this.tabControl1.Controls.Add(this.filesTab);
      this.tabControl1.Controls.Add(this.tabPage1);
      this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl1.Location = new System.Drawing.Point(0, 0);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(543, 396);
      this.tabControl1.TabIndex = 1;
      // 
      // headerTab
      // 
      this.headerTab.Controls.Add(this.treeView1);
      this.headerTab.Location = new System.Drawing.Point(4, 22);
      this.headerTab.Name = "headerTab";
      this.headerTab.Padding = new System.Windows.Forms.Padding(3);
      this.headerTab.Size = new System.Drawing.Size(535, 370);
      this.headerTab.TabIndex = 2;
      this.headerTab.Text = "Header";
      this.headerTab.UseVisualStyleBackColor = true;
      // 
      // treeView1
      // 
      this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.treeView1.Location = new System.Drawing.Point(3, 3);
      this.treeView1.Name = "treeView1";
      this.treeView1.Size = new System.Drawing.Size(529, 364);
      this.treeView1.TabIndex = 0;
      // 
      // entriesTab
      // 
      this.entriesTab.Controls.Add(this.entriesListView);
      this.entriesTab.Location = new System.Drawing.Point(4, 22);
      this.entriesTab.Name = "entriesTab";
      this.entriesTab.Padding = new System.Windows.Forms.Padding(3);
      this.entriesTab.Size = new System.Drawing.Size(535, 370);
      this.entriesTab.TabIndex = 0;
      this.entriesTab.Text = "Entries";
      this.entriesTab.UseVisualStyleBackColor = true;
      // 
      // entriesListView
      // 
      this.entriesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderId,
            this.columnHeaderSize,
            this.columnHeaderOffset,
            this.columnHeaderEnc,
            this.columnHeader1});
      this.entriesListView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.entriesListView.FullRowSelect = true;
      this.entriesListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
      this.entriesListView.Location = new System.Drawing.Point(3, 3);
      this.entriesListView.Name = "entriesListView";
      this.entriesListView.Size = new System.Drawing.Size(529, 364);
      this.entriesListView.TabIndex = 7;
      this.entriesListView.UseCompatibleStateImageBehavior = false;
      this.entriesListView.View = System.Windows.Forms.View.Details;
      this.entriesListView.DoubleClick += new System.EventHandler(this.entriesListView_DoubleClick);
      // 
      // columnHeaderId
      // 
      this.columnHeaderId.Text = "ID";
      this.columnHeaderId.Width = 223;
      // 
      // columnHeaderSize
      // 
      this.columnHeaderSize.Text = "Size";
      this.columnHeaderSize.Width = 85;
      // 
      // columnHeaderOffset
      // 
      this.columnHeaderOffset.Text = "Offset";
      this.columnHeaderOffset.Width = 116;
      // 
      // columnHeaderEnc
      // 
      this.columnHeaderEnc.Text = "Enc.";
      this.columnHeaderEnc.Width = 39;
      // 
      // columnHeader1
      // 
      this.columnHeader1.Text = "KeyIdx";
      // 
      // filesTab
      // 
      this.filesTab.Controls.Add(this.label1);
      this.filesTab.Controls.Add(this.passcodeTextBox);
      this.filesTab.Controls.Add(this.button1);
      this.filesTab.Location = new System.Drawing.Point(4, 22);
      this.filesTab.Name = "filesTab";
      this.filesTab.Padding = new System.Windows.Forms.Padding(3);
      this.filesTab.Size = new System.Drawing.Size(535, 370);
      this.filesTab.TabIndex = 1;
      this.filesTab.Text = "Files";
      this.filesTab.UseVisualStyleBackColor = true;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(6, 3);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(303, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "The package\'s passcode is required to decrypt the PFS image:";
      // 
      // passcodeTextBox
      // 
      this.passcodeTextBox.Location = new System.Drawing.Point(9, 19);
      this.passcodeTextBox.MaxLength = 32;
      this.passcodeTextBox.Name = "passcodeTextBox";
      this.passcodeTextBox.Size = new System.Drawing.Size(247, 20);
      this.passcodeTextBox.TabIndex = 1;
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(9, 45);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(75, 23);
      this.button1.TabIndex = 0;
      this.button1.Text = "Open";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // tabPage1
      // 
      this.tabPage1.Controls.Add(this.button4);
      this.tabPage1.Controls.Add(this.button3);
      this.tabPage1.Controls.Add(this.button2);
      this.tabPage1.Location = new System.Drawing.Point(4, 22);
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage1.Size = new System.Drawing.Size(535, 370);
      this.tabPage1.TabIndex = 3;
      this.tabPage1.Text = "Dump";
      this.tabPage1.UseVisualStyleBackColor = true;
      // 
      // button4
      // 
      this.button4.Location = new System.Drawing.Point(6, 64);
      this.button4.Name = "button4";
      this.button4.Size = new System.Drawing.Size(190, 23);
      this.button4.TabIndex = 2;
      this.button4.Text = "Dump Inner PFS";
      this.button4.UseVisualStyleBackColor = true;
      // 
      // button3
      // 
      this.button3.Location = new System.Drawing.Point(6, 35);
      this.button3.Name = "button3";
      this.button3.Size = new System.Drawing.Size(190, 23);
      this.button3.TabIndex = 1;
      this.button3.Text = "Dump Outer PFS (Decrypted)";
      this.button3.UseVisualStyleBackColor = true;
      // 
      // button2
      // 
      this.button2.Location = new System.Drawing.Point(6, 6);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(190, 23);
      this.button2.TabIndex = 0;
      this.button2.Text = "Dump Outer PFS (Encrypted)";
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new System.EventHandler(this.button2_Click);
      // 
      // PkgView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tabControl1);
      this.Name = "PkgView";
      this.Size = new System.Drawing.Size(543, 396);
      this.tabControl1.ResumeLayout(false);
      this.headerTab.ResumeLayout(false);
      this.entriesTab.ResumeLayout(false);
      this.filesTab.ResumeLayout(false);
      this.filesTab.PerformLayout();
      this.tabPage1.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage headerTab;
    private System.Windows.Forms.TreeView treeView1;
    private System.Windows.Forms.TabPage entriesTab;
    private System.Windows.Forms.TabPage filesTab;
    private System.Windows.Forms.ListView entriesListView;
    private System.Windows.Forms.ColumnHeader columnHeaderId;
    private System.Windows.Forms.ColumnHeader columnHeaderSize;
    private System.Windows.Forms.ColumnHeader columnHeaderOffset;
    private System.Windows.Forms.ColumnHeader columnHeaderEnc;
    private System.Windows.Forms.ColumnHeader columnHeader1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox passcodeTextBox;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.TabPage tabPage1;
    private System.Windows.Forms.Button button4;
    private System.Windows.Forms.Button button3;
    private System.Windows.Forms.Button button2;
  }
}
