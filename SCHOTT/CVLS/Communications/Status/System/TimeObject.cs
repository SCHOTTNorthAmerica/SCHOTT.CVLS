using SCHOTT.CVLS.Enums;
using System;
using System.Globalization;

namespace SCHOTT.CVLS.Communications
{
    /// <summary>
    /// TimeObject section of the CVLS Legacy Protocol
    /// </summary>
    public class TimeObject
    {
        /// <summary>
        /// Gets the Raw System Time
        /// </summary>
        public uint TimeRaw { get; private set; }

        /// <summary>
        /// Gets the System Time
        /// </summary>
        public DateTime Time { get; private set; }

        /// <summary>
        /// Gets the System Time in string form
        /// </summary>
        public string TimeString { get; private set; }

        /// <summary>
        /// Gets the Time status indicator
        /// </summary>
        public StatusIndicators Status { get; private set; }

        /// <summary>
        /// Create a TimeObject from a epoch UINT
        /// </summary>
        /// <param name="systemTime"></param>
        public TimeObject(uint systemTime)
        {
            TimeRaw = systemTime;
            if (TimeRaw == 0)
            {
                Time = new DateTime();
                TimeString = "NTP Server Unreachable";
                Status = StatusIndicators.Off;
            }
            else
            {
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                Time = epoch.AddSeconds(TimeRaw).ToLocalTime();
                TimeString = Time.ToString(CultureInfo.CurrentCulture);
                Status = StatusIndicators.Info;
            }
        }
    }
}
