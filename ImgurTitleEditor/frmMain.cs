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
            ShowImages((ImageFilter)S.UI.LastView);
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
                    return (ImgurImage)PrevItem.Tag;
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
                    return (ImgurImage)NextItem.Tag;
                }
            }
            return null;
        }

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
                    ShowImages(ImageFilter.All);
                }
            }
        }

        private void authorizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (S.Token.Expires < DateTime.UtcNow || MessageBox.Show("This app is already authorized and connected to your account. Reauthorization will erase the cache.\r\nContinue?", "Authorization", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                using (var fAuth = new frmAuth(S))
                {
                    if (fAuth.ShowDialog() == DialogResult.OK)
                    {
                        Cache.Images = null;
                        Cache.ClearThumbnails();
                        Cache.ClearImages();
                        ShowImages(ImageFilter.All);
                    }
                }
            }
        }

        private void ShowImages(ImageFilter Filter, string Search = null)
        {
            lvImages.SuspendLayout();
            lvImages.Tag = Filter;
            lvImages.Items.Clear();
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

            var T = new Thread(delegate ()
            {
                var Index = 0;
                var Items = new List<ListViewItem>();
                var IL = new ImageList()
                {
                    ImageSize = new Size(160, 160),
                    ColorDepth = ColorDepth.Depth32Bit
                };
                foreach (var I in Cache.Images)
                {
                    if (
                        Filter == ImageFilter.All ||
                        (Filter == ImageFilter.WithTitle && !string.IsNullOrWhiteSpace(I.title)) ||
                        (Filter == ImageFilter.WithoutTitle && string.IsNullOrWhiteSpace(I.title)))
                    {
                        if (string.IsNullOrEmpty(Search) || IsMatch(I, Search))
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
                    }
                }
                Invoke((MethodInvoker)delegate
                {
                    var Times = new List<TimeSpan>();
                    var Start = DateTime.UtcNow;
                    var Current = Start;
                    if (lvImages.LargeImageList != null)
                    {
                        lvImages.LargeImageList.Dispose();
                    }
                    Times.Add(DateTime.UtcNow - Current);
                    Current = DateTime.UtcNow;

                    lvImages.LargeImageList = IL;
                    Times.Add(DateTime.UtcNow - Current);
                    Current = DateTime.UtcNow;

                    lvImages.Items.AddRange(Items.ToArray());
                    Times.Add(DateTime.UtcNow - Current);
                    Current = DateTime.UtcNow;

                    lvImages.ResumeLayout();
                    Times.Add(DateTime.UtcNow - Current);
                    Current = DateTime.UtcNow;
                    /*
#if DEBUG
                    var msg = string.Format(@"Total time: {0}
Disposing IL: {1}
Assigning IL: {2}
Assigning Items: {3}
Resume Layout: {4}",
                        (int)(Current - Start).TotalMilliseconds,
                        (int)Times[0].TotalMilliseconds,
                        (int)Times[1].TotalMilliseconds,
                        (int)Times[2].TotalMilliseconds,
                        (int)Times[3].TotalMilliseconds);
                    MessageBox.Show(msg, "Total Times (ms)", MessageBoxButtons.OK, MessageBoxIcon.Information);
#endif
                    //*/
                });
            });
            T.IsBackground = true;
            T.Start();
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

        private void allImagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowImages(ImageFilter.WithTitle, (bool)tbFilter.Tag ? null : tbFilter.Text);
        }

        private void withTitleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Show images with title and keep filter intact (if set)
            ShowImages(ImageFilter.WithTitle, (bool)tbFilter.Tag ? null : tbFilter.Text);
        }

        private void withoutTitleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowImages(ImageFilter.WithoutTitle);
        }

        private void lvImages_DoubleClick(object sender, EventArgs e)
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
                ShowImages((ImageFilter)lvImages.Tag, tbFilter.Text);
            }
        }

        private void uploadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var fUpload = new frmUpload(S))
            {
                if (fUpload.ShowDialog() == DialogResult.OK)
                {
                    ShowImages((ImageFilter)lvImages.Tag, (bool)tbFilter.Tag ? null : tbFilter.Text);
                }
            }
        }

        private void frmMain_ResizeEnd(object sender, EventArgs e)
        {
            S.UI.MainWindowMaximized = WindowState == FormWindowState.Maximized;
            S.UI.MainWindowSize = Size;
            Tools.SaveSettings(S, Program.SettingsFile);
        }
    }
}
