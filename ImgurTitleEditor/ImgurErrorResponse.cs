using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImgurTitleEditor
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Imgur API")]
    public class ImgurErrorResponse
    {
        public string error { get; set; }
    }
}
