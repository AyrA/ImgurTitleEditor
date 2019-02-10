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
        private enum ImageFilter
        {
            All,
            WithTitle,
            WithoutTitle
        }

        private Imgur I;
        private Settings S;

        public frmMain(Settings S)
        {
            this.S = S;
            InitializeComponent();
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
            ShowImages(ImageFilter.All);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void buildCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Cache.Images == null || Cache.Images.Length == 0 || MessageBox.Show("Your cache is not empty. Only rebuild the cache if you uploaded or deleted images outside of this application. This process invokes a lot of API requests which can get you blocked if you do it too often.\r\nAre you sure you want to rescan it?", "Reset Cache", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
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
            if (S.Token.Expires < DateTime.UtcNow || MessageBox.Show("This app is already authorized and connected to your account. Reauthorization will erase the cache.\r\nContinue?", "Authorization", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                using (var fAuth = new frmAuth(S))
                {
                    if (fAuth.ShowDialog() == DialogResult.OK)
                    {
                        Cache.Images = null;
                        Cache.ClearThumbnails();
                        ShowImages(ImageFilter.All);
                    }
                }
            }
        }

        private void ShowImages(ImageFilter Filter)
        {
            //const int BATCHSIZE = 100;
            lvImages.Items.Clear();
            var T = new Thread(delegate ()
            {
                var Items = new List<ListViewItem>();
                var IL = new ImageList()
                {
                    ImageSize = new Size(128, 128),
                    ColorDepth = ColorDepth.Depth16Bit
                };
                foreach (var I in Cache.Images)
                {
                    if (
                        Filter == ImageFilter.All ||
                        (Filter == ImageFilter.WithTitle && !string.IsNullOrWhiteSpace(I.title)) ||
                        (Filter == ImageFilter.WithoutTitle && string.IsNullOrWhiteSpace(I.title)))
                    {
                        using (var MS = new MemoryStream(Cache.GetThumbnail(I)))
                        {
                            IL.Images.Add(Image.FromStream(MS));
                        }
                        var Item = new ListViewItem(I.title == null ? string.Empty : I.title);
                        Item.ImageIndex = IL.Images.Count - 1;
                        Item.Tag = I;
                        Item.ToolTipText = $"[{I.name}] {I.description}";
                        Items.Add(Item);
                    }
                }
                Invoke((MethodInvoker)delegate
                {
                    lvImages.Items.Clear();
                    if (lvImages.LargeImageList != null)
                    {
                        lvImages.LargeImageList.Dispose();
                    }
                    lvImages.LargeImageList = IL;
                    lvImages.Items.AddRange(Items.ToArray());
                });

                /*
                for (var i = 0; i < Items.Count; i += BATCHSIZE)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        lvImages.Items.AddRange(Items.Skip(i).Take(BATCHSIZE).ToArray());
                    });
                }
                //*/
            });
            T.IsBackground = true;
            T.Start();
        }

        private void allImagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowImages(ImageFilter.All);
        }

        private void withTitleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowImages(ImageFilter.WithTitle);
        }

        private void withoutTitleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowImages(ImageFilter.WithoutTitle);
        }
    }
}
