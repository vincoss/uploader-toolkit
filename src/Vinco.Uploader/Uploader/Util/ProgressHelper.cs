using System;


namespace Vinco.Uploader.Util
{
    public class ProgressHelper
    {
        public static TimeSpan CalculateAverageEta(DateTime startDate, long length, long sent)
        {
            if (startDate > DateTime.Now)
            {
                throw new ArgumentOutOfRangeException("startDate");
            }
            TimeSpan elapsed = DateTime.Now - startDate;
            if(length == 0 || sent == 0 || elapsed.Ticks == 0 || length < sent)
            {
                return elapsed;
            }
            TimeSpan estimated = TimeSpan.FromMilliseconds((length - sent) / (sent / elapsed.TotalMilliseconds));
            return estimated;
        }

        public static long CalculateAverageUploadSpeed(long sent, TimeSpan duration, MovingAverage movingAverage)
        {
            if (movingAverage == null)
            {
                throw new ArgumentNullException("movingAverage");
            }
            if (duration.Ticks == 0)
            {
                return 0;
            }
            long average = movingAverage.NextValue(duration.Ticks);
            TimeSpan timeSpan = new TimeSpan(average);

            double result = 100 / timeSpan.TotalMilliseconds;
            double bandwidth = result * sent;

            return (long) bandwidth;
        }
    }
}
