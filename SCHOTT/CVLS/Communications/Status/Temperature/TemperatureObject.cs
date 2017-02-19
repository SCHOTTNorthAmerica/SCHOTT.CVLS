using SCHOTT.CVLS.Enums;

namespace SCHOTT.CVLS.Communications
{
    /// <summary>
    /// TemperatureObject section of the CVLS Legacy Protocol
    /// </summary>
    public class TemperatureObject
    {
        /// <summary>
        /// Gets the temperature at the given location
        /// </summary>
        public double Temperature { get; private set; }

        /// <summary>
        /// Gets the temperature status at the given location
        /// </summary>
        public StatusIndicators Status { get; private set; }

        /// <summary>
        /// Gets the thermistor status at the given location
        /// </summary>
        public StatusIndicators ThermistorStatus { get; private set; }

        /// <summary>
        /// Fluent Builder class for the TemperatureObject
        /// </summary>
        public class Builder
        {
            private readonly TemperatureObject _obj = new TemperatureObject();

            /// <summary>
            /// Sets the temperature at the given location
            /// </summary>
            public double Temperature
            {
                set { _obj.Temperature = value; }
            }

            /// <summary>
            /// Sets the temperature status at the given location
            /// </summary>
            public StatusIndicators Status
            {
                set { _obj.Status = value; }
            }

            /// <summary>
            /// Sets the thermistor status at the given location
            /// </summary>
            public StatusIndicators ThermistorStatus
            {
                set { _obj.ThermistorStatus = value; }
            }

            /// <summary>
            /// Build the new TemperatureObject
            /// </summary>
            /// <returns></returns>
            public TemperatureObject Build()
            {
                return _obj;
            }
        }
    }
}
