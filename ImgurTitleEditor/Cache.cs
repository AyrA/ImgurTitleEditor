using System.IO;
using System.Linq;

namespace ImgurTitleEditor
{
    /// <summary>
    /// Imgur cache to take pressure off the API and increase speed
    /// </summary>
    public static class Cache
    {
        /// <summary>
        /// Cache directory
        /// </summary>
        private static string _cacheDir;
        /// <summary>
        /// Thumbnail directory
        /// </summary>
        private static string _thumbDir;
        /// <summary>
        /// Image directory
        /// </summary>
        private static string _imageDir;
        /// <summary>
        /// Image metadata list
        /// </summary>
        private static string _imageList;

        /// <summary>
        /// Gets or sets Imgur images
        /// </summary>
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

        /// <summary>
        /// Removes an image (meta+thumb+image) from cache
        /// </summary>
        /// <param name="I">Image to remove</param>
        /// <returns>True, if thumbnail and image could be removed too. False on error</returns>
        public static bool RemoveImage(ImgurImage I)
        {
            var ImageFile = Path.Combine(_imageDir, GetImageName(I));
            var ThumbFile = Path.Combine(_thumbDir, GetThumbnailName(I));
            try
            {
                File.Delete(ImageFile);
            }
            catch
            {
                return false;
            }
            try
            {
                File.Delete(ThumbFile);
            }
            catch
            {
                return false;
            }
            Images = Images.Where(m => m.id != I.id).ToArray();
            return true;
        }

        /// <summary>
        /// Gets the image file associated with the given metadata
        /// </summary>
        /// <param name="I">Image metadata</param>
        /// <returns>Image file, null if not found</returns>
        /// <remarks>Downloads the image to cache if necessary</remarks>
        public static byte[] GetImage(ImgurImage I)
        {
            var ImageFile = Path.Combine(_imageDir, GetImageName(I));

            if (!File.Exists(ImageFile))
            {
                //Add image to cache if not found
                File.WriteAllBytes(ImageFile, Imgur.GetImage(I, ImgurImageSize.Original, false));
                //New images are at the start
                Images = (new ImgurImage[] { I }).Concat(Images).ToArray();
            }
            return File.ReadAllBytes(ImageFile);
        }

        /// <summary>
        /// Remove an image by file name
        /// </summary>
        /// <param name="Name">File name</param>
        /// <returns>True if removed</returns>
        public static bool RemoveImage(string Name)
        {
            return rm(_imageDir, Name);
        }

        /// <summary>
        /// Gets the file name of an image from the cache
        /// </summary>
        /// <param name="I">Image metadata</param>
        /// <returns>File name</returns>
        public static string GetImageName(ImgurImage I)
        {
            return I.GetImageUrl(ImgurImageSize.Original).Segments.Last();
        }

        /// <summary>
        /// Gets all image file names from the cache
        /// </summary>
        /// <returns>Image file names</returns>
        /// <remarks>This scans the image directory and not the metadata list</remarks>
        public static string[] GetImages()
        {
            return Directory.GetFiles(_imageDir).Select(m => Path.GetFileName(m)).ToArray();
        }

        /// <summary>
        /// Delete all images from the cache
        /// </summary>
        /// <returns>true, if all images deleted</returns>
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

        /// <summary>
        /// Gets the thumbnail associated with the given metadata
        /// </summary>
        /// <param name="I">Image metadata</param>
        /// <returns>Image file, null if not found</returns>
        /// <remarks>Downloads the image to cache if necessary</remarks>
        public static byte[] GetThumbnail(ImgurImage I)
        {
            var ThumbFile = Path.Combine(_thumbDir, GetThumbnailName(I));

            if (!File.Exists(ThumbFile))
            {
                File.WriteAllBytes(ThumbFile, Imgur.GetImage(I, ImgurImageSize.BigSquare, false));
            }
            return File.ReadAllBytes(ThumbFile);
        }

        /// <summary>
        /// Gets the thumbnail associated with the given image id
        /// </summary>
        /// <param name="ImageId">Image id</param>
        /// <returns>Image file, null if not found</returns>
        /// <remarks>Downloads the image to cache if necessary</remarks>
        public static byte[] GetThumbnail(string ImageId)
        {
            var Img = Images.FirstOrDefault(m => m.id == ImageId);
            if (Img != null)
            {
                return GetThumbnail(Img);
            }
            return null;
        }

        /// <summary>
        /// Remove a thumbnail by file name
        /// </summary>
        /// <param name="Name">File name</param>
        /// <returns>True if removed</returns>
        public static bool RemoveThumbnail(string Name)
        {
            return rm(_thumbDir, Name);
        }

        /// <summary>
        /// Gets the file name of a thumbnail from the cache
        /// </summary>
        /// <param name="I">Image metadata</param>
        /// <returns>File name</returns>
        public static string GetThumbnailName(ImgurImage I)
        {
            return I.GetImageUrl(ImgurImageSize.Original).Segments.Last();
        }

        /// <summary>
        /// Gets all thumbnail file names from the cache
        /// </summary>
        /// <returns>Thumbnail file names</returns>
        /// <remarks>This scans the thumbnail directory and not the metadata list</remarks>
        public static string[] GetThumbnails()
        {
            return Directory.GetFiles(_thumbDir).Select(m => Path.GetFileName(m)).ToArray();
        }

        /// <summary>
        /// Delete all thumbnails from the cache
        /// </summary>
        /// <returns>true, if all thumbnails deleted</returns>
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

        /// <summary>
        /// Static cache initializer
        /// </summary>
        static Cache()
        {
            _cacheDir = Path.Combine(Path.GetDirectoryName(Program.SettingsFile), "Cache");
            _thumbDir = MD("Thumbnail");
            _imageDir = MD("Image");
            _imageList = Path.Combine(MD("Meta"), "images.xml");
        }

        /// <summary>
        /// Removes a file
        /// </summary>
        /// <param name="p1">Base path</param>
        /// <param name="p2">File path</param>
        /// <returns>True, if deleted</returns>
        /// <remarks>This makes sure <paramref name="p2"/> is inside of <paramref name="p1"/></remarks>
        private static bool rm(string p1, string p2)
        {
            var p = Path.GetFullPath(Path.Combine(p1, p2));
            if (p.StartsWith(Path.GetFullPath(_imageDir)))
            {
                try
                {
                    File.Delete(p);
                    return true;
                }
                catch
                {
                    //NOOP
                }
            }
            return false;
        }

        /// <summary>
        /// Creates a directory inside the cache if it doesn't exists
        /// </summary>
        /// <param name="DirName">Directory path/name</param>
        /// <returns>Absolute directory path</returns>
        /// <remarks>This makes sure that <paramref name="DirName"/> is inside of <see cref="_cacheDir"/></remarks>
        private static string MD(string DirName)
        {
            var D = Path.Combine(_cacheDir, DirName);
            if (D.StartsWith(_cacheDir) && !Directory.Exists(D))
            {
                Directory.CreateDirectory(D);
            }
            return D;
        }
    }
}
