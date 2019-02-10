using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ImgurTitleEditor
{
    public partial class frmDownload : Form
    {
        private bool Exit = false;
        private List<ImgurImage> Images;
        private Settings S;
        private string Path;

        public frmDownload(Settings S, IEnumerable<ImgurImage> Images, string Path)
        {
            this.S = S;
            this.Images = Images.ToList();
            this.Path = Path;
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            btnCancel.Text = "Cancelling...";
            btnCancel.Enabled = false;
            Exit = true;
            //Close();
        }

        private void frmDownload_Shown(object sender, EventArgs e)
        {
            pbStatus.Maximum = Images.Count;
            Thread T = new Thread(delegate ()
            {
                while (Images.Count > 0 && !Exit)
                {
                    var I = Images[0];
                    Images.RemoveAt(0);
                    //Just cache if path is not defined
                    if (string.IsNullOrEmpty(Path))
                    {
                        Cache.GetImage(I);
                    }
                    else
                    {
                        var FileName = System.IO.Path.Combine(Path, I.GetImageUrl().Segments.Last());
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
            });
            T.IsBackground = true;
            T.Start();
        }

        private void Done()
        {
            Close();
        }
    }
}
