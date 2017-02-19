namespace SCHOTT.CVLS.Communications
{
    /// <summary>
    /// MemoryProtocol section of the CVLS Legacy Protocol
    /// </summary>
    public class MemoryProtocol
    {
        private readonly ILegacyProtocol _port;
        private readonly bool _echoComTraffic;

        /// <summary>
        /// Creates a new MemoryProtocol for the given port
        /// </summary>
        /// <param name="port">The port for the MemoryProtocol to use.</param>
        /// <param name="echoComTraffic">When True, the protocol object will echo com traffic to the subscribed message functions.</param>
        public MemoryProtocol(ILegacyProtocol port, bool echoComTraffic = false)
        {
            _port = port;
            _echoComTraffic = echoComTraffic;
        }

        /// <summary>
        /// Gets the CustomerSettings Write Counts
        /// </summary>
        /// <returns>Current CustomerSettings Write Counts, -1 if there is a com error</returns>
        public int CustomerSettings
        {
            get
            {
                string workingLine;
                var command = "&?ms";
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

        /// <summary>
        /// Gets the FactorySettings Write Counts
        /// </summary>
        /// <returns>Current FactorySettings Write Counts, -1 if there is a com error</returns>
        public int FactorySettings
        {
            get
            {
                string workingLine;
                var command = "&?mf";
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

        /// <summary>
        /// Gets the Firmware Write Counts
        /// </summary>
        /// <returns>Current Firmware Write Counts, -1 if there is a com error</returns>
        public int Firmware
        {
            get
            {
                string workingLine;
                var command = "&?mp";
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

        /// <summary>
        /// Gets the Exceptions Write Counts
        /// </summary>
        /// <returns>Current Exceptions Write Counts, -1 if there is a com error</returns>
        public int Exceptions
        {
            get
            {
                string workingLine;
                var command = "&?ml";
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

    }
}
