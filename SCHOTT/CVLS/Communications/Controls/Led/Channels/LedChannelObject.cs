using SCHOTT.CVLS.Enums;

namespace SCHOTT.CVLS.Communications
{
    /// <summary>
    /// LedChannelObject section of the CVLS Legacy Protocol
    /// </summary>
    public class LedChannelObject
    {
        /// <summary>
        /// Returns the enable status
        /// </summary>
        public bool Enabled { get; private set; }

        /// <summary>
        /// Returns the power (0-100%, rounded to the nearest 0.1% resolution).
        /// </summary>
        public double Power { get; private set; }

        /// <summary>
        /// Returns the shutdown polarity
        /// </summary>
        public SignalPolarity ShutdownPolarity { get; private set; }

        /// <summary>
        /// Fluent Builder class for the LedChannelObject
        /// </summary>
        public class Builder
        {
            private readonly LedChannelObject _obj = new LedChannelObject();

            /// <summary>
            /// Sets the enable status
            /// </summary>
            public bool Enabled
            {
                set { _obj.Enabled = value; }
            }

            /// <summary>
            /// Sets the power (0-100%, rounded to the nearest 0.1% resolution).
            /// </summary>
            public double Power
            {
                set
                {;
                    _obj.Power = value;
                }
            }

            /// <summary>
            /// Sets the shutdown polarity
            /// </summary>
            public SignalPolarity ShutdownPolarity
            {
                set { _obj.ShutdownPolarity = value; }
            }

            /// <summary>
            /// Build the new LedChannelObject
            /// </summary>
            /// <returns></returns>
            public LedChannelObject Build()
            {
                return _obj;
            }
        }
    }
}
