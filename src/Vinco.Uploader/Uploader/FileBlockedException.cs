using System;


namespace Vinco.Uploader
{
    public class FileBlockedException : Exception
    {
        public FileBlockedException()
        {
        }

        public FileBlockedException(string message) : base(message)
        {
        }
    }
}
