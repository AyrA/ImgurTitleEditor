using System;

namespace ImgurTitleEditor
{
    /// <summary>
    /// Contains Imgur account settings from the API
    /// </summary>
    [Serializable]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Imgur API")]
    public class ImgurAccountSettings
    {
        /// <summary>
        /// Absolute URL to the account
        /// </summary>
        public string account_url { get; set; }
        /// <summary>
        /// Registered (primary) E-mail address
        /// </summary>
        public string email { get; set; }
        /// <summary>
        /// Avatar image
        /// </summary>
        public string avatar { get; set; }
        /// <summary>
        /// Cover image
        /// </summary>
        public string cover { get; set; }
        /// <summary>
        /// Gets if uploaded images are public by default
        /// </summary>
        public bool public_images { get; set; }
        /// <summary>
        /// Gets default album privacy (public, hidden, private)
        /// </summary>
        public string album_privacy { get; set; }
        /// <summary>
        /// Gets if this account accepted the Imgur terms
        /// </summary>
        public bool accepted_gallery_terms { get; set; }
        /// <summary>
        /// Gets all active E-mail addresses for this account
        /// </summary>
        public string[] active_emails { get; set; }
        /// <summary>
        /// Gets if messaging is enabled
        /// </summary>
        public bool messaging_enabled { get; set; }
        /// <summary>
        /// Gets if comment reply notifications are enabled
        /// </summary>
        public bool comment_replies { get; set; }
        /// <summary>
        /// Gets a list of blocked users
        /// </summary>
        public string[] blocked_users { get; set; }
        /// <summary>
        /// Gets if mature content is to be shown
        /// </summary>
        public bool show_mature { get; set; }
        /// <summary>
        /// Gets if the account is subscribed to the newsletter
        /// </summary>
        public bool newsletter_subscribed { get; set; }
        /// <summary>
        /// Gets if this is a first party registered account (not created by social media login)
        /// </summary>
        public bool first_party { get; set; }
        //public bool pro_expiration { get; set; } //No pro
        //public int pro_expiration { get; set; } //Expiration in seconds

    }
}
