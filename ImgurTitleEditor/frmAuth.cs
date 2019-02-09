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
            wbAuth.Navigate($"https://api.imgur.com/oauth2/authorize?client_id={S.Client.Id}&response_type=token&state={DateTime.UtcNow.Ticks}");
            DialogResult = DialogResult.Cancel;
        }

        private void wbAuth_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            tbURL.Text = wbAuth.Url.ToString();
            var Parts = Tools.ParseFragment(wbAuth.Url);
            if ("access_token|refresh_token|expires_in".Split('|').All(m => Parts.ContainsKey(m) && !string.IsNullOrWhiteSpace(Parts[m])))
            {
                if (Tools.LongOrDefault(Parts["expires_in"]) > 0)
                {
                    wbAuth.Stop();
                    wbAuth.AllowNavigation = false;
                    S.Token.Expires = DateTime.UtcNow.AddSeconds(int.Parse(Parts["expires_in"]));
                    S.Token.Access = Parts["access_token"];
                    S.Token.Refresh = Parts["refresh_token"];
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
        }
    }
}
