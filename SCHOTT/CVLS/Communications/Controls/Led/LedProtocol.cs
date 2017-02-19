using SCHOTT.CVLS.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCHOTT.CVLS.Communications
{
    /// <summary>
    /// LedProtocol section of the CVLS Legacy Protocol
    /// </summary>
    public class LedProtocol
    {
        private readonly ILegacyProtocol _port;
        private readonly bool _echoComTraffic;
        private readonly LedChannelProtocol[] _ledChannels = new LedChannelProtocol[4];

        /// <summary>
        /// Creates a new LedProtocol for the given port
        /// </summary>
        /// <param name="port">The port for the LedProtocol to use.</param>
        /// <param name="echoComTraffic">When True, the protocol object will echo com traffic to the subscribed message functions.</param>
        public LedProtocol(ILegacyProtocol port, bool echoComTraffic = false)
        {
            _port = port;
            _echoComTraffic = echoComTraffic;
            for (var i = 1; i <= 4; i++)
            {
                _ledChannels[i - 1] = new LedChannelProtocol(port, (Enums.Channels)i, echoComTraffic);
            }
        }

        /// <summary>
        /// Returns the LedChannelProtocol for Channel 1
        /// </summary>
        public LedChannelProtocol Ch1 => _ledChannels[0];

        /// <summary>
        /// Returns the LedChannelProtocol for Channel 2
        /// </summary>
        public LedChannelProtocol Ch2 => _ledChannels[1];

        /// <summary>
        /// Returns the LedChannelProtocol for Channel 3
        /// </summary>
        public LedChannelProtocol Ch3 => _ledChannels[2];

        /// <summary>
        /// Returns the LedChannelProtocol for Channel 4
        /// </summary>
        public LedChannelProtocol Ch4 => _ledChannels[3];

        /// <summary>
        /// Returns the LedChannelProtocol for the selected channel
        /// </summary>
        /// <param name="selectedChannel">The channel to return.</param>
        /// <returns>selected channel object</returns>
        public LedChannelProtocol Channel(Enums.Channels selectedChannel)
        {
            return _ledChannels[(int)selectedChannel - 1];
        }

        /// <summary>
        /// Returns a list of LedChannelProtocol for all 4 channels
        /// </summary>
        /// <returns>List of LedChannelProtocols</returns>
        public List<LedChannelProtocol> ChannelList()
        {
            return _ledChannels.ToList();
        }

        /// <summary>
        /// Get/Set the demo mode enabled status
        /// </summary>
        public bool DemoMode
        {
            get
            {
                return _port.SendCommandSingle("&d?", echoComTraffic: _echoComTraffic).Contains("&d1");
            }
            set
            {
                _port.SendCommandSingle($"&d{(value ? "1" : "0")}", echoComTraffic: _echoComTraffic);
            }
        }

        /// <summary>
        /// Get/Set the trigger functionality of the standard mode
        /// </summary>
        public TriggerModes TriggerMode
        {
            get
            {
                return _port.SendCommandSingle("&j0,?", echoComTraffic: _echoComTraffic).Contains("&j0,1") ? TriggerModes.Combined : TriggerModes.Independent;
            }
            set
            {
                _port.SendCommandSingle($"&j0,{(int)value}", echoComTraffic: _echoComTraffic);
            }
        }

        /// <summary>
        /// Get/Set the knob control mode of the unit
        /// </summary>
        public KnobControl KnobMode
        {
            get
            {
                return (KnobControl)int.Parse(_port.SendCommandSingle("&n?", echoComTraffic: _echoComTraffic).Substring(2));
            }
            set
            {
                _port.SendCommandSingle($"&n{(int)value}", echoComTraffic: _echoComTraffic);
            }
        }

        /// <summary>
        /// Get/Set the channel mode of the standard mode
        /// </summary>
        public ChannelModes ChannelMode
        {
            get
            {
                return _port.SendCommandSingle("&b?", echoComTraffic: _echoComTraffic).Contains("&b1") ? ChannelModes.Single : ChannelModes.Quad;
            }
            set
            {
                _port.SendCommandSingle($"&b{(value == ChannelModes.Single ? "1" : "0")}", echoComTraffic: _echoComTraffic);
            }
        }

        /// <summary>
        /// Get/Set the Common Output Enable status
        /// </summary>
        public bool Enable
        {
            get
            {
                return _port.SendCommandSingle("&l0,?", echoComTraffic: _echoComTraffic).Contains("&l0,1");
            }
            set
            {
                _port.SendCommandSingle($"&l0,{(value ? "1" : "0")}", echoComTraffic: _echoComTraffic);
            }
        }

        /// <summary>
        /// Get/Set the Common Output Power (0-100%, rounded to the nearest 0.1% resolution).
        /// </summary>
        public double Power
        {
            get
            {
                var returnString = _port.SendCommandSingle("&i0,?", echoComTraffic: _echoComTraffic);
                var tokens = returnString.Split(',');

                if (tokens.Length == 2)
                    return int.Parse(tokens[1]) * 0.1;

                return 0;
            }
            set
            {
                var power = (int)Math.Round(value * 10.0, 0);
                _port.SendCommandSingle($"&i0,{Math.Min(Math.Max(power, 0), 1000)}", echoComTraffic: _echoComTraffic);
            }
        }

    }
}
