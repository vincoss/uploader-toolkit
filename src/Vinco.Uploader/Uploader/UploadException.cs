using System;


namespace Vinco.Uploader
{
    public class UploadException : Exception
    {
        public UploadException()
        {
        }

        public UploadException(string message) : base(message)
        {
        }

        public UploadException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
