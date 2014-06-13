using System;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Text.RegularExpressions;
using Vinco.Uploader;
using System.Diagnostics;
using System.IO;
using System.Web.Script.Serialization;
using System.Configuration;


namespace UploaderWebSite.Services
{
    public class UploadFileRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new ChunkedHttpUploadHandler();
        }
    }

    public class ChunkedHttpUploadHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            long partition = 0;
            HttpContextBase httpContext = new HttpContextWrapper(context);
            HttpResponseBase httpResponse = httpContext.Response;
            try
            {
                string fileName = GetValue(httpContext, "Name");
                fileName = Regex.Replace(fileName, "%2b", " ");
                Guid id = new Guid(GetValue(httpContext, "Id"));
                partition = Convert.ToInt64(GetValue(httpContext, "Partition"));

                string last = GetValue(httpContext, "Last");
                string first = GetValue(httpContext, "First");

                bool isLast = false;
                bool isFirst = false;

                if (string.IsNullOrWhiteSpace(last) == false)
                {
                    isLast = Boolean.Parse((last));
                }

                if (string.IsNullOrWhiteSpace(first) == false)
                {
                    isFirst = Boolean.Parse((first));
                }

                string uploadPath = GetUploadFilePath(httpContext, id, fileName);

                var uploadService = new UploadService();

                // Check whether file is allowed.
                if (uploadService.IsBlockedExtension(uploadPath))
                {
                    Json(httpResponse, GetUploadInfo(HttpUploadCode.BlockedFile, partition, null));
                }
                if (isFirst)
                {
                    if (httpContext.Request.QueryString.AllKeys.Any(x => string.Equals(x, "OverwriteExisting", StringComparison.OrdinalIgnoreCase)))
                    {
                        Debug.WriteLine("Overwrite Existing Files");

                        bool overwriteExisting = Convert.ToBoolean(GetValue(httpContext, "OverwriteExisting"));
                        if (!overwriteExisting && uploadService.FileExists(uploadPath))
                        {
                            Json(httpResponse, GetUploadInfo(HttpUploadCode.FileExists, partition, null));
                        }
                    }
                }

                //Write the file
                Debug.WriteLine(string.Format("Write data to disk FOLDER: {0}", uploadPath));

                using (FileStream fs = new FileStream(uploadPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                {
                    fs.Position = partition;
                    using (Stream requestStream = context.Request.InputStream)
                    {
                        SaveFile(requestStream, fs);
                    }
                }
                Debug.WriteLine("Write data to disk SUCCESS");
            }
            catch (Exception ex)
            {
                // NOTE: this is an error if you throw exception then response does not have the result

                Json(httpResponse, GetUploadInfo(HttpUploadCode.UploadError, partition, ex.ToString()));
                Debug.WriteLine(ex.ToString());

                // TODO: no need to throw response must return something
                //throw;
            }
            Json(httpResponse, GetUploadInfo(HttpUploadCode.Ok, partition, null));
        }

        #region Private methods

        private static void Json(HttpResponseBase response, object data)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            response.Write(serializer.Serialize(data));
        }

        private static string GetValue(HttpContextBase httpContext, string key)
        {
            return httpContext.Request.QueryString[key];
        }

        private static void SaveFile(Stream stream, FileStream fileStream)
        {
            int bytesRead;
            byte[] buffer = new byte[4096];
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                fileStream.Write(buffer, 0, bytesRead);
            }
        }

        private static string GetUploadFilePath(HttpContextBase httpContext, Guid id, string fileName)
        {
            string uploadPath = GetValueFromConfig("UploadPath", true);
            if (Directory.Exists(uploadPath) == false)
            {
                throw new DirectoryNotFoundException("uploadPath");
            }
            uploadPath = uploadPath.Trim('\\');
            return string.Format(@"{0}\{1}-{2}", uploadPath, id, fileName);
        }

        private static UploadInfo GetUploadInfo(HttpUploadCode code, long partition, string message)
        {
            return new UploadInfo
            {
                Partition = partition,
                StatusCode = (int)code,
                ErrorMessage = HttpUtility.HtmlEncode(message)
            };
        }

        private static string GetValueFromConfig(string propertyName, bool throwIfMissing)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName");
            }
            propertyName = propertyName.Trim(new char[] { ' ' });
            if (propertyName.Length == 0)
            {
                throw new ArgumentNullException("propertyName");
            }
            string value = ConfigurationManager.AppSettings[propertyName];
            if (string.IsNullOrWhiteSpace(value) && throwIfMissing)
            {
                throw new ArgumentNullException(string.Format("Missing_Configuration_Property_Value : {0}", propertyName));
            }
            return value;
        }

        #endregion

        public bool IsReusable
        {
            get { return true; }
        }

    }
}