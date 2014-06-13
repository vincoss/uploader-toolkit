using System.Linq;
using Vinco.Silverlight.Framework.Commands;
using Vinco.Silverlight.ViewModels;
using Vinco.Uploader;


namespace Vinco.Silverlight.Commands
{
    public class PauseCommand : Command
    {
        private readonly UploadItemListViewModel _model;

        public PauseCommand(UploadItemListViewModel model)
        {
            _model = model;
        }

        public override void Execute(object parameter)
        {
            var items = _model.SelectedItems.Where(x => x.UploadItem.Status == FileUploadStatus.Uploading || x.UploadItem.Status == FileUploadStatus.Paused);
            foreach (var item in items)
            {
                if (item.UploadItem.Status == FileUploadStatus.Paused)
                {
                    item.UploadItem.Resume();
                }
                else if (item.UploadItem.Status == FileUploadStatus.Uploading)
                {
                    item.UploadItem.Pause();
                }
            }
        }

        public override bool CanExecute(object parameter)
        {
            bool flag = _model.SelectedItems.Any(x => x.UploadItem.Status == FileUploadStatus.Uploading || x.UploadItem.Status == FileUploadStatus.Paused);
            return flag;
        }
    }
}
