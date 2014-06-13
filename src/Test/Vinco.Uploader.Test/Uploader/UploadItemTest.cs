using System;
using NUnit.Framework;
using Vinco.Uploader.Mock;
using System.Threading;


namespace Vinco.Uploader
{
    [TestFixture]
    public class UploadItemTest
    {
        [Test]
        public void Constructor_Test()
        {
            var file = new MemoryFileInfo("Uploader test...");
            var item = new UploadItem(file);

            Assert.AreNotEqual(Guid.Empty, item.Id);
            Assert.AreEqual(file.Name, item.Name);
            Assert.AreEqual(file.Length, item.Length);
            Assert.AreEqual(null, item.Eta);
            Assert.AreEqual(default(TimeSpan), item.Elapsed);
            Assert.AreEqual(null, item.UploadSpeed);
            Assert.AreEqual(0, item.TotalBytesUploaded);
            Assert.AreEqual(null, item.StartDate);
            Assert.AreEqual(null, item.EndDate);
            Assert.AreEqual(null, item.Message);
            Assert.AreEqual(null, item.AverageChunkUpload);
            Assert.AreEqual(FileUploadStatus.Pending, item.Status);
            Assert.AreEqual(file, item.FileInfo);
            Assert.AreEqual(0, item.FailedRetries);
            Assert.AreEqual(file.Name, item.ToString());
        }

        [Test]
        public void GetUploadEvent_Test()
        {
            var file = new MemoryFileInfo("Uploader test...");
            var item = new UploadItem(file);

            var args = item.GetUploadEvent();

            Assert.AreEqual(item, args.UploadItem);
        }

        [Test]
        public void OnBeginUpload_Test()
        {
            var file = new MemoryFileInfo("Uploader test...");
            var item = new UploadItem(file);
            var called = false;

            item.SetResumeAction(() =>
                {
                    called = true;
                });

            var args = item.GetUploadEvent();
            var date = DateTime.Now;

            // Act
            args.BeginUpload();

            Assert.AreEqual(date, item.StartDate);
            Assert.AreEqual(FileUploadStatus.Uploading, item.Status);
            Assert.IsTrue(called);
        }

        [Test]
        public void OnUploadProgressChanged_Test()
        {
            var file = new MemoryFileInfo("Uploader test...");
            var item = new UploadItem(file);

            item.SetResumeAction(() => { });

            var args = item.GetUploadEvent();

            // Act
            args.BeginUpload();
            Thread.Sleep(1000);
            args.UploadProgressChanged(10, new TimeSpan(0, 0, 0, 1));

            // Assert
            Assert.AreEqual(10, item.TotalBytesUploaded, "TotalBytesUploaded");
            Assert.AreEqual(1, item.Elapsed.Seconds, "Elapsed");
            Assert.AreEqual(1, item.UploadSpeed, "UploadSpeed");
            Assert.AreEqual(new TimeSpan(0, 0, 0, 0, 601), item.Eta, "Eta");
            Assert.AreEqual(new TimeSpan(0, 0, 0, 1), item.AverageChunkUpload, "AverageChunkUpload");
        }

        [Test]
        public void OnUploadCompleted_Test()
        {
            var file = new MemoryFileInfo("Uploader test...");
            var item = new UploadItem(file);

            item.SetResumeAction(() => { });

            var args = item.GetUploadEvent();
            var date = DateTime.Now;

            // Act
            args.BeginUpload();
            args.UploadCompleted(null);

            // Assert
            Assert.AreEqual(FileUploadStatus.Complete, item.Status);
            Assert.IsNull(item.UploadSpeed);
            Assert.AreEqual(date, item.EndDate);
            Assert.IsNull(item.AverageChunkUpload);

        }

        [Test]
        public void Pause_Test()
        {
            var file = new MemoryFileInfo("Uploader test...");
            var item = new UploadItem(file);

            item.SetResumeAction(() => { });

            var args = item.GetUploadEvent();

            args.BeginUpload();

            // Act
            item.Pause();

            // Assert
            Assert.AreEqual(FileUploadStatus.Paused, item.Status);
        }

        [Test]
        public void Resume_Test()
        {
            var file = new MemoryFileInfo("Uploader test...");
            var item = new UploadItem(file);
            var progress = false;

            item.SetResumeAction(() => { progress = true; });

            var args = item.GetUploadEvent();

            args.BeginUpload();

            // Act
            item.Pause();
            item.Resume();

            // Assert
            Assert.AreEqual(FileUploadStatus.Uploading, item.Status);
            Assert.IsTrue(progress);
        }

        [Test]
        public void Cancel_Test()
        {
            var file = new MemoryFileInfo("Uploader test...");
            var item = new UploadItem(file);

            item.SetResumeAction(() => { });

            var args = item.GetUploadEvent();

            args.BeginUpload();

            // Act
            item.Cancel();

            Assert.AreEqual(FileUploadStatus.Canceled, item.Status);
        }
    }
}