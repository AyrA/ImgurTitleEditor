using System;
using System.Windows.Forms;

namespace ImgurTitleEditor
{
    public partial class frmSettings : Form
    {
        Settings S;
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
    }
}
