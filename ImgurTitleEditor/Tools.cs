using ImgurTitleEditor.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace ImgurTitleEditor
{
    /// <summary>
    /// Contains various utility functions
    /// </summary>
    public static class Tools
    {
        private static readonly DateTime Epoch = DateTime.Parse("1970-01-01T00:00:00Z").ToUniversalTime();

        /// <summary>
        /// Deserializes a JSON string into an object
        /// </summary>
        /// <typeparam name="T">Target object type</typeparam>
        /// <param name="Source">JSON data</param>
        /// <param name="Default">Default to return on error</param>
        /// <returns>Deserialized object on success, <paramref name="Default"/> on error</returns>
        public static T FromJson<T>(this string Source, T Default = default)
        {
#if DEBUG
            Debug.WriteLine($"Deserialize JSON for {typeof(T).Name}. Source={Source}");
#endif

            try
            {
                return JsonConvert.DeserializeObject<T>(Source);
            }
            catch (Exception ex)
            {
                Debug.Print("JSON deserialization failed. {0}: {1}", ex.GetType().Name, ex.Message);
                Debug.Print(ex.StackTrace);
                return Default;
            }
        }

        /// <summary>
        /// Converts a unix timestamp into a Date object
        /// </summary>
        /// <param name="v">Unix timestamp</param>
        /// <returns>Date object</returns>
        public static DateTime ToDateTime(double v)
        {
            return Epoch.AddSeconds(v);
        }

        /// <summary>
        /// Converts a date object into a unix timestamp
        /// </summary>
        /// <param name="DT">Date object</param>
        /// <returns>Unix timestamp</returns>
        public static double FromDateTime(DateTime DT)
        {
            return DT.ToUniversalTime().Subtract(Epoch).TotalSeconds;
        }

        /// <summary>
        /// Serializes almost any object into a JSON string
        /// </summary>
        /// <param name="o">Object to serialize</param>
        /// <returns>JSON string</returns>
        public static string ToJson(this object o)
        {
            return o == null ? "null" : JsonConvert.SerializeObject(o);
        }

        /// <summary>
        /// Serializes an object into an XML string
        /// </summary>
        /// <param name="o">Object to serialize</param>
        /// <returns>XML string</returns>
        /// <remarks>Object and all properties (recursively) must have the <see cref="SerializableAttribute"/></remarks>
        public static string ToXml(this object o)
        {
            if (o == null)
            {
                return null;
            }
            XmlSerializer S = new XmlSerializer(o.GetType());
            using (MemoryStream MS = new MemoryStream())
            {
                S.Serialize(MS, o);
                return Encoding.UTF8.GetString(MS.ToArray());
            }
        }

        /// <summary>
        /// Deserializes an XML string into an object
        /// </summary>
        /// <typeparam name="T">Target object type</typeparam>
        /// <param name="s">XML string</param>
        /// <returns></returns>
        /// <remarks>Object and all properties (recursively) must have the <see cref="SerializableAttribute"/></remarks>
        public static T FromXml<T>(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return default;
            }
            XmlSerializer S = new XmlSerializer(typeof(T));
            using (StringReader TW = new StringReader(s))
            {
                return (T)S.Deserialize(TW);
            }
        }

        /// <summary>
        /// Saves the given settings to a file
        /// </summary>
        /// <param name="S">Settings</param>
        /// <param name="FileName">File path/name</param>
        /// <returns>Serialized settings data</returns>
        public static string SaveSettings(Settings S, string FileName)
        {
            string Ret = S.Save();
            File.WriteAllText(FileName, Ret);
            return Ret;
        }

        /// <summary>
        /// Loads settings from a file
        /// </summary>
        /// <param name="FileName">File path/name</param>
        /// <returns>Deserialized <see cref="Settings"/> object</returns>
        public static Settings LoadSettings(string FileName)
        {
            return Settings.Load(File.ReadAllText(FileName));
        }

        /// <summary>
        /// Tries to convert a string to a <see cref="long"/>
        /// </summary>
        /// <param name="Source">String to convert</param>
        /// <param name="Default">Value to return on error</param>
        /// <returns>Int64 or default value</returns>
        public static long LongOrDefault(string Source, long Default = long.MinValue)
        {
            return long.TryParse(Source, out long L) ? L : Default;
        }

        /// <summary>
        /// Tries to convert a string to a <see cref="int"/>
        /// </summary>
        /// <param name="Source">String to convert</param>
        /// <param name="Default">Value to return on error</param>
        /// <returns>Int32 or default value</returns>
        public static long IntOrDefault(string Source, int Default = int.MinValue)
        {
            return int.TryParse(Source, out int I) ? I : Default;
        }

        /// <summary>
        /// Parses an URL fragment or query string into a simple dictionary
        /// </summary>
        /// <param name="U">URL</param>
        /// <param name="UseQueryString">Use query string (?...) instead of fragment (#...)</param>
        /// <returns>URL parameter dictionary</returns>
        public static Dictionary<string, string> ParseFragment(Uri U, bool UseQueryString)
        {
            string F = UseQueryString ? U.Query : U.Fragment;
            Dictionary<string, string> ret = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(F) && (F[0] == '#' || F[0] == '?'))
            {
                foreach (string S in F.Substring(1).Split('&'))
                {
                    string K = S.Substring(0, S.Contains('=') ? S.IndexOf('=') : S.Length);
                    string V = K.Length == S.Length ? null : S.Substring(K.Length + 1);
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

        /// <summary>
        /// Gets an embedded resource from the application
        /// </summary>
        /// <param name="ResourceName">Full resource name</param>
        /// <returns>Resource content</returns>
        public static byte[] GetResource(string ResourceName)
        {
            Assembly EA = Assembly.GetExecutingAssembly();
            string FullName = $"ImgurTitleEditor.{ResourceName}";
            if (EA.GetManifestResourceNames().Contains(FullName))
            {
                using (Stream S = EA.GetManifestResourceStream(FullName))
                {
                    using (MemoryStream MS = new MemoryStream())
                    {
                        S.CopyTo(MS);
                        return MS.ToArray();
                    }
                }
            }
#if DEBUG
            Debug.WriteLine($"Attempted to get non-existing resource. Name={FullName}");
#endif
            return null;
        }

        public static byte[] ConvertImage(byte[] Input, ImageFormat Output)
        {
            using (MemoryStream MS = new MemoryStream(Input, false))
            {
                using (MemoryStream OUT = new MemoryStream())
                {
                    using (Image I = Image.FromStream(MS))
                    {
                        I.Save(OUT, Output);
                        return OUT.ToArray();
                    }
                }
            }
        }
    }
}
