using System;
using Vinco.Silverlight.Views;
using Vinco.Silverlight.ViewModels;
using Vinco.Uploader.Handlers;


namespace Vinco.Silverlight.Controllers
{   
    // TODO:
    public class UploadController
    {
        private static UploadController _instance;

        private UploadController()
        {
        }

        public UploadHandlerBase GetHandler(/* TODO Get hander form handler factory*/)
        {
            return new ChunkedHttpUploader(0)
            {
                UploadUri = UploadUri,
                CancelUri = CancelUri
            };
        }

        internal IUploadView RootVisual()
        {
            var uploadVivewModel = new UploadViewModel();
            return uploadVivewModel.View;
        }

        public static UploadController Instance
        {
            get { return _instance ?? (_instance = new UploadController()); }
        }

        public Uri UploadUri { get; set; }

        public Uri CancelUri { get; set; }
    }
}
