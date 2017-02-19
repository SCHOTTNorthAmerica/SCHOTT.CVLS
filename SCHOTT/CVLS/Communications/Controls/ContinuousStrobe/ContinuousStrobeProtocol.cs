using SCHOTT.CVLS.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCHOTT.CVLS.Communications
{
    /// <summary>
    /// ContinuousStrobeProtocol section of the CVLS Legacy Protocol
    /// </summary>
    public class ContinuousStrobeProtocol
    {
        private readonly ILegacyProtocol _port;
        private readonly bool _echoComTraffic;
        private readonly ContinuousStrobeChannelProtocol[] _ledChannels = new ContinuousStrobeChannelProtocol[4];

        /// <summary>
        /// Creates a new ContinuousStrobeProtocol for the given port
        /// </summary>
        /// <param name="port">The port for the ContinuousStrobeProtocol to use.</param>
        /// <param name="echoComTraffic">When True, the protocol object will echo com traffic to the subscribed message functions.</param>
        public ContinuousStrobeProtocol(ILegacyProtocol port, bool echoComTraffic = false)
        {
            _port = port;
            _echoComTraffic = echoComTraffic;
            for (var i = 1; i <= 4; i++)
            {
                _ledChannels[i - 1] = new ContinuousStrobeChannelProtocol(port, (Enums.Channels)i, echoComTraffic: _echoComTraffic);
            }
        }

        /// <summary>
        /// Returns the ContinuousStrobeChannelProtocol for Channel 1
        /// </summary>
        public ContinuousStrobeChannelProtocol Ch1 => _ledChannels[0];

        /// <summary>
        /// Returns the ContinuousStrobeChannelProtocol for Channel 2
        /// </summary>
        public ContinuousStrobeChannelProtocol Ch2 => _ledChannels[1];

        /// <summary>
        /// Returns the ContinuousStrobeChannelProtocol for Channel 3
        /// </summary>
        public ContinuousStrobeChannelProtocol Ch3 => _ledChannels[2];

        /// <summary>
        /// Returns the ContinuousStrobeChannelProtocol for Channel 4
        /// </summary>
        public ContinuousStrobeChannelProtocol Ch4 => _ledChannels[3];

        /// <summary>
        /// Returns the ContinuousStrobeChannelProtocol for the selected channel
        /// </summary>
        /// <param name="selectedChannel">The channel to return.</param>
        /// <returns>selected channel object</returns>
        public ContinuousStrobeChannelProtocol Channel(Enums.Channels selectedChannel)
        {
            return _ledChannels[(int)selectedChannel - 1];
        }

        /// <summary>
        /// Returns a list of ContinuousStrobeChannelProtocol for all 4 channels
        /// </summary>
        /// <returns>List of LedChannelProtocols</returns>
        public List<ContinuousStrobeChannelProtocol> ChannelList()
        {
            return _ledChannels.ToList();
        }
        
        /// <summary>
        /// Get/Set the Continuous Strobe Enable status
        /// </summary>
        public bool Enable
        {
            get
            {
                return _port.SendCommandSingle("&rm?", echoComTraffic: _echoComTraffic).Contains("&rm1");
            }
            set
            {
                _port.SendCommandSingle($"&rm{(value ? "1" : "0")}", echoComTraffic: _echoComTraffic);
            }
        }

        /// <summary>
        /// Get/Set the Continuous Strobe channel mode
        /// </summary>
        public ChannelModes ChannelMode
        {
            get
            {
                return _port.SendCommandSingle("&rb?", echoComTraffic: _echoComTraffic).Contains("&rb1") ? ChannelModes.Single : ChannelModes.Quad;
            }
            set
            {
                _port.SendCommandSingle($"&rb{(value == ChannelModes.Single ? "1" : "0")}", echoComTraffic: _echoComTraffic);
            }
        }

        /// <summary>
        /// Get/Set the Continuous Strobe Frequency (6-20,000)
        /// </summary>
        public int Frequency
        {
            get
            {
                int value;
                return int.TryParse(_port.SendCommandSingle("&rf?", true, echoComTraffic: _echoComTraffic), out value) ? value : -1;
            }
            set
            {
                _port.SendCommandSingle($"&rf{Math.Min(Math.Max(value, 6), 20000)}", echoComTraffic: _echoComTraffic);
            }
        }

    }
}
