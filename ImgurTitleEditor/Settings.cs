using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace ImgurTitleEditor
{
    /// <summary>
    /// Contains application settings
    /// </summary>
    [Serializable]
    public class Settings
    {
        /// <summary>
        /// API client related settings
        /// </summary>
        public Client Client { get; set; }
        /// <summary>
        /// API tokens
        /// </summary>
        public Token Token { get; set; }
        /// <summary>
        /// User interface configuration/state
        /// </summary>
        public UI UI { get; set; }

        /// <summary>
        /// Initializes empty settings
        /// </summary>
        public Settings()
        {
            Client = new Client();
            Token = new Token();
            UI = new UI();
        }

        /// <summary>
        /// Serializes settings
        /// </summary>
        /// <returns>Serialized settings</returns>
        public string Save()
        {
            var X = new XmlSerializer(typeof(Settings));
            using (var MS = new MemoryStream())
            {
                X.Serialize(MS,this);
                return Encoding.UTF8.GetString(MS.ToArray());
            }
        }

        /// <summary>
        /// Deserializes settings
        /// </summary>
        /// <param name="Content">Serialized settings</param>
        /// <returns>Deserialized settings</returns>
        public static Settings Load(string Content)
        {
            var X = new XmlSerializer(typeof(Settings));
            using (var SR = new StringReader(Content))
            {
                return (Settings)X.Deserialize(SR);
            }
        }
    }

    /// <summary>
    /// User interface related settings
    /// </summary>
    [Serializable]
    public class UI
    {
        /// <summary>
        /// Default page size (number of images per page)
        /// </summary>
        public const int DEFAULT_PAGESIZE = 50;

        /// <summary>
        /// Page size
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// Last type of view
        /// </summary>
        /// <remarks>See <see cref="frmMain.ImageFilter"/></remarks>
        public int LastView { get; set; }
        /// <summary>
        /// <see cref="true"/>, if the window was maximized
        /// </summary>
        public bool MainWindowMaximized { get; set; }
        /// <summary>
        /// Non-maximized window size
        /// </summary>
        public Size MainWindowSize { get; set; }
        /// <summary>
        /// <see cref="true"/>, if the window was maximized
        /// </summary>
        public bool PropertyWindowMaximized { get; set; }
        /// <summary>
        /// Non-maximized window size
        /// </summary>
        public Size PropertyWindowSize { get; set; }

        /// <summary>
        /// Initializes default values
        /// </summary>
        public UI()
        {
            PageSize = DEFAULT_PAGESIZE;
        }
    }

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
