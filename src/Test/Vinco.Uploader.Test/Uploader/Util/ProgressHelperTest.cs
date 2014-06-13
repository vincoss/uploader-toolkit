using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Vinco.Uploader.Util
{
    [TestFixture]
    public class ProgressHelperTest
    {
        [Test]
        public void CalculateAverageEta_Test()
        {
            var fileLength = 1000;
            var sent = 100;
            var startDate = DateTime.Now.AddSeconds(-1);

            var eta = ProgressHelper.CalculateAverageEta(startDate, fileLength, sent);

            Assert.AreEqual(new TimeSpan(0, 0, 0, 0, 9000), eta);
        }

        [Test]
        public void CalculateAverageUploadSpeed_Test()
        {
            var sent = 100;
            var movingAverate = new MovingAverage(0);
            var duration = new TimeSpan(0, 0, 0, 0, 1);

            var result = ProgressHelper.CalculateAverageUploadSpeed(sent, duration, movingAverate);
            
            Assert.AreEqual(10000, result);
        }
    }
}
