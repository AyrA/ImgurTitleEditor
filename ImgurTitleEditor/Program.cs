using System;
using System.IO;
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
                        Id = "a5e26e2dac343b6"
                    }
                };
                Tools.SaveSettings(S, SettingsFile);
            }

            var I = new Imgur(S);
            if (S.Token.Expires < DateTime.UtcNow.AddDays(7))
            {
                if (I.RenewToken().Result)
                {
                    Tools.SaveSettings(S, SettingsFile);
                }
            }

            var Settings = I.GetAccountSettings().Result;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain(S));
        }
    }
}
