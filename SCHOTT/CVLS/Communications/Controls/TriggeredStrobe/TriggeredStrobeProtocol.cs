using SCHOTT.CVLS.Enums;
using System.Collections.Generic;
using System.Linq;

namespace SCHOTT.CVLS.Communications
{
    /// <summary>
    /// TriggeredStrobeProtocol section of the CVLS Legacy Protocol
    /// </summary>
    public class TriggeredStrobeProtocol
    {
        private readonly ILegacyProtocol _port;
        private readonly bool _echoComTraffic;
        private readonly TriggeredStrobeChannelProtocol[] _ledChannels = new TriggeredStrobeChannelProtocol[4];

        /// <summary>
        /// Creates a new TriggeredStrobeProtocol for the given port
        /// </summary>
        /// <param name="port">The port for the TriggeredStrobeProtocol to use.</param>
        /// <param name="echoComTraffic">When True, the protocol object will echo com traffic to the subscribed message functions.</param>
        public TriggeredStrobeProtocol(ILegacyProtocol port, bool echoComTraffic = false)
        {
            _port = port;
            _echoComTraffic = echoComTraffic;
            for (var i = 1; i <= 4; i++)
            {
                _ledChannels[i - 1] = new TriggeredStrobeChannelProtocol(port, (Enums.Channels)i, echoComTraffic: _echoComTraffic);
            }
        }

        /// <summary>
        /// Returns the TriggeredStrobeChannelProtocol for Channel 1
        /// </summary>
        public TriggeredStrobeChannelProtocol Ch1 => _ledChannels[0];

        /// <summary>
        /// Returns the TriggeredStrobeChannelProtocol for Channel 2
        /// </summary>
        public TriggeredStrobeChannelProtocol Ch2 => _ledChannels[1];

        /// <summary>
        /// Returns the TriggeredStrobeChannelProtocol for Channel 3
        /// </summary>
        public TriggeredStrobeChannelProtocol Ch3 => _ledChannels[2];

        /// <summary>
        /// Returns the TriggeredStrobeChannelProtocol for Channel 4
        /// </summary>
        public TriggeredStrobeChannelProtocol Ch4 => _ledChannels[3];

        /// <summary>
        /// Returns the TriggeredStrobeChannelProtocol for the selected channel
        /// </summary>
        /// <param name="selectedChannel">The channel to return.</param>
        /// <returns>selected channel object</returns>
        public TriggeredStrobeChannelProtocol Channel(Enums.Channels selectedChannel)
        {
            return _ledChannels[(int)selectedChannel - 1];
        }

        /// <summary>
        /// Returns a list of TriggeredStrobeChannelProtocol for all 4 channels
        /// </summary>
        /// <returns>List of LedChannelProtocols</returns>
        public List<TriggeredStrobeChannelProtocol> ChannelList()
        {
            return _ledChannels.ToList();
        }

        /// <summary>
        /// Get/Set the Triggered Strobe Enable status
        /// </summary>
        public bool Enable
        {
            get
            {
                return _port.SendCommandSingle("&pm?", echoComTraffic: _echoComTraffic).Contains("&pm1");
            }
            set
            {
                _port.SendCommandSingle($"&pm{(value ? "1" : "0")}", echoComTraffic: _echoComTraffic);
            }
        }

        /// <summary>
        /// Get/Set the Triggered Strobe trigger mode
        /// </summary>
        public TriggerModes TriggerMode
        {
            get
            {
                return _port.SendCommandSingle("&pj0,?", echoComTraffic: _echoComTraffic).Contains("&pj0,1") ? TriggerModes.Combined : TriggerModes.Independent;
            }
            set
            {
                _port.SendCommandSingle($"&pj0,{(value == TriggerModes.Combined ? "1" : "0")}", echoComTraffic: _echoComTraffic);
            }
        }

        /// <summary>
        /// Get/Set the Triggered Strobe channel mode
        /// </summary>
        public ChannelModes ChannelMode
        {
            get
            {
                return _port.SendCommandSingle("&pb?", echoComTraffic: _echoComTraffic).Contains("&pb1") ? ChannelModes.Single : ChannelModes.Quad;
            }
            set
            {
                _port.SendCommandSingle($"&pb{(value == ChannelModes.Single ? "1" : "0")}", echoComTraffic: _echoComTraffic);
            }
        }
        
    }
}
