namespace ImgurTitleEditor
{
    partial class frmAuth
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
            this.wbAuth = new System.Windows.Forms.WebBrowser();
            this.tbURL = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // wbAuth
            // 
            this.wbAuth.AllowWebBrowserDrop = false;
            this.wbAuth.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wbAuth.IsWebBrowserContextMenuEnabled = false;
            this.wbAuth.Location = new System.Drawing.Point(0, 20);
            this.wbAuth.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbAuth.Name = "wbAuth";
            this.wbAuth.ScriptErrorsSuppressed = true;
            this.wbAuth.Size = new System.Drawing.Size(792, 553);
            this.wbAuth.TabIndex = 1;
            this.wbAuth.WebBrowserShortcutsEnabled = false;
            this.wbAuth.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.wbAuth_Navigated);
            // 
            // tbURL
            // 
            this.tbURL.Dock = System.Windows.Forms.DockStyle.Top;
            this.tbURL.Location = new System.Drawing.Point(0, 0);
            this.tbURL.Name = "tbURL";
            this.tbURL.ReadOnly = true;
            this.tbURL.Size = new System.Drawing.Size(792, 20);
            this.tbURL.TabIndex = 0;
            // 
            // frmAuth
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 573);
            this.Controls.Add(this.wbAuth);
            this.Controls.Add(this.tbURL);
            this.Name = "frmAuth";
            this.Text = "Authentication";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbURL;
        private System.Windows.Forms.WebBrowser wbAuth;
    }
}