using System.Collections.Generic;
using System.Linq;

namespace SCHOTT.CVLS.Utilities
{
    /// <summary>
    /// Data conversions used in the Binary Socket
    /// </summary>
    public static class DataConversions
    {
        private static readonly List<byte> BlankArray = new List<byte> { 0, 0, 0, 0 };

        /// <summary>
        /// Converts a UInt16 to a List of bytes.
        /// </summary>
        /// <param name="value">UInt16 value</param>
        /// <returns>List of bytes</returns>
        public static List<byte> ConvertUInt16ToList(ushort value)
        {
            var list = new List<byte>
            {
                (byte) ((value >> 8) & 0xFF),
                (byte) (value & 0xFF)
            };
            return list;
        }

        /// <summary>
        /// Converts a List of bytes to a UInt16 value.
        /// </summary>
        /// <param name="list">List of bytes to convert</param>
        /// <returns>UInt16 value</returns>
        public static ushort ConvertListToUint16(List<byte> list)
        {
            list.InsertRange(0, BlankArray.Take(2 - list.Count));
            return (ushort)((list[0] << 8) + list[1]);
        }

        /// <summary>
        /// Converts a UInt32 to a List of bytes.
        /// </summary>
        /// <param name="value">UInt32 value</param>
        /// <returns>List of bytes</returns>
        public static List<byte> ConvertUInt32ToList(uint value)
        {
            var list = new List<byte>
            {
                (byte) ((value >> 24) & 0xFF),
                (byte) ((value >> 16) & 0xFF),
                (byte) ((value >> 8) & 0xFF),
                (byte) (value & 0xFF)
            };
            return list;
        }

        /// <summary>
        /// Converts a List of bytes to a UInt32 value.
        /// </summary>
        /// <param name="list">List of bytes to convert</param>
        /// <returns>UInt32 value</returns>
        public static uint ConvertListToUint32(List<byte> list)
        {
            list.InsertRange(0, BlankArray.Take(4 - list.Count));
            return ((uint)list[0] << 24) + ((uint)list[1] << 16) + ((uint)list[2] << 8) + list[3];
        }

    }
}
