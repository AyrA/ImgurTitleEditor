using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Linq;

namespace ImgurTitleEditor
{
    [Serializable]
    public enum ImgurImageSize : int
    {
        Original = 0,
        SmallSquare = Original + 1,
        BigSquare = SmallSquare + 1,
        SmallThumbnail = BigSquare + 1,
        MediumThumbnail = SmallThumbnail + 1,
        LargeThumbnail = MediumThumbnail + 1,
        HugeThumbnail = LargeThumbnail + 1
    }

    [Serializable]
    public class ImgurImage
    {
        public string id;
        public string title;
        public string description;
        public string type;
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime datetime;
        public bool animated;
        public int width;
        public int height;
        public int size;
        public long views;
        public long bandwidth;
        public bool favorite;
        public string account_url;
        public int account_id;
        public bool is_ad;
        public bool in_most_viral;
        public bool has_sound;
        public string[] tags;
        public int ad_type;
        public string ad_url;
        public bool in_gallery;
        public string deletehash;
        public string name;
        public string link;
        //unknown data types
        //public ? vote;
        //public ? nsfw;
        //public ? section;

        public Uri GetImageUrl(ImgurImageSize Size = ImgurImageSize.Original)
        {
            string s = "";
            var ext = link.Split('.').Last();
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
                    throw new ArgumentException("Size is not a valid enumeration");
            }

            return new Uri($"https://i.imgur.com/{id}{s}.{ext}");
        }
    }
}
