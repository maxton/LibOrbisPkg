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
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GP4View));
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.dirsTreeView = new System.Windows.Forms.TreeView();
      this.filesListView = new System.Windows.Forms.ListView();
      this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.fileViewContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.newFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.folderFileIcons = new System.Windows.Forms.ImageList(this.components);
      this.passcodeTextBox = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.contentIdTextBox = new System.Windows.Forms.TextBox();
      this.buildPfsButton = new System.Windows.Forms.Button();
      this.buildPkgButton = new System.Windows.Forms.Button();
      this.label3 = new System.Windows.Forms.Label();
      this.entitlementKeyTextbox = new System.Windows.Forms.TextBox();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.fileViewContextMenu.SuspendLayout();
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
      this.dirsTreeView.HideSelection = false;
      this.dirsTreeView.Location = new System.Drawing.Point(0, 0);
      this.dirsTreeView.Name = "dirsTreeView";
      this.dirsTreeView.Size = new System.Drawing.Size(232, 317);
      this.dirsTreeView.TabIndex = 0;
      this.dirsTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.dirsTreeView_AfterSelect);
      // 
      // filesListView
      // 
      this.filesListView.AllowDrop = true;
      this.filesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
      this.filesListView.ContextMenuStrip = this.fileViewContextMenu;
      this.filesListView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.filesListView.FullRowSelect = true;
      this.filesListView.LabelEdit = true;
      this.filesListView.Location = new System.Drawing.Point(0, 0);
      this.filesListView.Name = "filesListView";
      this.filesListView.Size = new System.Drawing.Size(461, 317);
      this.filesListView.SmallImageList = this.folderFileIcons;
      this.filesListView.TabIndex = 0;
      this.filesListView.UseCompatibleStateImageBehavior = false;
      this.filesListView.View = System.Windows.Forms.View.Details;
      this.filesListView.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.filesListView_AfterLabelEdit);
      this.filesListView.DragDrop += new System.Windows.Forms.DragEventHandler(this.filesListView_DragDrop);
      this.filesListView.DragEnter += new System.Windows.Forms.DragEventHandler(this.filesListView_DragEnter);
      this.filesListView.DoubleClick += new System.EventHandler(this.filesListView_DoubleClick);
      // 
      // columnHeader1
      // 
      this.columnHeader1.Text = "Name";
      this.columnHeader1.Width = 219;
      // 
      // columnHeader2
      // 
      this.columnHeader2.Text = "Source";
      this.columnHeader2.Width = 144;
      // 
      // fileViewContextMenu
      // 
      this.fileViewContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renameToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.newFolderToolStripMenuItem});
      this.fileViewContextMenu.Name = "contextMenuStrip1";
      this.fileViewContextMenu.Size = new System.Drawing.Size(135, 70);
      this.fileViewContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.fileViewContextMenu_Opening);
      // 
      // renameToolStripMenuItem
      // 
      this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
      this.renameToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
      this.renameToolStripMenuItem.Text = "Rename";
      this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
      // 
      // deleteToolStripMenuItem
      // 
      this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
      this.deleteToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
      this.deleteToolStripMenuItem.Text = "Delete";
      this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
      // 
      // newFolderToolStripMenuItem
      // 
      this.newFolderToolStripMenuItem.Name = "newFolderToolStripMenuItem";
      this.newFolderToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
      this.newFolderToolStripMenuItem.Text = "New Folder";
      this.newFolderToolStripMenuItem.Click += new System.EventHandler(this.newFolderToolStripMenuItem_Click);
      // 
      // folderFileIcons
      // 
      this.folderFileIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("folderFileIcons.ImageStream")));
      this.folderFileIcons.TransparentColor = System.Drawing.Color.Transparent;
      this.folderFileIcons.Images.SetKeyName(0, "Folder_small.png");
      this.folderFileIcons.Images.SetKeyName(1, "File_small.png");
      // 
      // passcodeTextBox
      // 
      this.passcodeTextBox.Location = new System.Drawing.Point(3, 55);
      this.passcodeTextBox.MaxLength = 32;
      this.passcodeTextBox.Name = "passcodeTextBox";
      this.passcodeTextBox.Size = new System.Drawing.Size(232, 20);
      this.passcodeTextBox.TabIndex = 1;
      this.passcodeTextBox.TextChanged += new System.EventHandler(this.propertyChanged);
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
      this.label2.Size = new System.Drawing.Size(108, 13);
      this.label2.TabIndex = 4;
      this.label2.Text = "Content ID (36 chars)";
      // 
      // contentIdTextBox
      // 
      this.contentIdTextBox.Location = new System.Drawing.Point(3, 16);
      this.contentIdTextBox.MaxLength = 36;
      this.contentIdTextBox.Name = "contentIdTextBox";
      this.contentIdTextBox.Size = new System.Drawing.Size(275, 20);
      this.contentIdTextBox.TabIndex = 3;
      this.contentIdTextBox.TextChanged += new System.EventHandler(this.propertyChanged);
      // 
      // buildPfsButton
      // 
      this.buildPfsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.buildPfsButton.Location = new System.Drawing.Point(625, 3);
      this.buildPfsButton.Name = "buildPfsButton";
      this.buildPfsButton.Size = new System.Drawing.Size(75, 49);
      this.buildPfsButton.TabIndex = 5;
      this.buildPfsButton.Text = "Build PFS";
      this.buildPfsButton.UseVisualStyleBackColor = true;
      this.buildPfsButton.Click += new System.EventHandler(this.buildPfs_Click);
      // 
      // buildPkgButton
      // 
      this.buildPkgButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.buildPkgButton.Location = new System.Drawing.Point(544, 3);
      this.buildPkgButton.Name = "buildPkgButton";
      this.buildPkgButton.Size = new System.Drawing.Size(75, 49);
      this.buildPkgButton.TabIndex = 6;
      this.buildPkgButton.Text = "Build PKG";
      this.buildPkgButton.UseVisualStyleBackColor = true;
      this.buildPkgButton.Click += new System.EventHandler(this.buildPkg_Click);
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(238, 39);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(150, 13);
      this.label3.TabIndex = 8;
      this.label3.Text = "Entitlement Key (32 hex chars)";
      // 
      // entitlementKeyTextbox
      // 
      this.entitlementKeyTextbox.Location = new System.Drawing.Point(241, 55);
      this.entitlementKeyTextbox.MaxLength = 32;
      this.entitlementKeyTextbox.Name = "entitlementKeyTextbox";
      this.entitlementKeyTextbox.Size = new System.Drawing.Size(232, 20);
      this.entitlementKeyTextbox.TabIndex = 7;
      this.entitlementKeyTextbox.TextChanged += new System.EventHandler(this.propertyChanged);
      // 
      // GP4View
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.label3);
      this.Controls.Add(this.entitlementKeyTextbox);
      this.Controls.Add(this.buildPkgButton);
      this.Controls.Add(this.buildPfsButton);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.contentIdTextBox);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.passcodeTextBox);
      this.Controls.Add(this.splitContainer1);
      this.Name = "GP4View";
      this.Size = new System.Drawing.Size(703, 418);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.fileViewContextMenu.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.TreeView dirsTreeView;
    private System.Windows.Forms.ListView filesListView;
    private System.Windows.Forms.TextBox passcodeTextBox;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox contentIdTextBox;
    private System.Windows.Forms.ColumnHeader columnHeader1;
    private System.Windows.Forms.ColumnHeader columnHeader2;
    private System.Windows.Forms.Button buildPfsButton;
    private System.Windows.Forms.ImageList folderFileIcons;
    private System.Windows.Forms.ContextMenuStrip fileViewContextMenu;
    private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem newFolderToolStripMenuItem;
    private System.Windows.Forms.Button buildPkgButton;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox entitlementKeyTextbox;
  }
}
