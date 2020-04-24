using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ImgurTitleEditor
{
    /// <summary>
    /// Main form that handles gallery browsing
    /// </summary>
    public partial class FrmMain : Form
    {
        /// <summary>
        /// Current gallery page
        /// </summary>
        private int CurrentPage;
        /// <summary>
        /// Total number of gallery pages
        /// </summary>
        private int Pages;

        /// <summary>
        /// Gallery view mode
        /// </summary>
        private enum ImageFilter : int
        {
            /// <summary>
            /// All images
            /// </summary>
            All = 0,
            /// <summary>
            /// Only images with a title
            /// </summary>
            WithTitle = 1,
            /// <summary>
            /// Only images without a title
            /// </summary>
            WithoutTitle = 2
        }

        /// <summary>
        /// Imgur API handler
        /// </summary>
        private Imgur I;
        /// <summary>
        /// Current settings
        /// </summary>
        private readonly Settings S;

        /// <summary>
        /// Initializes a new main form
        /// </summary>
        /// <param name="S">Current settings</param>
        public FrmMain(Settings S)
        {
            this.S = S;
            InitializeComponent();
            TbFilter_Leave(null, null);
            //Program.Main already tries to authorize.
            //If the date is still in the past, the user has to do so manually.
            if (S.Token.Expires < DateTime.UtcNow)
            {
                using (var f = new frmAuth(S))
                {
                    if (f.ShowDialog() == DialogResult.OK)
                    {
                        Tools.SaveSettings(S, Program.SettingsFile);
                    }
                    else
                    {
                        MessageBox.Show("Could not authorize this application. Will exit now.", "No Authorization", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(1);
                        return;
                    }
                }
            }
            I = new Imgur(S);
            if (S.UI.MainWindowMaximized)
            {
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                var ConfigSize = S.UI.MainWindowSize;
                if (ConfigSize.Height >= MinimumSize.Height && ConfigSize.Width >= MinimumSize.Width)
                {
                    Size = ConfigSize;
                }
            }
            InitPages();
        }

        #region Events

        /// <summary>
        /// Exits the application
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// (Re-)builds the cache
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void BuildCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Cache.Images == null || Cache.Images.Length == 0 || MessageBox.Show("Your cache is not empty. Only rebuild the cache if you uploaded or deleted images outside of this application. This process invokes a lot of API requests which can get you blocked if you do it too often.\r\nAre you sure you want to rescan it?", "Reset Cache", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                using (var fCache = new frmCacheBuilder(S))
                {
                    fCache.ShowDialog();
                    InitPages();
                }
            }
        }

        /// <summary>
        /// Authorizes the application using OAuth2
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void AuthorizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (S.Token.Expires < DateTime.UtcNow || MessageBox.Show($"This app is already authorized until {S.Token.Expires.ToShortDateString()} and connected to your account. Reauthorization will erase the cache.\r\nContinue?", "Authorization", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                using (var fAuth = new frmAuth(S))
                {
                    if (fAuth.ShowDialog() == DialogResult.OK)
                    {
                        Cache.ClearThumbnails();
                        Cache.ClearImages();
                        using (var fCache = new frmCacheBuilder(S))
                        {
                            fCache.ShowDialog();
                        }
                        I = new Imgur(S);
                        InitPages();
                    }
                    else
                    {
                        MessageBox.Show("Unable to authorize your client. Your current authorization will be kept. Please try again", "Authorization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Shows all images
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void AllImagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowImages(ImageFilter.All, CurrentPage = 1, (bool)tbFilter.Tag ? null : tbFilter.Text);
        }

        /// <summary>
        /// Shows images with a title
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void WithTitleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Show images with title and keep filter intact (if set)
            ShowImages(ImageFilter.WithTitle, CurrentPage = 1, (bool)tbFilter.Tag ? null : tbFilter.Text);
        }

        /// <summary>
        /// Shows images without a title
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void WithoutTitleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowImages(ImageFilter.WithoutTitle, CurrentPage = 1, null);
        }

        /// <summary>
        /// Shows image properties
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void LvImages_DoubleClick(object sender, EventArgs e)
        {
            EditSelectedImageTitle();
        }

        /// <summary>
        /// Hides placeholder text
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void TbFilter_Enter(object sender, EventArgs e)
        {
            if ((bool)tbFilter.Tag)
            {
                tbFilter.Tag = false;
                tbFilter.Font = DefaultFont;
                tbFilter.Text = string.Empty;
            }
        }

        /// <summary>
        /// Shows placeholder text
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void TbFilter_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbFilter.Text))
            {
                tbFilter.Tag = true;
                tbFilter.Text = "Filter";
                tbFilter.Font = new Font(tbFilter.Font, FontStyle.Italic);
            }
        }

        /// <summary>
        /// Handles ENTER key
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void TbFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = e.SuppressKeyPress = true;
                ShowImages((ImageFilter)lvImages.Tag, CurrentPage = 1, tbFilter.Text);
            }
        }

        /// <summary>
        /// Opens the upload form
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void UploadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var fUpload = new frmUpload(S))
            {
                if (fUpload.ShowDialog() == DialogResult.OK)
                {
                    ShowImages((ImageFilter)lvImages.Tag, CurrentPage = 1, (bool)tbFilter.Tag ? null : tbFilter.Text);
                }
            }
        }

        /// <summary>
        /// Saves new size to settings
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void FrmMain_ResizeEnd(object sender, EventArgs e)
        {
            S.UI.MainWindowMaximized = WindowState == FormWindowState.Maximized;
            S.UI.MainWindowSize = Size;
            Tools.SaveSettings(S, Program.SettingsFile);
        }

        /// <summary>
        /// Copies the URL of selected images
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void CopyURLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopySelectedURL();
        }

        /// <summary>
        /// Saves the selected images to disk
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void SaveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveSelectedImages(false);
        }

        /// <summary>
        /// Shows the properties of the first selected image
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void EditTitleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditSelectedImageTitle();
        }

        /// <summary>
        /// Deletes the selected image from cache and optionally imgur
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelectedImages();
        }

        /// <summary>
        /// Downloads selected images to the cache
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void AddToCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveSelectedImages(true);
        }

        /// <summary>
        /// Handles various common key combinations
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void LvImages_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control)
            {
                if (e.KeyCode == Keys.A)
                {
                    e.Handled = e.SuppressKeyPress = true;
                    //It's correct to not use "==" here
                    lvImages.Items.OfType<ListViewItem>().All(m => m.Selected = true);
                }
                if (e.KeyCode == Keys.C)
                {
                    e.Handled = e.SuppressKeyPress = true;
                    CopySelectedURL();
                }
                if (e.KeyCode == Keys.S)
                {
                    SaveSelectedImages(false);
                }
            }
            else if (e.KeyCode == Keys.Enter)
            {
                e.Handled = e.SuppressKeyPress = true;
                EditSelectedImageTitle();
            }
            else if (e.KeyCode == Keys.Delete)
            {
                e.Handled = e.SuppressKeyPress = true;
                DeleteSelectedImages();
            }
        }

        /// <summary>
        /// Shows previous page
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void BtnPrevPage_Click(object sender, EventArgs e)
        {
            if (CurrentPage > 1)
            {
                ShowImages((ImageFilter)lvImages.Tag, --CurrentPage, (bool)tbFilter.Tag ? null : tbFilter.Text);
            }
        }

        /// <summary>
        /// Shows next page
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void BtnNextPage_Click(object sender, EventArgs e)
        {
            if (CurrentPage < Pages)
            {
                ShowImages((ImageFilter)lvImages.Tag, ++CurrentPage, (bool)tbFilter.Tag ? null : tbFilter.Text);
            }
        }

        /// <summary>
        /// Opens the settings
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var f = new frmSettings(S))
            {
                var PS = S.UI.PageSize;
                if (f.ShowDialog() == DialogResult.OK)
                {
                    if (PS != S.UI.PageSize)
                    {
                        InitPages();
                    }
                }
            }
        }

        /// <summary>
        /// Shows a short info message
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void InfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show($@"ImgurTitleEditor - A tool to manage your imgur library
Copyright (c) 2019 by Kevin Gut
Licensed under MIT
Version: {Application.ProductVersion}

Imgur and the Imgur logo are a trademark of Imgur Inc.
Imgur Inc. is in no way affiliated with the creator of ImgurTitleEditor.",
"About ImgurTitleEditor", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Opens the Github repository
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void VisitWebsiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/AyrA/ImgurTitleEditor");
        }

        /// <summary>
        /// Opens the Album form
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void AlbumsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (S.Token == null || S.Token.Expires.ToUniversalTime() < DateTime.UtcNow)
            {
                MessageBox.Show(
                    "An access token is required for this feature. Use the 'Authorization' menu item to get one.",
                    "Token required", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                using (var f = new frmAlbums(S))
                {
                    f.ShowDialog();
                }
            }
        }

        /// <summary>
        /// Uploads multiple files
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void BulkUploadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var Uploader = new FrmBulkUpload(S))
            {
                if (Uploader.ShowDialog() == DialogResult.OK)
                {
                    ShowImages((ImageFilter)lvImages.Tag, CurrentPage = 1, (bool)tbFilter.Tag ? null : tbFilter.Text);
                }
            }
        }

        /// <summary>
        /// Start of Drag and Drop handler.
        /// Accepts any file drops
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void FrmMain_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetFormats().Contains("FileDrop"))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        /// <summary>
        /// End of Drag and Drop handler.
        /// Processes file drops
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void FrmMain_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetFormats().Contains("FileDrop"))
            {
                //This is always a string array, even for single file drops
                var Data = (string[])e.Data.GetData("FileDrop");
                //BeginInvoke seems stupid because the event is already fired in the correct thread,
                //but this prevents the Drop event from getting "stuck"
                BeginInvoke((MethodInvoker)delegate
                {
                    var Render = false;
                    foreach (var FileName in Data)
                    {
                        if (File.Exists(FileName))
                        {
                            try
                            {
                                Image.FromFile(FileName).Dispose();
                            }
                            catch
                            {
                                MessageBox.Show($"{Path.GetFileName(FileName)} is not a valid image", "Invalid Image", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                continue;
                            }
                            using (var f = new frmUpload(S, FileName))
                            {
                                if (f.ShowDialog() != DialogResult.OK)
                                {
                                    break;
                                }
                                else
                                {
                                    Render = true;
                                }
                            }
                        }
                    }
                    if (Render)
                    {
                        ShowImages((ImageFilter)lvImages.Tag, CurrentPage = 1, (bool)tbFilter.Tag ? null : tbFilter.Text);
                    }
                });
            }
            else
            {
                BeginInvoke((MethodInvoker)delegate
                {
                    MessageBox.Show("Only files are allowed to be dropped", "Invalid Drag&Drop Operation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                });
            }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Initializes the page counters and goes to the first page
        /// </summary>
        private void InitPages()
        {
            CurrentPage = 1;
            Pages = S.UI.PageSize <= 0 ? 0 : (int)Math.Ceiling(FilterImages((ImageFilter)S.UI.LastView, null).Count() * 1.0 / S.UI.PageSize);
            ShowImages((ImageFilter)S.UI.LastView, CurrentPage, (bool)tbFilter.Tag ? null : tbFilter.Text);
        }

        /// <summary>
        /// Gets a filteres image list
        /// </summary>
        /// <param name="IF">Image filter</param>
        /// <param name="Search">Title/Desc/Name filter</param>
        /// <returns></returns>
        private IEnumerable<ImgurImage> FilterImages(ImageFilter IF, string Search)
        {
            if (IF == ImageFilter.WithoutTitle)
            {
                return Cache.Images.Where(m => string.IsNullOrEmpty(m.title));
            }
            if (string.IsNullOrEmpty(Search))
            {
                return IF == ImageFilter.All ? Cache.Images : Cache.Images.Where(m => !string.IsNullOrEmpty(m.title));
            }
            else
            {
                return Cache.Images.Where(m => IsMatch(m, Search));
            }
        }

        /// <summary>
        /// Selects and returns the previous image
        /// </summary>
        /// <returns>Previous image</returns>
        /// <remarks>Switches pages if needed</remarks>
        public ImgurImage PrevImage()
        {
            if (lvImages.SelectedItems.Count > 0)
            {
                var I = lvImages.SelectedItems[0];
                //Update title in case it was changed in the form
                I.Text = ((ImgurImage)I.Tag).title;
                if (I.Index > 0)
                {
                    var PrevItem = lvImages.Items[I.Index - 1];
                    lvImages.SelectedItems.Clear();
                    PrevItem.Selected = true;
                    PrevItem.EnsureVisible();
                    return (ImgurImage)PrevItem.Tag;
                }
                else if (CurrentPage > 1)
                {
                    ShowImages((ImageFilter)lvImages.Tag, --CurrentPage, (bool)tbFilter.Tag ? null : tbFilter.Text);
                    I = lvImages.Items.OfType<ListViewItem>().Last();
                    I.Selected = true;
                    I.EnsureVisible();
                    return (ImgurImage)I.Tag;
                }
            }
            return null;
        }

        /// <summary>
        /// Selects and returns the next image
        /// </summary>
        /// <returns>Next image</returns>
        /// <remarks>Switches pages if needed</remarks>
        public ImgurImage NextImage()
        {
            if (lvImages.SelectedItems.Count > 0)
            {
                var I = lvImages.SelectedItems[0];
                I.Text = ((ImgurImage)I.Tag).title;
                if (I.Index < lvImages.Items.Count - 1)
                {
                    var NextItem = lvImages.Items[I.Index + 1];
                    lvImages.SelectedItems.Clear();
                    NextItem.Selected = true;
                    NextItem.EnsureVisible();
                    return (ImgurImage)NextItem.Tag;
                }
                else if (CurrentPage < Pages)
                {
                    ShowImages((ImageFilter)lvImages.Tag, ++CurrentPage, (bool)tbFilter.Tag ? null : tbFilter.Text);
                    I = lvImages.Items[0];
                    I.Selected = true;
                    I.EnsureVisible();
                    return (ImgurImage)I.Tag;
                }
            }
            return null;
        }

        /// <summary>
        /// Shows images according to the given criteria
        /// </summary>
        /// <param name="Filter">Image filter</param>
        /// <param name="Page">Page</param>
        /// <param name="Search">Title/Desc/Name filter</param>
        private void ShowImages(ImageFilter Filter, int Page, string Search)
        {
            var RealPage = Math.Min(Pages, Math.Max(1, Page));
            lvImages.Tag = Filter;
            foreach (var i in listToolStripMenuItem.DropDownItems.OfType<ToolStripMenuItem>())
            {
                i.Checked = false;
            }
            switch (Filter)
            {
                case ImageFilter.All:
                    allImagesToolStripMenuItem.Checked = true;
                    break;
                case ImageFilter.WithoutTitle:
                    withoutTitleToolStripMenuItem.Checked = true;
                    break;
                case ImageFilter.WithTitle:
                    withTitleToolStripMenuItem.Checked = true;
                    break;
            }
            S.UI.LastView = (int)Filter;
            Tools.SaveSettings(S, Program.SettingsFile);

            var Index = 0;
            var Items = new List<ListViewItem>();
            var IL = new ImageList()
            {
                ImageSize = new Size(160, 160),
                ColorDepth = ColorDepth.Depth32Bit
            };
            var Iterator = FilterImages(Filter, Search);
            Pages = (int)Math.Ceiling(Iterator.Count() * 1.0 / S.UI.PageSize);
            lblPage.Text = Pages < 1 ? "" : $"Current Page: {Page}/{Pages}";
            if (S.UI.PageSize > 0)
            {
                Iterator = Iterator.Skip(S.UI.PageSize * (RealPage - 1)).Take(S.UI.PageSize);
            }
            foreach (var I in Iterator)
            {
                using (var MS = new MemoryStream(Cache.GetThumbnail(I)))
                {
                    IL.Images.Add(Image.FromStream(MS));
                }
                var Item = new ListViewItem(I.title == null ? string.Empty : I.title)
                {
                    ImageIndex = Index,
                    Tag = I,
                    ToolTipText = $"[{I.name}] {I.description}"
                };
                Items.Add(Item);
                ++Index;
            }
            if (lvImages.LargeImageList != null)
            {
                lvImages.LargeImageList.Dispose();
            }
            lvImages.SuspendLayout();
            lvImages.LargeImageList = IL;
            lvImages.Items.Clear();
            lvImages.Items.AddRange(Items.ToArray());
            lvImages.ResumeLayout();
        }

        /// <summary>
        /// Checks if the given search matches the given image
        /// </summary>
        /// <param name="I">Image</param>
        /// <param name="Search">Search string</param>
        /// <returns>"True", if match</returns>
        /// <remarks>Case insensitive, will always match if search is null or empty</remarks>
        private static bool IsMatch(ImgurImage I, string Search)
        {
            if (string.IsNullOrEmpty(Search))
            {
                return true;
            }
            var Strings = new string[] {
                I.title,
                I.description,
                I.name
            }.Where(m => !string.IsNullOrEmpty(m));
            foreach (var s in Strings)
            {
                if (s.ToLower().Contains(Search.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Deletes the selected images
        /// </summary>
        private async void DeleteSelectedImages()
        {
            var DelImgur = false;
            var Images = lvImages.SelectedItems.OfType<ListViewItem>().Select(m => (ImgurImage)m.Tag).ToArray();
            switch (MessageBox.Show($"You are about to delete {Images.Length} images from the cache.\r\nRemove them from your Imgur Account too?", "Delete Images", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button3))
            {
                case DialogResult.Yes:
                    DelImgur = true;
                    break;
                case DialogResult.No:
                    DelImgur = false;
                    break;
                default:
                    MessageBox.Show("No images deleted. Operation cancelled by user.", "Delete Images", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
            }
            foreach (var Img in Images)
            {
                Cache.RemoveImage(Img);
                if (DelImgur)
                {
                    await I.DeleteImage(Img);
                }
            }
            lvImages.SuspendLayout();
            foreach (var Item in lvImages.SelectedItems.OfType<ListViewItem>().ToArray())
            {
                lvImages.Items.Remove(Item);
            }
            lvImages.ResumeLayout();
        }

        /// <summary>
        /// Downloads the selected image(s)
        /// </summary>
        /// <param name="CacheOnly">Don't prompt, download to cache only</param>
        /// <remarks>Will show folder browse dialog if multiple images are selected and file save dialog if only one</remarks>
        private void SaveSelectedImages(bool CacheOnly)
        {
            switch (lvImages.SelectedItems.Count)
            {
                case 0:
                    break;
                case 1:
                    if (CacheOnly)
                    {
                        Cache.GetImage((ImgurImage)lvImages.SelectedItems[0].Tag);
                    }
                    else
                    {
                        using (var SFD = new SaveFileDialog())
                        {
                            var Img = (ImgurImage)lvImages.SelectedItems[0].Tag;
                            SFD.DefaultExt = Img.link.Split('.').Last();
                            if (Img.name != null)
                            {
                                SFD.FileName = $"{Img.name}.{SFD.DefaultExt}";
                            }
                            else
                            {
                                SFD.FileName = Img.link.Split('/').Last();
                            }
                            SFD.Filter = $"{Img.type}|*.{SFD.DefaultExt}";
                            SFD.Title = $"Downloading {Img.link}";
                            if (SFD.ShowDialog(this) == DialogResult.OK)
                            {
                                File.WriteAllBytes(SFD.FileName, Cache.GetImage(Img));
                            }
                        }
                    }
                    break;
                default:
                    if (CacheOnly)
                    {
                        var Images = lvImages.SelectedItems
                                    .OfType<ListViewItem>()
                                    .Select(m => (ImgurImage)m.Tag);
                        using (var fDownload = new frmDownload(S, Images, null))
                        {
                            fDownload.ShowDialog();
                        }
                    }
                    else
                    {
                        using (var FBD = new FolderBrowserDialog())
                        {
                            FBD.Description = $"Saving {lvImages.SelectedItems.Count} Images";
                            FBD.ShowNewFolderButton = true;
                            if (FBD.ShowDialog(this) == DialogResult.OK)
                            {
                                var Images = lvImages.SelectedItems
                                    .OfType<ListViewItem>()
                                    .Select(m => (ImgurImage)m.Tag);
                                using (var fDownload = new frmDownload(S, Images, FBD.SelectedPath))
                                {
                                    fDownload.ShowDialog();
                                }
                            }
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Opens the properties of the selected image
        /// </summary>
        private void EditSelectedImageTitle()
        {
            if (lvImages.SelectedItems.Count > 0)
            {
                var I = (ImgurImage)lvImages.SelectedItems[0].Tag;
                using (var prop = new frmProperties(S, I))
                {
                    prop.ShowDialog();
                }
            }
        }

        /// <summary>
        /// Copies the URL(s) of the selected image(s) to clipboard
        /// </summary>
        private void CopySelectedURL()
        {
            var Links = string.Join("\r\n", lvImages.SelectedItems
                .OfType<ListViewItem>()
                .Select(m => ((ImgurImage)m.Tag).GetImageUrl()));
            Clipboard.SetText(Links);
        }

        #endregion
    }
}
