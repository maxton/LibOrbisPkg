namespace PkgEditor.Views
{
  partial class FileView
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
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileView));
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.directoryTreeView = new System.Windows.Forms.TreeView();
      this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.extractToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.extractCompressedPFSCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.imageList1 = new System.Windows.Forms.ImageList(this.components);
      this.currentFolderListView = new System.Windows.Forms.ListView();
      this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.imageList2 = new System.Windows.Forms.ImageList(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.contextMenuStrip1.SuspendLayout();
      this.SuspendLayout();
      // 
      // splitContainer1
      // 
      this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer1.Location = new System.Drawing.Point(0, 0);
      this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
      this.splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.directoryTreeView);
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.currentFolderListView);
      this.splitContainer1.Size = new System.Drawing.Size(800, 470);
      this.splitContainer1.SplitterDistance = 265;
      this.splitContainer1.SplitterWidth = 5;
      this.splitContainer1.TabIndex = 0;
      // 
      // directoryTreeView
      // 
      this.directoryTreeView.ContextMenuStrip = this.contextMenuStrip1;
      this.directoryTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.directoryTreeView.HideSelection = false;
      this.directoryTreeView.ImageIndex = 1;
      this.directoryTreeView.ImageList = this.imageList1;
      this.directoryTreeView.Location = new System.Drawing.Point(0, 0);
      this.directoryTreeView.Margin = new System.Windows.Forms.Padding(4);
      this.directoryTreeView.Name = "directoryTreeView";
      this.directoryTreeView.SelectedImageIndex = 1;
      this.directoryTreeView.Size = new System.Drawing.Size(265, 470);
      this.directoryTreeView.TabIndex = 0;
      this.directoryTreeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.DirectoryTreeView_BeforeExpand);
      this.directoryTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.DirectoryTreeView_AfterSelect);
      // 
      // contextMenuStrip1
      // 
      this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
      this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extractToolStripMenuItem,
            this.extractCompressedPFSCToolStripMenuItem});
      this.contextMenuStrip1.Name = "contextMenuStrip1";
      this.contextMenuStrip1.Size = new System.Drawing.Size(254, 80);
      this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
      // 
      // extractToolStripMenuItem
      // 
      this.extractToolStripMenuItem.Name = "extractToolStripMenuItem";
      this.extractToolStripMenuItem.Size = new System.Drawing.Size(253, 24);
      this.extractToolStripMenuItem.Text = "Extract";
      this.extractToolStripMenuItem.Click += new System.EventHandler(this.ExtractToolStripMenuItem_Click);
      // 
      // extractCompressedPFSCToolStripMenuItem
      // 
      this.extractCompressedPFSCToolStripMenuItem.Enabled = false;
      this.extractCompressedPFSCToolStripMenuItem.Name = "extractCompressedPFSCToolStripMenuItem";
      this.extractCompressedPFSCToolStripMenuItem.Size = new System.Drawing.Size(253, 24);
      this.extractCompressedPFSCToolStripMenuItem.Text = "Extract compressed (PFSC)";
      this.extractCompressedPFSCToolStripMenuItem.Click += new System.EventHandler(this.extractCompressedPFSCToolStripMenuItem_Click);
      // 
      // imageList1
      // 
      this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
      this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
      this.imageList1.Images.SetKeyName(0, "File_small.png");
      this.imageList1.Images.SetKeyName(1, "Folder_small.png");
      // 
      // currentFolderListView
      // 
      this.currentFolderListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
      this.currentFolderListView.ContextMenuStrip = this.contextMenuStrip1;
      this.currentFolderListView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.currentFolderListView.HideSelection = false;
      this.currentFolderListView.LargeImageList = this.imageList2;
      this.currentFolderListView.Location = new System.Drawing.Point(0, 0);
      this.currentFolderListView.Margin = new System.Windows.Forms.Padding(4);
      this.currentFolderListView.Name = "currentFolderListView";
      this.currentFolderListView.Size = new System.Drawing.Size(530, 470);
      this.currentFolderListView.SmallImageList = this.imageList1;
      this.currentFolderListView.TabIndex = 0;
      this.currentFolderListView.UseCompatibleStateImageBehavior = false;
      this.currentFolderListView.View = System.Windows.Forms.View.Details;
      // 
      // columnHeader1
      // 
      this.columnHeader1.Text = "Name";
      this.columnHeader1.Width = 200;
      // 
      // columnHeader2
      // 
      this.columnHeader2.Text = "Size";
      // 
      // columnHeader3
      // 
      this.columnHeader3.Text = "Comp. Size";
      this.columnHeader3.Width = 75;
      // 
      // imageList2
      // 
      this.imageList2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
      this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
      this.imageList2.Images.SetKeyName(0, "File_large.png");
      this.imageList2.Images.SetKeyName(1, "Folder_large.png");
      // 
      // FileView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.splitContainer1);
      this.Margin = new System.Windows.Forms.Padding(4);
      this.Name = "FileView";
      this.Size = new System.Drawing.Size(800, 470);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.contextMenuStrip1.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.TreeView directoryTreeView;
    private System.Windows.Forms.ListView currentFolderListView;
    private System.Windows.Forms.ImageList imageList1;
    private System.Windows.Forms.ImageList imageList2;
    private System.Windows.Forms.ColumnHeader columnHeader1;
    private System.Windows.Forms.ColumnHeader columnHeader2;
    private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
    private System.Windows.Forms.ToolStripMenuItem extractToolStripMenuItem;
    private System.Windows.Forms.ColumnHeader columnHeader3;
    private System.Windows.Forms.ToolStripMenuItem extractCompressedPFSCToolStripMenuItem;
  }
}
