using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vinco.Uploader
{
    [TestFixture]
    public class HttpUploadCodeTest
    {
        [Test]
        public void Test()
        {
            Assert.AreEqual(200, (int)HttpUploadCode.Ok);
            Assert.AreEqual(401, (int)HttpUploadCode.Unauthorized);
            Assert.AreEqual(404, (int)HttpUploadCode.NotFound);
            Assert.AreEqual(600, (int)HttpUploadCode.FileExists);
            Assert.AreEqual(610, (int)HttpUploadCode.BlockedFile);
            Assert.AreEqual(620, (int)HttpUploadCode.FileLocked);
            Assert.AreEqual(666, (int)HttpUploadCode.DeleteFailed);
            Assert.AreEqual(500, (int)HttpUploadCode.UploadError);
        }
    }
}
