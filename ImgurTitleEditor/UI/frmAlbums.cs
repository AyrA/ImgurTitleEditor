using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ImgurTitleEditor
{
    public partial class frmAlbums : Form
    {
        private Settings S;

        public frmAlbums(Settings S)
        {
            this.S = S;
            InitializeComponent();
            LoadAlbums();
            WindowState = Application.OpenForms.OfType<frmMain>().First().WindowState;
        }

        private void LoadAlbums()
        {
            Imgur I = new Imgur(S);
            var IL = new ImageList()
            {
                ImageSize = new Size(160, 160),
                ColorDepth = ColorDepth.Depth32Bit
            };
            lvAlbums.Items.Clear();
            lvAlbums.Items.Add("Loading...");
            lvAlbums.Enabled = false;
            Thread T = new Thread(delegate ()
            {
                using (var MS = new MemoryStream(Tools.ConvertImage(Tools.GetResource("imgur.ico"), ImageFormat.Jpeg), false))
                {
                    IL.Images.Add(Image.FromStream(MS));
                }
                var Entries = new List<ListViewItem>();

                foreach (var Album in I.GetAccountAlbums())
                {
                    if (Album.cover == null)
                    {
                        if (Album.images_count > 0)
                        {
                            var AlbumImages = I.GetAlbumImages(Album.id).Result;
                            if (AlbumImages != null)
                            {
                                //Use first image as cover
                                Album.cover = AlbumImages[0].id;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(Album.cover))
                    {
                        using (var MS = new MemoryStream(Cache.GetThumbnail(Album.cover), false))
                        {
                            IL.Images.Add(Image.FromStream(MS));
                        }
                    }
                    var Item = new ListViewItem(Album.title == null ? string.Empty : Album.title);
                    Item.ImageIndex = string.IsNullOrEmpty(Album.cover) ? 0 : IL.Images.Count - 1;
                    Item.Tag = Album;
                    Item.ToolTipText = $"[{Album.title}] {Album.description}";
                    Entries.Add(Item);
                }
                Invoke((MethodInvoker)delegate
                {
                    lvAlbums.SuspendLayout();
                    lvAlbums.Items.Clear();
                    if (lvAlbums.LargeImageList != null)
                    {
                        lvAlbums.LargeImageList.Dispose();
                    }
                    lvAlbums.LargeImageList = IL;
                    lvAlbums.Items.AddRange(Entries.ToArray());
                    lvAlbums.ResumeLayout();
                    lvAlbums.Enabled = true;
                });
            });
            T.IsBackground = true;
            T.Start();
        }

        private ImgurAlbum[] GetSelectedAlbums()
        {
            return lvAlbums.SelectedItems
                .OfType<ListViewItem>()
                .Select(m => (ImgurAlbum)m.Tag)
                .ToArray();
        }

        private bool EditAlbum(ImgurAlbum Album)
        {
            return false;
        }

        private bool CopyUrl(ImgurAlbum[] Albums)
        {
            var Links = string.Join("\r\n", Albums.Select(m => m.link));
            try
            {
                Clipboard.Clear();
                Clipboard.SetText(Links);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private bool DeleteAlbums(ImgurAlbum[] Album)
        {
            return false;
        }

        private void lvAlbums_KeyDown(object sender, KeyEventArgs e)
        {
            var needReload = false;
            var Albums = GetSelectedAlbums();

            switch (e.KeyCode)
            {
                case Keys.Enter:
                    e.SuppressKeyPress = e.Handled = true;
                    needReload = EditAlbum(Albums[0]);
                    break;
                case Keys.A:
                    if (e.Modifiers == Keys.Control)
                    {
                        lvAlbums.Items
                            .OfType<ListViewItem>()
                            .All(m => m.Selected = true);
                    }
                    break;
                case Keys.C:
                    if (e.Modifiers == Keys.Control)
                    {
                        e.SuppressKeyPress = e.Handled = true;
                        needReload = CopyUrl(Albums);
                    }
                    break;
                case Keys.Delete:
                    e.SuppressKeyPress = e.Handled = true;
                    needReload = DeleteAlbums(Albums);
                    break;
            }
            if (needReload)
            {
                LoadAlbums();
            }
        }
    }
}
