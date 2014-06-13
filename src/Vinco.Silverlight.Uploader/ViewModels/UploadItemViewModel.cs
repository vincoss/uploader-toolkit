using System;
using Vinco.Uploader;
using Vinco.Silverlight.Framework.Models;
using Vinco.Uploader.Util;


namespace Vinco.Silverlight.ViewModels
{
    // TODO: Validate on add items
    public class UploadItemViewModel : PropertyChangedBase
    {
        private string _eta;
        private string _percent;
        private string _message;
        private readonly UploadItemBase _uploadItem;

        public UploadItemViewModel(UploadItemBase uploadItem)
        {
            if (uploadItem == null)
            {
                throw new ArgumentNullException("uploadItem");
            }
            _uploadItem = uploadItem;
            _uploadItem.ProgressChanged += UploadItem_ProgressChanged;
            _uploadItem.UploadCompleted += UploadItem_UploadCompleted;
        }

        #region Events

        private void UploadItem_UploadCompleted(object sender, EventArgs e)
        {
            // TODO: Not testable
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                NotifyPropertyChanged(() => EndDate);
                NotifyPropertyChanged(() => Message);
                NotifyPropertyChanged(() => Status);
            });
        }

        private void UploadItem_ProgressChanged(object sender, EventArgs e)
        {
            // TODO: Not testable
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    Percent = GetPercentString();

                    //Eta = CalculateEtaString(_uploadItem.TotalBytesUploaded, _uploadItem.Length);

                    NotifyPropertyChanged(() => Eta);
                    NotifyPropertyChanged(() => Elapsed);
                    NotifyPropertyChanged(() => UploadSpeed);
                    NotifyPropertyChanged(() => Uploaded);
                    NotifyPropertyChanged(() => StartDate);
                    NotifyPropertyChanged(() => Status);
                    NotifyPropertyChanged(() => FailedRetries);
                    NotifyPropertyChanged(() => AverageResponse);
                });
        }

        #endregion

        #region Private methods

        // TODO: Needs to complete
        private string CalculateEtaString(long completed, long total)
        {
            TimeSpan startTime = _uploadItem.StartDate.Value.TimeOfDay;
            TimeSpan endTime = DateTime.Now.TimeOfDay;

            TimeSpan elapsedTime = endTime - startTime;

            long estimatedRemaining = elapsedTime.Ticks * (total - completed);

            var est = new TimeSpan(estimatedRemaining);
            return string.Format("{0}:{1}:{2}:{3}", est.Hours, est.Minutes, est.Seconds, est.Milliseconds);
        }

        private string GetPercentString()
        {
            double percent = CalculateUploadPercent();
            return string.Format("{0}%", percent);
        }

        // TODO: Move into utils
        private double CalculateUploadPercent()
        {
            double percent = ((UploadItem.TotalBytesUploaded / (double)UploadItem.Length) * 100);
            return Math.Round(percent, 2);
        }

        #endregion

        #region Properties

        public string Name
        {
            get { return _uploadItem.Name; }
        }

        public string Size
        {
            get { return FileHelper.FormatBytes(_uploadItem.Length); }
        }
        
        public string Eta
        {
            get
            {
                if(UploadItem.Eta.HasValue)
                {
                    return UploadItem.Eta.ToString(); 
                }
                return null;
            } // TODO: User friendly
        }

        public string Elapsed
        {
            get { return UploadItem.Elapsed.ToString(); } // TODO: User friendly
        }

        public string UploadSpeed
        {
            get
            {
                if(_uploadItem.UploadSpeed.HasValue)
                {
                    return FileHelper.FormatSpeedBytes(UploadItem.UploadSpeed.Value);
                }
                return null;
            }
        }

        public string Uploaded
        {
            get { return FileHelper.FormatBytes(UploadItem.TotalBytesUploaded); }
        }

        public string StartDate
        {
            get { return UploadItem.StartDate.ToString(); } // TODO: User friendly
        }

        public string EndDate
        {
            get
            {
                if (UploadItem.EndDate.HasValue)
                {
                    return UploadItem.EndDate.ToString(); // TODO: User friendly
                }
                return null;
            }
        }

        public string Status
        {
            get { return UploadItem.Status.ToString(); } // TODO: User friendly
        }

        public string Percent
        {
            get { return _percent; }
            private set
            {
                if (_percent != value)
                {
                    _percent = value;
                    NotifyPropertyChanged(() => Percent);
                }
            }
        }

        public string Message
        {
            get { return UploadItem.Message; }
        }

        public string FailedRetries
        {
            get { return UploadItem.FailedRetries.ToString();  }
        }

        public string AverageResponse
        {
            get { return UploadItem.AverageChunkUpload.ToString(); }
        }

        public UploadItemBase UploadItem
        {
            get { return _uploadItem; }
        }
        
        #endregion
    }
}
