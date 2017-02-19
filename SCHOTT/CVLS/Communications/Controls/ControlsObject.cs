namespace SCHOTT.CVLS.Communications
{
    /// <summary>
    /// ControlsObject section of the CVLS Legacy Protocol
    /// </summary>
    public class ControlsObject
    {
        /// <summary>
        /// Returns the LedObject
        /// </summary>
        public LedObject Led { get; private set; }

        /// <summary>
        /// Returns the ContinuousStrobeObject
        /// </summary>
        public ContinuousStrobeObject ContinuousStrobe { get; private set; }

        /// <summary>
        /// Returns the TriggeredStrobeObject
        /// </summary>
        public TriggeredStrobeObject TriggeredStrobe { get; private set; }

        /// <summary>
        /// Returns the EqualizerObject
        /// </summary>
        public EqualizerObject Equalizer { get; private set; }

        /// <summary>
        /// Returns the FanObject
        /// </summary>
        public FanObject Fan { get; private set; }

        /// <summary>
        /// Fluent Builder class for the LedChannelObject
        /// </summary>
        public class Builder
        {
            private readonly ControlsObject _obj = new ControlsObject();

            /// <summary>
            /// Sets the LedObject
            /// </summary>
            public LedObject Led
            {
                set { _obj.Led = value; }
            }

            /// <summary>
            /// Sets the ContinuousStrobeObject
            /// </summary>
            public ContinuousStrobeObject ContinuousStrobe
            {
                set { _obj.ContinuousStrobe = value; }
            }

            /// <summary>
            /// Sets the TriggeredStrobeObject
            /// </summary>
            public TriggeredStrobeObject TriggeredStrobe
            {
                set { _obj.TriggeredStrobe = value; }
            }

            /// <summary>
            /// Sets the EqualizerObject
            /// </summary>
            public EqualizerObject Equalizer
            {
                set { _obj.Equalizer = value; }
            }

            /// <summary>
            /// Sets the FanObject
            /// </summary>
            public FanObject Fan
            {
                set { _obj.Fan = value; }
            }

            /// <summary>
            /// Build the new LedObject
            /// </summary>
            /// <returns></returns>
            public ControlsObject Build()
            {
                return _obj;
            }
        }
    }
}
