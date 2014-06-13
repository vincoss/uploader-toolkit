using System;
using System.IO;
using System.Windows.Controls;

using Vinco.Silverlight.Framework.Commands;
using Vinco.Silverlight.ViewModels;
using Vinco.Uploader;


namespace Vinco.Silverlight.Commands
{
    // TODO: Remove (drag and drop files use instead)
    public class AddFilesCommand : Command
    {
        private readonly UploadItemListViewModel _model;

        public AddFilesCommand(UploadItemListViewModel model)
        {
            _model = model;
        }

        public override void Execute(object parameter)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            fileDialog.Multiselect = _model.Multiselect;
            //fileDialog.Filter = _model.Filter;

            if ((bool)fileDialog.ShowDialog())
            {
                foreach (FileInfo info in fileDialog.Files)
                {
                    //var fileInfoWrapper = new FileInfoWrapper(info);
                    //UploadItem item = new UploadItem(_model.Controller.GetHandler(), fileInfoWrapper);
                    //UploadItemViewModel model = new UploadItemViewModel(item);

                    //var task = new Task(handler, infoWrap, OnBeginUpload, OnUploadCompleted, OnUploadProgressChanged);

                    //UploadItemViewModel model = new UploadItemViewModel(uri, file.Name, file.OpenRead(), _model.MaxReadSize);

                    //_model.CurrentTotalUploadSize = _model.CurrentTotalUploadSize + (ulong)file.Length;

                    //if (((ulong)info.Length) > _model.MaximumUploadSize)
                    //{
                    //    // Max upload size
                    //}
                    //if ((_model.CurrentTotalUploadSize + (ulong)info.Length) > _model.MaximumTotalUploadSize)
                    //{
                    //    // Max total upload
                    //}
                    //if (_model.ItemsSource.Count >= _model.MaxConcurrentUploads)
                    //{
                    //    // Max concurent uploads
                    //}

                    // if valid
                    //if (true)
                    //{
                    //    _model.ItemsSource.Add(model);
                    //}
                    //model.UploadItem.Upload();
                }
            }
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }
    }
}
