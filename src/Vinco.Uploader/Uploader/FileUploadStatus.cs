using System;


namespace Vinco.Uploader
{
    public enum FileUploadStatus : byte
    {
        Pending = 0,
        Uploading = 1,
        Complete = 2,
        Error = 3,
        Canceled = 4,
        Paused = 5
    }
}