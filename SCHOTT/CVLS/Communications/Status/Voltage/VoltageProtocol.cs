using SCHOTT.CVLS.Enums;

namespace SCHOTT.CVLS.Communications
{
    /// <summary>
    /// Locations to create a VoltageProtocol for
    /// </summary>
    public enum VoltageLocations
    {
        /// <summary>
        /// Monitors input voltage of the PCB
        /// </summary>
        Input,

        /// <summary>
        /// Monitors output 5V reference from the multiport
        /// </summary>
        Reference5V
    }

    /// <summary>
    /// VoltageProtocol section of the CVLS Legacy Protocol
    /// </summary>
    public class VoltageProtocol
    {
        private readonly ILegacyProtocol _port;
        private readonly bool _echoComTraffic;
        private readonly string _char;

        /// <summary>
        /// Creates a new VoltageProtocol for the given port
        /// </summary>
        /// <param name="port">The port for the VoltageProtocol to use.</param>
        /// <param name="location">The location to set up the VoltageProtocol for.</param>
        /// <param name="echoComTraffic">When True, the protocol object will echo com traffic to the subscribed message functions.</param>
        public VoltageProtocol(ILegacyProtocol port, VoltageLocations location, bool echoComTraffic = false)
        {
            _port = port;
            _echoComTraffic = echoComTraffic;

            switch (location)
            {
                case VoltageLocations.Input:
                    _char = "i";
                    break;

                case VoltageLocations.Reference5V:
                    _char = "o";
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Gets the voltage at the given location
        /// </summary>
        /// <returns>voltage at the given location, -1 if there is a com error</returns>
        public double Voltage
        {
            get
            {
                double value;
                return double.TryParse(_port.SendCommandSingle($"&?v{_char}", true, "", _echoComTraffic), out value) ? value : -1;
            }
        }

        /// <summary>
        /// Gets the voltage status at the given location
        /// </summary>
        /// <returns>Status of voltage measurement, StatusIndicators.ComError if there is a com error</returns>
        public StatusIndicators Status
        {
            get
            {
                string workingLine;
                var command = $"&?v{_char}s";
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
