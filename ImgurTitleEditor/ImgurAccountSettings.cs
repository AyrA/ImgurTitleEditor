using System;

namespace ImgurTitleEditor
{
    /// <summary>
    /// Contains Imgur account settings from the API
    /// </summary>
    [Serializable]
    public class ImgurAccountSettings
    {
        /// <summary>
        /// Absolute URL to the account
        /// </summary>
        public string account_url;
        /// <summary>
        /// Registered (primary) E-mail address
        /// </summary>
        public string email;
        /// <summary>
        /// Avatar image
        /// </summary>
        public string avatar;
        /// <summary>
        /// Cover image
        /// </summary>
        public string cover;
        /// <summary>
        /// Gets if uploaded images are public by default
        /// </summary>
        public bool public_images;
        /// <summary>
        /// Gets default album privacy (public, hidden, private)
        /// </summary>
        public string album_privacy;
        /// <summary>
        /// Gets if this account accepted the Imgur terms
        /// </summary>
        public bool accepted_gallery_terms;
        /// <summary>
        /// Gets all active E-mail addresses for this account
        /// </summary>
        public string[] active_emails;
        /// <summary>
        /// Gets if messaging is enabled
        /// </summary>
        public bool messaging_enabled;
        /// <summary>
        /// Gets if comment reply notifications are enabled
        /// </summary>
        public bool comment_replies;
        /// <summary>
        /// Gets a list of blocked users
        /// </summary>
        public string[] blocked_users;
        /// <summary>
        /// Gets if mature content is to be shown
        /// </summary>
        public bool show_mature;
        /// <summary>
        /// Gets if the account is subscribed to the newsletter
        /// </summary>
        public bool newsletter_subscribed;
        /// <summary>
        /// Gets if this is a first party registered account (not created by social media login)
        /// </summary>
        public bool first_party;
        //public bool pro_expiration; //No pro
        //public int pro_expiration; //Expiration in seconds

    }
}
