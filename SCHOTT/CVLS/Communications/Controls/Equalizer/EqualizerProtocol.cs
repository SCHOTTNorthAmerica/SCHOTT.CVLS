using System;
using System.Globalization;

namespace SCHOTT.CVLS.Communications
{
    /// <summary>
    /// EqualizerProtocol section of the CVLS Legacy Protocol
    /// </summary>
    public class EqualizerProtocol
    {
        private readonly ILegacyProtocol _port;
        private readonly bool _echoComTraffic;

        /// <summary>
        /// Creates a new EqualizerProtocol for the given port
        /// </summary>
        /// <param name="port">The port for the EqualizerProtocol to use.</param>
        /// <param name="echoComTraffic">When True, the protocol object will echo com traffic to the subscribed message functions.</param>
        public EqualizerProtocol(ILegacyProtocol port, bool echoComTraffic = false)
        {
            _port = port;
            _echoComTraffic = echoComTraffic;
        }
        
        /// <summary>
        /// Get/Set the Continuous Strobe Enable status
        /// </summary>
        public bool Enable
        {
            get
            {
                return _port.SendCommandSingle("&e?", echoComTraffic: _echoComTraffic).Contains("&e1");
            }
            set
            {
                _port.SendCommandSingle($"&e{(value ? "1" : "0")}", echoComTraffic: _echoComTraffic);
            }
        }
        
        /// <summary>
        /// Get/Set the Equalizer start up delay
        /// </summary>
        public int Delay
        {
            get
            {
                var returnString = _port.SendCommandSingle("&ei?", echoComTraffic: _echoComTraffic);

                if(returnString.Length <= 3)
                    return -1;

                return int.Parse(returnString.Substring(3));
            }
            set
            {
                _port.SendCommandSingle($"&ei{Math.Min(Math.Max(value, 0), 300)}", echoComTraffic: _echoComTraffic);
            }
        }

        /// <summary>
        /// Get/Set the Equalizer target (matched by the light feed back)
        /// NOTE: THIS NUMBER IS NOT IN LUMENS AND IS NOT THE SAME UNIT TO UNIT
        /// </summary>
        public int Target
        {
            get
            {
                var returnString = _port.SendCommandSingle("&ee?", echoComTraffic: _echoComTraffic);

                if (returnString.Length <= 3)
                    return -1;

                return int.Parse(returnString.Substring(3),
                    NumberStyles.HexNumber, 
                    CultureInfo.InvariantCulture);
            }
            set
            {
                _port.SendCommandSingle($"&ee{Math.Min(Math.Max(value, 0), 4095):X}", echoComTraffic: _echoComTraffic);
            }
        }

        /// <summary>
        /// Gets the Equalizer Averaged Light Feed Back (matched by the light feed back)
        /// NOTE: THIS NUMBER IS NOT IN LUMENS AND IS NOT THE SAME UNIT TO UNIT
        /// </summary>
        public int LightOutput
        {
            get
            {
                var returnString = _port.SendCommandSingle("&ev?", echoComTraffic: _echoComTraffic);

                if (returnString.Length <= 3)
                    return -1;

                return int.Parse(returnString.Substring(3),
                    NumberStyles.HexNumber,
                    CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Gets the Equalizer Driver Output
        /// </summary>
        public double PowerOutput
        {
            get
            {
                var returnString = _port.SendCommandSingle("&ed?", echoComTraffic: _echoComTraffic);

                if (returnString.Length <= 3)
                    return -1;

                return int.Parse(returnString.Substring(3),
                    NumberStyles.HexNumber,
                    CultureInfo.InvariantCulture) * 0.00024420024420024420024420024420024;
            }
        }

    }
}
