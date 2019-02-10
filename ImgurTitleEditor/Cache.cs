using System.IO;
using System.Linq;

namespace ImgurTitleEditor
{
    public static class Cache
    {
        private static string _cacheDir;
        private static string _thumbDir;
        private static string _imageDir;
        private static string _imageList;

        public static ImgurImage[] Images
        {
            get
            {
                if (File.Exists(_imageList))
                {
                    return File.ReadAllText(_imageList).FromXml<ImgurImage[]>();
                }
                return null;
            }
            set
            {
                if (value != null)
                {
                    File.WriteAllText(_imageList, value.ToXml());
                }
                else
                {
                    File.Delete(_imageList);
                }
            }
        }

        public static byte[] GetImage(ImgurImage I)
        {
            var ImageFile = Path.Combine(_imageDir, I.GetImageUrl(ImgurImageSize.Original).Segments.Last());

            if (!File.Exists(ImageFile))
            {
                File.WriteAllBytes(ImageFile, Imgur.GetImage(I, ImgurImageSize.Original));
            }
            return File.ReadAllBytes(ImageFile);
        }

        public static bool ClearImages()
        {
            var ret = true;
            foreach (var img in Directory.GetFiles(_imageDir))
            {
                try
                {
                    File.Delete(img);
                }
                catch
                {
                    ret = false;
                }
            }
            return ret;
        }

        public static byte[] GetThumbnail(ImgurImage I)
        {
            var ThumbFile = Path.Combine(_thumbDir, I.GetImageUrl(ImgurImageSize.Original).Segments.Last());

            if (!File.Exists(ThumbFile))
            {
                File.WriteAllBytes(ThumbFile, Imgur.GetImage(I, ImgurImageSize.BigSquare));
            }
            return File.ReadAllBytes(ThumbFile);
        }

        public static bool ClearThumbnails()
        {
            var ret = true;
            foreach (var thumb in Directory.GetFiles(_thumbDir))
            {
                try
                {
                    File.Delete(thumb);
                }
                catch
                {
                    ret = false;
                }
            }
            return ret;
        }

        static Cache()
        {
            _cacheDir = Path.Combine(Path.GetDirectoryName(Program.SettingsFile), "Cache");
            _thumbDir = MD("Thumbnail");
            _imageDir = MD("Image");
            _imageList = Path.Combine(MD("Meta"), "images.xml");
        }

        private static string MD(string DirName)
        {
            var D = Path.Combine(_cacheDir, DirName);
            if (!Directory.Exists(D))
            {
                Directory.CreateDirectory(D);
            }
            return D;
        }
    }
}
