using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace ImgurTitleEditor
{
    /// <summary>
    /// An imgur Album
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Imgur API")]
    public class ImgurAlbum
    {
        /// <summary>
        /// Album Id (sometimes called "hash")
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// Album title
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// Album description
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// Creation date
        /// </summary>
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime datetime { get; set; }
        /// <summary>
        /// Cover image
        /// </summary>
        public string cover { get; set; }
        /// <summary>
        /// Cover width
        /// </summary>
        [JsonConverter(typeof(NullIntConverter))]
        public int cover_width { get; set; }
        /// <summary>
        /// Cover height
        /// </summary>
        [JsonConverter(typeof(NullIntConverter))]
        public int cover_height { get; set; }
        /// <summary>
        /// URL of the owning account
        /// </summary>
        public string account_url { get; set; }
        /// <summary>
        /// Id of the owning account
        /// </summary>
        public int? account_id { get; set; }
        /// <summary>
        /// Privacy settings
        /// </summary>
        public string privacy { get; set; }
        /// <summary>
        /// Album layout
        /// </summary>
        public string layout { get; set; }
        /// <summary>
        /// Album views
        /// </summary>
        public long views { get; set; }
        /// <summary>
        /// Album link
        /// </summary>
        public string link { get; set; }
        /// <summary>
        /// Is in favorites
        /// </summary>
        public bool favorite { get; set; }
        /// <summary>
        /// Marked as NSFW
        /// </summary>
        [JsonConverter(typeof(BoolConverter))]
        public bool nsfw { get; set; }
        /// <summary>
        /// Section?
        /// </summary>
        public string section { get; set; }
        /// <summary>
        /// Ordering of the images/album?
        /// </summary>
        public int order { get; set; }
        /// <summary>
        /// Delete hash
        /// </summary>
        public string deletehash { get; set; }
        /// <summary>
        /// Image count
        /// </summary>
        public int images_count { get; set; }
        /// <summary>
        /// Album images
        /// </summary>
        public ImgurImage[] images { get; set; }
        /// <summary>
        /// true, if listed in imgur gallery
        /// </summary>
        public bool in_gallery { get; set; }
    }
}
