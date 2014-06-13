using NUnit.Framework;
using System;
using NSubstitute;
using Vinco.Uploader.Handlers;
using Vinco.Uploader.Mock;


namespace Vinco.Uploader.Tasks
{
    [TestFixture]
    public class TaskTest
    {
        [Test]
        public void Initialize_Test()
        {
            var file = new MemoryFileInfo("Hello from uploader...");
            var uploadItem = new UploadItem(file);
            var handler = Substitute.For<UploadHandlerBase>();

            var task = new Task(handler, uploadItem);

            Assert.AreEqual(task.Handler, handler);
            Assert.AreEqual(task.UploadItem, uploadItem);
        }

        [Test]
        public void Dispose_Test()
        {
            var file = new MemoryFileInfo("Hello from uploader...");
            var uploadItem = new UploadItem(file);
            var handler = Substitute.For<UploadHandlerBase>();

            var task = new Task(handler, uploadItem);

            task.Dispose();

            handler.Received(1).Dispose();
        }

        [Test]
        public void Run_Test()
        {
            var file = new MemoryFileInfo("Hello from uploader...");
            var uploadItem = new UploadItem(file);
            var handler = new LocalUploadHandler();

            var completed = false;

            var task = new Task(handler, uploadItem);
            task.Run(() =>
                {
                    completed = true;
                });

            Assert.IsTrue(completed);
        }

        private class LocalUploadHandler : UploadHandlerBase
        {
            public override void BeginUpload(UploadEventArgs args)
            {
                args.UploadItem.SetResumeAction(this.ProcessUpload);
                args.BeginUpload();
                args.UploadProgressChanged(args.UploadItem.Length/2, new TimeSpan(0, 0, 0, 1));
                args.UploadCompleted(null);
            }

            public override void Dispose()
            {
                throw new NotImplementedException();
            }

            protected override void ProcessUpload()
            {
            }
        }
    }
}
