using SCHOTT.CVLS.Enums;

namespace SCHOTT.CVLS.Communications
{
    /// <summary>
    /// EqualizerStatusProtocol section of the CVLS Legacy Protocol
    /// </summary>
    public class EqualizerStatusProtocol
    {
        private readonly ILegacyProtocol _port;
        private readonly bool _echoComTraffic;

        /// <summary>
        /// Creates a new EqualizerStatusProtocol for the given port
        /// </summary>
        /// <param name="port">The port for the EqualizerStatusProtocol to use.</param>
        /// <param name="echoComTraffic">When True, the protocol object will echo com traffic to the subscribed message functions.</param>
        public EqualizerStatusProtocol(ILegacyProtocol port, bool echoComTraffic = false)
        {
            _port = port;
            _echoComTraffic = echoComTraffic;
        }

        /// <summary>
        /// Gets the Equalizer status
        /// </summary>
        public EqualizerStatus Mode
        {
            get
            {
                string workingLine;
                var command = "&es?";
                var expectedResponce = "&es";
                if (_port.SendCommandSingleTest(command, expectedResponce, out workingLine, true, "", _echoComTraffic))
                {
                    int value;
                    if (int.TryParse(workingLine, out value))
                    {
                        return (EqualizerStatus)value;
                    }
                }
                return EqualizerStatus.ComError;
            }
        }

        /// <summary>
        /// Gets the Equalizer status indicator
        /// </summary>
        /// <returns>Current Equalizer status, StatusIndicators.ComError if there is a com error</returns>
        public StatusIndicators Status
        {
            get
            {
                string workingLine;
                var command = "&esd?";
                var expectedResponce = "&esd";
                if (_port.SendCommandSingleTest(command, expectedResponce, out workingLine, true, "", _echoComTraffic))
                {
                    int value;
                    if (int.TryParse(workingLine, out value))
                    {
                        return (StatusIndicators)value;
                    }
                }
                return StatusIndicators.ComError;
            }
        }
    }
}
