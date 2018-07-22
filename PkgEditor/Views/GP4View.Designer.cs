namespace PkgEditor.Views
{
  partial class GP4View
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dirsTreeView = new System.Windows.Forms.TreeView();
            this.filesListView = new System.Windows.Forms.ListView();
            this.textBoxPasscode = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxContentId = new System.Windows.Forms.TextBox();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(3, 98);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dirsTreeView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.filesListView);
            this.splitContainer1.Size = new System.Drawing.Size(697, 317);
            this.splitContainer1.SplitterDistance = 232;
            this.splitContainer1.TabIndex = 0;
            // 
            // dirsTreeView
            // 
            this.dirsTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dirsTreeView.Location = new System.Drawing.Point(0, 0);
            this.dirsTreeView.Name = "dirsTreeView";
            this.dirsTreeView.Size = new System.Drawing.Size(232, 317);
            this.dirsTreeView.TabIndex = 0;
            this.dirsTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.dirsTreeView_AfterSelect);
            // 
            // filesListView
            // 
            this.filesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.filesListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filesListView.FullRowSelect = true;
            this.filesListView.Location = new System.Drawing.Point(0, 0);
            this.filesListView.Name = "filesListView";
            this.filesListView.Size = new System.Drawing.Size(461, 317);
            this.filesListView.TabIndex = 0;
            this.filesListView.UseCompatibleStateImageBehavior = false;
            this.filesListView.View = System.Windows.Forms.View.Details;
            // 
            // textBoxPasscode
            // 
            this.textBoxPasscode.Location = new System.Drawing.Point(3, 55);
            this.textBoxPasscode.Name = "textBoxPasscode";
            this.textBoxPasscode.Size = new System.Drawing.Size(232, 20);
            this.textBoxPasscode.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Passcode (32 chars)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Content ID";
            // 
            // textBoxContentId
            // 
            this.textBoxContentId.Location = new System.Drawing.Point(3, 16);
            this.textBoxContentId.Name = "textBoxContentId";
            this.textBoxContentId.Size = new System.Drawing.Size(275, 20);
            this.textBoxContentId.TabIndex = 3;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 200;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Size";
            this.columnHeader2.Width = 100;
            // 
            // GP4View
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxContentId);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxPasscode);
            this.Controls.Add(this.splitContainer1);
            this.Name = "GP4View";
            this.Size = new System.Drawing.Size(703, 418);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.TreeView dirsTreeView;
    private System.Windows.Forms.ListView filesListView;
    private System.Windows.Forms.TextBox textBoxPasscode;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox textBoxContentId;
    private System.Windows.Forms.ColumnHeader columnHeader1;
    private System.Windows.Forms.ColumnHeader columnHeader2;
  }
}
