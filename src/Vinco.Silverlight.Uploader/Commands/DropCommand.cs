using System;
using System.IO;
using Vinco.Silverlight.Controllers;
using Vinco.Silverlight.Framework.Commands;
using Vinco.Silverlight.ViewModels;
using Vinco.Uploader;


namespace Vinco.Silverlight.Commands
{
    public class DropCommand : Command
    {
        private readonly UploadItemListViewModel _model;

        public DropCommand(UploadItemListViewModel model)
        {
            _model = model;
        }

        public override void Execute(object parameter)
        {
            // TODO: replace with FileInfoBase
            FileInfo[] files = (FileInfo[])parameter;
            foreach (FileInfo info in files)
            {
                if (info.Exists) // TODO: not testable
                {
                    FileInfoBase fileInfoWrapper = new FileInfoWrapper(info);
                    UploadItem item = new UploadItem(fileInfoWrapper);
                    UploadItemViewModel model = new UploadItemViewModel(item);
                    _model.ItemsSource.Add(model);

                    var task = Vinco.Uploader.Tasks.ScheduledTasks.Instance.CreateTask(UploadController.Instance.GetHandler(), item);
                    Vinco.Uploader.Tasks.ScheduledTasks.Instance.Add(task);
                }
            }
        }
    }
}