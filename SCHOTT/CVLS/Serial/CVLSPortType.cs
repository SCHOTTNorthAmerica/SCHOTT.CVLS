using System;

namespace SCHOTT.CVLS.Serial
{
    /// <summary>
    /// Allows the user to specify which type of port they want to connect to
    /// </summary>
    [Flags]
    public enum CVLSPortType
    {
        /// <summary>
        /// Allows connection to USB ports
        /// </summary>
        Usb = 1,

        /// <summary>
        /// Allows connection to RS232 ports
        /// </summary>
        Rs232 = 2
    }

}
