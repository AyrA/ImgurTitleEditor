using System;
using System.Drawing;

namespace ImgurTitleEditor.Configuration
{
    /// <summary>
    /// User interface related settings
    /// </summary>
    [Serializable]
    public class UI
    {
        /// <summary>
        /// Default page size (number of images per page)
        /// </summary>
        public const int DEFAULT_PAGESIZE = 50;

        /// <summary>
        /// Page size
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// Last type of view
        /// </summary>
        /// <remarks>See <see cref="FrmMain.ImageFilter"/></remarks>
        public int LastView { get; set; }
        /// <summary>
        /// <see cref="true"/>, if the window was maximized
        /// </summary>
        public bool MainWindowMaximized { get; set; }
        /// <summary>
        /// Non-maximized window size
        /// </summary>
        public Size MainWindowSize { get; set; }
        /// <summary>
        /// <see cref="true"/>, if the window was maximized
        /// </summary>
        public bool PropertyWindowMaximized { get; set; }
        /// <summary>
        /// Non-maximized window size
        /// </summary>
        public Size PropertyWindowSize { get; set; }

        /// <summary>
        /// Initializes default values
        /// </summary>
        public UI()
        {
            PageSize = DEFAULT_PAGESIZE;
        }
    }
}
