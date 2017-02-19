namespace SCHOTT.CVLS.Communications
{
    /// <summary>
    /// FanObject section of the CVLS Legacy Protocol
    /// </summary>
    public class FanObject
    {
        /// <summary>
        /// Gets the Fan ManualOverride status
        /// </summary>
        public bool ManualOverride { get; private set; }

        /// <summary>
        /// Gets the Fan Speed (0-100%, rounded to the nearest 0.1% resolution).
        /// </summary>
        public double Speed { get; private set; }

        /// <summary>
        /// Gets the Target LED Temperature (0-100C, rounded to the nearest 0.1C resolution).
        /// </summary>
        public double TargetTemperature { get; private set; }

        /// <summary>
        /// Fluent Builder class for the FanChannelObject
        /// </summary>
        public class Builder
        {
            private readonly FanObject _obj = new FanObject();

            /// <summary>
            /// Sets the Fan ManualOverride status
            /// </summary>
            public bool ManualOverride
            {
                set { _obj.ManualOverride = value; }
            }

            /// <summary>
            /// Sets the Common Output Power (0-100%, rounded to the nearest 0.1% resolution).
            /// </summary>
            public double Speed
            {
                set { _obj.Speed = value; }
            }

            /// <summary>
            /// Sets the Target LED Temperature (0-100C, rounded to the nearest 0.1C resolution).
            /// </summary>
            public double TargetTemperature
            {
                set { _obj.TargetTemperature = value; }
            }

            /// <summary>
            /// Build the new FanObject
            /// </summary>
            /// <returns></returns>
            public FanObject Build()
            {
                return _obj;
            }
        }
    }
}
