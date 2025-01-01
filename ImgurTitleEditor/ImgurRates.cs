using System;
using System.Linq;
using System.Net.Http;

namespace ImgurTitleEditor
{
    /// <summary>
    /// Represents the rate limit values from imgur
    /// </summary>
    public class ImgurRates
    {
        /// <summary>
        /// Limit for user specific actions
        /// </summary>
        public long UserLimit { get; private set; }
        /// <summary>
        /// Remaining user specific actions
        /// </summary>
        public long UserRemaining { get; private set; }
        /// <summary>
        /// Time when <see cref="UserRemaining"/> is reset
        /// </summary>
        public DateTime UserReset { get; private set; }

        /// <summary>
        /// Application limits
        /// </summary>
        public long ClientLimit { get; private set; }
        /// <summary>
        /// Remaining application actions
        /// </summary>
        public long ClientRemaining { get; private set; }

        /// <summary>
        /// HTTP POST limit
        /// </summary>
        public long PostLimit { get; private set; }
        /// <summary>
        /// Remaining HTTP Post actions
        /// </summary>
        public long PostRemaining { get; private set; }
        /// <summary>
        /// Time when <see cref="PostRemaining"/> is reset
        /// </summary>
        public DateTime PostReset { get; private set; }

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public ImgurRates()
        {
            //The numbers here are the defaults from the API
            UserLimit = 12500;
            UserRemaining = 0;
            UserReset = DateTime.UtcNow.Date;

            ClientLimit = 12500;
            ClientRemaining = 0;

            PostLimit = 1250;
            PostRemaining = 0;
            PostReset = DateTime.UtcNow.Date;
        }

        /// <summary>
        /// Checks if any of the values indicate a limit hit
        /// </summary>
        /// <returns><see cref="true"/>, if we can still make requests</returns>
        public bool CanMakeRequest()
        {
            DateTime Now = DateTime.UtcNow;
            if (PostReset > Now && PostRemaining <= 0)
            {
                return false;
            }

            if (UserReset > Now && UserRemaining <= 0)
            {
                return false;
            }

            return ClientRemaining > 0;
        }

        /// <summary>
        /// Sets the limit values from HTTP headers
        /// </summary>
        /// <param name="result">HTTP response</param>
        public void SetValues(HttpResponseMessage result)
        {
            string hv(string s)
            {
                if (result.Headers.TryGetValues(s, out var values))
                {
                    return values.FirstOrDefault();
                }
                return null;
            }
            if (result != null)
            {
                UserLimit = Tools.LongOrDefault(hv("X-RateLimit-UserLimit"), UserLimit);
                UserRemaining = Tools.LongOrDefault(hv("X-RateLimit-UserRemaining"), UserRemaining);
                UserReset = Tools.ToDateTime(Tools.LongOrDefault(hv("X-RateLimit-UserReset"), (long)Tools.FromDateTime(UserReset)));
                ClientLimit = Tools.LongOrDefault(hv("X-RateLimit-ClientLimit"), ClientLimit);
                ClientRemaining = Tools.LongOrDefault(hv("X-RateLimit-ClientRemaining"), ClientRemaining);

                PostLimit = Tools.LongOrDefault(hv("X-Post-Rate-Limit-Limit"), PostLimit);
                PostRemaining = Tools.LongOrDefault(hv("X-Post-Rate-Limit-Remaining"), PostRemaining);
                PostReset = Tools.ToDateTime(Tools.LongOrDefault(hv("X-Post-Rate-Limit-Reset"), (long)Tools.FromDateTime(PostReset)));
            }
        }

        /// <summary>
        /// Sets the limit values from <see cref="Imgur.CheckCredits"/>
        /// </summary>
        /// <param name="Res">Imgur API response</param>
        /// <remarks>
        /// This is done automatically when <see cref="Imgur.CheckCredits"/> is used.
        /// It's not necessary for the user to call this function.
        /// </remarks>
        public void SetValues(ImgurLimitResponse Res)
        {
            if (Res != null)
            {
                ClientLimit = Res.ClientLimit;
                ClientLimit = Res.ClientRemaining;
                UserLimit = Res.UserLimit;
                UserRemaining = Res.UserRemaining;
                UserReset = Res.UserReset.ToUniversalTime();
            }
        }
    }
}
