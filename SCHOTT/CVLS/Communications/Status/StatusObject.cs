namespace SCHOTT.CVLS.Communications
{
    /// <summary>
    /// Core status object for the CVLS
    /// </summary>
    public class StatusObject
    {
        /// <summary>
        /// Gets the TemperatureObject for the Led location
        /// </summary>
        public TemperatureObject TemperatureLed { get; private set; }

        /// <summary>
        /// Gets the TemperatureObject for the Board location
        /// </summary>
        public TemperatureObject TemperatureBoard { get; private set; }

        /// <summary>
        /// Gets the VoltageObject for the system input voltage
        /// </summary>
        public VoltageObject VoltageInput { get; private set; }

        /// <summary>
        /// Gets the VoltageObject for the system 5V reference output
        /// </summary>
        public VoltageObject VoltageRefOut { get; private set; }

        /// <summary>
        /// Gets the FanStatusObject for the system
        /// </summary>
        public FanStatusObject Fan { get; private set; }

        /// <summary>
        /// Gets the EqualizerStatusObject for the system
        /// </summary>
        public EqualizerStatusObject Equalizer { get; private set; }

        /// <summary>
        /// Gets the SystemObject for the system
        /// </summary>
        public SystemObject System { get; private set; }

        /// <summary>
        /// Gets the MemoryObject for the system
        /// </summary>
        public MemoryObject Memory { get; private set; }

        /// <summary>
        /// Gets the IdentificationObject for the system
        /// </summary>
        public IdentificationObject Identification { get; private set; }

        /// <summary>
        /// Fluent Builder class for the TriggeredStrobeChannelObject
        /// </summary>
        public class Builder
        {
            private readonly StatusObject _obj = new StatusObject();

            /// <summary>
            /// Sets the TemperatureObject for the Led location
            /// </summary>
            public TemperatureObject TemperatureLed
            {
                set { _obj.TemperatureLed = value; }
            }

            /// <summary>
            /// Sets the TemperatureObject for the Board location
            /// </summary>
            public TemperatureObject TemperatureBoard
            {
                set { _obj.TemperatureBoard = value; }
            }

            /// <summary>
            /// Sets the VoltageObject for the system input voltage
            /// </summary>
            public VoltageObject VoltageInput
            {
                set { _obj.VoltageInput = value; }
            }

            /// <summary>
            /// Sets the VoltageObject for the system 5V reference output
            /// </summary>
            public VoltageObject VoltageRefOut
            {
                set { _obj.VoltageRefOut = value; }
            }

            /// <summary>
            /// Gets the FanStatusObject for the system
            /// </summary>
            public FanStatusObject Fan
            {
                set { _obj.Fan = value; }
            }

            /// <summary>
            /// Sets the EqualizerObject for the system
            /// </summary>
            public EqualizerStatusObject Equalizer
            {
                set { _obj.Equalizer = value; }
            }

            /// <summary>
            /// Sets the SystemObject for the system
            /// </summary>
            public SystemObject System
            {
                set { _obj.System = value; }
            }

            /// <summary>
            /// Sets the MemoryObject for the system
            /// </summary>
            public MemoryObject Memory
            {
                set { _obj.Memory = value; }
            }

            /// <summary>
            /// Gets the IdentificationObject for the system
            /// </summary>
            public IdentificationObject Identification
            {
                set { _obj.Identification = value; }
            }

            /// <summary>
            /// Build the new StatusObject
            /// </summary>
            /// <returns></returns>
            public StatusObject Build()
            {
                return _obj;
            }
        }
    }
}
