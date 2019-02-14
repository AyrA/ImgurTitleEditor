using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ImgurTitleEditor
{
    public partial class frmUpload : Form
    {
        private Settings S;
        private bool UseClipboard = false;

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
                try
                {
                    pbPreview.Image = Image.FromFile(tbImage.Text);
                }
                catch
                {
                    MessageBox.Show("Unable to display the selected image. Imgur might deny your upload request.", "Image Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                UseClipboard = false;
            }
        }

        private async void btnUpload_Click(object sender, EventArgs e)
        {
            byte[] Data = null;
            try
            {
                if (UseClipboard)
                {
                    using (var MS = new MemoryStream())
                    {
                        pbPreview.Image.Save(MS, ImageFormat.Png);
                        Data = MS.ToArray();
                    }
                }
                else
                {
                    Data = File.ReadAllBytes(tbImage.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to read file. Reason: {ex.Message}", "File read Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (Data != null)
            {
                var I = new Imgur(S);
                try
                {
                    var img = await I.UploadImage(Data, Path.GetFileName(tbImage.Text), tbTitle.Text, tbDescription.Text);
                    if (img != null)
                    {
                        //Add new image
                        Cache.Images = (new ImgurImage[] { img }).Concat(Cache.Images).ToArray();
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                    else
                    {
                        throw new Exception("Imgur API did not accept the image");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Unable to upload your image. Reason: {ex.Message}", "File read Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnClipboard_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsImage())
            {
                pbPreview.Image = Clipboard.GetImage();
                UseClipboard = true;
            }
            else
            {
                MessageBox.Show("Your Clipboard has no image", "No Image", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
