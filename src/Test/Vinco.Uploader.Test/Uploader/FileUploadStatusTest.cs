using NUnit.Framework;


namespace Vinco.Uploader
{
    [TestFixture]
    public class FileUploadStatusTest
    {
        [Test]
        public void Test()
        {
            Assert.AreEqual(0, (byte)FileUploadStatus.Pending);
            Assert.AreEqual(1, (byte)FileUploadStatus.Uploading);
            Assert.AreEqual(2, (byte)FileUploadStatus.Complete);
            Assert.AreEqual(3, (byte)FileUploadStatus.Error);
            Assert.AreEqual(4, (byte)FileUploadStatus.Canceled);
            Assert.AreEqual(5, (byte)FileUploadStatus.Paused);
        }
    }
}