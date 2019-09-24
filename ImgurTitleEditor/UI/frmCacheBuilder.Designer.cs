namespace ImgurTitleEditor
{
    partial class frmCacheBuilder
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
            this.pbMeta = new System.Windows.Forms.ProgressBar();
            this.pbThumbnail = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // pbMeta
            // 
            this.pbMeta.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbMeta.Location = new System.Drawing.Point(12, 12);
            this.pbMeta.Name = "pbMeta";
            this.pbMeta.Size = new System.Drawing.Size(468, 23);
            this.pbMeta.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbMeta.TabIndex = 0;
            // 
            // pbThumbnail
            // 
            this.pbThumbnail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbThumbnail.Location = new System.Drawing.Point(12, 41);
            this.pbThumbnail.Name = "pbThumbnail";
            this.pbThumbnail.Size = new System.Drawing.Size(468, 23);
            this.pbThumbnail.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbThumbnail.TabIndex = 1;
            // 
            // frmCacheBuilder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 73);
            this.Controls.Add(this.pbThumbnail);
            this.Controls.Add(this.pbMeta);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCacheBuilder";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cache Builder";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmCacheBuilder_FormClosed);
            this.Shown += new System.EventHandler(this.frmCacheBuilder_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar pbMeta;
        private System.Windows.Forms.ProgressBar pbThumbnail;
    }
}