namespace SCHOTT.CVLS.Communications
{
    /// <summary>
    /// MemoryObject section of the CVLS Legacy Protocol
    /// </summary>
    public class MemoryObject
    {
        /// <summary>
        /// Gets the CustomerSettings Write Counts
        /// </summary>
        public uint CustomerSettings { get; private set; }

        /// <summary>
        /// Gets the FactorySettings Write Counts
        /// </summary>
        public uint FactorySettings { get; private set; }

        /// <summary>
        /// Gets the Firmware Write Counts
        /// </summary>
        public uint Firmware { get; private set; }

        /// <summary>
        /// Gets the Exceptions Write Counts
        /// </summary>
        public uint Exceptions { get; private set; }

        /// <summary>
        /// Fluent Builder class for the MemoryObject
        /// </summary>
        public class Builder
        {
            private readonly MemoryObject _obj = new MemoryObject();

            /// <summary>
            /// Sets the CustomerSettings Write Counts
            /// </summary>
            public uint CustomerSettings
            {
                set { _obj.CustomerSettings = value; }
            }

            /// <summary>
            /// Sets the FactorySettings Write Counts
            /// </summary>
            public uint FactorySettings
            {
                set { _obj.FactorySettings = value; }
            }

            /// <summary>
            /// Sets the Firmware Write Counts
            /// </summary>
            public uint Firmware
            {
                set { _obj.Firmware = value; }
            }

            /// <summary>
            /// Sets the Exceptions Write Counts
            /// </summary>
            public uint Exceptions
            {
                set { _obj.Exceptions = value; }
            }
            
            /// <summary>
            /// Build the new FanStatusObject
            /// </summary>
            /// <returns></returns>
            public MemoryObject Build()
            {
                return _obj;
            }
        }
    }
}
