using SCHOTT.CVLS.Enums;

namespace SCHOTT.CVLS.Communications
{
    /// <summary>
    /// StatusProtocol section of the CVLS Legacy Protocol
    /// </summary>
    public class StatusProtocol
    {
        private readonly ILegacyProtocol _port;
        private readonly bool _echoComTraffic;

        /// <summary>
        /// Gets the LED TemperatureProtocol
        /// </summary>
        public TemperatureProtocol TemperatureLed { get; }

        /// <summary>
        /// Gets the Board TemperatureProtocol
        /// </summary>
        public TemperatureProtocol TemperatureBoard { get; }

        /// <summary>
        /// Gets the VoltageProtocol for the system input voltage
        /// </summary>
        public VoltageProtocol VoltageInput { get; }

        /// <summary>
        /// Gets the VoltageProtocol for the system 5V reference output
        /// </summary>
        public VoltageProtocol VoltageRefOut { get; }

        /// <summary>
        /// Gets the FanStatusProtocol for the system
        /// </summary>
        public FanStatusProtocol Fan { get; }

        /// <summary>
        /// Gets the EqualizerProtocol for the system
        /// </summary>
        public EqualizerStatusProtocol Equalizer { get; }

        /// <summary>
        /// Gets the SystemProtocol for the system
        /// </summary>
        public SystemProtocol System { get; }

        /// <summary>
        /// Gets the MemoryProtocol for the system
        /// </summary>
        public MemoryProtocol Memory { get; }

        /// <summary>
        /// Gets the IdentificationProtocol for the system
        /// </summary>
        public IdentificationProtocol Identification { get; }

        /// <summary>
        /// Creates a new ControlsProtocol for the given port
        /// </summary>
        /// <param name="port">The port for the ControlsProtocol to use.</param>
        /// <param name="echoComTraffic">When True, the protocol object will echo com traffic to the subscribed message functions.</param>
        public StatusProtocol(ILegacyProtocol port, bool echoComTraffic = false)
        {
            _port = port;
            _echoComTraffic = echoComTraffic;

            TemperatureLed = new TemperatureProtocol(port, TemperatureLocations.Led, echoComTraffic);
            TemperatureBoard = new TemperatureProtocol(port, TemperatureLocations.Board, echoComTraffic);
            VoltageInput = new VoltageProtocol(port, VoltageLocations.Input, echoComTraffic);
            VoltageRefOut = new VoltageProtocol(port, VoltageLocations.Reference5V, echoComTraffic);
            Fan = new FanStatusProtocol(port, echoComTraffic);
            Equalizer = new EqualizerStatusProtocol(port, echoComTraffic);
            System = new SystemProtocol(port, echoComTraffic);
            Memory = new MemoryProtocol(port, echoComTraffic);
            Identification = new IdentificationProtocol(port, echoComTraffic);
        }

        /// <summary>
        /// Gets a StatusObject that represents the current status of the CVLS Unit
        /// </summary>
        /// <returns>The current StatusObject</returns>
        public StatusObject GetAll()
        {
            var statusObjectBuilder = new StatusObject.Builder();

            var i = 0;
            string line;
            if (!_port.SendCommandSingleTest("&*s", "&*s", out line, true))
                return null;

            var tokens = line.Split(',');

            if (tokens.Length != 25)
                return null;

            try
            {
                #region Parse Data Stream

                statusObjectBuilder.TemperatureLed = new TemperatureObject.Builder
                {
                    Temperature = double.Parse(tokens[i++].Split(' ')[0]),
                    Status = (StatusIndicators)int.Parse(tokens[i++]),
                    ThermistorStatus = (StatusIndicators)int.Parse(tokens[i++])
                }.Build();

                statusObjectBuilder.TemperatureBoard = new TemperatureObject.Builder
                {
                    Temperature = double.Parse(tokens[i++].Split(' ')[0]),
                    Status = (StatusIndicators)int.Parse(tokens[i++]),
                    ThermistorStatus = (StatusIndicators)int.Parse(tokens[i++])
                }.Build();

                statusObjectBuilder.VoltageRefOut = new VoltageObject.Builder
                {
                    Voltage = double.Parse(tokens[i++].Split(' ')[0]),
                    Status = (StatusIndicators)int.Parse(tokens[i++])
                }.Build();

                statusObjectBuilder.VoltageInput = new VoltageObject.Builder
                {
                    Voltage = double.Parse(tokens[i++].Split(' ')[0]),
                    Status = (StatusIndicators)int.Parse(tokens[i++])
                }.Build();

                statusObjectBuilder.Fan = new FanStatusObject.Builder
                {
                    Speed = int.Parse(tokens[i++].Split(' ')[0]),
                    Status = (StatusIndicators)int.Parse(tokens[i++])
                }.Build();

                var systemObjectBuilder = new SystemObject.Builder
                {
                    SystemMode = (SystemMode)int.Parse(tokens[i++])
                };

                statusObjectBuilder.Equalizer = new EqualizerStatusObject.Builder
                {
                    Mode = (EqualizerStatus)int.Parse(tokens[i++]),
                    Status = (StatusIndicators)int.Parse(tokens[i++])
                }.Build();

                systemObjectBuilder.LightFeedBack = int.Parse(tokens[i++]);
                systemObjectBuilder.LastCommandSource = (CommandSource)int.Parse(tokens[i++]);
                systemObjectBuilder.UserMode = (UserMode)int.Parse(tokens[i++]);
                systemObjectBuilder.KnobMode = (KnobControl)int.Parse(tokens[i++]);
                systemObjectBuilder.Time = new TimeObject(uint.Parse(tokens[i++]));
                statusObjectBuilder.System = systemObjectBuilder.Build();

                statusObjectBuilder.Memory = new MemoryObject.Builder
                {
                    CustomerSettings = uint.Parse(tokens[i++]),
                    FactorySettings = uint.Parse(tokens[i++]),
                    Firmware = uint.Parse(tokens[i++]),
                    Exceptions = uint.Parse(tokens[i++])
                }.Build();

                statusObjectBuilder.Identification = new IdentificationObject.Builder
                {
                    SerialFull = tokens[i],
                    Serial = uint.Parse(tokens[i].Split(':')[1]),
                    Model = tokens[i].Split(':')[0]
                }.Build();

                #endregion
            }
            catch
            {
                return null;
            }

            return statusObjectBuilder.Build();
        }

    }
}
