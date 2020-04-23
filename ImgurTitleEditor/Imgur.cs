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
    /// <summary>
    /// Handles the Imgur API
    /// </summary>
    public class Imgur
    {
        /// <summary>
        /// OAuth2 application registration URL
        /// </summary>
        public const string IMGUR_REGISTRATION = "https://api.imgur.com/oauth2/addclient";
        /// <summary>
        /// Self reference
        /// </summary>
        public const string SELF = "me";

        /// <summary>
        /// Current settings
        /// </summary>
        private readonly Settings S;

        /// <summary>
        /// Initializes a new Imgur client
        /// </summary>
        /// <param name="S">Settings</param>
        public Imgur(Settings S)
        {
            this.S = S;
        }

        #region Public

        /// <summary>
        /// Tries to renew the current access token
        /// </summary>
        /// <returns><see cref="true"/> on success</returns>
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

        /// <summary>
        /// Sets the title and description of an image
        /// </summary>
        /// <param name="I">Imgur image</param>
        /// <param name="Title">New title</param>
        /// <param name="Description">New description</param>
        /// <returns><see cref="true"/> on success</returns>
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

        /// <summary>
        /// Deletes an image
        /// </summary>
        /// <param name="I">Imgur image</param>
        /// <returns><see cref="true"/> on success</returns>
        public async Task<bool> DeleteImage(ImgurImage I)
        {
            var R = Req(new Uri($"https://api.imgur.com/3/image/{I.deletehash}"));
            R.Method = "DELETE";
            using (var Res = await GetResponse(R))
            {
                return !IsErrorCode(Res.StatusCode);
            }
        }

        /// <summary>
        /// Uploads an image
        /// </summary>
        /// <param name="Data">Image data</param>
        /// <param name="Filename">File name (fake or real, no path)</param>
        /// <param name="Title">Image title</param>
        /// <param name="Description">Image description</param>
        /// <returns>Uploaded image</returns>
        public async Task<ImgurImage> UploadImage(byte[] Data, string Filename, string Title, string Description, string AlbumId = null)
        {
            var fd = BuildFormData(new Dictionary<string, string>() {
                {"image", Convert.ToBase64String(Data) },
                {"type", "base64" },
                {"name", Filename },
                {"title", Title },
                {"description", Description },
                {"album", AlbumId }
            }, true);

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

        /// <summary>
        /// Gets the current account settings
        /// </summary>
        /// <returns>Account settings</returns>
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

        /// <summary>
        /// Gets the number of uploaded images
        /// </summary>
        /// <param name="AccountName">Account name</param>
        /// <returns>Number of images</returns>
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

        /// <summary>
        /// Gets all account images
        /// </summary>
        /// <param name="AccountName">Account name</param>
        /// <returns>List of images</returns>
        /// <remarks>If you use the .ToArray() method of Linq this can take a long time if there are many pages.</remarks>
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

        /// <summary>
        /// Gets all account images
        /// </summary>
        /// <param name="AccountName">Account name</param>
        /// <returns>List of images</returns>
        /// <remarks>If you use the .ToArray() method of Linq this can take a long time if there are many pages.</remarks>
        public IEnumerable<ImgurAlbum> GetAccountAlbums(string AccountName = SELF)
        {
            int Page = 0;
            while (Page >= 0)
            {
                var R = Req(new Uri($"https://api.imgur.com/3/account/{AccountName}/albums/{Page++}"));
                using (var Res = GetResponse(R).Result)
                {
                    if (!IsErrorCode(Res.StatusCode))
                    {
                        var data = ReadAll(Res.GetResponseStream()).Result.FromJson<ImgurResponse<ImgurAlbum[]>>().data;
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

        /// <summary>
        /// Gets all images of an album
        /// </summary>
        /// <param name="AlbumId">Album Id</param>
        /// <returns>List of images</returns>
        public async Task<ImgurImage[]> GetAlbumImages(string AlbumId)
        {
            var R = Req(new Uri($"https://api.imgur.com/3/album/{AlbumId}/images"));
            using (var Res = await GetResponse(R))
            {
                if (!IsErrorCode(Res.StatusCode))
                {
                    var data = await ReadAll(Res.GetResponseStream());
                    return data.FromJson<ImgurResponse<ImgurImage[]>>().data;
                }
            }
            return null;
        }

        /// <summary>
        /// Adds images to an existing album
        /// </summary>
        /// <param name="AlbumId">Album Id</param>
        /// <param name="ImageId">Image Ids</param>
        /// <param name="Clear">true, to clear existing images from the album first</param>
        /// <returns><see cref="true"/> on success</returns>
        public async Task<bool> AddImagesToAlbum(string AlbumId, IEnumerable<string> ImageId, bool Clear = false)
        {
            var URL = $"https://api.imgur.com/3/album/{AlbumId}";
            if (!Clear)
            {
                URL += "/add";
            }
            var Joined = string.Join("&", ImageId.Select(m => $"ids[]={m}").ToArray());
            var R = Req(new Uri(URL), Joined);
            using (var Res = await GetResponse(R))
            {
                return !IsErrorCode(Res.StatusCode);
            }
        }

        /// <summary>
        /// Sets images of an album, replacing any existing ones
        /// </summary>
        /// <param name="AlbumId">Album Id</param>
        /// <param name="ImageId">Image Ids</param>
        /// <returns><see cref="true"/> on success</returns>
        public async Task<bool> SetAlbumImages(string AlbumId, IEnumerable<string> ImageId)
        {
            return await AddImagesToAlbum(AlbumId, ImageId, true);
        }


        /// <summary>
        /// Deletes an album
        /// </summary>
        /// <param name="I">Imgur album</param>
        /// <returns><see cref="true"/> on success</returns>
        /// <remarks>This will not delete images</remarks>
        public async Task<bool> DeleteAlbum(ImgurAlbum A)
        {
            var R = Req(new Uri($"https://api.imgur.com/3/album/{A.deletehash}"));
            R.Method = "DELETE";
            using (var Res = await GetResponse(R))
            {
                return !IsErrorCode(Res.StatusCode);
            }
        }

        #endregion

        #region Private

        /// <summary>
        /// Generates a request with the given properties
        /// </summary>
        /// <param name="URL">Request URL</param>
        /// <param name="BodyContent">URL encoded body content (automatically makes this into POST)</param>
        /// <param name="UseAuth">Use authenticated mode</param>
        /// <returns>HTTP Request</returns>
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

        /// <summary>
        /// Checks if the given code is an error code
        /// </summary>
        /// <param name="Code">HTTP status code</param>
        /// <returns><see cref="true"/>, if error</returns>
        private static bool IsErrorCode(HttpStatusCode Code)
        {
            return (int)Code >= 400;
        }

        /// <summary>
        /// Checks if the given code is a redirection or "no content" code
        /// </summary>
        /// <param name="Code">HTTP status code</param>
        /// <returns><see cref="true"/>, if 300 code (redirection or no content)</returns>
        private static bool Is300Code(HttpStatusCode Code)
        {
            return (int)Code >= 300 && (int)Code < 400;
        }

        /// <summary>
        /// Checks if the given code is a success code
        /// </summary>
        /// <param name="Code">HTTP status code</param>
        /// <returns><see cref="true"/>, if OK code (200-299)</returns>
        private static bool IsOkCode(HttpStatusCode Code)
        {
            return (int)Code >= 200 && (int)Code < 300;
        }

        /// <summary>
        /// Gets the response for a request
        /// </summary>
        /// <param name="R">Request</param>
        /// <returns>Response</returns>
        /// <remarks>This also returns a response on error instrad of raising an exception</remarks>
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

        /// <summary>
        /// Reads all content from a stream as string
        /// </summary>
        /// <param name="S">Stream</param>
        /// <param name="LeaveOpen">Leave open after reading (will still not seek back)</param>
        /// <returns>Stream content</returns>
        private static async Task<string> ReadAll(Stream S, bool LeaveOpen = false)
        {
            using (var SR = new StreamReader(S, Encoding.UTF8, false, 1024, LeaveOpen))
            {
                return await SR.ReadToEndAsync();
            }
        }

        /// <summary>
        /// Builds url encoded form data
        /// </summary>
        /// <param name="Params">URL parameter dictionary</param>
        /// <param name="StripNull">Strip params where the key or value is <see cref="null"/></param>
        /// <returns>URL encoded form data</returns>
        /// <remarks>No further URL encoding necessary</remarks>
        private static string BuildFormData(IDictionary<string, string> Params, bool StripNull = false)
        {
            var Selector = Params
                .Where(m => !StripNull || (m.Key != null & m.Value != null))
                .Select(m => $"{UrlEncode(m.Key)}={UrlEncode(m.Value)}")
                .ToArray();
            return string.Join("&", Selector);
        }

        /// <summary>
        /// URL encodes a string of any length
        /// </summary>
        /// <param name="S">String</param>
        /// <returns>URL encoded string</returns>
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

        #endregion

        #region Statics

        /// <summary>
        /// Downloads an image from Imgur
        /// </summary>
        /// <param name="I">Imgur image</param>
        /// <param name="Size">Image size</param>
        /// <param name="AllowVideo">Allow video files</param>
        /// <returns>Image data</returns>
        /// <remarks>If <paramref name="AllowVideo"/> is <see cref="false"/>, it will use <see cref="ImgurImageSize.HugeThumbnail"/> for video files</remarks>
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

        #endregion
    }
}
