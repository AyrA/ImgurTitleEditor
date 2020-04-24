using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImgurTitleEditor
{
    /// <summary>
    /// Represents an Imgur API response for rate limits
    /// </summary>
    public class ImgurLimitResponse
    {
        /// <summary>
        /// Limit for user specific actions
        /// </summary>
        public int UserLimit { get; set; }
        /// <summary>
        /// Remaining user specific actions
        /// </summary>
        public int UserRemaining { get; set; }
        /// <summary>
        /// Time when <see cref="UserRemaining"/> is reset
        /// </summary>
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime UserReset { get; set; }
        /// <summary>
        /// Application limits
        /// </summary>
        public int ClientLimit { get; set; }
        /// <summary>
        /// Remaining application actions
        /// </summary>
        public int ClientRemaining { get; set; }
    }
}
