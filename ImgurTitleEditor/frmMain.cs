using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ImgurTitleEditor
{
    public partial class frmMain : Form
    {
        private int CurrentPage;
        private int Pages;

        private enum ImageFilter : int
        {
            All = 0,
            WithTitle = 1,
            WithoutTitle = 2
        }

        private Imgur I;
        private Settings S;

        public frmMain(Settings S)
        {
            this.S = S;
            InitializeComponent();
            tbFilter_Leave(null, null);
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
                        MessageBox.Show("Could not authorize this application", "No Authorization", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void buildCacheToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void authorizeToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void allImagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowImages(ImageFilter.WithTitle, CurrentPage = 1, (bool)tbFilter.Tag ? null : tbFilter.Text);
        }

        private void withTitleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Show images with title and keep filter intact (if set)
            ShowImages(ImageFilter.WithTitle, CurrentPage = 1, (bool)tbFilter.Tag ? null : tbFilter.Text);
        }

        private void withoutTitleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowImages(ImageFilter.WithoutTitle, CurrentPage = 1, null);
        }

        private void lvImages_DoubleClick(object sender, EventArgs e)
        {
            EditSelectedImageTitle();
        }

        private void tbFilter_Enter(object sender, EventArgs e)
        {
            if ((bool)tbFilter.Tag)
            {
                tbFilter.Tag = false;
                tbFilter.Font = DefaultFont;
                tbFilter.Text = string.Empty;
            }
        }

        private void tbFilter_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbFilter.Text))
            {
                tbFilter.Tag = true;
                tbFilter.Text = "Filter";
                tbFilter.Font = new Font(tbFilter.Font, FontStyle.Italic);
            }
        }

        private void tbFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = e.SuppressKeyPress = true;
                ShowImages((ImageFilter)lvImages.Tag, CurrentPage = 1, tbFilter.Text);
            }
        }

        private void uploadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var fUpload = new frmUpload(S))
            {
                if (fUpload.ShowDialog() == DialogResult.OK)
                {
                    ShowImages((ImageFilter)lvImages.Tag, CurrentPage = 1, (bool)tbFilter.Tag ? null : tbFilter.Text);
                }
            }
        }

        private void frmMain_ResizeEnd(object sender, EventArgs e)
        {
            S.UI.MainWindowMaximized = WindowState == FormWindowState.Maximized;
            S.UI.MainWindowSize = Size;
            Tools.SaveSettings(S, Program.SettingsFile);
        }

        private void copyURLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopySelectedURL();
        }

        private void saveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveSelectedImages(false);
        }

        private void editTitleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditSelectedImageTitle();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelectedImages();
        }

        private void addToCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveSelectedImages(true);
        }

        private void lvImages_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A && e.Modifiers == Keys.Control)
            {
                e.Handled = e.SuppressKeyPress = true;
                //It's correct to not use "==" here
                lvImages.Items.OfType<ListViewItem>().All(m => m.Selected = true);
            }
            if (e.KeyCode == Keys.C && e.Modifiers == Keys.Control)
            {
                e.Handled = e.SuppressKeyPress = true;
                CopySelectedURL();
            }
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = e.SuppressKeyPress = true;
                EditSelectedImageTitle();
            }
            if (e.KeyCode == Keys.Delete)
            {
                e.Handled = e.SuppressKeyPress = true;
                DeleteSelectedImages();
            }
        }

        private void btnPrevPage_Click(object sender, EventArgs e)
        {
            if (CurrentPage > 1)
            {
                ShowImages((ImageFilter)lvImages.Tag, --CurrentPage, (bool)tbFilter.Tag ? null : tbFilter.Text);
            }
        }

        private void btnNextPage_Click(object sender, EventArgs e)
        {
            if (CurrentPage < Pages)
            {
                ShowImages((ImageFilter)lvImages.Tag, ++CurrentPage, (bool)tbFilter.Tag ? null : tbFilter.Text);
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
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

        #endregion

        #region Functions

        private void InitPages()
        {
            CurrentPage = 1;
            Pages = S.UI.PageSize <= 0 ? 0 : (int)Math.Ceiling(FilterImages((ImageFilter)S.UI.LastView, null).Count() * 1.0 / S.UI.PageSize);
            ShowImages((ImageFilter)S.UI.LastView, CurrentPage, (bool)tbFilter.Tag ? null : tbFilter.Text);
        }

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

        private void ShowImages(ImageFilter Filter, int Page, string Search)
        {
            var RealPage = Math.Min(Pages, Math.Max(1, Page));
            lblPage.Text = Pages < 1 ? "" : $"Current Page: {Page}/{Pages}";
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
            var Iterator = S.UI.PageSize <= 0 ? FilterImages(Filter, Search) : FilterImages(Filter, Search).Skip(S.UI.PageSize * (RealPage - 1)).Take(S.UI.PageSize);
            foreach (var I in Iterator)
            {
                using (var MS = new MemoryStream(Cache.GetThumbnail(I)))
                {
                    IL.Images.Add(Image.FromStream(MS));
                }
                var Item = new ListViewItem(I.title == null ? string.Empty : I.title);
                Item.ImageIndex = Index;
                Item.Tag = I;
                Item.ToolTipText = $"[{I.name}] {I.description}";
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
                if (Cache.RemoveImage(Img) && DelImgur)
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
