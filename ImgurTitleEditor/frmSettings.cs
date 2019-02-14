using System;
using System.Windows.Forms;

namespace ImgurTitleEditor
{
    public partial class frmSettings : Form
    {
        private const string IMGUR_REGISTRATION = "https://api.imgur.com/oauth2/addclient";
        private Settings S;

        public frmSettings(Settings S)
        {
            this.S = S;
            InitializeComponent();
            tbApiId.Text = S.Client.Id;
            tbApiSecret.Text = S.Client.Secret;
            nudPageSize.Value = Math.Max(nudPageSize.Minimum, Math.Min(nudPageSize.Maximum, S.UI.PageSize));
            DialogResult = DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            bool Reauth = tbApiId.Text != S.Client.Id;
            if (string.IsNullOrEmpty(tbApiId.Text))
            {
                MessageBox.Show("The Client Id field can't be empty", "Invalid Settings", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            S.Client.Id = tbApiId.Text;
            S.Client.Secret = tbApiSecret.Text;
            S.UI.PageSize = (int)Math.Max(nudPageSize.Minimum, Math.Min(nudPageSize.Maximum, nudPageSize.Value));
            Tools.SaveSettings(S, Program.SettingsFile);
            DialogResult = DialogResult.OK;
            if (Reauth)
            {
                MessageBox.Show("The Client Id field changed. For the changes to take effect, please authenticate again.", "Client Id change", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will open your webbrowser. Please register an \"OAuth 2 Application without a callback URL\", then come back here and fill in the ID and secret.\r\n\r\nContinue? (Imgur Account Required)", "OAuth2 Registration", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                try
                {
                    System.Diagnostics.Process.Start(IMGUR_REGISTRATION);
                }
                catch (Exception ex)
                {
                    Clipboard.SetText(IMGUR_REGISTRATION);
                    MessageBox.Show($"Unable to navigate to {IMGUR_REGISTRATION}\r\nReason: {ex.Message}\r\n\r\nThe URL was copied to your clipboard to open it manually.", "OAuth2 Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void llCopy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Clipboard.SetText(IMGUR_REGISTRATION);
            MessageBox.Show($"{IMGUR_REGISTRATION} copied to clipboard.", "OAuth2 Registration", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
