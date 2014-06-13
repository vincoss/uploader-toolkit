using System;


namespace Vinco.Uploader
{
    public class FileExistsException : Exception
    {
        public FileExistsException()
        {
        }

        public FileExistsException(string message) : base(message)
        {
        }
    }
}
