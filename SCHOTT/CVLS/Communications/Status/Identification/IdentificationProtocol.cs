namespace SCHOTT.CVLS.Communications
{
    /// <summary>
    /// IdentificationProtocol section of the CVLS Legacy Protocol
    /// </summary>
    public class IdentificationProtocol
    {
        private readonly ILegacyProtocol _port;
        private readonly bool _echoComTraffic;

        /// <summary>
        /// Creates a new IdentificationProtocol for the given port
        /// </summary>
        /// <param name="port">The port for the IdentificationProtocol to use.</param>
        /// <param name="echoComTraffic">When True, the protocol object will echo com traffic to the subscribed message functions.</param>
        public IdentificationProtocol(ILegacyProtocol port, bool echoComTraffic = false)
        {
            _port = port;
            _echoComTraffic = echoComTraffic;
        }

        /// <summary>
        /// Gets the Model:Serial number of the unit
        /// </summary>
        /// <returns>The Model:Serial number of the unit, "Com Error!" if there is a com error</returns>
        public string SerialFull
        {
            get
            {
                string workingLine;
                var command = "&zf?";
                var expectedResponce = "&zf";
                if (_port.SendCommandSingleTest(command, expectedResponce, out workingLine, true, "", _echoComTraffic))
                {
                    return workingLine;
                }
                return "Com Error!";
            }
        }

        /// <summary>
        /// Gets the Serial number of the unit
        /// </summary>
        /// <returns>Current Firmware Write Counts, -1 if there is a com error</returns>
        public int Serial
        {
            get
            {
                string workingLine;
                var command = "&z?";
                var expectedResponce = "&z";
                if (_port.SendCommandSingleTest(command, expectedResponce, out workingLine, true, "", _echoComTraffic))
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

        /// <summary>
        /// Gets the model of the unit
        /// </summary>
        /// <returns>The model of the unit, "Com Error!" if there is a com error</returns>
        public string Model
        {
            get
            {
                string workingLine;
                var command = "&zm?";
                var expectedResponce = "&zm";
                if (_port.SendCommandSingleTest(command, expectedResponce, out workingLine, true, "", _echoComTraffic))
                {
                    return workingLine;
                }
                return "Com Error!";
            }
        }

        /// <summary>
        /// Gets the firmware version of the unit
        /// </summary>
        /// <returns>The firmware version of the unit, "Com Error!" if there is a com error</returns>
        public string Firmware
        {
            get
            {
                string workingLine;
                var command = "&f?";
                var expectedResponce = "&f";
                if (_port.SendCommandSingleTest(command, expectedResponce, out workingLine, true, "", _echoComTraffic))
                {
                    return workingLine;
                }
                return "Com Error!";
            }
        }

        /// <summary>
        /// Gets the product name of the unit
        /// </summary>
        /// <returns>The product name of the unit, "Com Error!" if there is a com error</returns>
        public string Name
        {
            get
            {
                string workingLine;
                var command = "&q";
                if (_port.SendCommandSingleTest(command, command, out workingLine, true, "", _echoComTraffic))
                {
                    return workingLine;
                }
                return "Com Error!";
            }
        }

    }
}
