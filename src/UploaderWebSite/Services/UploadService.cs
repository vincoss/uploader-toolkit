using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using Vinco.Uploader;


namespace UploaderWebSite.Services
{
    public interface IUploadService
    {
        bool FileExists(string path);
        bool IsBlockedExtension(string path);

        UploadInfo GetUploadInfo(HttpUploadCode code, long partition, string message);
        UploadInfo CancelUpload(string path);
    }

    public class UploadService : IUploadService
    {
        public bool FileExists(string path)
        {
            // TODO: Here just query completed uploads (directory or database), to check whether file exists.

            return false;
        }

        public bool IsBlockedExtension(string path)
        {
            return false;

            // TODO: Just uncoment to use this filter

            //string extension = Path.GetExtension(path);
            //extension = extension.TrimStart(new char[] { '.' });
            //return !AllowedExtensionList.Exists(extension);
        }

        public UploadInfo GetUploadInfo(HttpUploadCode code, long partition, string message)
        {
            return new UploadInfo
            {
                Partition = partition,
                StatusCode = (int)code,
                ErrorMessage = message
            };
        }

        public UploadInfo CancelUpload(string path)
        {
            UploadInfo info = null;
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);

                    info = GetUploadInfo(HttpUploadCode.Ok, 0, null);
                }
                else
                {
                    info = GetUploadInfo(HttpUploadCode.NotFound, 0, null);
                }
            }
            catch (IOException ex)
            {
                info = GetUploadInfo(HttpUploadCode.DeleteFailed, 0, ex.Message);
            }
            return info;
        }
    }
}
