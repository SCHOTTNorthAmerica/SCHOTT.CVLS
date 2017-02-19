namespace SCHOTT.CVLS.Communications
{
    /// <summary>
    /// IdentificationObject section of the CVLS Legacy Protocol
    /// </summary>
    public class IdentificationObject
    {
        /// <summary>
        /// Gets the Model:Serial number of the unit
        /// </summary>
        public string SerialFull { get; private set; }

        /// <summary>
        /// Gets the Serial number of the unit
        /// </summary>
        public uint Serial { get; private set; }

        /// <summary>
        /// Gets the model of the unit
        /// </summary>
        public string Model { get; private set; }

        /// <summary>
        /// Gets the firmware version of the unit
        /// </summary>
        public string Firmware { get; private set; }

        /// <summary>
        /// Gets the product name of the unit
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Fluent Builder class for the IdentificationObject
        /// </summary>
        public class Builder
        {
            private readonly IdentificationObject _obj = new IdentificationObject();

            /// <summary>
            /// Sets the Model:Serial number of the unit
            /// </summary>
            public string SerialFull
            {
                set { _obj.SerialFull = value; }
            }

            /// <summary>
            /// Sets the Serial number of the unit
            /// </summary>
            public uint Serial
            {
                set { _obj.Serial = value; }
            }

            /// <summary>
            /// Sets the model of the unit
            /// </summary>
            public string Model
            {
                set { _obj.Model = value; }
            }

            /// <summary>
            /// Sets the firmware version of the unit
            /// </summary>
            public string Firmware
            {
                set { _obj.Firmware = value; }
            }

            /// <summary>
            /// Sets the product name of the unit
            /// </summary>
            public string Name
            {
                set { _obj.Name = value; }
            }

            /// <summary>
            /// Build the new FanStatusObject
            /// </summary>
            /// <returns></returns>
            public IdentificationObject Build()
            {
                return _obj;
            }
        }
    }
}
