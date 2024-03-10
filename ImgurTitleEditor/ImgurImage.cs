using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Linq;

namespace ImgurTitleEditor
{
    /// <summary>
    /// Possible image sizes
    /// </summary>
    /// <remarks>Thumbnails are generally not upscaled. Squares cut off content to keep the aspect ratio</remarks>
    [Serializable]
    public enum ImgurImageSize : int
    {
        /// <summary>
        /// Original image. Needed to get video and animated gif files
        /// </summary>
        Original = 0,
        /// <summary>
        /// Small square (exactly 90x90)
        /// </summary>
        SmallSquare = Original + 1,
        /// <summary>
        /// Big square (exactly 160x160)
        /// </summary>
        BigSquare = SmallSquare + 1,
        /// <summary>
        /// Small thumbnail (no more than 160x160)
        /// </summary>
        SmallThumbnail = BigSquare + 1,
        /// <summary>
        /// Medium thumbnail (no more than 320x320)
        /// </summary>
        MediumThumbnail = SmallThumbnail + 1,
        /// <summary>
        /// Large thumbnail (no more than 640x640)
        /// </summary>
        LargeThumbnail = MediumThumbnail + 1,
        /// <summary>
        /// Huge thumbnail (no more than 1024x1024)
        /// </summary>
        HugeThumbnail = LargeThumbnail + 1
    }

    /// <summary>
    /// Imgur image
    /// </summary>
    [Serializable]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Imgur API")]
    public class ImgurImage
    {
        /// <summary>
        /// Image id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// Image title
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// Image description
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// Mime type
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// Date and time of upload
        /// </summary>
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime datetime { get; set; }
        /// <summary>
        /// True, if animated gif or video
        /// </summary>
        public bool animated { get; set; }
        /// <summary>
        /// Width in pixels
        /// </summary>
        public int width { get; set; }
        /// <summary>
        /// Height in pixels
        /// </summary>
        public int height { get; set; }
        /// <summary>
        /// Size in bytes
        /// </summary>
        public int size { get; set; }
        /// <summary>
        /// Number of views
        /// </summary>
        public long views { get; set; }
        /// <summary>
        /// Amount of bandwidth used (in bytes)
        /// </summary>
        public long bandwidth { get; set; }
        /// <summary>
        /// True if in your favorites
        /// </summary>
        public bool favorite { get; set; }
        /// <summary>
        /// Account URL of owner
        /// </summary>
        public string account_url { get; set; }
        /// <summary>
        /// Account ID of owner
        /// </summary>
        public int? account_id { get; set; }
        /// <summary>
        /// "True" if this image is an ad
        /// </summary>
        public bool is_ad { get; set; }
        /// <summary>
        /// "True" if currently listed in the most viral section
        /// </summary>
        public bool in_most_viral { get; set; }
        /// <summary>
        /// "True" if the video has sound
        /// </summary>
        public bool has_sound { get; set; }
        /// <summary>
        /// Tags associated with the image
        /// </summary>
        public string[] tags { get; set; }
        /// <summary>
        /// Ad type
        /// </summary>
        public int? ad_type { get; set; }
        /// <summary>
        /// Ad URL
        /// </summary>
        public string ad_url { get; set; }
        /// <summary>
        /// True if listed in the gallery (front page)
        /// </summary>
        public bool in_gallery { get; set; }
        /// <summary>
        /// Delete hash for anonymous removal and edit requests
        /// </summary>
        public string deletehash { get; set; }
        /// <summary>
        /// Original image file name
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Image URL
        /// </summary>
        public string link { get; set; }
        //unknown data types
        //public ? vote { get; set; }
        //public ? nsfw { get; set; }
        //public ? section { get; set; }

        /// <summary>
        /// Gets the full image URL according to the given size parameter
        /// </summary>
        /// <param name="Size">Size parameter</param>
        /// <returns>Image url</returns>
        public Uri GetImageUrl(ImgurImageSize Size = ImgurImageSize.Original)
        {
            string s;
            string ext = link.Split('.').Last();
            switch (Size)
            {
                case ImgurImageSize.Original:
                    s = "";
                    break;
                case ImgurImageSize.BigSquare:
                    s = "b";
                    break;
                case ImgurImageSize.HugeThumbnail:
                    s = "h";
                    break;
                case ImgurImageSize.LargeThumbnail:
                    s = "l";
                    break;
                case ImgurImageSize.MediumThumbnail:
                    s = "m";
                    break;
                case ImgurImageSize.SmallSquare:
                    s = "s";
                    break;
                case ImgurImageSize.SmallThumbnail:
                    s = "t";
                    break;
                default:
                    throw new ArgumentException("Size is not a valid enumeration value");
            }

            return new Uri($"https://i.imgur.com/{id}{s}.{ext}");
        }
    }
}
