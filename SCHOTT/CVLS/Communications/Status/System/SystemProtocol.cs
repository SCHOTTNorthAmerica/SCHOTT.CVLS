using SCHOTT.CVLS.Enums;

namespace SCHOTT.CVLS.Communications
{
    /// <summary>
    /// SystemProtocol section of the CVLS Legacy Protocol
    /// </summary>
    public class SystemProtocol
    {
        private readonly ILegacyProtocol _port;
        private readonly bool _echoComTraffic;

        /// <summary>
        /// Creates a new SystemProtocol for the given port
        /// </summary>
        /// <param name="port">The port for the SystemProtocol to use.</param>
        /// <param name="echoComTraffic">When True, the protocol object will echo com traffic to the subscribed message functions.</param>
        public SystemProtocol(ILegacyProtocol port, bool echoComTraffic = false)
        {
            _port = port;
            _echoComTraffic = echoComTraffic;
        }

        /// <summary>
        /// Gets the System Mode
        /// </summary>
        /// <returns>Current System Mode, SystemMode.ComError if there is a com error</returns>
        public SystemMode SystemMode
        {
            get
            {
                string workingLine;
                var command = "&?sm";
                if (_port.SendCommandSingleTest(command, command, out workingLine, true, "", _echoComTraffic))
                {
                    int value;
                    if (int.TryParse(workingLine, out value))
                    {
                        return (SystemMode)value;
                    }
                }
                return SystemMode.ComError;
            }
        }

        /// <summary>
        /// Gets the System Last Command Source
        /// </summary>
        /// <returns>Current System Last Command Source, CommandSource.ComError if there is a com error</returns>
        public CommandSource LastCommandSource
        {
            get
            {
                string workingLine;
                var command = "&m?";
                var expectedResponce = "&m";
                if (_port.SendCommandSingleTest(command, expectedResponce, out workingLine, true, "", _echoComTraffic))
                {
                    int value;
                    if (int.TryParse(workingLine, out value))
                    {
                        return (CommandSource)value;
                    }
                }
                return CommandSource.ComError;
            }
        }

        /// <summary>
        /// Gets the System User Mode
        /// </summary>
        /// <returns>Current System User Mode, UserMode.ComError if there is a com error</returns>
        public UserMode SystemUserMode
        {
            get
            {
                string workingLine;
                var command = "&?su";
                if (_port.SendCommandSingleTest(command, command, out workingLine, true, "", _echoComTraffic))
                {
                    int value;
                    if (int.TryParse(workingLine, out value))
                    {
                        return (UserMode)value;
                    }
                }
                return UserMode.ComError;
            }
        }

        /// <summary>
        /// Gets the System Knob Control Mode
        /// </summary>
        /// <returns>Current Knob Control Mode, KnobControl.ComError if there is a com error</returns>
        public KnobControl KnobMode
        {
            get
            {
                string workingLine;
                var command = "&n?";
                var expectedResponce = "&n";
                if (_port.SendCommandSingleTest(command, expectedResponce, out workingLine, true, "", _echoComTraffic))
                {
                    int value;
                    if (int.TryParse(workingLine, out value))
                    {
                        return (KnobControl)value;
                    }
                }
                return KnobControl.ComError;
            }
        }

        /// <summary>
        /// Gets the system LightFeedBack reading. This number is NOT Lumens. 
        /// It is an uncalibrated feed back number used by the equalizer and 
        /// is NOT guaranteed to be consistent unit to unit. 
        /// </summary>
        /// <returns>Light Feed Back Value, -1 if there is a com error</returns>
        public int LightFeedBack
        {
            get
            {
                string workingLine;
                var command = "&?i";
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
        /// Gets the TimeProtocol for the system
        /// </summary>
        /// <returns>Gets the system time, null time if there is a com error</returns>
        public TimeObject Time
        {
            get
            {
                string workingLine;
                var command = "&?st";
                if (_port.SendCommandSingleTest(command, command, out workingLine, true, "", _echoComTraffic))
                {
                    uint value;
                    if (uint.TryParse(workingLine, out value))
                    {
                        return new TimeObject(value);
                    }
                }
                return new TimeObject(0);
            }
        }
    }
}
