// Nicholas Ventimiglia 2016-09-05

namespace Foundation.Architecture
{
    public static class BufferExt
    {
        /// <summary>
        /// For Human Readability
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes/1024f)/1024f;
        }

        /// <summary>
        /// For Human Readability
        /// </summary>
        /// <param name="kilobytes"></param>
        /// <returns></returns>
        public static double ConvertKilobytesToMegabytes(long kilobytes)
        {
            return kilobytes/1024f;
        }
    }
}