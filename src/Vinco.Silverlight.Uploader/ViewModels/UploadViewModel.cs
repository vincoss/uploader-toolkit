using Vinco.Silverlight.Views;
using Vinco.Silverlight.Framework.Models;


namespace Vinco.Silverlight.ViewModels
{
    public class UploadViewModel : PropertyChangedBase
    {
        private readonly IUploadView _view;

        public UploadViewModel() : this(new UploadView())
        {
        }

        public UploadViewModel(IUploadView view)
        {
            _view = view;
            _view.DataContext = this;
        }

        public IUploadView View
        {
            get { return _view; }
        }

        public IUploadItemListView Uploader
        {
            get { return new UploadItemListViewModel().View; }
        }
    }
}
