using System;

namespace ImgurTitleEditor.Configuration
{
    /// <summary>
    /// API client settings
    /// </summary>
    [Serializable]
    public class Client
    {
        /// <summary>
        /// Client ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Client secret
        /// </summary>
        /// <remarks>
        /// The Imgur API is fully functional without the secret.
        /// Not having a secret merely prevents you from automatically renewing the token.
        /// Tokens seem to last for 10 years though
        /// </remarks>
        public string Secret { get; set; }
    }
}
