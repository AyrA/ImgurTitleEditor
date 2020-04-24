using System;
using System.Windows.Forms;

namespace ImgurTitleEditor
{
    /// <summary>
    /// Handles application settings
    /// </summary>
    public partial class FrmSettings : Form
    {
        /// <summary>
        /// Current settings
        /// </summary>
        private readonly Settings S;

        /// <summary>
        /// Initializes a new settings form
        /// </summary>
        /// <param name="S">Current settings</param>
        public FrmSettings(Settings S)
        {
            this.S = S;
            InitializeComponent();
            tbApiId.Text = S.Client.Id;
            tbApiSecret.Text = S.Client.Secret;
            nudPageSize.Value = Math.Max(nudPageSize.Minimum, Math.Min(nudPageSize.Maximum, S.UI.PageSize));
            DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// Validates and saves settings
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void BtnOK_Click(object sender, EventArgs e)
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

        /// <summary>
        /// Closes settings form
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Opens the OAuth2 registration website in the default browser
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void BtnRegister_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will open your webbrowser. Please register an \"OAuth 2 Application without a callback URL\", then come back here and fill in the ID and secret.\r\n\r\nContinue? (Imgur Account Required)", "OAuth2 Registration", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                try
                {
                    System.Diagnostics.Process.Start(Imgur.IMGUR_REGISTRATION);
                }
                catch (Exception ex)
                {
                    Clipboard.SetText(Imgur.IMGUR_REGISTRATION);
                    MessageBox.Show($"Unable to navigate to {Imgur.IMGUR_REGISTRATION}\r\nReason: {ex.Message}\r\n\r\nThe URL was copied to your clipboard to open it manually.", "OAuth2 Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Copies <see cref="Imgur.IMGUR_REGISTRATION"/> to the clipboard
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void LlCopy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Clipboard.SetText(Imgur.IMGUR_REGISTRATION);
            MessageBox.Show($"{Imgur.IMGUR_REGISTRATION} copied to clipboard.", "OAuth2 Registration", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Shows the client secret
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void TbApiSecret_Enter(object sender, EventArgs e)
        {
            tbApiSecret.UseSystemPasswordChar = false;
        }

        /// <summary>
        /// Hides the client secret
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void TbApiSecret_Leave(object sender, EventArgs e)
        {
            tbApiSecret.UseSystemPasswordChar = true;
        }
    }
}
