namespace ImgurTitleEditor.UI
{
    partial class FrmBulkUpload
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmBulkUpload));
            this.btnAddImages = new System.Windows.Forms.Button();
            this.lbFileList = new System.Windows.Forms.ListBox();
            this.btnStartUpload = new System.Windows.Forms.Button();
            this.OFD = new System.Windows.Forms.OpenFileDialog();
            this.lblRemoveInfo = new System.Windows.Forms.Label();
            this.lblAlbum = new System.Windows.Forms.Label();
            this.cbAlbum = new System.Windows.Forms.ComboBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.tbTitle = new System.Windows.Forms.TextBox();
            this.cbDescDate = new System.Windows.Forms.CheckBox();
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
            this.btnAddImages.Click += new System.EventHandler(this.BtnAddImages_Click);
            // 
            // lbFileList
            // 
            this.lbFileList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbFileList.FormattingEnabled = true;
            this.lbFileList.Location = new System.Drawing.Point(12, 41);
            this.lbFileList.Name = "lbFileList";
            this.lbFileList.Size = new System.Drawing.Size(568, 277);
            this.lbFileList.TabIndex = 2;
            this.lbFileList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LbFileList_KeyDown);
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
            this.btnStartUpload.Click += new System.EventHandler(this.BtnStartUpload_Click);
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
            this.lblRemoveInfo.Location = new System.Drawing.Point(12, 329);
            this.lblRemoveInfo.Name = "lblRemoveInfo";
            this.lblRemoveInfo.Size = new System.Drawing.Size(568, 16);
            this.lblRemoveInfo.TabIndex = 3;
            this.lblRemoveInfo.Text = "Use [DEL] to remove items and [ALT]+[ARROW] to reorder items";
            // 
            // lblAlbum
            // 
            this.lblAlbum.AutoSize = true;
            this.lblAlbum.Location = new System.Drawing.Point(106, 17);
            this.lblAlbum.Name = "lblAlbum";
            this.lblAlbum.Size = new System.Drawing.Size(73, 13);
            this.lblAlbum.TabIndex = 4;
            this.lblAlbum.Text = "Add to Album:";
            // 
            // cbAlbum
            // 
            this.cbAlbum.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbAlbum.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAlbum.FormattingEnabled = true;
            this.cbAlbum.Location = new System.Drawing.Point(188, 12);
            this.cbAlbum.Name = "cbAlbum";
            this.cbAlbum.Size = new System.Drawing.Size(302, 21);
            this.cbAlbum.TabIndex = 5;
            // 
            // lblTitle
            // 
            this.lblTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(12, 366);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(30, 13);
            this.lblTitle.TabIndex = 6;
            this.lblTitle.Text = "Title:";
            // 
            // tbTitle
            // 
            this.tbTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbTitle.Location = new System.Drawing.Point(48, 363);
            this.tbTitle.Name = "tbTitle";
            this.tbTitle.Size = new System.Drawing.Size(532, 20);
            this.tbTitle.TabIndex = 7;
            this.tbTitle.Text = "%N.%X";
            // 
            // cbDescDate
            // 
            this.cbDescDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbDescDate.AutoSize = true;
            this.cbDescDate.Location = new System.Drawing.Point(48, 392);
            this.cbDescDate.Name = "cbDescDate";
            this.cbDescDate.Size = new System.Drawing.Size(158, 17);
            this.cbDescDate.TabIndex = 9;
            this.cbDescDate.Text = "Put UTC date in Description";
            this.cbDescDate.UseVisualStyleBackColor = true;
            // 
            // frmBulkUpload
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(592, 420);
            this.Controls.Add(this.cbDescDate);
            this.Controls.Add(this.tbTitle);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.cbAlbum);
            this.Controls.Add(this.lblAlbum);
            this.Controls.Add(this.lblRemoveInfo);
            this.Controls.Add(this.btnStartUpload);
            this.Controls.Add(this.lbFileList);
            this.Controls.Add(this.btnAddImages);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(200, 150);
            this.Name = "frmBulkUpload";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Bulk File Upload";
            this.Load += new System.EventHandler(this.FrmBulkUpload_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAddImages;
        private System.Windows.Forms.ListBox lbFileList;
        private System.Windows.Forms.Button btnStartUpload;
        private System.Windows.Forms.OpenFileDialog OFD;
        private System.Windows.Forms.Label lblRemoveInfo;
        private System.Windows.Forms.Label lblAlbum;
        private System.Windows.Forms.ComboBox cbAlbum;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TextBox tbTitle;
        private System.Windows.Forms.CheckBox cbDescDate;
    }
}