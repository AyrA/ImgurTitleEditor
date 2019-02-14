namespace ImgurTitleEditor
{
    partial class frmSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSettings));
            this.gbAPI = new System.Windows.Forms.GroupBox();
            this.tbApiSecret = new System.Windows.Forms.TextBox();
            this.tbApiId = new System.Windows.Forms.TextBox();
            this.lblApiId = new System.Windows.Forms.Label();
            this.lblSecretInfo = new System.Windows.Forms.Label();
            this.lblApiSecret = new System.Windows.Forms.Label();
            this.gbUI = new System.Windows.Forms.GroupBox();
            this.lblPageSizeInfo = new System.Windows.Forms.Label();
            this.lblPageSize = new System.Windows.Forms.Label();
            this.nudPageSize = new System.Windows.Forms.NumericUpDown();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbAPI.SuspendLayout();
            this.gbUI.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPageSize)).BeginInit();
            this.SuspendLayout();
            // 
            // gbAPI
            // 
            this.gbAPI.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbAPI.Controls.Add(this.tbApiSecret);
            this.gbAPI.Controls.Add(this.tbApiId);
            this.gbAPI.Controls.Add(this.lblApiId);
            this.gbAPI.Controls.Add(this.lblSecretInfo);
            this.gbAPI.Controls.Add(this.lblApiSecret);
            this.gbAPI.Location = new System.Drawing.Point(12, 12);
            this.gbAPI.Name = "gbAPI";
            this.gbAPI.Size = new System.Drawing.Size(466, 123);
            this.gbAPI.TabIndex = 0;
            this.gbAPI.TabStop = false;
            this.gbAPI.Text = "API Settings";
            // 
            // tbApiSecret
            // 
            this.tbApiSecret.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbApiSecret.Location = new System.Drawing.Point(82, 42);
            this.tbApiSecret.Name = "tbApiSecret";
            this.tbApiSecret.Size = new System.Drawing.Size(378, 20);
            this.tbApiSecret.TabIndex = 3;
            // 
            // tbApiId
            // 
            this.tbApiId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbApiId.Location = new System.Drawing.Point(82, 16);
            this.tbApiId.Name = "tbApiId";
            this.tbApiId.Size = new System.Drawing.Size(378, 20);
            this.tbApiId.TabIndex = 1;
            // 
            // lblApiId
            // 
            this.lblApiId.AutoSize = true;
            this.lblApiId.Location = new System.Drawing.Point(10, 19);
            this.lblApiId.Name = "lblApiId";
            this.lblApiId.Size = new System.Drawing.Size(47, 13);
            this.lblApiId.TabIndex = 0;
            this.lblApiId.Text = "Client ID";
            // 
            // lblSecretInfo
            // 
            this.lblSecretInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSecretInfo.AutoEllipsis = true;
            this.lblSecretInfo.Location = new System.Drawing.Point(10, 80);
            this.lblSecretInfo.Name = "lblSecretInfo";
            this.lblSecretInfo.Size = new System.Drawing.Size(450, 40);
            this.lblSecretInfo.TabIndex = 4;
            this.lblSecretInfo.Text = "The Client Secret is optional, but you will not be able to renew access tokens au" +
    "tomatically if it\'s not set.";
            // 
            // lblApiSecret
            // 
            this.lblApiSecret.AutoSize = true;
            this.lblApiSecret.Location = new System.Drawing.Point(10, 45);
            this.lblApiSecret.Name = "lblApiSecret";
            this.lblApiSecret.Size = new System.Drawing.Size(67, 13);
            this.lblApiSecret.TabIndex = 2;
            this.lblApiSecret.Text = "Client Secret";
            // 
            // gbUI
            // 
            this.gbUI.Controls.Add(this.lblPageSizeInfo);
            this.gbUI.Controls.Add(this.lblPageSize);
            this.gbUI.Controls.Add(this.nudPageSize);
            this.gbUI.Location = new System.Drawing.Point(12, 141);
            this.gbUI.Name = "gbUI";
            this.gbUI.Size = new System.Drawing.Size(466, 100);
            this.gbUI.TabIndex = 1;
            this.gbUI.TabStop = false;
            this.gbUI.Text = "User Interface";
            // 
            // lblPageSizeInfo
            // 
            this.lblPageSizeInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPageSizeInfo.AutoEllipsis = true;
            this.lblPageSizeInfo.Location = new System.Drawing.Point(10, 42);
            this.lblPageSizeInfo.Name = "lblPageSizeInfo";
            this.lblPageSizeInfo.Size = new System.Drawing.Size(450, 55);
            this.lblPageSizeInfo.TabIndex = 2;
            this.lblPageSizeInfo.Text = "This setting configures how many items are shown at once on a page. Setting this " +
    "beyond 200 items can cause serious delay when building pages. Set to 0 to show a" +
    "ll items at once.";
            // 
            // lblPageSize
            // 
            this.lblPageSize.AutoSize = true;
            this.lblPageSize.Location = new System.Drawing.Point(10, 21);
            this.lblPageSize.Name = "lblPageSize";
            this.lblPageSize.Size = new System.Drawing.Size(55, 13);
            this.lblPageSize.TabIndex = 0;
            this.lblPageSize.Text = "Page Size";
            // 
            // nudPageSize
            // 
            this.nudPageSize.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudPageSize.Location = new System.Drawing.Point(71, 19);
            this.nudPageSize.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudPageSize.Name = "nudPageSize";
            this.nudPageSize.Size = new System.Drawing.Size(84, 20);
            this.nudPageSize.TabIndex = 1;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(322, 256);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(403, 256);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(490, 291);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.gbUI);
            this.Controls.Add(this.gbAPI);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSettings";
            this.ShowInTaskbar = false;
            this.Text = "Settings";
            this.gbAPI.ResumeLayout(false);
            this.gbAPI.PerformLayout();
            this.gbUI.ResumeLayout(false);
            this.gbUI.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPageSize)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbAPI;
        private System.Windows.Forms.TextBox tbApiSecret;
        private System.Windows.Forms.TextBox tbApiId;
        private System.Windows.Forms.Label lblApiId;
        private System.Windows.Forms.Label lblApiSecret;
        private System.Windows.Forms.Label lblSecretInfo;
        private System.Windows.Forms.GroupBox gbUI;
        private System.Windows.Forms.Label lblPageSizeInfo;
        private System.Windows.Forms.Label lblPageSize;
        private System.Windows.Forms.NumericUpDown nudPageSize;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}