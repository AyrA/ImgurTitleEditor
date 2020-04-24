using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImgurTitleEditor
{
    /// <summary>
    /// Edits image properties
    /// </summary>
    public partial class frmProperties : Form
    {
        /// <summary>
        /// Action to perform on ENTER key
        /// </summary>
        private enum ActionType
        {
            /// <summary>
            /// Close form
            /// </summary>
            None,
            /// <summary>
            /// Go to previously uploaded image
            /// </summary>
            Older,
            /// <summary>
            /// Go to next uploaded image
            /// </summary>
            Newer
        }

        /// <summary>
        /// Current image
        /// </summary>
        private ImgurImage I;
        /// <summary>
        /// Current settings
        /// </summary>
        private readonly Settings S;
        /// <summary>
        /// Next action to perform
        /// </summary>
        private ActionType LastAction;

        /// <summary>
        /// Initializes the property form
        /// </summary>
        /// <param name="S">Current settings</param>
        /// <param name="I">Current image</param>
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
                Size ConfigSize = S.UI.PropertyWindowSize;
                if (ConfigSize.Height >= MinimumSize.Height && ConfigSize.Width >= MinimumSize.Width)
                {
                    Size = ConfigSize;
                }
            }

            SetImage(I);
        }

        #region Events

        /// <summary>
        /// Opens previous image
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void BtnPrev_Click(object sender, EventArgs e)
        {
            if (Save())
            {
                LastAction = ActionType.Newer;
                ImgurImage img = Application.OpenForms.OfType<FrmMain>().First().PrevImage();
                if (img != null)
                {
                    SetImage(img);
                }
            }
        }

        /// <summary>
        /// Opens next image
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void BtnNext_Click(object sender, EventArgs e)
        {
            if (Save())
            {
                LastAction = ActionType.Older;
                ImgurImage img = Application.OpenForms.OfType<FrmMain>().First().NextImage();
                if (img != null)
                {
                    SetImage(img);
                }
            }
        }

        /// <summary>
        /// Saves changes
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void BtnOK_Click(object sender, EventArgs e)
        {
            LastAction = ActionType.None;
            if (Save())
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        /// <summary>
        /// Closes the form
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// Saves new size settings
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void FrmProperties_Resize(object sender, EventArgs e)
        {
            ScaleImage();
        }

        /// <summary>
        /// Handles enter key in title box
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void TbTitle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = e.SuppressKeyPress = true;
                HandleNextImage();
            }
        }

        /// <summary>
        /// Initial form show event
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void FrmProperties_Shown(object sender, EventArgs e)
        {
            ScaleImage();
        }

        /// <summary>
        /// Handles description CTRL+ENTER key press
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void TbDesc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && e.Modifiers == Keys.Control)
            {
                e.Handled = e.SuppressKeyPress = true;
                HandleNextImage();
            }
        }

        /// <summary>
        /// Saves new size settings
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void FrmProperties_SizeChanged(object sender, EventArgs e)
        {
            S.UI.PropertyWindowMaximized = WindowState == FormWindowState.Maximized;
            S.UI.PropertyWindowSize = Size;
            Tools.SaveSettings(S, Program.SettingsFile);
        }

        #endregion

        /// <summary>
        /// Sets the currently shown image
        /// </summary>
        /// <param name="I">Current image</param>
        private void SetImage(ImgurImage I)
        {
            this.I = I;
            Text = $"Image Properties [{I.name}] ({I.views} views)";
            tbTitle.Text = I.title;
            //Imgur uses \n only
            if (string.IsNullOrEmpty(I.description))
            {
                tbDesc.Text = string.Empty;
            }
            else
            {
                tbDesc.Lines = I.description.Split('\n');
            }
            if (!I.animated)
            {
                using (MemoryStream MS = new MemoryStream(Cache.GetImage(I)))
                {
                    using (Image img = Image.FromStream(MS))
                    {
                        pbImage.Image = (Image)img.Clone();
                    }
                }
            }
            else
            {
                //var Imgur = new Imgur(S);
                //This is a temporary solution until I can figure out how to play GIF animations again
                using (MemoryStream MS = new MemoryStream(Imgur.GetImage(I, ImgurImageSize.HugeThumbnail, false)))
                {
                    using (Image img = Image.FromStream(MS))
                    {
                        pbImage.Image = (Image)img.Clone();
                    }
                }
            }
            ScaleImage();
            tbTitle.Focus();
            tbTitle.SelectAll();
        }

        /// <summary>
        /// Saves changes
        /// </summary>
        /// <returns>"True", if saved</returns>
        private bool Save()
        {
            string newTitle = string.IsNullOrEmpty(tbTitle.Text) ? null : tbTitle.Text;
            string newDesc = string.IsNullOrEmpty(tbDesc.Text) ? null : tbDesc.Text;
            if (I.title != newTitle || I.description != newDesc)
            {
                //Replace empty strings with null
                I.title = newTitle;
                I.description = newDesc;

                Imgur imgur = new Imgur(S);
                bool Result = false;
                Exception TaskException = null;
                Task.Run(async delegate
                {
                    try
                    {
                        Result = await imgur.SetImageDescription(I, I.title, I.description);
                    }
                    catch (Exception ex)
                    {
                        TaskException = ex;
                        Result = false;
                    }
                }).Wait();
                if (Result)
                {
                    //Replace updated image in cache
                    ImgurImage[] All = Cache.Images;
                    for (int i = 0; i < All.Length; i++)
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
                    if (TaskException != null)
                    {
                        MessageBox.Show("Unable to set new image properties", "Unable to update image", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show($"Unable to set new image properties. Details: {TaskException.Message}", "Unable to update image", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// Scales the picture box to show the image properly
        /// </summary>
        private void ScaleImage()
        {
            if (pbImage.Image != null)
            {
                if (pbImage.Width >= pbImage.Image.Width && pbImage.Height >= pbImage.Image.Height)
                {
                    pbImage.SizeMode = PictureBoxSizeMode.CenterImage;
                }
                else
                {
                    pbImage.SizeMode = PictureBoxSizeMode.Zoom;
                }
            }
        }

        /// <summary>
        /// Handle next image action
        /// </summary>
        private void HandleNextImage()
        {
            switch (LastAction)
            {
                case ActionType.Newer:
                    BtnPrev_Click(null, null);
                    break;
                case ActionType.Older:
                    BtnNext_Click(null, null);
                    break;
                case ActionType.None:
                    BtnOK_Click(null, null);
                    break;
            }
        }
    }
}
