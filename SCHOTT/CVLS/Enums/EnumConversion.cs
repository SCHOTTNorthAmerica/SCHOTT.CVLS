namespace SCHOTT.CVLS.Enums
{
    /// <summary>
    /// Converts the CVLS Enums into strings for printing statuses
    /// </summary>
    public static class EnumConversion
    {
        /// <summary>
        /// Converts the CommandSource enum to a string description
        /// </summary>
        public static string[] CommandSourceStrings = { "Front", "Multiport", "RS232", "Legacy Socket", "USB", "Webpage", "Binary Socket" };

        /// <summary>
        /// Converts the UserMode enum to a string description
        /// </summary>
        public static string[] UserModeStrings = { "Home", "Industrial", "Heavy Industrial" };

        /// <summary>
        /// Converts the KnobControl enum to a string description
        /// </summary>
        public static string[] KnobControlStrings = { "Common", "Channel 1", "Channel 2", "Channel 3", "Channel 4", "Demo" };

        /// <summary>
        /// Converts the Indicator enum for thermistors to a string description
        /// </summary>
        public static string[] ThermistorIndicatorStrings = { "", "Functional", "Degraded", "Faulty", "" };

        /// <summary>
        /// Converts the Equalizer enum to a string description
        /// </summary>
        public static string[] EqualizerStrings = { "Unstable", "Locked", "Initializing", "", "Light Low", "", "Light High", "", "Over Range", "", "Under Range" };

        /// <summary>
        /// Converts the SystemMode enum to a string description
        /// </summary>
        public static string[] SystemModeStrings = { "Off", "Constant", "Trig. Strobe", "Cont. Strobe", "Equalizer", "Demo" };

        /// <summary>
        /// Converts the PollerFrequency enum to a int divisor
        /// </summary>
        public static ushort[] PollerDividers = { 0, 1, 2, 4, 5, 8, 10, 20, 25, 40, 50, 100, 125, 200, 250, 500, 625, 1000, 1250, 2500, 5000 };

        /// <summary>
        /// Converts the StatusIndicators enum to a color.
        /// </summary>
        /// <param name="status">StatusIndicators value</param>
        /// <returns>Color</returns>
        public static System.Drawing.Color StatusColor(StatusIndicators status)
        {
            switch (status)
            {
                case StatusIndicators.Off:
                    return System.Drawing.Color.DarkGray;

                case StatusIndicators.Good:
                    return System.Drawing.Color.LimeGreen;

                case StatusIndicators.Warning:
                    return System.Drawing.Color.Yellow;

                case StatusIndicators.Error:
                    return System.Drawing.Color.Red;

                case StatusIndicators.Info:
                    return System.Drawing.Color.DeepSkyBlue;

                default:
                    return System.Drawing.SystemColors.Control;
            }
        }

    }
}
