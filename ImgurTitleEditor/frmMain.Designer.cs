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
            this.components = new System.ComponentModel.Container();
            this.lvImages = new System.Windows.Forms.ListView();
            this.CMS = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyURLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editTitleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMain = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.authorizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buildCacheToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uploadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allImagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.withTitleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.withoutTitleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tbFilter = new System.Windows.Forms.ToolStripTextBox();
            this.addToCacheToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CMS.SuspendLayout();
            this.mnuMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvImages
            // 
            this.lvImages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvImages.ContextMenuStrip = this.CMS;
            this.lvImages.Location = new System.Drawing.Point(12, 50);
            this.lvImages.Name = "lvImages";
            this.lvImages.Size = new System.Drawing.Size(568, 511);
            this.lvImages.TabIndex = 0;
            this.lvImages.UseCompatibleStateImageBehavior = false;
            this.lvImages.DoubleClick += new System.EventHandler(this.lvImages_DoubleClick);
            this.lvImages.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvImages_KeyDown);
            // 
            // CMS
            // 
            this.CMS.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyURLToolStripMenuItem,
            this.saveImageToolStripMenuItem,
            this.editTitleToolStripMenuItem,
            this.addToCacheToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.CMS.Name = "CMS";
            this.CMS.Size = new System.Drawing.Size(140, 114);
            // 
            // copyURLToolStripMenuItem
            // 
            this.copyURLToolStripMenuItem.Name = "copyURLToolStripMenuItem";
            this.copyURLToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.copyURLToolStripMenuItem.Text = "&Copy URL";
            this.copyURLToolStripMenuItem.Click += new System.EventHandler(this.copyURLToolStripMenuItem_Click);
            // 
            // saveImageToolStripMenuItem
            // 
            this.saveImageToolStripMenuItem.Name = "saveImageToolStripMenuItem";
            this.saveImageToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveImageToolStripMenuItem.Text = "&Save Image";
            this.saveImageToolStripMenuItem.Click += new System.EventHandler(this.saveImageToolStripMenuItem_Click);
            // 
            // editTitleToolStripMenuItem
            // 
            this.editTitleToolStripMenuItem.Name = "editTitleToolStripMenuItem";
            this.editTitleToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.editTitleToolStripMenuItem.Text = "&Edit Title";
            this.editTitleToolStripMenuItem.Click += new System.EventHandler(this.editTitleToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.deleteToolStripMenuItem.Text = "&Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // mnuMain
            // 
            this.mnuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.listToolStripMenuItem,
            this.tbFilter});
            this.mnuMain.Location = new System.Drawing.Point(0, 0);
            this.mnuMain.Name = "mnuMain";
            this.mnuMain.Size = new System.Drawing.Size(592, 25);
            this.mnuMain.TabIndex = 1;
            this.mnuMain.Text = "Main Menu";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.authorizeToolStripMenuItem,
            this.buildCacheToolStripMenuItem,
            this.uploadToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 21);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // authorizeToolStripMenuItem
            // 
            this.authorizeToolStripMenuItem.Name = "authorizeToolStripMenuItem";
            this.authorizeToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.authorizeToolStripMenuItem.Text = "&Authorize";
            this.authorizeToolStripMenuItem.Click += new System.EventHandler(this.authorizeToolStripMenuItem_Click);
            // 
            // buildCacheToolStripMenuItem
            // 
            this.buildCacheToolStripMenuItem.Name = "buildCacheToolStripMenuItem";
            this.buildCacheToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.buildCacheToolStripMenuItem.Text = "&Build Cache";
            this.buildCacheToolStripMenuItem.Click += new System.EventHandler(this.buildCacheToolStripMenuItem_Click);
            // 
            // uploadToolStripMenuItem
            // 
            this.uploadToolStripMenuItem.Name = "uploadToolStripMenuItem";
            this.uploadToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.uploadToolStripMenuItem.Text = "&Upload";
            this.uploadToolStripMenuItem.Click += new System.EventHandler(this.uploadToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
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
            this.listToolStripMenuItem.Size = new System.Drawing.Size(35, 21);
            this.listToolStripMenuItem.Text = "&List";
            // 
            // allImagesToolStripMenuItem
            // 
            this.allImagesToolStripMenuItem.Name = "allImagesToolStripMenuItem";
            this.allImagesToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.allImagesToolStripMenuItem.Text = "&All Images";
            this.allImagesToolStripMenuItem.Click += new System.EventHandler(this.allImagesToolStripMenuItem_Click);
            // 
            // withTitleToolStripMenuItem
            // 
            this.withTitleToolStripMenuItem.Name = "withTitleToolStripMenuItem";
            this.withTitleToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.withTitleToolStripMenuItem.Text = "&With Title";
            this.withTitleToolStripMenuItem.Click += new System.EventHandler(this.withTitleToolStripMenuItem_Click);
            // 
            // withoutTitleToolStripMenuItem
            // 
            this.withoutTitleToolStripMenuItem.Name = "withoutTitleToolStripMenuItem";
            this.withoutTitleToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.withoutTitleToolStripMenuItem.Text = "With&out Title";
            this.withoutTitleToolStripMenuItem.Click += new System.EventHandler(this.withoutTitleToolStripMenuItem_Click);
            // 
            // tbFilter
            // 
            this.tbFilter.Name = "tbFilter";
            this.tbFilter.Size = new System.Drawing.Size(100, 21);
            this.tbFilter.ToolTipText = "Filter";
            this.tbFilter.Enter += new System.EventHandler(this.tbFilter_Enter);
            this.tbFilter.Leave += new System.EventHandler(this.tbFilter_Leave);
            this.tbFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbFilter_KeyDown);
            // 
            // addToCacheToolStripMenuItem
            // 
            this.addToCacheToolStripMenuItem.Name = "addToCacheToolStripMenuItem";
            this.addToCacheToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.addToCacheToolStripMenuItem.Text = "&Add to Cache";
            this.addToCacheToolStripMenuItem.Click += new System.EventHandler(this.addToCacheToolStripMenuItem_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(592, 573);
            this.Controls.Add(this.lvImages);
            this.Controls.Add(this.mnuMain);
            this.MinimumSize = new System.Drawing.Size(300, 300);
            this.Name = "frmMain";
            this.Text = "Main Form";
            this.ResizeEnd += new System.EventHandler(this.frmMain_ResizeEnd);
            this.SizeChanged += new System.EventHandler(this.frmMain_ResizeEnd);
            this.CMS.ResumeLayout(false);
            this.mnuMain.ResumeLayout(false);
            this.mnuMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvImages;
        private System.Windows.Forms.MenuStrip mnuMain;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem authorizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem buildCacheToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem listToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allImagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem withTitleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem withoutTitleToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox tbFilter;
        private System.Windows.Forms.ToolStripMenuItem uploadToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip CMS;
        private System.Windows.Forms.ToolStripMenuItem copyURLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editTitleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addToCacheToolStripMenuItem;
    }
}

