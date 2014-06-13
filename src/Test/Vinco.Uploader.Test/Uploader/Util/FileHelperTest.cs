using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;


namespace Vinco.Uploader.Util
{
    [TestFixture]
    public class FileHelperTest
    {
        [Test]
        public void FormatBytes_Test()
        {
            // Act
            var result = FileHelper.FormatBytes(1024L);

            // Assert
            Assert.AreEqual("1024.000 Bytes", result);
        }

        [Test]
        public void FormatSpeedBytes_Test()
        {
            // Act
            var result = FileHelper.FormatSpeedBytes(1024L);

            // Assert
            Assert.AreEqual("1024.000 Bytes/s", result);
        }
    }
}
