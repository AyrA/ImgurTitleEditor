using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ImgurTitleEditor.UI
{
    public partial class FrmAddToAlbum : Form
    {
        private readonly Imgur imgur;
        private ImgurAlbum[] albums;

        public FrmAddToAlbum(Imgur i, IEnumerable<ImgurImage> images)
        {
            imgur = i;
            InitializeComponent();
            ImageList IL = new ImageList()
            {
                ImageSize = new Size(160, 160),
                ColorDepth = ColorDepth.Depth32Bit
            };
            var items = new List<ListViewItem>();
            foreach (var img in images)
            {
                using (MemoryStream MS = new MemoryStream(Cache.GetThumbnail(img)))
                {
                    IL.Images.Add(Image.FromStream(MS));
                }
                var entry = new ListViewItem(img.title ?? img.id)
                {
                    ImageIndex = IL.Images.Count - 1,
                    ToolTipText = $"[{img.name}] {img.description}"
                };
                items.Add(entry);
            }
            lvImages.LargeImageList?.Dispose();
            lvImages.SuspendLayout();
            lvImages.LargeImageList = IL;
            lvImages.Items.Clear();
            lvImages.Items.AddRange(items.ToArray());
            lvImages.ResumeLayout();

            LoadAlbums();
        }

        private void LoadAlbums()
        {
            CbAlbum.Items.Clear();
            CbAlbum.Items.Add("Loading...");
            CbAlbum.SelectedIndex = 0;
            CbAlbum.Enabled = false;
            new Thread(() =>
            {
                albums = imgur.GetAccountAlbums().ToArray();
                var titles = albums
                    .Select(m => m.title ?? $"Id={m.id}")
                    .Select(m => (object)m)
                    .ToArray();
                Invoke((MethodInvoker)delegate
                {
                    SuspendLayout();
                    CbAlbum.Items.Clear();
                    CbAlbum.Items.AddRange(titles);
                    CbAlbum.Enabled = true;
                    ResumeLayout();
                });
            })
            { IsBackground = true }.Start();
        }
    }
}
