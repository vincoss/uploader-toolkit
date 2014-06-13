using System.Linq;
using Vinco.Uploader;
using Vinco.Silverlight.ViewModels;
using Vinco.Silverlight.Framework.Commands;


namespace Vinco.Silverlight.Commands
{
    public class CancelCommand : Command
    {
        private readonly UploadItemListViewModel _model;

        public CancelCommand(UploadItemListViewModel model)
        {
            _model = model;
        }

        public override void Execute(object parameter)
        {
            var items = _model.SelectedItems.Where(x => x.UploadItem.Status == FileUploadStatus.Uploading || x.UploadItem.Status == FileUploadStatus.Pending || x.UploadItem.Status == FileUploadStatus.Paused);
            foreach (var item in items)
            {
                item.UploadItem.Cancel();   
            }
        }

        public override bool CanExecute(object parameter)
        {
            bool flag = _model.SelectedItems.Any(x => x.UploadItem.Status == FileUploadStatus.Uploading || x.UploadItem.Status == FileUploadStatus.Pending || x.UploadItem.Status == FileUploadStatus.Paused);
            return flag;
        }
    }
}