using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ImgurTitleEditor
{
    public class Imgur
    {
        public const string IMGUR_REGISTRATION = "https://api.imgur.com/oauth2/addclient";
        public const string SELF = "me";

        private Settings S;

        private HttpWebRequest Req(Uri URL, string BodyContent = null, bool UseAuth = true)
        {
#if DEBUG
            Debug.WriteLine($"Prepare Imgur Request. URL={URL} UseAuth={UseAuth} BodyContent={BodyContent}");
#endif

            var R = WebRequest.CreateHttp(URL);
            if (UseAuth)
            {
                R.Headers.Add($"Authorization: Bearer {S.Token.Access}");
            }
            else
            {
                R.Headers.Add($"Authorization: Client-ID {S.Client.Id}");
            }
            if (BodyContent != null)
            {
                R.Method = "POST";
                R.ContentType = "application/x-www-form-urlencoded";
                using (var SW = new StreamWriter(R.GetRequestStream()))
                {
                    SW.Write(BodyContent);
                }
            }
            return R;
        }

        public Imgur(Settings S)
        {
            this.S = S;
        }

        public async Task<bool> RenewToken()
        {
            if (string.IsNullOrEmpty(S.Token.Refresh) || string.IsNullOrEmpty(S.Client.Secret) || string.IsNullOrEmpty(S.Client.Id))
            {
                return false;
            }
            var fd = BuildFormData(new Dictionary<string, string>() {
                {"refresh_token",S.Token.Refresh },
                {"client_id",S.Client.Id },
                {"client_secret",S.Client.Secret },
                { "grant_type","refresh_token" }
            });

            var R = Req(new Uri("https://api.imgur.com/oauth2/token"), fd);

            using (var Res = await GetResponse(R))
            {
                if (!IsErrorCode(Res.StatusCode))
                {
                    var res = (await ReadAll(Res.GetResponseStream())).FromJson<ImgurResponse<ImgurAuthResponse>>().data;
                    S.Token.Access = res.access_token;
                    S.Token.Refresh = res.refresh_token;
                    S.Token.Expires = DateTime.UtcNow.AddSeconds(res.expires_in);
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> SetImageDescription(ImgurImage I, string Title, string Description)
        {
            var fd = BuildFormData(new Dictionary<string, string>() {
                {"title", string.IsNullOrEmpty(Title) ? string.Empty : Title },
                {"description", string.IsNullOrEmpty(Description) ? string.Empty : Description }
            });
            var R = Req(new Uri($"https://api.imgur.com/3/image/{I.id}"), fd);
            using (var Res = await GetResponse(R))
            {
                if (!IsErrorCode(Res.StatusCode))
                {
                    return (await ReadAll(Res.GetResponseStream())).FromJson<ImgurResponse<bool>>().data;
                }
            }
            return false;
        }

        public async Task<bool> DeleteImage(ImgurImage I)
        {
            var R = Req(new Uri($"https://api.imgur.com/3/image/{I.deletehash}"));
            R.Method = "DELETE";
            using (var Res = await GetResponse(R))
            {
                return !IsErrorCode(Res.StatusCode);
            }
        }

        public async Task<ImgurImage> UploadImage(byte[] Data, string Filename, string Title, string Description)
        {
            var fd = BuildFormData(new Dictionary<string, string>() {
                {"image", Convert.ToBase64String(Data) },
                {"type", "base64" },
                {"name", Filename },
                {"title", Title },
                {"description", Description }
            });

            var R = Req(new Uri($"https://api.imgur.com/3/image"), fd);
            using (var Res = await GetResponse(R))
            {
                if (!IsErrorCode(Res.StatusCode))
                {
                    return (await ReadAll(Res.GetResponseStream())).FromJson<ImgurResponse<ImgurImage>>().data;
                }
#if DEBUG
                else
                {
                    var Response = await ReadAll(Res.GetResponseStream());
                    Debug.WriteLine($"File upload error: {Response}");
                }
#endif
            }
            return null;
        }

        public async Task<ImgurAccountSettings> GetAccountSettings()
        {
            var R = Req(new Uri($"https://api.imgur.com/3/account/me/settings"));
            using (var Res = await GetResponse(R))
            {
                if (!IsErrorCode(Res.StatusCode))
                {
                    return (await ReadAll(Res.GetResponseStream())).FromJson<ImgurResponse<ImgurAccountSettings>>().data;
                }
            }
            return null;
        }

        public async Task<int> GetAccountImageCount(string AccountName = SELF)
        {
            var R = Req(new Uri($"https://api.imgur.com/3/account/{AccountName}/images/count"));
            using (var Res = await GetResponse(R))
            {
                if (!IsErrorCode(Res.StatusCode))
                {
                    return (await ReadAll(Res.GetResponseStream())).FromJson<ImgurResponse<int>>().data;
                }
            }
            return -1;
        }

        public IEnumerable<ImgurImage> GetAccountImages(string AccountName = SELF)
        {
            int Page = 0;
            while (Page >= 0)
            {
                var R = Req(new Uri($"https://api.imgur.com/3/account/{AccountName}/images/{Page++}"));
                using (var Res = GetResponse(R).Result)
                {
                    if (!IsErrorCode(Res.StatusCode))
                    {
                        var data = ReadAll(Res.GetResponseStream()).Result.FromJson<ImgurResponse<ImgurImage[]>>().data;
                        if (data.Length == 0)
                        {
                            //EOF
                            Page = -1;
                        }
                        else
                        {
                            foreach (var i in data)
                            {
                                yield return i;
                            }
                        }
                    }
                    else
                    {
                        Page = -1;
                    }
                }
            }
        }

        private static bool IsErrorCode(HttpStatusCode Code)
        {
            return (int)Code >= 400;
        }

        private static bool Is300Code(HttpStatusCode Code)
        {
            return (int)Code >= 300 && (int)Code < 400;
        }

        private static bool IsOkCode(HttpStatusCode Code)
        {
            return (int)Code >= 200 && (int)Code < 300;
        }

        private static async Task<HttpWebResponse> GetResponse(HttpWebRequest R)
        {
            try
            {
                return (HttpWebResponse)(await R.GetResponseAsync());
            }
            catch (WebException ex)
            {
                return (HttpWebResponse)ex.Response;
            }
            catch
            {
                return null;
            }
        }

        private static async Task<string> ReadAll(Stream S, bool LeaveOpen = false)
        {
            using (var SR = new StreamReader(S, Encoding.UTF8, false, 1024, LeaveOpen))
            {
                return await SR.ReadToEndAsync();
            }
        }

        private static string BuildFormData(IDictionary<string, string> Params)
        {
            return string.Join("&", Params.Select(m => $"{UrlEncode(m.Key)}={UrlEncode(m.Value)}"));
        }

        private static string UrlEncode(string S)
        {
            //Uri.EscapeDataString has a limit
            if (S.Length > short.MaxValue)
            {
                return string.Join("", Regex.Matches(S, @".{1," + short.MaxValue.ToString() + "}")
                    .OfType<Match>()
                    .Select(m => Uri.EscapeDataString(m.Value)));
            }
            return Uri.EscapeDataString(S);
        }

        public static byte[] GetImage(ImgurImage I, ImgurImageSize Size, bool AllowVideo)
        {
            using (var WC = new WebClient())
            {
                if (AllowVideo || !I.type.ToLower().StartsWith("video/"))
                {
                    return WC.DownloadData(I.GetImageUrl(Size));
                }
                return WC.DownloadData(I.GetImageUrl(Size == ImgurImageSize.Original ? ImgurImageSize.HugeThumbnail : Size));
            }
        }
    }
}
