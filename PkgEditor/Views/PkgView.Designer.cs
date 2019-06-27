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
      this.components = new System.ComponentModel.Container();
      this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.extractToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.extractDecryptedMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.tabPage2 = new System.Windows.Forms.TabPage();
      this.validateResult = new System.Windows.Forms.TextBox();
      this.checkDigestsButton = new System.Windows.Forms.Button();
      this.listView1 = new System.Windows.Forms.ListView();
      this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.tabPage1 = new System.Windows.Forms.TabPage();
      this.filesTab = new System.Windows.Forms.TabPage();
      this.button1 = new System.Windows.Forms.Button();
      this.passcodeTextBox = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.entriesTab = new System.Windows.Forms.TabPage();
      this.entriesListView = new System.Windows.Forms.ListView();
      this.columnHeaderId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.columnHeaderSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.columnHeaderOffset = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.columnHeaderEnc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.infoTab = new System.Windows.Forms.TabPage();
      this.pkgHeaderTabControl = new System.Windows.Forms.TabControl();
      this.outerPfsHeaderTabPage = new System.Windows.Forms.TabPage();
      this.pfsHeaderTreeView = new System.Windows.Forms.TreeView();
      this.tabPage3 = new System.Windows.Forms.TabPage();
      this.pkgHeaderTreeView = new System.Windows.Forms.TreeView();
      this.label3 = new System.Windows.Forms.Label();
      this.contentIdTextBox = new System.Windows.Forms.TextBox();
      this.label4 = new System.Windows.Forms.Label();
      this.titleTextBox = new System.Windows.Forms.TextBox();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.label5 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.label8 = new System.Windows.Forms.Label();
      this.typeLabel = new System.Windows.Forms.Label();
      this.sizeLabel = new System.Windows.Forms.Label();
      this.versionLabel = new System.Windows.Forms.Label();
      this.appVerLabelLabel = new System.Windows.Forms.Label();
      this.appVerLabel = new System.Windows.Forms.Label();
      this.mainTabControl = new System.Windows.Forms.TabControl();
      this.button2 = new System.Windows.Forms.Button();
      this.contextMenuStrip1.SuspendLayout();
      this.tabPage2.SuspendLayout();
      this.filesTab.SuspendLayout();
      this.entriesTab.SuspendLayout();
      this.infoTab.SuspendLayout();
      this.pkgHeaderTabControl.SuspendLayout();
      this.outerPfsHeaderTabPage.SuspendLayout();
      this.tabPage3.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.mainTabControl.SuspendLayout();
      this.SuspendLayout();
      // 
      // contextMenuStrip1
      // 
      this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extractToolStripMenuItem,
            this.extractDecryptedMenuItem});
      this.contextMenuStrip1.Name = "contextMenuStrip1";
      this.contextMenuStrip1.Size = new System.Drawing.Size(177, 48);
      this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStrip1_Opening);
      // 
      // extractToolStripMenuItem
      // 
      this.extractToolStripMenuItem.Name = "extractToolStripMenuItem";
      this.extractToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
      this.extractToolStripMenuItem.Text = "Extract";
      this.extractToolStripMenuItem.Click += new System.EventHandler(this.ExtractToolStripMenuItem_Click);
      // 
      // extractDecryptedMenuItem
      // 
      this.extractDecryptedMenuItem.Name = "extractDecryptedMenuItem";
      this.extractDecryptedMenuItem.Size = new System.Drawing.Size(176, 22);
      this.extractDecryptedMenuItem.Text = "Extract and Decrypt";
      this.extractDecryptedMenuItem.Click += new System.EventHandler(this.ToolStripMenuItem1_Click);
      // 
      // tabPage2
      // 
      this.tabPage2.Controls.Add(this.listView1);
      this.tabPage2.Controls.Add(this.validateResult);
      this.tabPage2.Controls.Add(this.checkDigestsButton);
      this.tabPage2.Location = new System.Drawing.Point(4, 22);
      this.tabPage2.Name = "tabPage2";
      this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage2.Size = new System.Drawing.Size(535, 370);
      this.tabPage2.TabIndex = 4;
      this.tabPage2.Text = "Validate";
      this.tabPage2.UseVisualStyleBackColor = true;
      // 
      // validateResult
      // 
      this.validateResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.validateResult.Location = new System.Drawing.Point(186, 35);
      this.validateResult.Multiline = true;
      this.validateResult.Name = "validateResult";
      this.validateResult.Size = new System.Drawing.Size(343, 329);
      this.validateResult.TabIndex = 0;
      // 
      // checkDigestsButton
      // 
      this.checkDigestsButton.Location = new System.Drawing.Point(6, 6);
      this.checkDigestsButton.Name = "checkDigestsButton";
      this.checkDigestsButton.Size = new System.Drawing.Size(126, 23);
      this.checkDigestsButton.TabIndex = 1;
      this.checkDigestsButton.Text = "Check PKG Digests";
      this.checkDigestsButton.UseVisualStyleBackColor = true;
      this.checkDigestsButton.Click += new System.EventHandler(this.checkDigests_Click);
      // 
      // listView1
      // 
      this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
      this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
      this.listView1.FullRowSelect = true;
      this.listView1.Location = new System.Drawing.Point(6, 35);
      this.listView1.MultiSelect = false;
      this.listView1.Name = "listView1";
      this.listView1.Size = new System.Drawing.Size(174, 329);
      this.listView1.TabIndex = 2;
      this.listView1.UseCompatibleStateImageBehavior = false;
      this.listView1.View = System.Windows.Forms.View.Details;
      this.listView1.SelectedIndexChanged += new System.EventHandler(this.ListView1_SelectedIndexChanged);
      // 
      // columnHeader2
      // 
      this.columnHeader2.Text = "Name";
      this.columnHeader2.Width = 150;
      // 
      // tabPage1
      // 
      this.tabPage1.Location = new System.Drawing.Point(4, 22);
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage1.Size = new System.Drawing.Size(535, 370);
      this.tabPage1.TabIndex = 3;
      this.tabPage1.Text = "SFO";
      this.tabPage1.UseVisualStyleBackColor = true;
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
      // passcodeTextBox
      // 
      this.passcodeTextBox.Location = new System.Drawing.Point(9, 19);
      this.passcodeTextBox.MaxLength = 32;
      this.passcodeTextBox.Name = "passcodeTextBox";
      this.passcodeTextBox.Size = new System.Drawing.Size(247, 20);
      this.passcodeTextBox.TabIndex = 1;
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
      this.entriesListView.ContextMenuStrip = this.contextMenuStrip1;
      this.entriesListView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.entriesListView.FullRowSelect = true;
      this.entriesListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
      this.entriesListView.Location = new System.Drawing.Point(3, 3);
      this.entriesListView.MultiSelect = false;
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
      // infoTab
      // 
      this.infoTab.Controls.Add(this.button2);
      this.infoTab.Controls.Add(this.appVerLabel);
      this.infoTab.Controls.Add(this.appVerLabelLabel);
      this.infoTab.Controls.Add(this.versionLabel);
      this.infoTab.Controls.Add(this.sizeLabel);
      this.infoTab.Controls.Add(this.typeLabel);
      this.infoTab.Controls.Add(this.label8);
      this.infoTab.Controls.Add(this.label6);
      this.infoTab.Controls.Add(this.label5);
      this.infoTab.Controls.Add(this.pictureBox1);
      this.infoTab.Controls.Add(this.titleTextBox);
      this.infoTab.Controls.Add(this.contentIdTextBox);
      this.infoTab.Controls.Add(this.label4);
      this.infoTab.Controls.Add(this.label3);
      this.infoTab.Controls.Add(this.pkgHeaderTabControl);
      this.infoTab.Location = new System.Drawing.Point(4, 22);
      this.infoTab.Name = "infoTab";
      this.infoTab.Padding = new System.Windows.Forms.Padding(3);
      this.infoTab.Size = new System.Drawing.Size(535, 370);
      this.infoTab.TabIndex = 2;
      this.infoTab.Text = "Info";
      this.infoTab.UseVisualStyleBackColor = true;
      // 
      // pkgHeaderTabControl
      // 
      this.pkgHeaderTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pkgHeaderTabControl.Controls.Add(this.tabPage3);
      this.pkgHeaderTabControl.Controls.Add(this.outerPfsHeaderTabPage);
      this.pkgHeaderTabControl.Location = new System.Drawing.Point(3, 184);
      this.pkgHeaderTabControl.Name = "pkgHeaderTabControl";
      this.pkgHeaderTabControl.SelectedIndex = 0;
      this.pkgHeaderTabControl.Size = new System.Drawing.Size(529, 183);
      this.pkgHeaderTabControl.TabIndex = 16;
      // 
      // outerPfsHeaderTabPage
      // 
      this.outerPfsHeaderTabPage.Controls.Add(this.pfsHeaderTreeView);
      this.outerPfsHeaderTabPage.Location = new System.Drawing.Point(4, 22);
      this.outerPfsHeaderTabPage.Name = "outerPfsHeaderTabPage";
      this.outerPfsHeaderTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.outerPfsHeaderTabPage.Size = new System.Drawing.Size(521, 172);
      this.outerPfsHeaderTabPage.TabIndex = 1;
      this.outerPfsHeaderTabPage.Text = "Outer PFS Header";
      this.outerPfsHeaderTabPage.UseVisualStyleBackColor = true;
      // 
      // pfsHeaderTreeView
      // 
      this.pfsHeaderTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.pfsHeaderTreeView.Location = new System.Drawing.Point(3, 3);
      this.pfsHeaderTreeView.Name = "pfsHeaderTreeView";
      this.pfsHeaderTreeView.Size = new System.Drawing.Size(515, 166);
      this.pfsHeaderTreeView.TabIndex = 1;
      // 
      // tabPage3
      // 
      this.tabPage3.Controls.Add(this.pkgHeaderTreeView);
      this.tabPage3.Location = new System.Drawing.Point(4, 22);
      this.tabPage3.Name = "tabPage3";
      this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage3.Size = new System.Drawing.Size(521, 157);
      this.tabPage3.TabIndex = 0;
      this.tabPage3.Text = "Header";
      this.tabPage3.UseVisualStyleBackColor = true;
      // 
      // pkgHeaderTreeView
      // 
      this.pkgHeaderTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.pkgHeaderTreeView.Location = new System.Drawing.Point(3, 3);
      this.pkgHeaderTreeView.Name = "pkgHeaderTreeView";
      this.pkgHeaderTreeView.Size = new System.Drawing.Size(515, 151);
      this.pkgHeaderTreeView.TabIndex = 0;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Cursor = System.Windows.Forms.Cursors.IBeam;
      this.label3.Location = new System.Drawing.Point(6, 9);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(58, 13);
      this.label3.TabIndex = 2;
      this.label3.Text = "Content ID";
      // 
      // contentIdTextBox
      // 
      this.contentIdTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.contentIdTextBox.Location = new System.Drawing.Point(70, 6);
      this.contentIdTextBox.Name = "contentIdTextBox";
      this.contentIdTextBox.ReadOnly = true;
      this.contentIdTextBox.Size = new System.Drawing.Size(281, 20);
      this.contentIdTextBox.TabIndex = 3;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Cursor = System.Windows.Forms.Cursors.IBeam;
      this.label4.Location = new System.Drawing.Point(37, 36);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(27, 13);
      this.label4.TabIndex = 4;
      this.label4.Text = "Title";
      // 
      // titleTextBox
      // 
      this.titleTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.titleTextBox.Location = new System.Drawing.Point(70, 33);
      this.titleTextBox.Name = "titleTextBox";
      this.titleTextBox.ReadOnly = true;
      this.titleTextBox.Size = new System.Drawing.Size(281, 20);
      this.titleTextBox.TabIndex = 5;
      // 
      // pictureBox1
      // 
      this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.pictureBox1.Location = new System.Drawing.Point(357, 3);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(175, 175);
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
      this.pictureBox1.TabIndex = 6;
      this.pictureBox1.TabStop = false;
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(33, 62);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(31, 13);
      this.label5.TabIndex = 7;
      this.label5.Text = "Type";
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(37, 88);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(27, 13);
      this.label6.TabIndex = 8;
      this.label6.Text = "Size";
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(-3, 114);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(67, 13);
      this.label8.TabIndex = 9;
      this.label8.Text = "PKG Version";
      // 
      // typeLabel
      // 
      this.typeLabel.AutoSize = true;
      this.typeLabel.Location = new System.Drawing.Point(67, 62);
      this.typeLabel.Name = "typeLabel";
      this.typeLabel.Size = new System.Drawing.Size(35, 13);
      this.typeLabel.TabIndex = 11;
      this.typeLabel.Text = "Game";
      // 
      // sizeLabel
      // 
      this.sizeLabel.AutoSize = true;
      this.sizeLabel.Location = new System.Drawing.Point(67, 88);
      this.sizeLabel.Name = "sizeLabel";
      this.sizeLabel.Size = new System.Drawing.Size(39, 13);
      this.sizeLabel.TabIndex = 12;
      this.sizeLabel.Text = "7.20 gj";
      // 
      // versionLabel
      // 
      this.versionLabel.AutoSize = true;
      this.versionLabel.Location = new System.Drawing.Point(67, 114);
      this.versionLabel.Name = "versionLabel";
      this.versionLabel.Size = new System.Drawing.Size(28, 13);
      this.versionLabel.TabIndex = 13;
      this.versionLabel.Text = "4.20";
      // 
      // appVerLabelLabel
      // 
      this.appVerLabelLabel.AutoSize = true;
      this.appVerLabelLabel.Location = new System.Drawing.Point(0, 137);
      this.appVerLabelLabel.Name = "appVerLabelLabel";
      this.appVerLabelLabel.Size = new System.Drawing.Size(64, 13);
      this.appVerLabelLabel.TabIndex = 14;
      this.appVerLabelLabel.Text = "App Version";
      // 
      // appVerLabel
      // 
      this.appVerLabel.AutoSize = true;
      this.appVerLabel.Location = new System.Drawing.Point(67, 137);
      this.appVerLabel.Name = "appVerLabel";
      this.appVerLabel.Size = new System.Drawing.Size(28, 13);
      this.appVerLabel.TabIndex = 15;
      this.appVerLabel.Text = "4.20";
      // 
      // mainTabControl
      // 
      this.mainTabControl.Controls.Add(this.infoTab);
      this.mainTabControl.Controls.Add(this.entriesTab);
      this.mainTabControl.Controls.Add(this.filesTab);
      this.mainTabControl.Controls.Add(this.tabPage1);
      this.mainTabControl.Controls.Add(this.tabPage2);
      this.mainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.mainTabControl.Location = new System.Drawing.Point(0, 0);
      this.mainTabControl.Name = "mainTabControl";
      this.mainTabControl.SelectedIndex = 0;
      this.mainTabControl.Size = new System.Drawing.Size(543, 396);
      this.mainTabControl.TabIndex = 1;
      // 
      // button2
      // 
      this.button2.Location = new System.Drawing.Point(6, 155);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(133, 23);
      this.button2.TabIndex = 17;
      this.button2.Text = "Export to GP4 Project";
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new System.EventHandler(this.Button2_Click);
      // 
      // PkgView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.mainTabControl);
      this.Name = "PkgView";
      this.Size = new System.Drawing.Size(543, 396);
      this.contextMenuStrip1.ResumeLayout(false);
      this.tabPage2.ResumeLayout(false);
      this.tabPage2.PerformLayout();
      this.filesTab.ResumeLayout(false);
      this.filesTab.PerformLayout();
      this.entriesTab.ResumeLayout(false);
      this.infoTab.ResumeLayout(false);
      this.infoTab.PerformLayout();
      this.pkgHeaderTabControl.ResumeLayout(false);
      this.outerPfsHeaderTabPage.ResumeLayout(false);
      this.tabPage3.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.mainTabControl.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
    private System.Windows.Forms.ToolStripMenuItem extractToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem extractDecryptedMenuItem;
    private System.Windows.Forms.TabPage tabPage2;
    private System.Windows.Forms.ListView listView1;
    private System.Windows.Forms.ColumnHeader columnHeader2;
    private System.Windows.Forms.TextBox validateResult;
    private System.Windows.Forms.Button checkDigestsButton;
    private System.Windows.Forms.TabPage tabPage1;
    private System.Windows.Forms.TabPage filesTab;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox passcodeTextBox;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.TabPage entriesTab;
    private System.Windows.Forms.ListView entriesListView;
    private System.Windows.Forms.ColumnHeader columnHeaderId;
    private System.Windows.Forms.ColumnHeader columnHeaderSize;
    private System.Windows.Forms.ColumnHeader columnHeaderOffset;
    private System.Windows.Forms.ColumnHeader columnHeaderEnc;
    private System.Windows.Forms.ColumnHeader columnHeader1;
    private System.Windows.Forms.TabPage infoTab;
    private System.Windows.Forms.Button button2;
    private System.Windows.Forms.Label appVerLabel;
    private System.Windows.Forms.Label appVerLabelLabel;
    private System.Windows.Forms.Label versionLabel;
    private System.Windows.Forms.Label sizeLabel;
    private System.Windows.Forms.Label typeLabel;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.TextBox titleTextBox;
    private System.Windows.Forms.TextBox contentIdTextBox;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TabControl pkgHeaderTabControl;
    private System.Windows.Forms.TabPage tabPage3;
    private System.Windows.Forms.TreeView pkgHeaderTreeView;
    private System.Windows.Forms.TabPage outerPfsHeaderTabPage;
    private System.Windows.Forms.TreeView pfsHeaderTreeView;
    private System.Windows.Forms.TabControl mainTabControl;
  }
}
