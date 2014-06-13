using System;


namespace Vinco.Uploader
{
    public class UploadEventArgs
    {
        public UploadEventArgs(UploadItemBase uploadItem, Action beginUpload, Action<Exception> uploadCompleted, Action<long, TimeSpan> uploadProgressChanged)
        {
            if (uploadItem == null)
            {
                throw new ArgumentNullException("uploadItem");
            }
            if (beginUpload == null)
            {
                throw new ArgumentNullException("beginUpload");
            }
            if (uploadCompleted == null)
            {
                throw new ArgumentNullException("uploadCompleted");
            }
            if (uploadProgressChanged == null)
            {
                throw new ArgumentNullException("uploadProgressChanged");
            }
            UploadItem = uploadItem;
            BeginUpload = beginUpload;
            UploadCompleted = uploadCompleted;
            UploadProgressChanged = uploadProgressChanged;
        }

        public UploadItemBase UploadItem { get; private set; }

        public Action BeginUpload { get; private set; }

        public Action<long, TimeSpan> UploadProgressChanged { get; private set; }

        public Action<Exception> UploadCompleted { get; private set; }
    }
}
