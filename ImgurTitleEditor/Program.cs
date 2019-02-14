using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ImgurTitleEditor
{
    static class Program
    {
        private const string CONFIG = "config.xml";
        private static string _SettingsFile;
        public static string SettingsFile
        {
            get
            {
                if (string.IsNullOrEmpty(_SettingsFile))
                {
                    _SettingsFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), CONFIG);
                }
                return _SettingsFile;
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Settings S;
            if (File.Exists(SettingsFile))
            {
                S = Tools.LoadSettings(SettingsFile);
            }
            else
            {
                S = new Settings()
                {
                    Client = new Client()
                    {
                        //I recommend you also set a secret in the settings.
                        //Without a secret we can get a token, but not refresh it
                        Id = "a5e26e2dac343b6"
                    }
                };
                Tools.SaveSettings(S, SettingsFile);
            }

            //Clear any invalid items from the cache
            if (Cache.Images != null && Cache.Images.Any(m => m == null))
            {
                Cache.Images = Cache.Images.Where(m => m != null).ToArray();
            }

            var I = new Imgur(S);
            if (!string.IsNullOrEmpty(S.Token.Access) && S.Token.Expires < DateTime.UtcNow.AddDays(7))
            {
                if (I.RenewToken().Result)
                {
                    Tools.SaveSettings(S, SettingsFile);
                }
                else
                {
                    MessageBox.Show("Unable to refresh your API token.\r\nPlease Authorize the application again.", "Token Refresh Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain(S));
        }
    }
}
