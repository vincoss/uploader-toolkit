using System;
using Vinco.Uploader.Handlers;


namespace Vinco.Uploader.Tasks
{
    public class Task : ITask
    {
        private readonly UploadHandlerBase _handler;
        private readonly UploadItemBase _uploadItem;
        private Action _completeAction;

        public Task(UploadHandlerBase handler, UploadItemBase uploadItem)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            if (handler == null)
            {
                throw new ArgumentNullException("uploadItem");
            }
            _handler = handler;
            _uploadItem = uploadItem;
            _uploadItem.UploadCompleted += _uploadItem_UploadCompleted;
        }

        public void Run(Action completeAction)
        {
            if (completeAction == null)
            {
                throw new ArgumentNullException("completeAction");
            }
            _completeAction = completeAction;
            _handler.BeginUpload(this.UploadItem.GetUploadEvent());
        }

        public void Dispose()
        {
            _handler.Dispose();
        }

        #region Private methods

        private void _uploadItem_UploadCompleted(object sender, EventArgs e)
        {
            _completeAction();
        } 

        #endregion

        #region Properties

        public UploadItemBase UploadItem
        {
            get { return _uploadItem; }
        }

        public UploadHandlerBase Handler
        {
            get { return _handler; }
        }

        #endregion
    }
}
