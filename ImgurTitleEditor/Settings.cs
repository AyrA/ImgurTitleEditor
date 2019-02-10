using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace ImgurTitleEditor
{
    [Serializable]
    public class Settings
    {
        public Client Client;
        public Token Token;
        public UI UI;

        public Settings()
        {
            Client = new Client();
            Token = new Token();
            UI = new UI();
        }

        public string Save()
        {
            var X = new XmlSerializer(typeof(Settings));
            using (var MS = new MemoryStream())
            {
                X.Serialize(MS,this);
                return Encoding.UTF8.GetString(MS.ToArray());
            }
        }

        public static Settings Load(string Content)
        {
            var X = new XmlSerializer(typeof(Settings));
            using (var SR = new StringReader(Content))
            {
                return (Settings)X.Deserialize(SR);
            }
        }
    }

    [Serializable]
    public class UI
    {
        public int LastView;
        public bool MainWindowMaximized;
        public Size MainWindowSize;
        public bool PropertyWindowMaximized;
        public Size PropertyWindowSize;
    }

    [Serializable]
    public class Token
    {
        public string Access;
        public string Refresh;
        public DateTime Expires;

        public Token()
        {
            Expires = DateTime.MinValue;
        }
    }

    [Serializable]
    public class Client
    {
        public string Id;
        public string Secret;
    }
}
