using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace ImgurTitleEditor.Configuration
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
            XmlSerializer X = new XmlSerializer(typeof(Settings));
            using (MemoryStream MS = new MemoryStream())
            {
                X.Serialize(MS, this);
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
            XmlSerializer X = new XmlSerializer(typeof(Settings));
            using (StringReader SR = new StringReader(Content))
            {
                return (Settings)X.Deserialize(SR);
            }
        }
    }
}
