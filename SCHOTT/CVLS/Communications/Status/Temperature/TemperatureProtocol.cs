using SCHOTT.CVLS.Enums;

namespace SCHOTT.CVLS.Communications
{
    /// <summary>
    /// Locations to create a TemperatureProtocol for
    /// </summary>
    public enum TemperatureLocations
    {
        /// <summary>
        /// Monitors temperature of the LED PCB
        /// </summary>
        Led,

        /// <summary>
        /// Monitors temperature of the Driver PCB
        /// </summary>
        Board
    }

    /// <summary>
    /// TemperatureProtocol section of the CVLS Legacy Protocol
    /// </summary>
    public class TemperatureProtocol
    {
        private readonly ILegacyProtocol _port;
        private readonly bool _echoComTraffic;
        private readonly string _char;

        /// <summary>
        /// Creates a new TemperatureProtocol for the given port
        /// </summary>
        /// <param name="port">The port for the TemperatureProtocol to use.</param>
        /// <param name="location">The location to set up the TemperatureProtocol for.</param>
        /// <param name="echoComTraffic">When True, the protocol object will echo com traffic to the subscribed message functions.</param>
        public TemperatureProtocol(ILegacyProtocol port, TemperatureLocations location, bool echoComTraffic = false)
        {
            _port = port;
            _echoComTraffic = echoComTraffic;

            switch (location)
            {
                case TemperatureLocations.Led:
                    _char = "l";
                    break;

                case TemperatureLocations.Board:
                    _char = "b";
                    break;
            }
        }

        /// <summary>
        /// Gets the temperature at the given location
        /// </summary>
        /// <returns>Temperature at the given location, -1 if there is a com error</returns>
        public double Temperature
        {
            get
            {
                string workingLine;
                var command = $"&?{_char}t";
                if (_port.SendCommandSingleTest(command, command, out workingLine, true, "", _echoComTraffic))
                {
                    double value;
                    if (double.TryParse(workingLine, out value))
                        return value;
                }
                return -1;
            }
        }

        /// <summary>
        /// Gets the temperature status at the given location
        /// </summary>
        /// <returns>Status of temperature measurement, StatusIndicators.ComError if there is a com error</returns>
        public StatusIndicators Status
        {
            get
            {
                string workingLine;
                var command = $"&?{_char}s";
                if (_port.SendCommandSingleTest(command, command, out workingLine, true, "", _echoComTraffic))
                {
                    if(workingLine.Contains("1"))
                        return StatusIndicators.Good;

                    if (workingLine.Contains("0"))
                        return StatusIndicators.Error;
                }
                return StatusIndicators.ComError;
            }
        }

        /// <summary>
        /// Gets the thermistor status at the given location
        /// </summary>
        /// <returns>Status of thermistor, StatusIndicators.ComError if there is a com error</returns>
        public StatusIndicators ThermistorStatus
        {
            get
            {
                string workingLine;
                var command = $"&?{_char}m";
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
