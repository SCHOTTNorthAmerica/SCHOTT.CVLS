using SCHOTT.CVLS.Enums;

namespace SCHOTT.CVLS.Communications
{
    /// <summary>
    /// ControlsProtocol section of the CVLS Legacy Protocol
    /// </summary>
    public class ControlsProtocol
    {
        private readonly ILegacyProtocol _port;
        private readonly bool _echoComTraffic;

        /// <summary>
        /// Returns the LedProtocol
        /// </summary>
        public LedProtocol Led { get; }

        /// <summary>
        /// Returns the ContinuousStrobeProtocol
        /// </summary>
        public ContinuousStrobeProtocol ContinuousStrobe { get; }

        /// <summary>
        /// Returns the TriggeredStrobeProtocol
        /// </summary>
        public TriggeredStrobeProtocol TriggeredStrobe { get; }

        /// <summary>
        /// Returns the EqualizerProtocol
        /// </summary>
        public EqualizerProtocol Equalizer { get; }

        /// <summary>
        /// Returns the FanProtocol
        /// </summary>
        public FanProtocol Fan { get; }

        /// <summary>
        /// Creates a new ControlsProtocol for the given port
        /// </summary>
        /// <param name="port">The port for the ControlsProtocol to use.</param>
        /// <param name="echoComTraffic">When True, the protocol object will echo com traffic to the subscribed message functions.</param>
        public ControlsProtocol(ILegacyProtocol port, bool echoComTraffic = false)
        {
            _port = port;
            _echoComTraffic = echoComTraffic;
            Led = new LedProtocol(port, echoComTraffic);
            ContinuousStrobe = new ContinuousStrobeProtocol(port, echoComTraffic);
            TriggeredStrobe = new TriggeredStrobeProtocol(port, echoComTraffic);
            Equalizer = new EqualizerProtocol(port, echoComTraffic);
            Fan = new FanProtocol(port, echoComTraffic);
        }

        /// <summary>
        /// Gets a ControlsObject that represents the current status of all CVLS Controls
        /// </summary>
        /// <returns>The current ControlsObject</returns>
        public ControlsObject GetAll()
        {
            var i = 0;
            string line;
            if (!_port.SendCommandSingleTest("&*c", "&*c", out line, true))
                return null;

            var tokens = line.Split(',');

            if (tokens.Length != 56)
                return null;

            var controlsBuilder = new ControlsObject.Builder();

            #region LED Settings

            var ledBuilder = new LedObject.Builder
            {
                DemoMode = tokens[i++] == "1",
                TriggerMode = tokens[i++] == "1" ? TriggerModes.Combined : TriggerModes.Independent,
                KnobMode = (KnobControl)int.Parse(tokens[i++]),
                ChannelMode = tokens[i++] == "1" ? ChannelModes.Single : ChannelModes.Quad,
                Enable = tokens[i++] == "1",
                Power = int.Parse(tokens[i++]) * 0.1
            };

            for (var j = 1; j <= 4; j++)
            {
                ledBuilder.Channel((Enums.Channels)j, new LedChannelObject.Builder
                {
                    Enabled = tokens[i++] == "1",
                    Power = int.Parse(tokens[i++]) * 0.1,
                    ShutdownPolarity = tokens[i++] == "1" ? SignalPolarity.ActiveHigh : SignalPolarity.ActiveLow
                }.Build());
            }

            controlsBuilder.Led = ledBuilder.Build();

            #endregion

            #region Continuous Strobe

            var continuousStrobeBuilder = new ContinuousStrobeObject.Builder
            {
                Enable = tokens[i++] == "1",
                ChannelMode = tokens[i++] == "1" ? ChannelModes.Single : ChannelModes.Quad,
                Frequency = int.Parse(tokens[i++])
            };

            for (var j = 1; j <= 4; j++)
            {
                continuousStrobeBuilder.Channel((Enums.Channels)j, new ContinuousStrobeChannelObject.Builder
                {
                    DutyCycle = int.Parse(tokens[i++]) * 0.1,
                    PhaseShift = int.Parse(tokens[i++]) * 0.1,
                    WavePolarity = tokens[i++] == "1" ? SignalPolarity.ActiveHigh : SignalPolarity.ActiveLow
                }.Build());
            }

            controlsBuilder.ContinuousStrobe = continuousStrobeBuilder.Build();

            #endregion

            #region Triggered Strobe

            var triggeredStrobeBuilder = new TriggeredStrobeObject.Builder
            {
                Enable = tokens[i++] == "1",
                TriggerMode = tokens[i++] == "1" ? TriggerModes.Combined : TriggerModes.Independent,
                ChannelMode = tokens[i++] == "1" ? ChannelModes.Single : ChannelModes.Quad
            };

            for (var j = 1; j <= 4; j++)
            {
                triggeredStrobeBuilder.Channel((Enums.Channels)j, new TriggeredStrobeChannelObject.Builder
                {
                    Delay = int.Parse(tokens[i++]),
                    OnTime = int.Parse(tokens[i++]),
                    EdgeType = tokens[i++] == "1" ? EdgeTypes.FallingEdge : EdgeTypes.RisingEdge
                }.Build());
            }

            controlsBuilder.TriggeredStrobe = triggeredStrobeBuilder.Build();

            #endregion

            #region Equalizer Settings

            controlsBuilder.Equalizer = new EqualizerObject.Builder
            {
                Enable = tokens[i++] == "1",
                Delay = int.Parse(tokens[i++]),
                Target = int.Parse(tokens[i++]),
                LightOutput = int.Parse(tokens[i++]),
                PowerOutput = double.Parse(tokens[i++])
            }.Build();
            
            #endregion

            #region Fan Settings

            controlsBuilder.Fan = new FanObject.Builder
            {
                ManualOverride = tokens[i++] == "1",
                Speed = int.Parse(tokens[i]) * 0.1
            }.Build();

            #endregion

            return controlsBuilder.Build();
        }

    }
}
