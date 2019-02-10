using System;
using System.Collections.Generic;
using System.Linq;
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
            Exit = true;
            Close();
        }

        private void frmDownload_Shown(object sender, EventArgs e)
        {

        }
    }
}
