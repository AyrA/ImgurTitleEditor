using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImgurTitleEditor
{
    public partial class frmProperties : Form
    {
        private ImgurImage I;
        private Settings S;
        public frmProperties(Settings S, ImgurImage I)
        {
            this.S = S;
            InitializeComponent();
            SetImage(I);
        }

        private void SetImage(ImgurImage I)
        {
            this.I = I;
            Text = $"Image Properties [{I.name}] ({I.views} views)";
            tbTitle.Text = I.title;
            tbDesc.Text = I.description;
            using (var MS = new MemoryStream(Cache.GetImage(I)))
            {
                pbImage.Image = Image.FromStream(MS);
            }
        }

        private bool Save()
        {
            var newTitle = string.IsNullOrEmpty(tbTitle.Text) ? null : tbTitle.Text;
            var newDesc = string.IsNullOrEmpty(tbDesc.Text) ? null : tbDesc.Text;
            if (I.title != newTitle || I.description != newDesc)
            {
                //Replace empty strings with null
                I.title = newTitle;
                I.description = newDesc;

                var imgur = new Imgur(S);
                var Result = false;
                Task.Run(async delegate
                {
                    Result = await imgur.SetImageDescription(I, I.title, I.description);
                }).Wait();
                if (Result)
                {
                    //Replace updated image in cache
                    var All = Cache.Images;
                    for (var i = 0; i < All.Length; i++)
                    {
                        if (All[i].id == I.id)
                        {
                            All[i] = I;
                        }
                    }
                    Cache.Images = All;
                    return true;
                }
                else
                {
                    MessageBox.Show("Unable to set new image properties", "Unable to update image", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }
            return true;
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (Save())
            {
                var img = Application.OpenForms.OfType<frmMain>().First().PrevImage();
                if (img != null)
                {
                    SetImage(img);
                }
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (Save())
            {
                var img = Application.OpenForms.OfType<frmMain>().First().NextImage();
                if (img != null)
                {
                    SetImage(img);
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (Save())
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
