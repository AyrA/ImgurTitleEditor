namespace ImgurTitleEditor
{
    partial class frmBulkUpload
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmBulkUpload));
            this.btnAddImages = new System.Windows.Forms.Button();
            this.lbFileList = new System.Windows.Forms.ListBox();
            this.btnStartUpload = new System.Windows.Forms.Button();
            this.OFD = new System.Windows.Forms.OpenFileDialog();
            this.lblRemoveInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnAddImages
            // 
            this.btnAddImages.Location = new System.Drawing.Point(12, 12);
            this.btnAddImages.Name = "btnAddImages";
            this.btnAddImages.Size = new System.Drawing.Size(75, 23);
            this.btnAddImages.TabIndex = 0;
            this.btnAddImages.Text = "&Add Images";
            this.btnAddImages.UseVisualStyleBackColor = true;
            this.btnAddImages.Click += new System.EventHandler(this.btnAddImages_Click);
            // 
            // lbFileList
            // 
            this.lbFileList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbFileList.FormattingEnabled = true;
            this.lbFileList.Location = new System.Drawing.Point(12, 41);
            this.lbFileList.Name = "lbFileList";
            this.lbFileList.Size = new System.Drawing.Size(568, 290);
            this.lbFileList.TabIndex = 2;
            this.lbFileList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbFileList_KeyDown);
            // 
            // btnStartUpload
            // 
            this.btnStartUpload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStartUpload.Location = new System.Drawing.Point(505, 12);
            this.btnStartUpload.Name = "btnStartUpload";
            this.btnStartUpload.Size = new System.Drawing.Size(75, 23);
            this.btnStartUpload.TabIndex = 1;
            this.btnStartUpload.Text = "&Upload all";
            this.btnStartUpload.UseVisualStyleBackColor = true;
            this.btnStartUpload.Click += new System.EventHandler(this.btnStartUpload_Click);
            // 
            // OFD
            // 
            this.OFD.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif|Video Files|*.mp4;*.gifv;*.webm|All Fi" +
    "les|*.*";
            this.OFD.Multiselect = true;
            this.OFD.Title = "Select Image File";
            // 
            // lblRemoveInfo
            // 
            this.lblRemoveInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRemoveInfo.AutoEllipsis = true;
            this.lblRemoveInfo.Location = new System.Drawing.Point(12, 345);
            this.lblRemoveInfo.Name = "lblRemoveInfo";
            this.lblRemoveInfo.Size = new System.Drawing.Size(568, 16);
            this.lblRemoveInfo.TabIndex = 3;
            this.lblRemoveInfo.Text = "Use [DEL] to remove items and [ALT]+[ARROW] to reorder items";
            // 
            // frmBulkUpload
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(592, 373);
            this.Controls.Add(this.lblRemoveInfo);
            this.Controls.Add(this.btnStartUpload);
            this.Controls.Add(this.lbFileList);
            this.Controls.Add(this.btnAddImages);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(200, 150);
            this.Name = "frmBulkUpload";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Bulk File Upload";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnAddImages;
        private System.Windows.Forms.ListBox lbFileList;
        private System.Windows.Forms.Button btnStartUpload;
        private System.Windows.Forms.OpenFileDialog OFD;
        private System.Windows.Forms.Label lblRemoveInfo;
    }
}