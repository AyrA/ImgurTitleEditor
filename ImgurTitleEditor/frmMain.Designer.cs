namespace ImgurTitleEditor
{
    partial class frmMain
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
            this.lvImages = new System.Windows.Forms.ListView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.authorizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buildCacheToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allImagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.withTitleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.withoutTitleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvImages
            // 
            this.lvImages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvImages.Location = new System.Drawing.Point(12, 50);
            this.lvImages.Name = "lvImages";
            this.lvImages.Size = new System.Drawing.Size(568, 511);
            this.lvImages.TabIndex = 0;
            this.lvImages.UseCompatibleStateImageBehavior = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.listToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(592, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.authorizeToolStripMenuItem,
            this.buildCacheToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // authorizeToolStripMenuItem
            // 
            this.authorizeToolStripMenuItem.Name = "authorizeToolStripMenuItem";
            this.authorizeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.authorizeToolStripMenuItem.Text = "&Authorize";
            this.authorizeToolStripMenuItem.Click += new System.EventHandler(this.authorizeToolStripMenuItem_Click);
            // 
            // buildCacheToolStripMenuItem
            // 
            this.buildCacheToolStripMenuItem.Name = "buildCacheToolStripMenuItem";
            this.buildCacheToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.buildCacheToolStripMenuItem.Text = "&Build Cache";
            this.buildCacheToolStripMenuItem.Click += new System.EventHandler(this.buildCacheToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // listToolStripMenuItem
            // 
            this.listToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allImagesToolStripMenuItem,
            this.withTitleToolStripMenuItem,
            this.withoutTitleToolStripMenuItem});
            this.listToolStripMenuItem.Name = "listToolStripMenuItem";
            this.listToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.listToolStripMenuItem.Text = "&List";
            // 
            // allImagesToolStripMenuItem
            // 
            this.allImagesToolStripMenuItem.Name = "allImagesToolStripMenuItem";
            this.allImagesToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.allImagesToolStripMenuItem.Text = "&All Images";
            this.allImagesToolStripMenuItem.Click += new System.EventHandler(this.allImagesToolStripMenuItem_Click);
            // 
            // withTitleToolStripMenuItem
            // 
            this.withTitleToolStripMenuItem.Name = "withTitleToolStripMenuItem";
            this.withTitleToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.withTitleToolStripMenuItem.Text = "&With Title";
            this.withTitleToolStripMenuItem.Click += new System.EventHandler(this.withTitleToolStripMenuItem_Click);
            // 
            // withoutTitleToolStripMenuItem
            // 
            this.withoutTitleToolStripMenuItem.Name = "withoutTitleToolStripMenuItem";
            this.withoutTitleToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.withoutTitleToolStripMenuItem.Text = "With&out Title";
            this.withoutTitleToolStripMenuItem.Click += new System.EventHandler(this.withoutTitleToolStripMenuItem_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(592, 573);
            this.Controls.Add(this.lvImages);
            this.Controls.Add(this.menuStrip1);
            this.Name = "frmMain";
            this.Text = "Main Form";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvImages;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem authorizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem buildCacheToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem listToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allImagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem withTitleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem withoutTitleToolStripMenuItem;
    }
}

