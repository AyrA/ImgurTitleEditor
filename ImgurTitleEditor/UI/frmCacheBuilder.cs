using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace ImgurTitleEditor
{
    /// <summary>
    /// Form that builds the thumbnail cache
    /// </summary>
    public partial class frmCacheBuilder : Form
    {
        /// <summary>
        /// Imgur API handler
        /// </summary>
        private Imgur I;
        /// <summary>
        /// Cache builder thread
        /// </summary>
        private Thread T;
        /// <summary>
        /// Cancels thread if "True"
        /// </summary>
        private bool Exit = false;

        /// <summary>
        /// Initializes a new cache builder
        /// </summary>
        /// <param name="S">Current settings</param>
        public frmCacheBuilder(Settings S)
        {
            InitializeComponent();
            I = new Imgur(S);
        }

        /// <summary>
        /// Cancels the cach builder
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void frmCacheBuilder_FormClosed(object sender, FormClosedEventArgs e)
        {
            Exit = true;
            if(T.IsAlive)
            {
                //Give the thread some time to exit
                T.Join(4000);
            }
        }

        /// <summary>
        /// Starts the cache builder
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void frmCacheBuilder_Shown(object sender, EventArgs e)
        {
            T = new Thread(delegate ()
            {
                var Images = new List<ImgurImage>();
                var ImageCount = I.GetAccountImageCount().Result;
                Invoke((MethodInvoker)delegate {
                    pbMeta.Maximum = pbThumbnail.Maximum = ImageCount;
                });
                foreach (var img in I.GetAccountImages())
                {
                    Images.Add(img);
                    if (Exit)
                    {
                        return;
                    }
                    Invoke((MethodInvoker)delegate { ++pbMeta.Value; });
                }
                Cache.Images = Images.ToArray();
                var ThumbNames = new List<string>(Cache.GetThumbnails());
                var ImageNames = new List<string>(Cache.GetImages());

                foreach (var img in Images)
                {
                    Cache.GetThumbnail(img);
                    ThumbNames.Remove(Cache.GetThumbnailName(img));
                    ImageNames.Remove(Cache.GetImageName(img));
                    if (Exit)
                    {
                        return;
                    }
                    Invoke((MethodInvoker)delegate { ++pbThumbnail.Value; });
                }
                foreach (var tName in ThumbNames)
                {
                    Cache.RemoveThumbnail(tName);
                }
                foreach (var tName in ImageNames)
                {
                    Cache.RemoveImage(tName);
                }
                if (!Exit)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        DialogResult = Exit ? DialogResult.Cancel : DialogResult.OK;
                    });
                }
            });
            T.IsBackground = true;
            T.Start();
        }
    }
}
