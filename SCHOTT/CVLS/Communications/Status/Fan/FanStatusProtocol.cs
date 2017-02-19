using SCHOTT.CVLS.Enums;

namespace SCHOTT.CVLS.Communications
{
    /// <summary>
    /// FanStatusProtocol section of the CVLS Legacy Protocol
    /// </summary>
    public class FanStatusProtocol
    {
        private readonly ILegacyProtocol _port;
        private readonly bool _echoComTraffic;

        /// <summary>
        /// Creates a new FanStatusProtocol for the given port
        /// </summary>
        /// <param name="port">The port for the FanStatusProtocol to use.</param>
        /// <param name="echoComTraffic">When True, the protocol object will echo com traffic to the subscribed message functions.</param>
        public FanStatusProtocol(ILegacyProtocol port, bool echoComTraffic = false)
        {
            _port = port;
            _echoComTraffic = echoComTraffic;
        }

        /// <summary>
        /// Gets the Fan Speed in RPM
        /// </summary>
        /// <returns>Current Fan Speed in RPM, -1 if there is a com error</returns>
        public int Speed
        {
            get
            {
                string workingLine;
                var command = "&?g";
                if (_port.SendCommandSingleTest(command, command, out workingLine, true, "", _echoComTraffic))
                {
                    int value;
                    if (int.TryParse(workingLine, out value))
                    {
                        return value;
                    }
                }
                return -1;
            }
        }

        /// ;<summary>
        /// Gets the fan status
        /// </summary>
        /// <returns>Current Fan status, StatusIndicators.ComError if there is a com error</returns>
        public StatusIndicators Status
        {
            get
            {
                string workingLine;
                var command = "&?gs";
                if (_port.SendCommandSingleTest(command, command, out workingLine, true, "", _echoComTraffic))
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
