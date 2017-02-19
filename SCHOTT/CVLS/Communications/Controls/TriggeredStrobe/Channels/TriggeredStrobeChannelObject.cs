using SCHOTT.CVLS.Enums;

namespace SCHOTT.CVLS.Communications
{
    /// <summary>
    /// TriggeredStrobeChannelObject section of the CVLS Legacy Protocol
    /// </summary>
    public class TriggeredStrobeChannelObject
    {
        /// <summary>
        /// Returns the delay (0-1,000,000 microseconds).
        /// </summary>
        public int Delay { get; private set; }

        /// <summary>
        /// Returns the On Time (25-1,000,000 microseconds).
        /// </summary>
        public int OnTime { get; private set; }

        /// <summary>
        /// Returns the edge detection type
        /// </summary>
        public EdgeTypes EdgeType { get; private set; }

        /// <summary>
        /// Fluent Builder class for the TriggeredStrobeChannelObject
        /// </summary>
        public class Builder
        {
            private readonly TriggeredStrobeChannelObject _obj = new TriggeredStrobeChannelObject();
            
            /// <summary>
            /// Sets the delay
            /// </summary>
            public int Delay
            {
                set { _obj.Delay = value; }
            }

            /// <summary>
            /// Sets the on time
            /// </summary>
            public int OnTime
            {
                set { _obj.OnTime = value; }
            }

            /// <summary>
            /// Sets the edge detection type
            /// </summary>
            public EdgeTypes EdgeType
            {
                set { _obj.EdgeType = value; }
            }

            /// <summary>
            /// Build the new TriggeredStrobeChannelObject
            /// </summary>
            /// <returns></returns>
            public TriggeredStrobeChannelObject Build()
            {
                return _obj;
            }
        }
    }
}
