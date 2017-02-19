using SCHOTT.CVLS.Enums;

namespace SCHOTT.CVLS.Communications
{
    /// <summary>
    /// FanStatusObject section of the CVLS Legacy Protocol
    /// </summary>
    public class FanStatusObject
    {
        /// <summary>
        /// Gets the Fan Speed in RPM
        /// </summary>
        public int Speed { get; private set; }

        /// <summary>
        /// Gets the fan status
        /// </summary>
        public StatusIndicators Status { get; private set; }

        /// <summary>
        /// Fluent Builder class for the FanStatusObject
        /// </summary>
        public class Builder
        {
            private readonly FanStatusObject _obj = new FanStatusObject();

            /// <summary>
            /// Sets the Fan Speed in RPM
            /// </summary>
            public int Speed
            {
                set { _obj.Speed = value; }
            }

            /// <summary>
            /// Sets the fan status
            /// </summary>
            public StatusIndicators Status
            {
                set { _obj.Status = value; }
            }

            /// <summary>
            /// Build the new FanStatusObject
            /// </summary>
            /// <returns></returns>
            public FanStatusObject Build()
            {
                return _obj;
            }
        }
    }
}
