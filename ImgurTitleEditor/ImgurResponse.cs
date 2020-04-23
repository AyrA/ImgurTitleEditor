namespace ImgurTitleEditor
{
    /// <summary>
    /// Generic Imgur API response
    /// </summary>
    /// <typeparam name="T">Type of <see cref="data"/> property</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Imgur API")]
    public class ImgurResponse<T>
    {
        /// <summary>
        /// Default status code upon initialization
        /// </summary>
        public const int DEFAULT_STATUS = int.MinValue;
        /// <summary>
        /// Imgur API status code
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// True if answer is considered successful
        /// </summary>
        public bool success { get; set; }
        /// <summary>
        /// API specific data type
        /// </summary>
        public T data { get; set; }

        /// <summary>
        /// Initializes a new Imgur response
        /// </summary>
        public ImgurResponse()
        {
            status = DEFAULT_STATUS;
        }
    }
}
