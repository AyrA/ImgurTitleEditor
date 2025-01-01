using ImgurTitleEditor.Configuration;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace ImgurTitleEditor.UI
{
    /// <summary>
    /// Form that builds the thumbnail cache
    /// </summary>
    public partial class frmCacheBuilder : Form
    {
        /// <summary>
        /// Imgur API handler
        /// </summary>
        private readonly Imgur I;
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
        private void FrmCacheBuilder_FormClosed(object sender, FormClosedEventArgs e)
        {
            Exit = true;
            if (T.IsAlive)
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
        private void FrmCacheBuilder_Shown(object sender, EventArgs e)
        {
            T = new Thread(delegate ()
            {
                List<ImgurImage> Images = new List<ImgurImage>();
                int ImageCount = I.GetAccountImageCount().Result;
                Invoke((MethodInvoker)delegate
                {
                    pbMeta.Maximum = pbThumbnail.Maximum = ImageCount;
                });
                foreach (ImgurImage img in I.GetAccountImages())
                {
                    Images.Add(img);
                    if (Exit)
                    {
                        return;
                    }
                    Invoke((MethodInvoker)delegate { ++pbMeta.Value; });
                }
                Cache.Images = Images.ToArray();
                List<string> ThumbNames = new List<string>(Cache.GetThumbnails());
                List<string> ImageNames = new List<string>(Cache.GetImages());

                foreach (ImgurImage img in Images)
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
                foreach (string tName in ThumbNames)
                {
                    Cache.RemoveThumbnail(tName);
                }
                foreach (string tName in ImageNames)
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
            })
            {
                IsBackground = true
            };
            T.Start();
        }
    }
}
