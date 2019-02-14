namespace ImgurTitleEditor
{
    /// <summary>
    /// Imgur API response for token renewal
    /// </summary>
    public class ImgurAuthResponse
    {
        /// <summary>
        /// Gets the new access token
        /// </summary>
        public string access_token;
        /// <summary>
        /// Gets the new refresh token
        /// </summary>
        public string refresh_token;
        /// <summary>
        /// Gets the token type
        /// </summary>
        /// <remarks>This should always be "Bearer"</remarks>
        public string token_type;
        /// <summary>
        /// Gets the number of seconds this new token expires in
        /// </summary>
        public int expires_in;
        /// <summary>
        /// Gets the account username
        /// </summary>
        public string account_username;
    }
}
