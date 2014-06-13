using System;
using System.Text;
using System.Net;
using System.IO;
using System.Globalization;

#if SILVERLIGHT
using System.Windows.Browser;
#else
using System.Web;
#endif


namespace Vinco.Uploader.Handlers
{
    public class ChunkedHttpUploader : UploadHandlerBase
    {
        private Stream _uploadStream;
        private UploadEventArgs _uploadEventArgs;
        private readonly int _readChunkSize;

        private bool _isLastChunk;
        private bool _isFirstChunk;
        private long _totalBytesUploaded;
        private bool _isUploading;

        private const ushort READ_BUFFER_SIZE = 4096;
        private const int DEFAULT_CHUNK_SIZE = 1048576;
        
        public ChunkedHttpUploader(ushort readChunkSize)
        {
            if (readChunkSize == 0)
            {
                _readChunkSize = DEFAULT_CHUNK_SIZE;
            }
            _isUploading = false;
        }

        #region Public methods

        public override void BeginUpload(UploadEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }
            try
            {
                _uploadEventArgs = args;
                _uploadEventArgs.UploadItem.SetResumeAction(ProcessUpload);
                _uploadStream = args.UploadItem.FileInfo.OpenRead();
                _uploadEventArgs.BeginUpload();
            }
            catch (Exception ex)
            {
                Exception = new UploadException("Response Callback Exception", ex);

                _uploadEventArgs.UploadCompleted(Exception);
            }
        }

        public override void Dispose()
        {
            if (_uploadStream != null)
            {
                _uploadStream.Close();
                _uploadStream.Dispose();
            }
        }

        #endregion

        #region Private methods

        protected override void ProcessUpload()
        {
            if (IsPausedOrCanceled() && _isUploading)
            {
                return;
            }
            _isUploading = true;
            _isFirstChunk = _totalBytesUploaded == 0;
            _isLastChunk = (_uploadEventArgs.UploadItem.Length - _totalBytesUploaded) <= _readChunkSize;

            UriBuilder httpHandlerUrlBuilder = new UriBuilder(UploadUri)
            {
                Query = GetParametersPayload()
            };
            HttpWebRequest webRequest = CreateRequest(httpHandlerUrlBuilder.Uri);
            webRequest.BeginGetRequestStream(WriteToStreamCallback, webRequest);
        } 

        private void WriteToStreamCallback(IAsyncResult asynchronousResult)
        {
            DateTime startTime = DateTime.Now;
            long currentPosition = _totalBytesUploaded;
            HttpWebRequest webRequest = (HttpWebRequest)asynchronousResult.AsyncState;
            using (Stream requestStream = webRequest.EndGetRequestStream(asynchronousResult))
            {
                webRequest.Headers["File-Size"] = _uploadEventArgs.UploadItem.Length.ToString();
                webRequest.Headers["File-Name"] = HttpUtility.UrlEncode(_uploadEventArgs.UploadItem.Name).Replace(" ", "%2b");

                // Set stream position.
                _uploadStream.Position = currentPosition;

                int bytesRead = 0;
                int chunkTotal = 0;
                byte[] buffer = new Byte[READ_BUFFER_SIZE];

                // Read chunks
                while ((bytesRead = _uploadStream.Read(buffer, 0, buffer.Length)) > 0 && ((chunkTotal + bytesRead) <= _readChunkSize) && IsPausedOrCanceled() == false)
                {
                    webRequest.Headers["Payload-Size"] = bytesRead.ToString();

                    requestStream.Write(buffer, 0, bytesRead);
                    requestStream.Flush();

                    _totalBytesUploaded += bytesRead;
                    chunkTotal += bytesRead;
                }
            }
            webRequest.BeginGetResponse(ReadHttpResponseCallback, new ChunkState { Request = webRequest, Position = currentPosition, StartTime = startTime });
        }

        private void ReadHttpResponseCallback(IAsyncResult asynchronousResult)
        {
            long lastPosition = 0;
            bool isCompleted = false;
            try
            {
                if (_uploadEventArgs.UploadItem.Status == FileUploadStatus.Canceled)
                {
                    isCompleted = true;
                }
                else
                {
                    _isUploading = false;
                    ChunkState chunkState = (ChunkState) asynchronousResult.AsyncState;
                    lastPosition = chunkState.Position;
                    TimeSpan uploadDuration = DateTime.Now - chunkState.StartTime;

                    string responseString = null;
                    using (HttpWebResponse webResponse = (HttpWebResponse) chunkState.Request.EndGetResponse(asynchronousResult))
                    {
                        StreamReader reader = new StreamReader(webResponse.GetResponseStream());
                        responseString = HttpUtility.HtmlDecode(reader.ReadToEnd());
                    }
                    CheckFileUploadStatus(responseString);
                    if (Exception == null)
                    {
                        _uploadEventArgs.UploadProgressChanged(_totalBytesUploaded - lastPosition, uploadDuration);
                        if (_totalBytesUploaded < _uploadEventArgs.UploadItem.Length)
                        {
                            if (_uploadEventArgs.UploadItem.Status == FileUploadStatus.Uploading)
                            {
                                ProcessUpload();
                            }
                        }
                        else
                        {
                            isCompleted = true;
                        }
                    }
                    else
                    {
                        if (_uploadEventArgs.UploadItem.CanRetry())
                        {
                            TryFailedUpload(lastPosition);
                        }
                        else
                        {
                            isCompleted = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (_uploadEventArgs.UploadItem.CanRetry())
                {
                    TryFailedUpload(lastPosition);
                }
                else
                {
                    isCompleted = true;
                    Exception = new UploadException("Response Callback Exception", ex);
                }
            }
            finally
            {
                if (isCompleted)
                {
                    _uploadEventArgs.UploadCompleted(Exception);
                }
            }
        }

        private void TryFailedUpload(long lastPosition)
        {
            Exception = null;
            _totalBytesUploaded = lastPosition;
            ProcessUpload();
        }

        private string GetParametersPayload()
        {
            var sb = new StringBuilder();
            sb.AppendFormat(CultureInfo.InvariantCulture, "Id={0}&", _uploadEventArgs.UploadItem.Id);
            sb.AppendFormat(CultureInfo.InvariantCulture, "Partition={0}&", _totalBytesUploaded);
            sb.AppendFormat(CultureInfo.InvariantCulture, "Name={0}&", HttpUtility.UrlEncode(_uploadEventArgs.UploadItem.Name).Replace(" ", "%2b"));
            if (_isFirstChunk)
            {
                sb.AppendFormat(CultureInfo.InvariantCulture, "First={0}&", _isFirstChunk);
                sb.AppendFormat(CultureInfo.InvariantCulture, "OverwriteExisting={0}&", this.OverwriteExistingFile);
            }
            if (_isLastChunk)
            {
                sb.AppendFormat(CultureInfo.InvariantCulture, "Last={0}&", _isLastChunk);
            }
            return sb.ToString();
        }

        private bool IsPausedOrCanceled()
        {
            // Paused or Canceled
            return _uploadEventArgs.UploadItem.Status == FileUploadStatus.Pending ||
                   _uploadEventArgs.UploadItem.Status == FileUploadStatus.Canceled;
        } 

        #endregion

        #region Nested types

        public class ChunkState
        {
            public DateTime StartTime;

            public long Position { get; set; }

            public HttpWebRequest Request { get; set; }
        } 

        #endregion
    }
}