using System;


namespace Vinco.Uploader
{
    public enum HttpUploadCode
    {
        Ok = 200,
        Unauthorized = 401,
        NotFound = 404,
        UploadError = 500,
        FileExists = 600,
        BlockedFile = 610,
        FileLocked = 620,
        DeleteFailed = 666
    }
}