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
            this.filesTab = new System.Windows.Forms.TabPage();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabControl1.SuspendLayout();
            this.headerTab.SuspendLayout();
            this.entriesTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.headerTab);
            this.tabControl1.Controls.Add(this.entriesTab);
            this.tabControl1.Controls.Add(this.filesTab);
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
            // filesTab
            // 
            this.filesTab.Location = new System.Drawing.Point(4, 22);
            this.filesTab.Name = "filesTab";
            this.filesTab.Padding = new System.Windows.Forms.Padding(3);
            this.filesTab.Size = new System.Drawing.Size(535, 370);
            this.filesTab.TabIndex = 1;
            this.filesTab.Text = "Files";
            this.filesTab.UseVisualStyleBackColor = true;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "KeyIdx";
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
  }
}
