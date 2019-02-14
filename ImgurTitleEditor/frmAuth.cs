using System;
using System.Linq;
using System.Windows.Forms;

namespace ImgurTitleEditor
{
    public partial class frmAuth : Form
    {
        private Settings S;
        public frmAuth(Settings Settings)
        {
            S = Settings;
            InitializeComponent();
            //This event can't be bound in the UI editor because it's missing
            wbAuth.DocumentTitleChanged += WbAuth_DocumentTitleChanged;
            Init();
        }

        private void WbAuth_DocumentTitleChanged(object sender, EventArgs e)
        {
            Text = $"Authentication - {wbAuth.DocumentTitle}";
        }

        private void Init()
        {
            //We don't really care about the "state" parameter but use it as a neat way to avoid caching
            wbAuth.Navigate($"https://api.imgur.com/oauth2/authorize?client_id={Uri.EscapeDataString(S.Client.Id)}&response_type=token&state={DateTime.UtcNow.Ticks}");
            DialogResult = DialogResult.Cancel;
        }

        private void wbAuth_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            //Set the current URL in the Textbox
            tbURL.Text = wbAuth.Url.ToString();
            //Extract fragments and check if certain values are there
            var Parts = Tools.ParseFragment(wbAuth.Url, false);
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
                var SearchParams = Tools.ParseFragment(wbAuth.Url, true);
                if (SearchParams.ContainsKey("error"))
                {
                    if (MessageBox.Show($@"There was a problem authorizing this application.
Message: {SearchParams["error"]}
Do you want to try again?", "Authorization error.", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    {
                        Init();
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
    }
}
