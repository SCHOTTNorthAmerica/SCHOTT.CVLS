using SCHOTT.CVLS.Enums;

namespace SCHOTT.CVLS.Communications
{
    /// <summary>
    /// SystemObject section of the CVLS Legacy Protocol
    /// </summary>
    public class SystemObject
    {
        /// <summary>
        /// Gets the System Mode
        /// </summary>
        public SystemMode Mode { get; private set; }

        /// <summary>
        /// Gets the system LightFeedBack reading. This number is NOT Lumens. 
        /// It is an uncalibrated feed back number used by the equalizer and 
        /// is NOT guaranteed to be consistent unit to unit. 
        /// </summary>
        public int LightFeedBack { get; private set; }

        /// <summary>
        /// Gets the System Last Command Source
        /// </summary>
        public CommandSource LastCommandSource { get; private set; }

        /// <summary>
        /// Gets the System User Mode
        /// </summary>
        public UserMode UserMode { get; private set; }

        /// <summary>
        /// Gets the System Knob Control Mode
        /// </summary>
        public KnobControl KnobMode { get; private set; }

        /// <summary>
        /// Gets the TimeObject for the system
        /// </summary>
        public TimeObject Time { get; private set; }

        /// <summary>
        /// Fluent Builder class for the SystemObject
        /// </summary>
        public class Builder
        {
            private readonly SystemObject _obj = new SystemObject();
            
            /// <summary>
            /// Sets the System Mode
            /// </summary>
            public SystemMode SystemMode
            {
                set { _obj.Mode = value; }
            }

            /// <summary>
            /// Sets the system LightFeedBack reading. This number is NOT Lumens. 
            /// It is an uncalibrated feed back number used by the equalizer and 
            /// is NOT guaranteed to be consistent unit to unit. 
            /// </summary>
            public int LightFeedBack
            {
                set { _obj.LightFeedBack = value; }
            }

            /// <summary>
            /// Sets the System Last Command Source
            /// </summary>
            public CommandSource LastCommandSource
            {
                set { _obj.LastCommandSource = value; }
            }

            /// <summary>
            /// Sets the System User Mode
            /// </summary>
            public UserMode UserMode
            {
                set { _obj.UserMode = value; }
            }

            /// <summary>
            /// Sets the System Knob Control Mode
            /// </summary>
            public KnobControl KnobMode
            {
                set { _obj.KnobMode = value; }
            }

            /// <summary>
            /// Sets the TimeObject for the system
            /// </summary>
            public TimeObject Time
            {
                set { _obj.Time = value; }
            }

            /// <summary>
            /// Build the new SystemObject
            /// </summary>
            /// <returns></returns>
            public SystemObject Build()
            {
                return _obj;
            }
        }
    }
}
