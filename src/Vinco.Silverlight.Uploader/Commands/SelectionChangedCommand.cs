using System;
using System.Linq;
using System.Collections;
using Vinco.Silverlight.Framework.Commands;
using Vinco.Silverlight.ViewModels;


namespace Vinco.Silverlight.Commands
{
    public class SelectionChangedCommand : Command
    {
        private readonly UploadItemListViewModel _model;

        public SelectionChangedCommand(UploadItemListViewModel model)
        {
            _model = model;
        }

        public override void Execute(object parameter)
        {
            var selectedRecords = (IList)parameter;
            if (selectedRecords != null)
            {
                _model.SelectedItems.Clear();
                var selectedItems = selectedRecords.Cast<UploadItemViewModel>().ToList();
                foreach (var item in selectedItems)
                {
                    _model.SelectedItems.Add(item);
                }
            }
        }
    }
}
