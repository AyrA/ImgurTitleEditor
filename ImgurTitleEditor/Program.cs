using ImgurTitleEditor.Configuration;
using ImgurTitleEditor.UI;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ImgurTitleEditor
{
    static class Program
    {
        /// <summary>
        /// Config file name
        /// </summary>
        private const string CONFIG = "config.xml";

        /// <summary>
        /// Absolute settings file path and name
        /// </summary>
        private static string _SettingsFile;
        /// <summary>
        /// Gets the full settings file path and name
        /// </summary>
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
                //Load existing settings
                S = Tools.LoadSettings(SettingsFile);
            }
            else
            {
                //Create default settings
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
            if (Cache.Images != null)
            {
                ImgurImage[] CacheImages = Cache.Images;
                if (CacheImages.Any(m => m == null))
                {
                    Cache.Images = CacheImages.Where(m => m != null).ToArray();
                }
            }

            //Try to refresh the token if expired or close to expiration
            Imgur I = new Imgur(S);
            //Update initial credits values
            I.CheckCredits().Wait();
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
            Application.Run(new FrmMain(S));
        }
    }
}
