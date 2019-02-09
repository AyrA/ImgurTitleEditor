using System;
using System.IO;
using System.Windows.Forms;

namespace ImgurTitleEditor
{
    public partial class frmMain : Form
    {
        public frmMain(Settings S)
        {
            InitializeComponent();
            if (S.Token.Expires < DateTime.UtcNow)
            {
                using (var f = new frmAuth(S))
                {
                    if (f.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllText(Program.SettingsFile, S.Save());
                    }
                    else
                    {
                        MessageBox.Show("Could not authorize this application", "No Authorization", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.Exit();
                    }
                }
            }
        }
    }
}
