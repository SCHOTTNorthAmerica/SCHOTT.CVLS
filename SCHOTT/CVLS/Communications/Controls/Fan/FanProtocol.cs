using System;

namespace SCHOTT.CVLS.Communications
{
    /// <summary>
    /// FanProtocol section of the CVLS Legacy Protocol
    /// </summary>
    public class FanProtocol
    {
        private readonly ILegacyProtocol _port;
        private readonly bool _echoComTraffic;

        /// <summary>
        /// Creates a new FanProtocol for the given port
        /// </summary>
        /// <param name="port">The port for the FanProtocol to use.</param>
        /// <param name="echoComTraffic">When True, the protocol object will echo com traffic to the subscribed message functions.</param>
        public FanProtocol(ILegacyProtocol port, bool echoComTraffic = false)
        {
            _port = port;
            _echoComTraffic = echoComTraffic;
        }

        /// <summary>
        /// Get/Set the Fan ManualOverride status
        /// </summary>
        public bool ManualOverride
        {
            get
            {
                return _port.SendCommandSingle("&ge?", echoComTraffic: _echoComTraffic).Contains("&ge1");
            }
            set
            {
                _port.SendCommandSingle($"&ge{(value ? "1" : "0")}", echoComTraffic: _echoComTraffic);
            }
        }

        /// <summary>
        /// Get/Set the Common Output Power (0-100%, rounded to the nearest 0.1% resolution).
        /// </summary>
        public double Speed
        {
            get
            {
                var returnString = _port.SendCommandSingle("&gs?", echoComTraffic: _echoComTraffic);

                if (returnString.Length <= 3)
                    return -1;

                return int.Parse(returnString.Substring(3)) * 0.1;
            }
            set
            {
                var speed = (int)Math.Round(value * 10.0, 0);
                _port.SendCommandSingle($"&gs{Math.Min(Math.Max(speed, 0), 1000)}", echoComTraffic: _echoComTraffic);
            }
        }

        /// <summary>
        /// Sets the Target LED Temperature (0-100C, rounded to the nearest 0.1C resolution).
        /// </summary>
        public double TargetTemperature
        {
            get
            {
                var returnString = _port.SendCommandSingle("&gt?", echoComTraffic: _echoComTraffic);

                if (returnString.Length <= 3)
                    return -1;

                return int.Parse(returnString.Substring(3)) * 0.1;
            }
            set
            {
                var temp = (int)Math.Round(value * 10.0, 0);
                _port.SendCommandSingle($"&gt{Math.Min(Math.Max(temp, 0), 1000)}", echoComTraffic: _echoComTraffic);
            }
        }

    }
}
