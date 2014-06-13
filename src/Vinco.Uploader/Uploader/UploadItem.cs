using System;
using Vinco.Uploader.Util;


namespace Vinco.Uploader
{
    public class UploadItem : UploadItemBase
    {
        private readonly Guid _id;

        private TimeSpan? _eta;
        private TimeSpan _elapsed;
        private long? _uploadSpeed;
        private long _totalBytesUploaded;

        private DateTime? _startDate;
        private DateTime? _endDate;
        private FileUploadStatus _fileUploadStatus;

        private Action _processUpload;
        private readonly FileInfoBase _fileInfo;
        private DateTime _lastRefresh;
        private TimeSpan? _averageChunkUpload;
        private readonly MovingAverage _movingAverage = new MovingAverage(10);

        public UploadItem(FileInfoBase fileInfo)
        {
            if (fileInfo == null)
            {
                throw new ArgumentNullException("fileInfo");
            }
            _id = Guid.NewGuid();
            _fileInfo = fileInfo;
            _lastRefresh = DateTime.Now;
        }

        #region Public methods

        public override void Pause()
        {
            if (_fileUploadStatus == FileUploadStatus.Uploading)
            {
                _fileUploadStatus = FileUploadStatus.Paused;
                OnProgressChanged();
            }
        }

        public override void Resume()
        {
            if (_fileUploadStatus == FileUploadStatus.Paused)
            {
                _fileUploadStatus = FileUploadStatus.Uploading;
                _processUpload();
                OnProgressChanged();
            }
        }

        public override void Cancel()
        {
            if (_fileUploadStatus == FileUploadStatus.Uploading || _fileUploadStatus == FileUploadStatus.Pending || _fileUploadStatus == FileUploadStatus.Paused)
            {
                _fileUploadStatus = FileUploadStatus.Canceled;
                OnProgressChanged();
            }
        }

        public override void SetResumeAction(Action processUpload)
        {
            if (processUpload == null)
            {
                throw new ArgumentNullException("processUpload");
            }
            _processUpload = processUpload;
        }

        public override UploadEventArgs GetUploadEvent()
        {
            return new UploadEventArgs(this, OnBeginUpload, OnUploadCompleted, OnUploadProgressChanged);
        }

        public override string ToString()
        {
            return _fileInfo.Name;
        }

        #endregion

        #region Private methods

        private void OnBeginUpload()
        {
            _startDate = DateTime.Now;
            this._fileUploadStatus = FileUploadStatus.Uploading;
            _processUpload();
        }

        private void OnUploadProgressChanged(long bytesSent, TimeSpan duration)
        {
            _totalBytesUploaded += bytesSent;

            // Refresh every second
            if ((DateTime.Now - _lastRefresh).TotalMilliseconds >= 500)
            {
                _lastRefresh = DateTime.Now;
                _elapsed = DateTime.Now - StartDate.Value;
                _uploadSpeed = ProgressHelper.CalculateAverageUploadSpeed(bytesSent, duration, _movingAverage);
                _eta = ProgressHelper.CalculateAverageEta(StartDate.Value, Length, TotalBytesUploaded);
                _averageChunkUpload = new TimeSpan(_movingAverage.Sum);
            }
            OnProgressChanged();
        }

        private void OnUploadCompleted(Exception ex)
        {
            if (ex != null)
            {
                this.Message = ex.ToString();
                _fileUploadStatus = FileUploadStatus.Error;
            }
            if(_fileUploadStatus == FileUploadStatus.Uploading)
            {
                _fileUploadStatus = FileUploadStatus.Complete;
            }
            _eta = null;
            _uploadSpeed = null;
            _endDate = DateTime.Now;
            _averageChunkUpload = null;
            OnUploadCompleted();
        }

        #endregion

        #region Properties

        public override Guid Id
        {
            get { return _id; }
        }

        public override string Name
        {
            get { return _fileInfo.Name; }
        }

        public override long Length
        {
            get { return _fileInfo.Length; }
        }

        public override TimeSpan? Eta
        {
            get { return _eta; }
        }

        public override TimeSpan Elapsed
        {
            get { return _elapsed; }
        }

        public override long? UploadSpeed
        {
            get { return _uploadSpeed; }
        }

        public override long TotalBytesUploaded
        {
            get { return _totalBytesUploaded; }
        }

        public override DateTime? StartDate
        {
            get { return _startDate; }
        }

        public override DateTime? EndDate
        {
            get { return _endDate; }
        }
        
        public override string Message { get; set; }
        
        public override TimeSpan? AverageChunkUpload
        {
            get { return _averageChunkUpload; }
        }

        public override FileUploadStatus Status
        {
            get { return _fileUploadStatus; }
        }

        public override FileInfoBase FileInfo
        {
            get { return _fileInfo; }
        }

        #endregion
    }
}
