using System;
using System.Linq;

using Vinco.Silverlight.Framework.Commands;
using Vinco.Silverlight.ViewModels;
using Vinco.Uploader.Tasks;


namespace Vinco.Silverlight.Commands
{
    // TODO: Remove
    public class StartUploadCommand : Command
    {
        private readonly UploadItemListViewModel _model;

        public StartUploadCommand(UploadItemListViewModel model)
        {
            _model = model;
        }

        public override void Execute(object parameter)
        {
            ScheduledTasks.Instance.Run();
        }

        public override bool CanExecute(object parameter)
        {
            if (_model.ItemsSource.Any() && ScheduledTasks.Instance.Status == SchedulerStatus.Pending || ScheduledTasks.Instance.Status == SchedulerStatus.Paused)
            {
                return true;
            }
            return false;
        }
    }
}
