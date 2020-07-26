namespace PkgEditor
{
  partial class MainWin
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWin));
      this.tabs = new System.Windows.Forms.TabControl();
      this.menuStrip = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
      this.gP4ProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.sFOFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.openGP4ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.combinePKGPartsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.cryptoDebuggerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.visitGitHubRepoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.setPKGPFSFileMetadataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.menuStrip.SuspendLayout();
      this.SuspendLayout();
      // 
      // tabs
      // 
      this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabs.Location = new System.Drawing.Point(0, 24);
      this.tabs.Name = "tabs";
      this.tabs.SelectedIndex = 0;
      this.tabs.Size = new System.Drawing.Size(800, 426);
      this.tabs.TabIndex = 0;
      this.tabs.TabIndexChanged += new System.EventHandler(this.UpdateSaveButtons);
      // 
      // menuStrip
      // 
      this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
      this.menuStrip.Location = new System.Drawing.Point(0, 0);
      this.menuStrip.Name = "menuStrip";
      this.menuStrip.Size = new System.Drawing.Size(800, 24);
      this.menuStrip.TabIndex = 1;
      this.menuStrip.Text = "menuStrip1";
      // 
      // fileToolStripMenuItem
      // 
      this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.openGP4ToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.exitToolStripMenuItem});
      this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
      this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
      this.fileToolStripMenuItem.Text = "&File";
      // 
      // toolStripMenuItem1
      // 
      this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gP4ProjectToolStripMenuItem,
            this.sFOFileToolStripMenuItem});
      this.toolStripMenuItem1.Name = "toolStripMenuItem1";
      this.toolStripMenuItem1.Size = new System.Drawing.Size(195, 22);
      this.toolStripMenuItem1.Text = "&New";
      // 
      // gP4ProjectToolStripMenuItem
      // 
      this.gP4ProjectToolStripMenuItem.Name = "gP4ProjectToolStripMenuItem";
      this.gP4ProjectToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
      this.gP4ProjectToolStripMenuItem.Text = "GP4 Project";
      this.gP4ProjectToolStripMenuItem.Click += new System.EventHandler(this.newGp4ProjectMenuItem_Click);
      // 
      // sFOFileToolStripMenuItem
      // 
      this.sFOFileToolStripMenuItem.Name = "sFOFileToolStripMenuItem";
      this.sFOFileToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
      this.sFOFileToolStripMenuItem.Text = "SFO File";
      this.sFOFileToolStripMenuItem.Click += new System.EventHandler(this.sfoFileToolStripMenuItem_Click);
      // 
      // openGP4ToolStripMenuItem
      // 
      this.openGP4ToolStripMenuItem.Name = "openGP4ToolStripMenuItem";
      this.openGP4ToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
      this.openGP4ToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
      this.openGP4ToolStripMenuItem.Text = "&Open...";
      this.openGP4ToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
      // 
      // saveToolStripMenuItem
      // 
      this.saveToolStripMenuItem.Enabled = false;
      this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
      this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
      this.saveToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
      this.saveToolStripMenuItem.Text = "&Save";
      this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
      // 
      // saveAsToolStripMenuItem
      // 
      this.saveAsToolStripMenuItem.Enabled = false;
      this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
      this.saveAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
      this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
      this.saveAsToolStripMenuItem.Text = "Save &As...";
      this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
      // 
      // closeToolStripMenuItem
      // 
      this.closeToolStripMenuItem.Enabled = false;
      this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
      this.closeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
      this.closeToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
      this.closeToolStripMenuItem.Text = "&Close";
      this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
      // 
      // exitToolStripMenuItem
      // 
      this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
      this.exitToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
      this.exitToolStripMenuItem.Text = "E&xit";
      this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
      // 
      // toolsToolStripMenuItem
      // 
      this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.combinePKGPartsToolStripMenuItem,
            this.cryptoDebuggerToolStripMenuItem,
            this.setPKGPFSFileMetadataToolStripMenuItem});
      this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
      this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
      this.toolsToolStripMenuItem.Text = "&Tools";
      // 
      // combinePKGPartsToolStripMenuItem
      // 
      this.combinePKGPartsToolStripMenuItem.Name = "combinePKGPartsToolStripMenuItem";
      this.combinePKGPartsToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
      this.combinePKGPartsToolStripMenuItem.Text = "Combine PKG parts";
      this.combinePKGPartsToolStripMenuItem.Click += new System.EventHandler(this.CombinePKGPartsToolStripMenuItem_Click);
      // 
      // cryptoDebuggerToolStripMenuItem
      // 
      this.cryptoDebuggerToolStripMenuItem.Name = "cryptoDebuggerToolStripMenuItem";
      this.cryptoDebuggerToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
      this.cryptoDebuggerToolStripMenuItem.Text = "Crypto debugger";
      this.cryptoDebuggerToolStripMenuItem.Click += new System.EventHandler(this.cryptoDebuggerToolStripMenuItem_Click);
      // 
      // helpToolStripMenuItem
      // 
      this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.visitGitHubRepoToolStripMenuItem,
            this.toolStripSeparator1,
            this.aboutToolStripMenuItem});
      this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
      this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
      this.helpToolStripMenuItem.Text = "&Help";
      // 
      // visitGitHubRepoToolStripMenuItem
      // 
      this.visitGitHubRepoToolStripMenuItem.Name = "visitGitHubRepoToolStripMenuItem";
      this.visitGitHubRepoToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
      this.visitGitHubRepoToolStripMenuItem.Text = "Visit GitHub Repo";
      this.visitGitHubRepoToolStripMenuItem.Click += new System.EventHandler(this.visitGitHubRepoToolStripMenuItem_Click);
      // 
      // toolStripSeparator1
      // 
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(164, 6);
      // 
      // aboutToolStripMenuItem
      // 
      this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
      this.aboutToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
      this.aboutToolStripMenuItem.Text = "&About...";
      this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
      // 
      // setPKGPFSFileMetadataToolStripMenuItem
      // 
      this.setPKGPFSFileMetadataToolStripMenuItem.Name = "setPKGPFSFileMetadataToolStripMenuItem";
      this.setPKGPFSFileMetadataToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
      this.setPKGPFSFileMetadataToolStripMenuItem.Text = "Set PKG/PFS file metadata";
      this.setPKGPFSFileMetadataToolStripMenuItem.Click += new System.EventHandler(this.setPKGPFSFileMetadataToolStripMenuItem_Click);
      // 
      // MainWin
      // 
      this.AllowDrop = true;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(800, 450);
      this.Controls.Add(this.tabs);
      this.Controls.Add(this.menuStrip);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.KeyPreview = true;
      this.MainMenuStrip = this.menuStrip;
      this.Name = "MainWin";
      this.Text = "PkgEditor";
      this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainWin_Drop);
      this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainWin_DragEnter);
      this.menuStrip.ResumeLayout(false);
      this.menuStrip.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TabControl tabs;
    private System.Windows.Forms.MenuStrip menuStrip;
    private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem openGP4ToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
    private System.Windows.Forms.ToolStripMenuItem gP4ProjectToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem sFOFileToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem combinePKGPartsToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem cryptoDebuggerToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem visitGitHubRepoToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.ToolStripMenuItem setPKGPFSFileMetadataToolStripMenuItem;
  }
}

