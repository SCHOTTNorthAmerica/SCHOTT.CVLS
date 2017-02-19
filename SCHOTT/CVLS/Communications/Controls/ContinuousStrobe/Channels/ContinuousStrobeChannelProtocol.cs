using SCHOTT.CVLS.Enums;
using System;

namespace SCHOTT.CVLS.Communications
{
    /// <summary>
    /// ContinuousStrobeChannelProtocol section of the CVLS Legacy Protocol
    /// </summary>
    public class ContinuousStrobeChannelProtocol
    {
        private readonly ILegacyProtocol _port;
        private readonly bool _echoComTraffic;
        private readonly Enums.Channels _channel;

        /// <summary>
        /// Creates a new ContinuousStrobeChannelProtocol for the given port and channel
        /// </summary>
        /// <param name="port">The port for the ContinuousStrobeChannelProtocol to use.</param>
        /// <param name="selectedChannel">The channel for the ContinuousStrobeChannelProtocol to use.</param>
        /// <param name="echoComTraffic">When True, the protocol object will echo com traffic to the subscribed message functions.</param>
        public ContinuousStrobeChannelProtocol(ILegacyProtocol port, Enums.Channels selectedChannel, bool echoComTraffic = false)
        {
            _port = port;
            _echoComTraffic = echoComTraffic;
            _channel = selectedChannel;
        }

        /// <summary>
        /// Get/Set the duty cycle (0-100%, rounded to the nearest 0.1% resolution).
        /// </summary>
        public double DutyCycle
        {
            get
            {
                var returnString = _port.SendCommandSingle($"&rd{(int)_channel},?", echoComTraffic: _echoComTraffic);
                var tokens = returnString.Split(',');

                if (tokens.Length == 2)
                    return int.Parse(tokens[1]) * 0.1;

                return 0;
            }
            set
            {
                var power = (int)Math.Round(value * 10.0, 0);
                _port.SendCommandSingle($"&rd{(int)_channel},{Math.Min(Math.Max(power, 0), 1000)}", echoComTraffic: _echoComTraffic);
            }
        }

        /// <summary>
        /// Get/Set the phase shift (0-100%, rounded to the nearest 0.1% resolution).
        /// </summary>
        public double PhaseShift
        {
            get
            {
                var returnString = _port.SendCommandSingle($"&rp{(int)_channel},?", echoComTraffic: _echoComTraffic);
                var tokens = returnString.Split(',');

                if (tokens.Length == 2)
                    return int.Parse(tokens[1]) * 0.1;

                return 0;
            }
            set
            {
                var power = (int)Math.Round(value * 10.0, 0);
                _port.SendCommandSingle($"&rp{(int)_channel},{Math.Min(Math.Max(power, 0), 1000)}", echoComTraffic: _echoComTraffic);
            }
        }

        /// <summary>
        /// Get/Set the wave polarity.
        /// </summary>
        public SignalPolarity WavePolarity
        {
            get
            {
                return _port.SendCommandSingle($"&rj{(int)_channel},?", echoComTraffic: _echoComTraffic).Contains($"&rj{(int)_channel},1")
                    ? SignalPolarity.ActiveHigh
                    : SignalPolarity.ActiveLow;
            }
            set
            {
                _port.SendCommandSingle($"&rj{(int)_channel},{(value == SignalPolarity.ActiveHigh ? "1" : "0")}", echoComTraffic: _echoComTraffic);
            }
        }
    }
}
