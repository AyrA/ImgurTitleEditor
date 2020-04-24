using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ImgurTitleEditor
{
    /// <summary>
    /// Form that handles large image downloads
    /// </summary>
    public partial class FrmDownload : Form
    {
        /// <summary>
        /// Cancels thread if "True"
        /// </summary>
        private bool Exit = false;
        /// <summary>
        /// Images to download
        /// </summary>
        private readonly List<ImgurImage> Images;
        /// <summary>
        /// Download path
        /// </summary>
        private readonly string Path;

        /// <summary>
        /// Initializes new downloader form
        /// </summary>
        /// <param name="S">Current settings</param>
        /// <param name="Images">Image list</param>
        /// <param name="Path">Save path</param>
        public FrmDownload(IEnumerable<ImgurImage> Images, string Path)
        {
            this.Images = Images.ToList();
            this.Path = Path;
            InitializeComponent();
        }

        /// <summary>
        /// Cancels the download
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            btnCancel.Text = "Cancelling...";
            btnCancel.Enabled = false;
            Exit = true;
        }

        /// <summary>
        /// Starts the download
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void FrmDownload_Shown(object sender, EventArgs e)
        {
            pbStatus.Maximum = Images.Count;
            Thread T = new Thread(delegate ()
            {
                while (Images.Count > 0 && !Exit)
                {
                    ImgurImage I = Images[0];
                    Images.RemoveAt(0);
                    //Just cache if path is not defined
                    if (string.IsNullOrEmpty(Path))
                    {
                        Cache.GetImage(I);
                    }
                    else
                    {
                        string FileName = System.IO.Path.Combine(Path, I.GetImageUrl().Segments.Last());
                        if (FileName.StartsWith(Path))
                        {
                            System.IO.File.WriteAllBytes(FileName, Cache.GetImage(I));
                        }
                    }
#if DEBUG
                    System.Diagnostics.Debug.WriteLine($"URL ({I.GetImageUrl()}) tried to get out of {Path}");
#endif
                    Invoke((MethodInvoker)delegate { ++pbStatus.Value; });
                }
                Invoke((MethodInvoker)Done);
            })
            {
                IsBackground = true
            };
            T.Start();
        }

        /// <summary>
        /// Download complete handler
        /// </summary>
        private void Done()
        {
            Close();
        }
    }
}
