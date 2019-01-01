namespace PkgEditor.Views
{
  partial class SFOView
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
      this.listView1 = new System.Windows.Forms.ListView();
      this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.toolStrip1 = new System.Windows.Forms.ToolStrip();
      this.defaultsDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
      this.loadACDefaultsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.loadGPDefaultsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.label1 = new System.Windows.Forms.Label();
      this.nameTextBox = new System.Windows.Forms.TextBox();
      this.typeDropDown = new System.Windows.Forms.ComboBox();
      this.label2 = new System.Windows.Forms.Label();
      this.valueTextBox = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.maxLengthInput = new System.Windows.Forms.NumericUpDown();
      this.sizeLabel = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.addKeyValueButton = new System.Windows.Forms.ToolStripButton();
      this.toolStrip1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.maxLengthInput)).BeginInit();
      this.SuspendLayout();
      // 
      // listView1
      // 
      this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
      this.listView1.FullRowSelect = true;
      this.listView1.Location = new System.Drawing.Point(0, 77);
      this.listView1.Name = "listView1";
      this.listView1.Size = new System.Drawing.Size(650, 299);
      this.listView1.TabIndex = 1;
      this.listView1.UseCompatibleStateImageBehavior = false;
      this.listView1.View = System.Windows.Forms.View.Details;
      this.listView1.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listView1_ItemSelectionChanged);
      this.listView1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listView1_KeyUp);
      // 
      // columnHeader1
      // 
      this.columnHeader1.Text = "Key";
      this.columnHeader1.Width = 150;
      // 
      // columnHeader2
      // 
      this.columnHeader2.Text = "Type";
      // 
      // columnHeader3
      // 
      this.columnHeader3.Text = "Value";
      this.columnHeader3.Width = 300;
      // 
      // toolStrip1
      // 
      this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
      this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.defaultsDropDownButton,
            this.addKeyValueButton});
      this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
      this.toolStrip1.Location = new System.Drawing.Point(0, 0);
      this.toolStrip1.Name = "toolStrip1";
      this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
      this.toolStrip1.Size = new System.Drawing.Size(650, 25);
      this.toolStrip1.TabIndex = 2;
      this.toolStrip1.Text = "toolStrip1";
      // 
      // defaultsDropDownButton
      // 
      this.defaultsDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadACDefaultsToolStripMenuItem,
            this.loadGPDefaultsToolStripMenuItem});
      this.defaultsDropDownButton.Name = "defaultsDropDownButton";
      this.defaultsDropDownButton.Size = new System.Drawing.Size(63, 22);
      this.defaultsDropDownButton.Text = "Defaults";
      // 
      // loadACDefaultsToolStripMenuItem
      // 
      this.loadACDefaultsToolStripMenuItem.Name = "loadACDefaultsToolStripMenuItem";
      this.loadACDefaultsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
      this.loadACDefaultsToolStripMenuItem.Text = "Load AC Defaults";
      this.loadACDefaultsToolStripMenuItem.Click += new System.EventHandler(this.loadACDefaultsToolStripMenuItem_Click);
      // 
      // loadGPDefaultsToolStripMenuItem
      // 
      this.loadGPDefaultsToolStripMenuItem.Enabled = false;
      this.loadGPDefaultsToolStripMenuItem.Name = "loadGPDefaultsToolStripMenuItem";
      this.loadGPDefaultsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
      this.loadGPDefaultsToolStripMenuItem.Text = "Load GP Defaults";
      this.loadGPDefaultsToolStripMenuItem.Click += new System.EventHandler(this.loadGPDefaultsToolStripMenuItem_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 28);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(35, 13);
      this.label1.TabIndex = 3;
      this.label1.Text = "Name";
      // 
      // nameTextBox
      // 
      this.nameTextBox.Location = new System.Drawing.Point(44, 25);
      this.nameTextBox.Name = "nameTextBox";
      this.nameTextBox.Size = new System.Drawing.Size(304, 20);
      this.nameTextBox.TabIndex = 5;
      this.nameTextBox.TextChanged += new System.EventHandler(this.UpdateValue);
      // 
      // typeDropDown
      // 
      this.typeDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.typeDropDown.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this.typeDropDown.FormattingEnabled = true;
      this.typeDropDown.Items.AddRange(new object[] {
            "String",
            "Integer",
            "Bytes"});
      this.typeDropDown.Location = new System.Drawing.Point(44, 50);
      this.typeDropDown.Name = "typeDropDown";
      this.typeDropDown.Size = new System.Drawing.Size(59, 21);
      this.typeDropDown.TabIndex = 6;
      this.typeDropDown.SelectedIndexChanged += new System.EventHandler(this.UpdateValue);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(354, 28);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(34, 13);
      this.label2.TabIndex = 7;
      this.label2.Text = "Value";
      // 
      // valueTextBox
      // 
      this.valueTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.valueTextBox.Location = new System.Drawing.Point(394, 25);
      this.valueTextBox.Multiline = true;
      this.valueTextBox.Name = "valueTextBox";
      this.valueTextBox.Size = new System.Drawing.Size(253, 46);
      this.valueTextBox.TabIndex = 8;
      this.valueTextBox.TextChanged += new System.EventHandler(this.UpdateValue);
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(109, 53);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(50, 13);
      this.label3.TabIndex = 9;
      this.label3.Text = "Max Size";
      // 
      // maxLengthInput
      // 
      this.maxLengthInput.Location = new System.Drawing.Point(165, 50);
      this.maxLengthInput.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
      this.maxLengthInput.Name = "maxLengthInput";
      this.maxLengthInput.Size = new System.Drawing.Size(84, 20);
      this.maxLengthInput.TabIndex = 11;
      this.maxLengthInput.ValueChanged += new System.EventHandler(this.UpdateValue);
      // 
      // sizeLabel
      // 
      this.sizeLabel.AutoSize = true;
      this.sizeLabel.Location = new System.Drawing.Point(255, 53);
      this.sizeLabel.Name = "sizeLabel";
      this.sizeLabel.Size = new System.Drawing.Size(30, 13);
      this.sizeLabel.TabIndex = 12;
      this.sizeLabel.Text = "Size:";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(3, 52);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(31, 13);
      this.label5.TabIndex = 13;
      this.label5.Text = "Type";
      // 
      // addKeyValueButton
      // 
      this.addKeyValueButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.addKeyValueButton.Name = "addKeyValueButton";
      this.addKeyValueButton.Size = new System.Drawing.Size(88, 22);
      this.addKeyValueButton.Text = "Add Key/Value";
      this.addKeyValueButton.Click += new System.EventHandler(this.addKeyValueButton_Click);
      // 
      // SFOView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.label5);
      this.Controls.Add(this.sizeLabel);
      this.Controls.Add(this.maxLengthInput);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.valueTextBox);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.typeDropDown);
      this.Controls.Add(this.nameTextBox);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.toolStrip1);
      this.Controls.Add(this.listView1);
      this.Name = "SFOView";
      this.Size = new System.Drawing.Size(650, 376);
      this.toolStrip1.ResumeLayout(false);
      this.toolStrip1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.maxLengthInput)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
    private System.Windows.Forms.ListView listView1;
    private System.Windows.Forms.ColumnHeader columnHeader1;
    private System.Windows.Forms.ColumnHeader columnHeader2;
    private System.Windows.Forms.ColumnHeader columnHeader3;
    private System.Windows.Forms.ToolStrip toolStrip1;
    private System.Windows.Forms.ToolStripDropDownButton defaultsDropDownButton;
    private System.Windows.Forms.ToolStripMenuItem loadACDefaultsToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem loadGPDefaultsToolStripMenuItem;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox nameTextBox;
    private System.Windows.Forms.ComboBox typeDropDown;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox valueTextBox;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.NumericUpDown maxLengthInput;
    private System.Windows.Forms.Label sizeLabel;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.ToolStripButton addKeyValueButton;
  }
}
