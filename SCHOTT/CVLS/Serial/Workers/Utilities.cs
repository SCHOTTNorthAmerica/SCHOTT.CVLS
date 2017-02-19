using System.Collections.Generic;

namespace SCHOTT.CVLS.Serial.Workers
{
    /// <summary>
    /// 
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        public static void EscapeList(this List<byte> array)
        {
            var escapeChars = new List<char> { (char)0xFE, '&', '\n', '\r' };
            for (int i = array.Count - 1; i >= 0; i--)
            {
                if (escapeChars.Contains((char)array[i]))
                    array.Insert(i, 0xFE);
            }
        }
    }
}
