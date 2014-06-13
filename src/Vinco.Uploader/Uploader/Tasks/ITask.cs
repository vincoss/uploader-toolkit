using System;
using Vinco.Uploader.Handlers;


namespace Vinco.Uploader.Tasks
{
    public interface ITask : IDisposable
    {
        void Run(Action completeAction);

        UploadHandlerBase Handler { get; }
        UploadItemBase UploadItem { get; }
    }
}