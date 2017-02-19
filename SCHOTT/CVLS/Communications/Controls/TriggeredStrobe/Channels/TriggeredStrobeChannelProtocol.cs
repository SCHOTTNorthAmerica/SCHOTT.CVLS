using SCHOTT.CVLS.Enums;
using System;

namespace SCHOTT.CVLS.Communications
{
    /// <summary>
    /// TriggeredStrobeChannelProtocol section of the CVLS Legacy Protocol
    /// </summary>
    public class TriggeredStrobeChannelProtocol
    {
        private readonly ILegacyProtocol _port;
        private readonly bool _echoComTraffic;
        private readonly Enums.Channels _channel;

        /// <summary>
        /// Creates a new TriggeredStrobeChannelProtocol for the given port and channel
        /// </summary>
        /// <param name="port">The port for the TriggeredStrobeChannelProtocol to use.</param>
        /// <param name="selectedChannel">The channel for the TriggeredStrobeChannelProtocol to use.</param>
        /// <param name="echoComTraffic">When True, the protocol object will echo com traffic to the subscribed message functions.</param>
        public TriggeredStrobeChannelProtocol(ILegacyProtocol port, Enums.Channels selectedChannel, bool echoComTraffic = false)
        {
            _port = port;
            _echoComTraffic = echoComTraffic;
            _channel = selectedChannel;
        }

        /// <summary>
        /// Returns the delay (0-1,000,000 microseconds).
        /// </summary>
        public int Delay
        {
            get
            {
                var returnString = _port.SendCommandSingle($"&pd{(int)_channel},?", echoComTraffic: _echoComTraffic);
                var tokens = returnString.Split(',');

                if (tokens.Length == 2)
                    return int.Parse(tokens[1]);

                return 0;
            }
            set
            {
                _port.SendCommandSingle($"&pd{(int)_channel},{Math.Min(Math.Max(value, 0), 1000000)}", echoComTraffic: _echoComTraffic);
            }
        }


        /// <summary>
        /// Returns the delay (25-1,000,000 microseconds).
        /// </summary>
        public int OnTime
        {
            get
            {
                var returnString = _port.SendCommandSingle($"&po{(int)_channel},?", echoComTraffic: _echoComTraffic);
                var tokens = returnString.Split(',');

                if (tokens.Length == 2)
                    return int.Parse(tokens[1]);

                return 0;
            }
            set
            {
                _port.SendCommandSingle($"&po{(int)_channel},{Math.Min(Math.Max(value, 25), 1000000)}", echoComTraffic: _echoComTraffic);
            }
        }
        /// <summary>
        /// Get/Set the edge detection type.
        /// </summary>
        public EdgeTypes EdgeType
        {
            get
            {
                return _port.SendCommandSingle($"&pj{(int)_channel},?", echoComTraffic: _echoComTraffic).Contains($"&pj{(int)_channel},1")
                    ? EdgeTypes.FallingEdge
                    : EdgeTypes.RisingEdge;
            }
            set
            {
                _port.SendCommandSingle($"&pj{(int)_channel},{(value == EdgeTypes.FallingEdge ? "1" : "0")}", echoComTraffic: _echoComTraffic);
            }
        }
    }
}
