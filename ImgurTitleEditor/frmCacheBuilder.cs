using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace ImgurTitleEditor
{
    public partial class frmCacheBuilder : Form
    {
        private Imgur I;
        private bool Exit = false;

        public frmCacheBuilder(Settings S)
        {
            InitializeComponent();
            I = new Imgur(S);
        }

        private void frmCacheBuilder_FormClosed(object sender, FormClosedEventArgs e)
        {
            Exit = true;
        }

        private void frmCacheBuilder_Shown(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate ()
            {
                var Images = new List<ImgurImage>();
                var ImageCount = I.GetAccountImageCount().Result;
                Invoke((MethodInvoker)delegate {
                    pbMeta.Maximum = pbThumbnail.Maximum = ImageCount;
                });
                foreach (var img in I.GetAccountImages())
                {
                    Images.Add(img);
                    Invoke((MethodInvoker)delegate { ++pbMeta.Value; });
                    if (Exit)
                    {
                        return;
                    }
                }
                Cache.Images = Images.ToArray();
                var ThumbNames = new List<string>(Cache.GetThumbnails());
                var ImageNames = new List<string>(Cache.GetImages());

                foreach (var img in Images)
                {
                    Cache.GetThumbnail(img);
                    ThumbNames.Remove(Cache.GetThumbnailName(img));
                    ImageNames.Remove(Cache.GetImageName(img));
                    Invoke((MethodInvoker)delegate { ++pbThumbnail.Value; });
                    if (Exit)
                    {
                        return;
                    }
                }
                foreach (var tName in ThumbNames)
                {
                    Cache.RemoveThumbnail(tName);
                }
                foreach (var tName in ImageNames)
                {
                    Cache.RemoveImage(tName);
                }
                Invoke((MethodInvoker)delegate {
                    DialogResult = Exit ? DialogResult.Cancel : DialogResult.OK;
                });
            });
            T.IsBackground = true;
            T.Start();
        }
    }
}
