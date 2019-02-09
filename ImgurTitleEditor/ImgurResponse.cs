namespace ImgurTitleEditor
{
    public class ImgurResponse<T>
    {
        public const int DEFAULT_STATUS = int.MinValue;
        public int status;
        public bool success;
        public T data;

        public ImgurResponse()
        {
            status = DEFAULT_STATUS;
        }
    }
}
