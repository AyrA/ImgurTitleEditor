using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            if (Album != null)
            {

            }
            return false;
        }

        private bool CopyUrl(ImgurAlbum[] Albums)
        {
            if (Albums.Length > 0)
            {
                var Links = string.Join("\r\n", Albums.Select(m => m.link));
                try
                {
                    Clipboard.Clear();
                    Clipboard.SetText(Links);
                }
                catch (Exception)
                {
                    MessageBox.Show("Unable to copy text to the clipboard. Maybe another application is using it right now.", "Clipboard error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            //This never needs changing the album list
            return false;
        }

        private bool DeleteAlbums(ImgurAlbum[] Albums)
        {
            if (Albums.Length > 0)
            {
                Imgur I = new Imgur(S);
                if (MessageBox.Show("Delete the selected albums? Images will not be deleted", $"Delete {Albums.Length} albums", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    Task.WaitAll(Albums.Select(m => I.DeleteAlbum(m)).ToArray());
                    return true;
                }
            }
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

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var Albums = GetSelectedAlbums();
            if (EditAlbum(Albums.FirstOrDefault()))
            {
                LoadAlbums();
            }
        }

        private void copyURLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var Albums = GetSelectedAlbums();
            if (CopyUrl(Albums))
            {
                LoadAlbums();
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var Albums = GetSelectedAlbums();
            if (DeleteAlbums(Albums))
            {
                LoadAlbums();
            }
        }

        private void lvAlbums_DoubleClick(object sender, EventArgs e)
        {
            var Albums = GetSelectedAlbums();
            if (EditAlbum(Albums.FirstOrDefault()))
            {
                LoadAlbums();
            }
        }
    }
}
