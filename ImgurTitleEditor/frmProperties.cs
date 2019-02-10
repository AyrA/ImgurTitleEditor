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
        private enum ActionType
        {
            None,
            Older,
            Newer
        }

        private ImgurImage I;
        private Settings S;
        private ActionType LastAction;

        public frmProperties(Settings S, ImgurImage I)
        {
            LastAction = ActionType.None;
            this.S = S;
            InitializeComponent();

            if (S.UI.PropertyWindowMaximized)
            {
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                var ConfigSize = S.UI.PropertyWindowSize;
                if (ConfigSize.Height >= MinimumSize.Height && ConfigSize.Width >= MinimumSize.Width)
                {
                    Size = ConfigSize;
                }
            }

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
                using (var img = Image.FromStream(MS))
                {
                    pbImage.Image = (Image)img.Clone();
                }
            }
            ScaleImage();
            tbTitle.Focus();
            tbTitle.SelectAll();
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
                LastAction = ActionType.Newer;
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
                LastAction = ActionType.Older;
                var img = Application.OpenForms.OfType<frmMain>().First().NextImage();
                if (img != null)
                {
                    SetImage(img);
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            LastAction = ActionType.None;
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

        private void frmProperties_Resize(object sender, EventArgs e)
        {
            ScaleImage();
        }

        private void ScaleImage()
        {
            if (pbImage.Image != null)
            {
                if (pbImage.Width >= pbImage.Image.Width && pbImage.Height >= pbImage.Image.Height)
                {
                    pbImage.SizeMode = PictureBoxSizeMode.Normal;
                }
                else
                {
                    pbImage.SizeMode = PictureBoxSizeMode.Zoom;
                }
            }
        }

        private void tbTitle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = e.SuppressKeyPress = true;
                HandleNextImage();
            }
        }

        private void HandleNextImage()
        {
            switch (LastAction)
            {
                case ActionType.Newer:
                    btnPrev_Click(null, null);
                    break;
                case ActionType.Older:
                    btnNext_Click(null, null);
                    break;
                case ActionType.None:
                    btnOK_Click(null, null);
                    break;
            }
        }

        private void frmProperties_Shown(object sender, EventArgs e)
        {
            ScaleImage();
        }

        private void tbDesc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && e.Modifiers == Keys.Control)
            {
                e.Handled = e.SuppressKeyPress = true;
                HandleNextImage();
            }
        }

        private void frmProperties_SizeChanged(object sender, EventArgs e)
        {
            S.UI.PropertyWindowMaximized = WindowState == FormWindowState.Maximized;
            S.UI.PropertyWindowSize = Size;
            Tools.SaveSettings(S, Program.SettingsFile);
        }
    }
}
