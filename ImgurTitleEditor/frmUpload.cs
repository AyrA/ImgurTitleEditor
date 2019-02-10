using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ImgurTitleEditor
{
    public partial class frmUpload : Form
    {
        private Settings S;

        public frmUpload(Settings S)
        {
            this.S = S;
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (OFD.ShowDialog() == DialogResult.OK)
            {
                tbImage.Text = OFD.FileName;
            }
        }

        private async void btnUpload_Click(object sender, EventArgs e)
        {
            byte[] Data = null;
            try
            {
                Data = File.ReadAllBytes(tbImage.Text);
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Unable to read file. Reason: {ex.Message}", "File read Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (Data != null)
            {
                var I = new Imgur(S);
                try
                {
                    var img = await I.UploadImage(Data, Path.GetFileName(tbImage.Text), tbTitle.Text, tbDescription.Text);
                    //Add new image
                    Cache.Images = (new ImgurImage[] { img }).Concat(Cache.Images).ToArray();
                    DialogResult = DialogResult.OK;
                    Close();
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"Unable to upload your image. Reason: {ex.Message}", "File read Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
