using SCHOTT.CVLS.Enums;

namespace SCHOTT.CVLS.Communications
{
    /// <summary>
    /// EqualizerStatusObject section of the CVLS Legacy Protocol
    /// </summary>
    public class EqualizerStatusObject
    {
        /// <summary>
        /// Gets the Equalizer status
        /// </summary>
        public EqualizerStatus Mode { get; private set; }

        /// <summary>
        /// Gets the Equalizer status indicator
        /// </summary>
        public StatusIndicators Status { get; private set; }

        /// <summary>
        /// Fluent Builder class for the EqualizerStatusObject
        /// </summary>
        public class Builder
        {
            private readonly EqualizerStatusObject _obj = new EqualizerStatusObject();

            /// <summary>
            /// Sets the Equalizer status
            /// </summary>
            public EqualizerStatus Mode
            {
                set { _obj.Mode = value; }
            }

            /// <summary>
            /// Sets the Equalizer status indicator
            /// </summary>
            public StatusIndicators Status
            {
                set { _obj.Status = value; }
            }
            
            /// <summary>
            /// Build the new EqualizerStatusObject
            /// </summary>
            /// <returns></returns>
            public EqualizerStatusObject Build()
            {
                return _obj;
            }
        }
    }
}
