using System;
using System.Diagnostics;


namespace Vinco.Uploader
{
    [DebuggerDisplay("Name : {Name}, Length : {Length}, Status : {Status}")]
    public abstract class UploadItemBase
    {
        private const int DefaultRetriesLeft = 6;
        private int _retriesLeft = DefaultRetriesLeft;

        public abstract void Pause();
        public abstract void Resume();
        public abstract void Cancel();
        public abstract UploadEventArgs GetUploadEvent();
        public abstract void SetResumeAction(Action resumeAction);

        public bool CanRetry()
        {
            _retriesLeft--;
            return _retriesLeft > 0;
        }

        protected void OnProgressChanged()
        {
            if (ProgressChanged != null)
            {
                ProgressChanged(this, EventArgs.Empty);
            }
        }

        protected void OnUploadCompleted()
        {
            if (UploadCompleted != null)
            {
                UploadCompleted(this, EventArgs.Empty);
            }
        }

        public abstract Guid Id { get; }

        public abstract string Name { get; }

        public abstract long Length { get; }

        public abstract TimeSpan? Eta { get; }

        public abstract TimeSpan Elapsed { get; }

        public abstract long? UploadSpeed { get; }

        public abstract long TotalBytesUploaded { get; }

        public abstract DateTime? StartDate { get; }

        public abstract DateTime? EndDate { get; }

        public virtual string Message { get; set; }

        public int FailedRetries
        {
            get { return DefaultRetriesLeft - _retriesLeft; }
        }

        public abstract TimeSpan? AverageChunkUpload { get; }

        public abstract FileUploadStatus Status { get; }

        public abstract FileInfoBase FileInfo { get; }

        public event EventHandler UploadCompleted;

        public event EventHandler ProgressChanged;

    }
}
