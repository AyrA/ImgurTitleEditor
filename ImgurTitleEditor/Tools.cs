using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ImgurTitleEditor
{
    public static class Tools
    {
        public static T FromJson<T>(this string Source, T Default = default(T))
        {
#if DEBUG
            Debug.WriteLine($"Deserialize JSON. Source={Source}");
#endif

            try
            {
                return JsonConvert.DeserializeObject<T>(Source);
            }
            catch
            {
                return Default;
            }
        }

        public static string ToJson(this object o)
        {
            return o == null ? "null" : JsonConvert.SerializeObject(o);
        }

        public static string ToXml(this object o)
        {
            if (o == null)
            {
                return null;
            }
            XmlSerializer S = new XmlSerializer(o.GetType());
            using (var MS = new MemoryStream())
            {
                S.Serialize(MS, o);
                return Encoding.UTF8.GetString(MS.ToArray());
            }
        }

        public static T FromXml<T>(this string s)
        {
            if(string.IsNullOrWhiteSpace(s))
            {
                return default(T);
            }
            XmlSerializer S = new XmlSerializer(typeof(T));
            using (var TW = new StringReader(s))
            {
                return (T)S.Deserialize(TW);
            }
        }

        public static string SaveSettings(Settings S, string FileName)
        {
            var Ret = S.Save();
            File.WriteAllText(FileName, Ret);
            return Ret;
        }

        public static Settings LoadSettings(string FileName)
        {
            return Settings.Load(File.ReadAllText(FileName));
        }

        public static long LongOrDefault(string Source, long Default = long.MinValue)
        {
            long L;
            return long.TryParse(Source, out L) ? L : Default;
        }

        public static long IntOrDefault(string Source, int Default = int.MinValue)
        {
            int I;
            return int.TryParse(Source, out I) ? I : Default;
        }

        public static Dictionary<string, string> ParseFragment(Uri U)
        {
            var F = U.Fragment;
            var ret = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(F) && F[0] == '#')
            {
                foreach (var S in F.Substring(1).Split('&'))
                {
                    var K = S.Substring(0, S.Contains('=') ? S.IndexOf('=') : S.Length);
                    var V = K.Length == S.Length ? null : S.Substring(K.Length + 1);
                    if (ret.ContainsKey(K))
                    {
                        ret[K] += $", {V}";
                    }
                    else
                    {
                        ret.Add(K, V);
                    }
                }
            }
            return ret;
        }
    }
}
