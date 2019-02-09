using System;

namespace ImgurTitleEditor
{
    [Serializable]
    public class ImgurAccountSettings
    {
        public string account_url;
        public string email;
        public string avatar;
        public string cover;
        public bool public_images;
        public string album_privacy;
        public bool accepted_gallery_terms;
        public string[] active_emails;
        public bool messaging_enabled;
        public bool comment_replies;
        public string[] blocked_users;
        public bool show_mature;
        public bool newsletter_subscribed;
        public bool first_party;
        //public bool pro_expiration; //No pro
        //public int pro_expiration; //Expiration in seconds

    }
}
