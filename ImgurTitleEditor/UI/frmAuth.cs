using ImgurTitleEditor.Configuration;
using System;
using System.Linq;
using System.Windows.Forms;

namespace ImgurTitleEditor.UI
{
    /// <summary>
    /// Form that handles OAuth2 authentication
    /// </summary>
    public partial class frmAuth : Form
    {
        /// <summary>
        /// Current settings
        /// </summary>
        private readonly Settings S;

        /// <summary>
        /// Initializes a new authentication form
        /// </summary>
        /// <param name="Settings">Current settings</param>
        public frmAuth(Settings Settings)
        {
            S = Settings;
            InitializeComponent();
            DialogResult = DialogResult.Cancel;
            //This event can't be bound in the UI editor because it's missing
            wbAuth.DocumentTitleChanged += WbAuth_DocumentTitleChanged;
            InitBrowser();
        }

        /// <summary>
        /// (Re-)initializes the browser control
        /// </summary>
        private void InitBrowser()
        {
            //We don't really care about the "state" parameter but use it as a neat way to avoid caching
            wbAuth.Navigate($"https://api.imgur.com/oauth2/authorize?client_id={Uri.EscapeDataString(S.Client.Id)}&response_type=token&state={DateTime.UtcNow.Ticks}");
        }

        #region Events

        /// <summary>
        /// Updates the window title
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void WbAuth_DocumentTitleChanged(object sender, EventArgs e)
        {
            Text = $"Authentication - {wbAuth.DocumentTitle}";
        }

        /// <summary>
        /// Checks if the navigation target is an authentication success or failure
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void WbAuth_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            //Set the current URL in the Textbox
            tbURL.Text = wbAuth.Url.ToString();
            //Extract fragments and check if certain values are there
            System.Collections.Generic.Dictionary<string, string> Parts = Tools.ParseFragment(wbAuth.Url, false);
            if ("access_token|refresh_token|expires_in".Split('|').All(m => Parts.ContainsKey(m) && !string.IsNullOrWhiteSpace(Parts[m])))
            {
                if (Tools.LongOrDefault(Parts["expires_in"]) > 0)
                {
                    wbAuth.Stop();
                    wbAuth.AllowNavigation = false;
                    S.Token.Expires = DateTime.UtcNow.AddSeconds(long.Parse(Parts["expires_in"]));
                    S.Token.Access = Parts["access_token"];
                    S.Token.Refresh = Parts["refresh_token"];
                    if (string.IsNullOrEmpty(S.Client.Secret))
                    {
                        MessageBox.Show($@"Authorization was successful but no client secret is present in your configuration.
You will not be able to extend this access token without authorizing again manually.
Please add the Secret at any time before {S.Token.Expires.ToShortDateString()}", "Missing Client Secret", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            else
            {
                System.Collections.Generic.Dictionary<string, string> SearchParams = Tools.ParseFragment(wbAuth.Url, true);
                if (SearchParams.ContainsKey("error"))
                {
                    if (MessageBox.Show($@"There was a problem authorizing this application.
Message: {SearchParams["error"]}
Do you want to try again?", "Authorization error.", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    {
                        InitBrowser();
                    }
                    else
                    {
                        wbAuth.Stop();
                        wbAuth.AllowNavigation = false;
                        Close();
                    }
                }
            }
        }

        /// <summary>
        /// Closes the authentication form
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Restarts the authentication process
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void RestartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InitBrowser();
        }

        /// <summary>
        /// Opens the settings
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FrmSettings f = new FrmSettings(S))
            {
                if (f.ShowDialog() == DialogResult.OK)
                {
                    InitBrowser();
                }
            }
        }

        #endregion
    }
}
