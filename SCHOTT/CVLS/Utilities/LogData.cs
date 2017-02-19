using System.Collections.Generic;

namespace SCHOTT.CVLS.Utilities
{
    /// <summary>
    /// Class to store exception log data
    /// </summary>
    public class LogData
    {
        /// <summary>
        /// The log count of this exception.
        /// </summary>
        public uint LogCount { get; private set; }

        /// <summary>
        /// When this exception happened if the unit had access to a NTP server, 0 otherwise.
        /// </summary>
        public uint Timestamp { get; private set; }

        /// <summary>
        /// The exception message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Create a new exception log.
        /// </summary>
        /// <param name="log">The stream of data from the BinarySocket</param>
        public LogData(List<byte> log)
        {
            LogCount = ((uint)log[3] << 24) + ((uint)log[2] << 16) + ((uint)log[1] << 8) + log[0];
            Timestamp = ((uint)log[7] << 24) + ((uint)log[6] << 16) + ((uint)log[5] << 8) + log[4];
            Message = System.Text.Encoding.UTF8.GetString(log.GetRange(8, log.Count - 8).ToArray());
        }

        /// <summary>
        /// Create a new exception log.
        /// </summary>
        /// <param name="count">The exception count when logged</param>
        /// <param name="timeStamp">The system timestamp when the exception was logged</param>
        /// <param name="message">The message description of the exception</param>
        public LogData(uint count, uint timeStamp, string message)
        {
            LogCount = count;
            Timestamp = timeStamp;
            Message = message;
        }
    }

}
