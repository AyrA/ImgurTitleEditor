using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace ImgurTitleEditor
{
    public class ImgurAlbum
    {
        public string id;
        public string title;
        public string description;
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime datetime;
        public string cover;
        [JsonConverter(typeof(NullIntConverter))]
        public int cover_width;
        [JsonConverter(typeof(NullIntConverter))]
        public int cover_height;
        public string account_url;
        public int account_id;
        public string privacy;
        public string layout;
        public long views;
        public string link;
        public bool favorite;
        [JsonConverter(typeof(BoolConverter))]
        public bool nsfw;
        public string section;
        public int order;
        public string deletehash;
        public int images_count;
        public ImgurImage[] images;
        public bool in_gallery;
    }
}
