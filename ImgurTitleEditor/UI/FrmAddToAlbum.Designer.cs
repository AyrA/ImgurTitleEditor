namespace ImgurTitleEditor.UI
{
    partial class FrmAddToAlbum
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
            this.CbAlbum = new System.Windows.Forms.ComboBox();
            this.BtnAdd = new System.Windows.Forms.Button();
            this.lvImages = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // CbAlbum
            // 
            this.CbAlbum.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CbAlbum.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CbAlbum.Enabled = false;
            this.CbAlbum.FormattingEnabled = true;
            this.CbAlbum.Location = new System.Drawing.Point(12, 12);
            this.CbAlbum.Name = "CbAlbum";
            this.CbAlbum.Size = new System.Drawing.Size(643, 21);
            this.CbAlbum.TabIndex = 0;
            // 
            // BtnAdd
            // 
            this.BtnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnAdd.Location = new System.Drawing.Point(661, 10);
            this.BtnAdd.Name = "BtnAdd";
            this.BtnAdd.Size = new System.Drawing.Size(127, 23);
            this.BtnAdd.TabIndex = 2;
            this.BtnAdd.Text = "&Add";
            this.BtnAdd.UseVisualStyleBackColor = true;
            // 
            // lvImages
            // 
            this.lvImages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvImages.HideSelection = false;
            this.lvImages.Location = new System.Drawing.Point(12, 39);
            this.lvImages.Name = "lvImages";
            this.lvImages.Size = new System.Drawing.Size(776, 399);
            this.lvImages.TabIndex = 5;
            this.lvImages.UseCompatibleStateImageBehavior = false;
            // 
            // FrmAddToAlbum
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lvImages);
            this.Controls.Add(this.BtnAdd);
            this.Controls.Add(this.CbAlbum);
            this.Name = "FrmAddToAlbum";
            this.Text = "Add images to an album";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox CbAlbum;
        private System.Windows.Forms.Button BtnAdd;
        private System.Windows.Forms.ListView lvImages;
    }
}