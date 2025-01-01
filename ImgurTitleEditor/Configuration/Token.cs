using System;

namespace ImgurTitleEditor.Configuration
{
    /// <summary>
    /// Token settings
    /// </summary>
    [Serializable]
    public class Token
    {
        /// <summary>
        /// Access token
        /// </summary>
        public string Access { get; set; }
        /// <summary>
        /// Refresh token
        /// </summary>
        public string Refresh { get; set; }
        /// <summary>
        /// Expiration time of access token
        /// </summary>
        public DateTime Expires { get; set; }

        /// <summary>
        /// Initializes empty token settings
        /// </summary>
        public Token()
        {
            Expires = DateTime.MinValue;
        }
    }
}
