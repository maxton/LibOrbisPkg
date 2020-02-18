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
      this.imageList1 = new System.Windows.Forms.ImageList(this.components);
      this.filesListView = new System.Windows.Forms.ListView();
      this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.fileViewContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.addFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.newFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.passcodeTextBox = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.contentIdTextBox = new System.Windows.Forms.TextBox();
      this.buildPfsButton = new System.Windows.Forms.Button();
      this.buildPkgButton = new System.Windows.Forms.Button();
      this.label3 = new System.Windows.Forms.Label();
      this.entitlementKeyTextbox = new System.Windows.Forms.TextBox();
      this.pkgTypeDropdown = new System.Windows.Forms.ComboBox();
      this.label4 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.volumeTimestampPicker = new System.Windows.Forms.DateTimePicker();
      this.label6 = new System.Windows.Forms.Label();
      this.creationTimePicker = new System.Windows.Forms.DateTimePicker();
      this.useTimeOfBuildCheckBox = new System.Windows.Forms.CheckBox();
      this.includeTimeCheckBox = new System.Windows.Forms.CheckBox();
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
      this.splitContainer1.Location = new System.Drawing.Point(3, 125);
      this.splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.dirsTreeView);
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.filesListView);
      this.splitContainer1.Size = new System.Drawing.Size(694, 272);
      this.splitContainer1.SplitterDistance = 230;
      this.splitContainer1.TabIndex = 0;
      // 
      // dirsTreeView
      // 
      this.dirsTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dirsTreeView.HideSelection = false;
      this.dirsTreeView.ImageIndex = 0;
      this.dirsTreeView.ImageList = this.imageList1;
      this.dirsTreeView.Location = new System.Drawing.Point(0, 0);
      this.dirsTreeView.Name = "dirsTreeView";
      this.dirsTreeView.SelectedImageIndex = 0;
      this.dirsTreeView.Size = new System.Drawing.Size(230, 272);
      this.dirsTreeView.TabIndex = 0;
      this.dirsTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.dirsTreeView_AfterSelect);
      // 
      // imageList1
      // 
      this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
      this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
      this.imageList1.Images.SetKeyName(0, "Folder_small.png");
      this.imageList1.Images.SetKeyName(1, "File_small.png");
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
      this.filesListView.HideSelection = false;
      this.filesListView.LabelEdit = true;
      this.filesListView.Location = new System.Drawing.Point(0, 0);
      this.filesListView.Name = "filesListView";
      this.filesListView.Size = new System.Drawing.Size(460, 272);
      this.filesListView.SmallImageList = this.imageList1;
      this.filesListView.TabIndex = 0;
      this.filesListView.UseCompatibleStateImageBehavior = false;
      this.filesListView.View = System.Windows.Forms.View.Details;
      this.filesListView.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.filesListView_AfterLabelEdit);
      this.filesListView.DragDrop += new System.Windows.Forms.DragEventHandler(this.filesListView_DragDrop);
      this.filesListView.DragEnter += new System.Windows.Forms.DragEventHandler(this.filesListView_DragEnter);
      this.filesListView.DoubleClick += new System.EventHandler(this.filesListView_DoubleClick);
      this.filesListView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.filesListView_KeyUp);
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
            this.addFilesToolStripMenuItem,
            this.newFolderToolStripMenuItem,
            this.toolStripSeparator1,
            this.deleteToolStripMenuItem});
      this.fileViewContextMenu.Name = "contextMenuStrip1";
      this.fileViewContextMenu.Size = new System.Drawing.Size(135, 98);
      this.fileViewContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.fileViewContextMenu_Opening);
      // 
      // renameToolStripMenuItem
      // 
      this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
      this.renameToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
      this.renameToolStripMenuItem.Text = "Rename";
      this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
      // 
      // addFilesToolStripMenuItem
      // 
      this.addFilesToolStripMenuItem.Name = "addFilesToolStripMenuItem";
      this.addFilesToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
      this.addFilesToolStripMenuItem.Text = "Add files...";
      this.addFilesToolStripMenuItem.Click += new System.EventHandler(this.addFilesToolStripMenuItem_Click);
      // 
      // newFolderToolStripMenuItem
      // 
      this.newFolderToolStripMenuItem.Name = "newFolderToolStripMenuItem";
      this.newFolderToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
      this.newFolderToolStripMenuItem.Text = "New Folder";
      this.newFolderToolStripMenuItem.Click += new System.EventHandler(this.newFolderToolStripMenuItem_Click);
      // 
      // toolStripSeparator1
      // 
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(131, 6);
      // 
      // deleteToolStripMenuItem
      // 
      this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
      this.deleteToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
      this.deleteToolStripMenuItem.Text = "Delete";
      this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
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
      this.buildPfsButton.Location = new System.Drawing.Point(622, 55);
      this.buildPfsButton.Name = "buildPfsButton";
      this.buildPfsButton.Size = new System.Drawing.Size(75, 21);
      this.buildPfsButton.TabIndex = 5;
      this.buildPfsButton.Text = "Build PFS";
      this.buildPfsButton.UseVisualStyleBackColor = true;
      this.buildPfsButton.Click += new System.EventHandler(this.buildPfs_Click);
      // 
      // buildPkgButton
      // 
      this.buildPkgButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.buildPkgButton.Location = new System.Drawing.Point(622, 3);
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
      this.label3.Location = new System.Drawing.Point(3, 78);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(195, 13);
      this.label3.TabIndex = 8;
      this.label3.Text = "Entitlement Key (32 hex chars) (AC only)";
      // 
      // entitlementKeyTextbox
      // 
      this.entitlementKeyTextbox.Location = new System.Drawing.Point(3, 94);
      this.entitlementKeyTextbox.MaxLength = 32;
      this.entitlementKeyTextbox.Name = "entitlementKeyTextbox";
      this.entitlementKeyTextbox.Size = new System.Drawing.Size(232, 20);
      this.entitlementKeyTextbox.TabIndex = 7;
      this.entitlementKeyTextbox.TextChanged += new System.EventHandler(this.propertyChanged);
      // 
      // pkgTypeDropdown
      // 
      this.pkgTypeDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.pkgTypeDropdown.FormattingEnabled = true;
      this.pkgTypeDropdown.Items.AddRange(new object[] {
            "Game Package",
            "Additional Content",
            "Additional Content w/ No Data"});
      this.pkgTypeDropdown.Location = new System.Drawing.Point(284, 16);
      this.pkgTypeDropdown.Name = "pkgTypeDropdown";
      this.pkgTypeDropdown.Size = new System.Drawing.Size(186, 21);
      this.pkgTypeDropdown.TabIndex = 9;
      this.pkgTypeDropdown.SelectedIndexChanged += new System.EventHandler(this.pkgTypeDropdown_SelectedIndexChanged);
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(281, 0);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(77, 13);
      this.label4.TabIndex = 10;
      this.label4.Text = "Package Type";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(281, 40);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(96, 13);
      this.label5.TabIndex = 11;
      this.label5.Text = "Volume Timestamp";
      // 
      // volumeTimestampPicker
      // 
      this.volumeTimestampPicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
      this.volumeTimestampPicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
      this.volumeTimestampPicker.Location = new System.Drawing.Point(284, 55);
      this.volumeTimestampPicker.Name = "volumeTimestampPicker";
      this.volumeTimestampPicker.Size = new System.Drawing.Size(186, 20);
      this.volumeTimestampPicker.TabIndex = 12;
      this.volumeTimestampPicker.Value = new System.DateTime(2018, 12, 25, 0, 0, 0, 0);
      this.volumeTimestampPicker.ValueChanged += new System.EventHandler(this.volumeTimestampPicker_ValueChanged);
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(281, 78);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(72, 13);
      this.label6.TabIndex = 13;
      this.label6.Text = "Creation Date";
      // 
      // creationTimePicker
      // 
      this.creationTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
      this.creationTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
      this.creationTimePicker.Location = new System.Drawing.Point(284, 91);
      this.creationTimePicker.Name = "creationTimePicker";
      this.creationTimePicker.Size = new System.Drawing.Size(186, 20);
      this.creationTimePicker.TabIndex = 14;
      this.creationTimePicker.Value = new System.DateTime(2018, 12, 25, 0, 0, 0, 0);
      this.creationTimePicker.ValueChanged += new System.EventHandler(this.creationTimePicker_ValueChanged);
      // 
      // useTimeOfBuildCheckBox
      // 
      this.useTimeOfBuildCheckBox.AutoSize = true;
      this.useTimeOfBuildCheckBox.Location = new System.Drawing.Point(476, 91);
      this.useTimeOfBuildCheckBox.Name = "useTimeOfBuildCheckBox";
      this.useTimeOfBuildCheckBox.Size = new System.Drawing.Size(110, 17);
      this.useTimeOfBuildCheckBox.TabIndex = 16;
      this.useTimeOfBuildCheckBox.Text = "Use time of build?";
      this.useTimeOfBuildCheckBox.UseVisualStyleBackColor = true;
      this.useTimeOfBuildCheckBox.CheckedChanged += new System.EventHandler(this.creationDateCheckBox_CheckedChanged);
      // 
      // includeTimeCheckBox
      // 
      this.includeTimeCheckBox.AutoSize = true;
      this.includeTimeCheckBox.Location = new System.Drawing.Point(476, 106);
      this.includeTimeCheckBox.Name = "includeTimeCheckBox";
      this.includeTimeCheckBox.Size = new System.Drawing.Size(89, 17);
      this.includeTimeCheckBox.TabIndex = 17;
      this.includeTimeCheckBox.Text = "Include time?";
      this.includeTimeCheckBox.UseVisualStyleBackColor = true;
      this.includeTimeCheckBox.CheckedChanged += new System.EventHandler(this.includeTimeCheckBox_CheckedChanged);
      // 
      // GP4View
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.includeTimeCheckBox);
      this.Controls.Add(this.useTimeOfBuildCheckBox);
      this.Controls.Add(this.creationTimePicker);
      this.Controls.Add(this.label6);
      this.Controls.Add(this.volumeTimestampPicker);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.pkgTypeDropdown);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.entitlementKeyTextbox);
      this.Controls.Add(this.buildPkgButton);
      this.Controls.Add(this.buildPfsButton);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.contentIdTextBox);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.passcodeTextBox);
      this.Controls.Add(this.splitContainer1);
      this.MinimumSize = new System.Drawing.Size(700, 400);
      this.Name = "GP4View";
      this.Size = new System.Drawing.Size(700, 400);
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
    private System.Windows.Forms.ContextMenuStrip fileViewContextMenu;
    private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem newFolderToolStripMenuItem;
    private System.Windows.Forms.Button buildPkgButton;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox entitlementKeyTextbox;
    private System.Windows.Forms.ComboBox pkgTypeDropdown;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.DateTimePicker volumeTimestampPicker;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.DateTimePicker creationTimePicker;
    private System.Windows.Forms.CheckBox useTimeOfBuildCheckBox;
    private System.Windows.Forms.CheckBox includeTimeCheckBox;
    private System.Windows.Forms.ImageList imageList1;
    private System.Windows.Forms.ToolStripMenuItem addFilesToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
  }
}
