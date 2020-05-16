namespace PkgEditor.Views
{
  partial class CryptoDebug
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
      this.ekpfsInput = new System.Windows.Forms.TextBox();
      this.dataKey = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.tweakKey = new System.Windows.Forms.TextBox();
      this.button1 = new System.Windows.Forms.Button();
      this.label2 = new System.Windows.Forms.Label();
      this.pfsSeed = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.objectView1 = new PkgEditor.Views.ObjectView();
      this.sectorPreview = new System.Windows.Forms.TextBox();
      this.label5 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.indexInput = new System.Windows.Forms.TextBox();
      this.xtsStartSector = new System.Windows.Forms.TextBox();
      this.label8 = new System.Windows.Forms.Label();
      this.xtsSectorSize = new System.Windows.Forms.TextBox();
      this.label9 = new System.Windows.Forms.Label();
      this.reloadButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // ekpfsInput
      // 
      this.ekpfsInput.Location = new System.Drawing.Point(81, 56);
      this.ekpfsInput.Name = "ekpfsInput";
      this.ekpfsInput.Size = new System.Drawing.Size(453, 20);
      this.ekpfsInput.TabIndex = 0;
      // 
      // dataKey
      // 
      this.dataKey.Location = new System.Drawing.Point(81, 82);
      this.dataKey.Name = "dataKey";
      this.dataKey.Size = new System.Drawing.Size(259, 20);
      this.dataKey.TabIndex = 1;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(34, 59);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(41, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "EKPFS";
      // 
      // tweakKey
      // 
      this.tweakKey.Location = new System.Drawing.Point(81, 108);
      this.tweakKey.Name = "tweakKey";
      this.tweakKey.Size = new System.Drawing.Size(259, 20);
      this.tweakKey.TabIndex = 3;
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(3, 3);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(75, 23);
      this.button1.TabIndex = 4;
      this.button1.Text = "Open PFS";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(4, 36);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(74, 13);
      this.label2.TabIndex = 5;
      this.label2.Text = "PFS Key seed";
      // 
      // pfsSeed
      // 
      this.pfsSeed.Location = new System.Drawing.Point(81, 33);
      this.pfsSeed.Name = "pfsSeed";
      this.pfsSeed.Size = new System.Drawing.Size(259, 20);
      this.pfsSeed.TabIndex = 6;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(45, 85);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(30, 13);
      this.label3.TabIndex = 7;
      this.label3.Text = "Data";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(35, 111);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(40, 13);
      this.label4.TabIndex = 8;
      this.label4.Text = "Tweak";
      // 
      // objectView1
      // 
      this.objectView1.Location = new System.Drawing.Point(540, 19);
      this.objectView1.Name = "objectView1";
      this.objectView1.Size = new System.Drawing.Size(327, 378);
      this.objectView1.TabIndex = 9;
      // 
      // sectorPreview
      // 
      this.sectorPreview.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.sectorPreview.Location = new System.Drawing.Point(3, 156);
      this.sectorPreview.Multiline = true;
      this.sectorPreview.Name = "sectorPreview";
      this.sectorPreview.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.sectorPreview.Size = new System.Drawing.Size(527, 241);
      this.sectorPreview.TabIndex = 10;
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(4, 140);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(60, 13);
      this.label5.TabIndex = 11;
      this.label5.Text = "First Sector";
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(537, 3);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(42, 13);
      this.label6.TabIndex = 12;
      this.label6.Text = "Header";
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(412, 36);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(33, 13);
      this.label7.TabIndex = 13;
      this.label7.Text = "Index";
      // 
      // indexInput
      // 
      this.indexInput.Location = new System.Drawing.Point(451, 33);
      this.indexInput.Name = "indexInput";
      this.indexInput.Size = new System.Drawing.Size(83, 20);
      this.indexInput.TabIndex = 14;
      this.indexInput.Text = "1";
      // 
      // xtsStartSector
      // 
      this.xtsStartSector.Location = new System.Drawing.Point(451, 104);
      this.xtsStartSector.Name = "xtsStartSector";
      this.xtsStartSector.Size = new System.Drawing.Size(83, 20);
      this.xtsStartSector.TabIndex = 16;
      this.xtsStartSector.Text = "16";
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(362, 107);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(83, 13);
      this.label8.TabIndex = 15;
      this.label8.Text = "XTS start sector";
      // 
      // xtsSectorSize
      // 
      this.xtsSectorSize.Location = new System.Drawing.Point(451, 127);
      this.xtsSectorSize.Name = "xtsSectorSize";
      this.xtsSectorSize.Size = new System.Drawing.Size(83, 20);
      this.xtsSectorSize.TabIndex = 18;
      this.xtsSectorSize.Text = "4096";
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.Location = new System.Drawing.Point(364, 130);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(81, 13);
      this.label9.TabIndex = 17;
      this.label9.Text = "XTS sector size";
      // 
      // reloadButton
      // 
      this.reloadButton.Location = new System.Drawing.Point(81, 130);
      this.reloadButton.Name = "reloadButton";
      this.reloadButton.Size = new System.Drawing.Size(75, 23);
      this.reloadButton.TabIndex = 19;
      this.reloadButton.Text = "Reload";
      this.reloadButton.UseVisualStyleBackColor = true;
      this.reloadButton.Click += new System.EventHandler(this.button2_Click);
      // 
      // CryptoDebug
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.reloadButton);
      this.Controls.Add(this.xtsSectorSize);
      this.Controls.Add(this.label9);
      this.Controls.Add(this.xtsStartSector);
      this.Controls.Add(this.label8);
      this.Controls.Add(this.indexInput);
      this.Controls.Add(this.label7);
      this.Controls.Add(this.label6);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.sectorPreview);
      this.Controls.Add(this.objectView1);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.pfsSeed);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.tweakKey);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.dataKey);
      this.Controls.Add(this.ekpfsInput);
      this.Name = "CryptoDebug";
      this.Size = new System.Drawing.Size(870, 401);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox ekpfsInput;
    private System.Windows.Forms.TextBox dataKey;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox tweakKey;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox pfsSeed;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private ObjectView objectView1;
    private System.Windows.Forms.TextBox sectorPreview;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.TextBox indexInput;
    private System.Windows.Forms.TextBox xtsStartSector;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.TextBox xtsSectorSize;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.Button reloadButton;
  }
}
