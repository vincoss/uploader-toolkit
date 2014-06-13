using System;


namespace Vinco.Uploader.Util
{
    public class FileHelper
    {
        public static string FormatBytes(long bytes)
        {
            const int scale = 1024;
            string[] units = { "TB", "GB", "MB", "KB", "Bytes" };
            long max = (long)Math.Pow(scale, units.Length - 1);

            foreach (string order in units)
            {
                if (bytes > max)
                {
                    return string.Format("{0:0.000} {1}", decimal.Divide(bytes, max), order);
                }
                max /= scale;
            }
            return "0 Bytes";
        }

        public static string FormatSpeedBytes(long bytes)
        {
            return string.Format("{0}/s", FormatBytes(bytes));
        }
    }
}
