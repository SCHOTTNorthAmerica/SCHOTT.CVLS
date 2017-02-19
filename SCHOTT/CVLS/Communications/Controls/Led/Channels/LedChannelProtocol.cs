using SCHOTT.CVLS.Enums;
using System;

namespace SCHOTT.CVLS.Communications
{
    /// <summary>
    /// LedChannelProtocol section of the CVLS Legacy Protocol
    /// </summary>
    public class LedChannelProtocol
    {
        private readonly ILegacyProtocol _port;
        private readonly bool _echoComTraffic;
        private readonly Enums.Channels _channel;

        /// <summary>
        /// Creates a new LedChannelProtocol for the given port and channel
        /// </summary>
        /// <param name="port">The port for the LedChannelProtocol to use.</param>
        /// <param name="selectedChannel">The channel for the LedChannelProtocol to use.</param>
        /// <param name="echoComTraffic">When True, the protocol object will echo com traffic to the subscribed message functions.</param>
        public LedChannelProtocol(ILegacyProtocol port, Enums.Channels selectedChannel, bool echoComTraffic = false)
        {
            _port = port;
            _echoComTraffic = echoComTraffic;
            _channel = selectedChannel;
        }

        /// <summary>
        /// Get/Set the enable status
        /// </summary>
        public bool Enabled
        {
            get
            {
                return _port.SendCommandSingle($"&l{(int)_channel},?", echoComTraffic: _echoComTraffic).Contains($"&l{(int)_channel},1");
            }
            set
            {
                _port.SendCommandSingle($"&l{(int) _channel},{(value ? "1" : "0")}", echoComTraffic: _echoComTraffic);
            }
        }

        /// <summary>
        /// Get/Set the power (0-100%, rounded to the nearest 0.1% resolution).
        /// </summary>
        public double Power
        {
            get
            {
                var returnString = _port.SendCommandSingle($"&i{(int)_channel},?", echoComTraffic: _echoComTraffic);
                var tokens = returnString.Split(',');

                if (tokens.Length == 2)
                    return int.Parse(tokens[1]) * 0.1;

                return 0;
            }
            set
            {
                var power = (int)Math.Round(value * 10.0, 0);
                _port.SendCommandSingle($"&i{(int)_channel},{Math.Min(Math.Max(power, 0), 1000)}", echoComTraffic: _echoComTraffic);
            }
        }

        /// <summary>
        /// Get/Set the shutdown polarity.
        /// </summary>
        public SignalPolarity ShutdownPolarity
        {
            get
            {
                return _port.SendCommandSingle($"&j{(int)_channel},?", echoComTraffic: _echoComTraffic).Contains($"&j{(int)_channel},1")
                    ? SignalPolarity.ActiveHigh
                    : SignalPolarity.ActiveLow;
            }
            set
            {
                _port.SendCommandSingle($"&j{(int)_channel},{(value == SignalPolarity.ActiveHigh ? "1" : "0")}", echoComTraffic: _echoComTraffic);
            }
        }
    }
}
