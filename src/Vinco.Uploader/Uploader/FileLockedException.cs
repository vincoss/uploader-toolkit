using System;


namespace Vinco.Uploader
{
    public class FileLockedException : Exception
    {
        public FileLockedException()
        {
        }

        public FileLockedException(string message) : base(message)
        {
        }
    }
}