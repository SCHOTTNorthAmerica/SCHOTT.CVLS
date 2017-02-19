using SCHOTT.CVLS.Enums;

namespace SCHOTT.CVLS.Communications
{
    /// <summary>
    /// VoltageObject section of the CVLS Legacy Protocol
    /// </summary>
    public class VoltageObject
    {
        /// <summary>
        /// Gets the temperature at the given location
        /// </summary>
        public double Voltage { get; private set; }

        /// <summary>
        /// Gets the temperature status at the given location
        /// </summary>
        public StatusIndicators Status { get; private set; }

        /// <summary>
        /// Fluent Builder class for the VoltageObject
        /// </summary>
        public class Builder
        {
            private readonly VoltageObject _obj = new VoltageObject();

            /// <summary>
            /// Sets the temperature at the given location
            /// </summary>
            public double Voltage
            {
                set { _obj.Voltage = value; }
            }

            /// <summary>
            /// Sets the temperature status at the given location
            /// </summary>
            public StatusIndicators Status
            {
                set { _obj.Status = value; }
            }

            /// <summary>
            /// Build the new VoltageObject
            /// </summary>
            /// <returns></returns>
            public VoltageObject Build()
            {
                return _obj;
            }
        }
    }
}
