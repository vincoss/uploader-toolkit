using System;


namespace Vinco.Uploader
{
    public class DeleteFailedException : Exception
    {
        public DeleteFailedException()
        {
        }

        public DeleteFailedException(string message) : base(message)
        {
        }
    }
}
