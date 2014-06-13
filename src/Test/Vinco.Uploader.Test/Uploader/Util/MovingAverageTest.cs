using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Vinco.Uploader.Util
{
    [TestFixture]
    public class MovingAverageTest
    {
        [Test]
        public void ToString_Test()
        {
            var movingAverage = new MovingAverage(0);

            Assert.AreEqual("0", movingAverage.ToString());
        }

        [Test]
        public void Reset_Test()
        {
            var movingAverage = new MovingAverage(0);

            movingAverage.NextValue(1);
            movingAverage.Reset();

            Assert.AreEqual(0, movingAverage.Sum);
        }

        [Test]
        public void NextValue_Test()
        {
            var movingAverage = new MovingAverage(0);

            Assert.AreEqual(1, movingAverage.NextValue(1));
            Assert.AreEqual(2, movingAverage.NextValue(3));
        }

        [Test]
        public void WithDefaultWindowSize_Test()
        {
            var movingAverage = new MovingAverage(0);

            movingAverage.NextValue(1);
            movingAverage.NextValue(1);
            movingAverage.NextValue(1);
            movingAverage.NextValue(1);
            movingAverage.NextValue(1);

            Assert.AreEqual(1, movingAverage.Sum);
        }

        [Test]
        public void Moving_Test()
        {
            var movingAverage = new MovingAverage(0);

            movingAverage.NextValue(1); // Will be removed
            movingAverage.NextValue(2);
            movingAverage.NextValue(3);
            movingAverage.NextValue(4);
            movingAverage.NextValue(5);
            movingAverage.NextValue(6);

            Assert.AreEqual(4, movingAverage.Sum);
        }
    }
}
