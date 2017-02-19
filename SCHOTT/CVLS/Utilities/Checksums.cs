using System.Collections.Generic;

namespace SCHOTT.CVLS.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    public static class Checksums
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Fletcher16(List<byte> data)
        {
            return Fletcher16(data.ToArray(), data.Count);
        }

        /// <summary>
        /// Computes the Fletcher16 checksum for the data[]
        /// </summary>
        /// <param name="data"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] Fletcher16(byte[] data, int bytes)
        {
            ushort sum1 = 0xff, sum2 = 0xff;
            var i = 0;

            while (bytes > 0)
            {
                var tlen = bytes > 20 ? 20 : bytes;
                bytes -= tlen;
                do
                {
                    sum2 += sum1 += data[i++];
                } while (--tlen > 0);
                /* First reduction step to reduce sums to 8 bits */
                sum1 = (ushort)((sum1 & 0xff) + (sum1 >> 8));
                sum2 = (ushort)((sum2 & 0xff) + (sum2 >> 8));
            }

            /* Second reduction step to reduce sums to 8 bits */
            sum1 = (ushort)((sum1 & 0xff) + (sum1 >> 8));
            sum2 = (ushort)((sum2 & 0xff) + (sum2 >> 8));

            return new[] { (byte)sum2, (byte)sum1 };
        }

    }
}
