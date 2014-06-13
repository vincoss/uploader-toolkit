using System.Linq;
using Vinco.Silverlight.Framework.Commands;
using Vinco.Silverlight.ViewModels;
using Vinco.Uploader;


namespace Vinco.Silverlight.Commands
{
    public class ClearCommand : Command
    {
        private readonly UploadItemListViewModel _model;

        public ClearCommand(UploadItemListViewModel model)
        {
            _model = model;
        }

        public override void Execute(object parameter)
        {
            var itemsToClear = _model.ItemsSource.Where(x => x.UploadItem.Status == FileUploadStatus.Error ||
                                                             x.UploadItem.Status == FileUploadStatus.Canceled ||
                                                             x.UploadItem.Status == FileUploadStatus.Complete).ToList();
            foreach (var item in itemsToClear)
            {
                _model.ItemsSource.Remove(item);
                _model.SelectedItems.Remove(item);
            }
        }
    }
}
