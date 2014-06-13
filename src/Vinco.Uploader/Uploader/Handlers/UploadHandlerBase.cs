using System;
using System.Net;
using Vinco.Uploader.Util;
using System.IO;
using System.Globalization;

#if SILVERLIGHT
using System.Windows.Browser;
#else
using System.Web;
#endif


namespace Vinco.Uploader.Handlers
{
    public abstract class UploadHandlerBase : IDisposable
    {
        public abstract void BeginUpload(UploadEventArgs args);

        protected abstract void ProcessUpload();

        protected virtual HttpWebRequest CreateRequest(Uri uri)
        {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(uri);
            request.Method = "POST";
            request.ContentType = "multipart/form-data";
            return request;
        }

        protected void CheckFileUploadStatus(string response)
        {
            UploadInfo info = DeserializeUploadResponse(response);
            if (info != null)
            {
                string errorMessage = HttpUtility.HtmlDecode(info.ErrorMessage);
                HttpUploadCode code = (HttpUploadCode) info.StatusCode;
                switch (code)
                {
                    case HttpUploadCode.Ok:
                        {
                            break;
                        }
                    case HttpUploadCode.Unauthorized:
                        {
                            Exception = new UnauthorizedException(errorMessage);
                            break;
                        }
                    case HttpUploadCode.NotFound:
                        {
                            Exception = new FileNotFoundException(errorMessage);
                            break;
                        }
                    case HttpUploadCode.FileExists:
                        {
                            Exception = new FileExistsException(errorMessage);
                            break;
                        }
                    case HttpUploadCode.BlockedFile:
                        {
                            Exception = new FileBlockedException(errorMessage);
                            break;
                        }
                    case HttpUploadCode.FileLocked:
                        {
                            Exception = new FileLockedException(errorMessage);
                            break;
                        }
                    case HttpUploadCode.DeleteFailed:
                        {
                            Exception = new DeleteFailedException(errorMessage);
                            break;
                        }
                    default:
                        {
                            Exception = new UploadException(string.Format(CultureInfo.InvariantCulture, "Invalid status code : {0}", code));
                            break;
                        }
                }
            }
        }

        protected UploadInfo DeserializeUploadResponse(string response)
        {
            UploadInfo info = null;
            try
            {
                info = JsonSerializer.Deserialize<UploadInfo>(response);
            }
            catch (Exception ex)
            {
                Exception = new UploadException("Unable to deserialize upload response", ex);
            }
            return info;
        }

        public bool OverwriteExistingFile { get; set; }

        public virtual Uri UploadUri { get; set; }

        public virtual Uri CancelUri { get; set; }

        public Exception Exception { get; protected set; }

        public abstract void Dispose();
    }
}