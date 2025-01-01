using ImgurTitleEditor.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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

        public string LastError { get; private set; }

        /// <summary>
        /// Current settings
        /// </summary>
        private readonly Settings S;

        /// <summary>
        /// Tracks rate limits for the Imgur API
        /// </summary>
        public static ImgurRates RateLimit { get; private set; }

        /// <summary>
        /// HTTP client to make requests
        /// </summary>
        private static readonly HttpClient client = new HttpClient();

        /// <summary>
        /// Static initializer
        /// </summary>
        static Imgur()
        {
            RateLimit = new ImgurRates();
        }

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
                { "refresh_token", S.Token.Refresh },
                { "client_id",     S.Client.Id },
                { "client_secret", S.Client.Secret },
                { "grant_type",   "refresh_token" }
            });

            using (fd)
            {
                using (var response = await ReqAuto(new Uri("https://api.imgur.com/oauth2/token"), fd))
                {
                    string data = await response.Content.ReadAsStringAsync();
                    if (!ProcessErrorResponse(data) && !IsErrorCode(response.StatusCode))
                    {
                        ImgurAuthResponse res = data.FromJson<ImgurResponse<ImgurAuthResponse>>().data;
                        S.Token.Access = res.access_token;
                        S.Token.Refresh = res.refresh_token;
                        S.Token.Expires = DateTime.UtcNow.AddSeconds(res.expires_in);
                        return true;
                    }
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
                { "title",       string.IsNullOrEmpty(Title)       ? string.Empty : Title },
                { "description", string.IsNullOrEmpty(Description) ? string.Empty : Description }
            });
            using (fd)
            {
                using (var response = await ReqAuto(new Uri($"https://api.imgur.com/3/image/{I.id}"), fd))
                {
                    string data = await response.Content.ReadAsStringAsync();
                    if (!ProcessErrorResponse(data) && !IsErrorCode(response.StatusCode))
                    {
                        return data.FromJson<ImgurResponse<bool>>().data;
                    }
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
            using (var response = await ReqDelete(new Uri($"https://api.imgur.com/3/image/{I.deletehash}")))
            {
                string data = await response.Content.ReadAsStringAsync();
                return !ProcessErrorResponse(data) && !IsErrorCode(response.StatusCode);
            }
        }

        /// <summary>
        /// Uploads an image
        /// </summary>
        /// <param name="data">Image data</param>
        /// <param name="fileName">File name (fake or real, no path)</param>
        /// <param name="title">Image title</param>
        /// <param name="description">Image description</param>
        /// <param name="albumId">
        /// Album to add the image to. Imgur seems to prefer to insert images as the second position
        /// </param>
        /// <returns>Uploaded image</returns>
        public async Task<ImgurImage> UploadImage(byte[] data, string fileName, string title, string description, string albumId = null)
        {
            var fd = BuildFormData(new Dictionary<string, string>() {
                {"type", "file" },
                {"name", fileName },
                {"title", title },
                {"description", description },
                {"album", albumId }
            }, true);
            using (fd)
            {
                fd.Add(new ByteArrayContent(data), "image", fileName);
                using (var response = await ReqAuto(new Uri($"https://api.imgur.com/3/image"), fd))
                {
                    string body = await response.Content.ReadAsStringAsync();
                    if (!ProcessErrorResponse(body) && !IsErrorCode(response.StatusCode))
                    {
                        return body.FromJson<ImgurResponse<ImgurImage>>().data;
                    }
#if DEBUG
                    else
                    {
                        Debug.WriteLine($"File upload error: {body}");
                    }
#endif
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the current account settings
        /// </summary>
        /// <returns>Account settings</returns>
        public async Task<ImgurAccountSettings> GetAccountSettings()
        {
            using (var response = await ReqAuto(new Uri($"https://api.imgur.com/3/account/me/settings")))
            {
                string data = await response.Content.ReadAsStringAsync();
                if (!ProcessErrorResponse(data) && !IsErrorCode(response.StatusCode))
                {
                    return data.FromJson<ImgurResponse<ImgurAccountSettings>>().data;
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
            using (var response = await ReqAuto(new Uri($"https://api.imgur.com/3/account/{AccountName}/images/count")))
            {
                string data = await response.Content.ReadAsStringAsync();
                if (!ProcessErrorResponse(data) && !IsErrorCode(response.StatusCode))
                {
                    return data.FromJson<ImgurResponse<int>>().data;
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
                using (var response = ReqAuto(new Uri($"https://api.imgur.com/3/account/{AccountName}/images/{Page++}")).Result)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    if (!ProcessErrorResponse(data) && !IsErrorCode(response.StatusCode))
                    {
                        ImgurImage[] images = data.FromJson<ImgurResponse<ImgurImage[]>>().data;
                        if (images.Length == 0)
                        {
                            //EOF
                            Page = -1;
                        }
                        else
                        {
                            foreach (ImgurImage i in images)
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
                using (var result = ReqAuto(new Uri($"https://api.imgur.com/3/account/{AccountName}/albums/{Page++}")).Result)
                {
                    string body = result.Content.ReadAsStringAsync().Result;
                    if (!ProcessErrorResponse(body) && !IsErrorCode(result.StatusCode))
                    {
                        ImgurAlbum[] data = body.FromJson<ImgurResponse<ImgurAlbum[]>>().data;
                        if (data.Length == 0)
                        {
                            //EOF
                            Page = -1;
                        }
                        else
                        {
                            foreach (ImgurAlbum i in data)
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
            using (var response = await ReqAuto(new Uri($"https://api.imgur.com/3/album/{AlbumId}/images")))
            {
                string data = await response.Content.ReadAsStringAsync();
                if (!ProcessErrorResponse(data) && !IsErrorCode(response.StatusCode))
                {
                    return data.FromJson<ImgurResponse<ImgurImage[]>>().data;
                }
            }
            return null;
        }

        /// <summary>
        /// Adds images to an existing album
        /// </summary>
        /// <param name="albumId">Album Id</param>
        /// <param name="imageIds">Image Ids</param>
        /// <param name="clear">true, to clear existing images from the album first</param>
        /// <returns><see cref="true"/> on success</returns>
        public async Task<bool> AddImagesToAlbum(string albumId, IEnumerable<string> imageIds, bool clear = false)
        {
            string URL = $"https://api.imgur.com/3/album/{albumId}";
            if (!clear)
            {
                URL += "/add";
            }

            using (var fd = new MultipartFormDataContent())
            {
                foreach (var item in imageIds)
                {
                    fd.Add(new StringContent(item), "ids[]");
                }
                //Type: application/x-www-form-urlencoded
                //URL = "https://demo.ayra.ch/response/";
                using (var response = await ReqAuto(new Uri(URL), fd))
                {
                    string data = await response.Content.ReadAsStringAsync();
                    return !ProcessErrorResponse(data) && !IsErrorCode(response.StatusCode);
                }
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
            using (var response = await ReqDelete(new Uri($"https://api.imgur.com/3/album/{A.deletehash}")))
            {
                string data = await response.Content.ReadAsStringAsync();
                return !ProcessErrorResponse(data) && !IsErrorCode(response.StatusCode);
            }
        }

        /// <summary>
        /// Checks how many credits are available and updates the <see cref="RateLimit"/> values
        /// </summary>
        /// <returns>Task</returns>
        public async Task<ImgurLimitResponse> CheckCredits()
        {
            using (var response = await ReqAuto(new Uri($"https://api.imgur.com/3/credits")))
            {
                string data = await response.Content.ReadAsStringAsync();
                if (!ProcessErrorResponse(data) && !IsErrorCode(response.StatusCode))
                {
                    var limits = data.FromJson<ImgurResponse<ImgurLimitResponse>>();
                    RateLimit.SetValues(limits.data);
                    return limits.data;
                }
                return null;
            }
        }

        #endregion

        #region Private

        /// <summary>
        /// Generates a request with the given properties
        /// </summary>
        /// <param name="url">Request URL</param>
        /// <param name="bodyContent">URL encoded body content (automatically makes this into POST)</param>
        /// <param name="useAuth">Use authenticated mode</param>
        /// <returns>HTTP Request</returns>
        private async Task<HttpResponseMessage> ReqAuto(Uri url, HttpContent bodyContent = null, bool useAuth = true)
        {
#if DEBUG
            Debug.WriteLine($"Prepare Imgur Request. URL={url} UseAuth={useAuth} Type={(bodyContent == null ? "GET" : "POST")}");
#endif
            HttpResponseMessage result;
            if (useAuth)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", S.Token.Access);
            }
            else
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Client-ID", S.Client.Id);
            }
            if (bodyContent != null)
            {
                result = await client.PostAsync(url, bodyContent);
            }
            else
            {
                result = await client.GetAsync(url);
            }
            return SetLimitValues(result);
        }

        /// <summary>
        /// Generates a request with the given properties
        /// </summary>
        /// <param name="url">Request URL</param>
        /// <param name="useAuth">Use authenticated mode</param>
        /// <returns>HTTP Request</returns>
        private async Task<HttpResponseMessage> ReqDelete(Uri url, bool useAuth = true)
        {
#if DEBUG
            Debug.WriteLine($"Prepare DELETE Imgur Request. URL={url} UseAuth={useAuth}");
#endif
            HttpWebRequest R = WebRequest.CreateHttp(url);
            if (useAuth)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", S.Token.Access);
            }
            else
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Client-ID", S.Client.Id);
            }
            return SetLimitValues(await client.DeleteAsync(url));
        }
        /// <summary>
        /// Updates the rate limit values
        /// </summary>
        /// <param name="result">HTTP Response</param>
        private static HttpResponseMessage SetLimitValues(HttpResponseMessage result)
        {
            RateLimit.SetValues(result);
            return result;
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
        /// Processes an error response
        /// </summary>
        /// <param name="JsonData">Json body data</param>
        /// <returns><see cref="true"/>, if the response was an error and was processed</returns>
        private bool ProcessErrorResponse(string JsonData)
        {
            ImgurResponse<ImgurErrorResponse> Err;
            try
            {
                Err = JsonData.FromJson<ImgurResponse<ImgurErrorResponse>>();
                if (Err == null || Err.data == null || Err.data.error == null)
                {
                    //Not an error
                    return false;
                }
            }
            catch
            {
                return false;
            }
            if (Err != null)
            {
                LastError = Err.data.error;
            }
            return Err != null && Err.data.error != null;
        }

        /// <summary>
        /// Builds url encoded form data
        /// </summary>
        /// <param name="formParams">URL parameter dictionary</param>
        /// <param name="stripNull">Strip params where the key or value is <see cref="null"/></param>
        /// <returns>URL encoded form data</returns>
        /// <remarks>No further URL encoding necessary</remarks>
        private static MultipartFormDataContent BuildFormData(IDictionary<string, string> formParams, bool stripNull = false)
        {
            var body = new MultipartFormDataContent();
            try
            {
                foreach (var kv in formParams)
                {
                    if (!stripNull || kv.Value != null)
                    {
                        body.Add(new StringContent(kv.Value), kv.Key);
                    }
                }
                return body;
            }
            catch
            {
                body.Dispose();
                throw;
            }
        }

        #endregion

        #region NonAPI

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
            using (WebClient WC = new WebClient())
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
