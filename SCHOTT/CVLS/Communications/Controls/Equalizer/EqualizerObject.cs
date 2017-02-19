namespace SCHOTT.CVLS.Communications
{
    /// <summary>
    /// EqualizerObject section of the CVLS Legacy Protocol
    /// </summary>
    public class EqualizerObject
    {
        /// <summary>
        /// Gets the Continuous Strobe Enable status
        /// </summary>
        public bool Enable { get; private set; }

        /// <summary>
        /// Gets the Equalizer start up delay
        /// </summary>
        public int Delay { get; private set; }

        /// <summary>
        /// Gets the Equalizer target (matched by the light feed back)
        /// NOTE: THIS NUMBER IS NOT IN LUMENS AND IS NOT THE SAME UNIT TO UNIT
        /// </summary>
        public int Target { get; private set; }

        /// <summary>
        /// Gets the Equalizer Averaged Light Feed Back (matched by the light feed back)
        /// NOTE: THIS NUMBER IS NOT IN LUMENS AND IS NOT THE SAME UNIT TO UNIT
        /// </summary>
        public int LightOutput { get; private set; }

        /// <summary>
        /// Gets the Equalizer Driver Output
        /// </summary>
        public double PowerOutput { get; private set; }

        /// <summary>
        /// Fluent Builder class for the EqualizerChannelObject
        /// </summary>
        public class Builder
        {
            private readonly EqualizerObject _obj = new EqualizerObject();
            
            /// <summary>
            /// Sets the Continuous Strobe Enable status
            /// </summary>
            public bool Enable
            {
                set { _obj.Enable = value; }
            }

            /// <summary>
            /// Sets the Equalizer start up delay
            /// </summary>
            public int Delay
            {
                set { _obj.Delay = value; }
            }

            /// <summary>
            /// Sets the Equalizer target (matched by the light feed back)
            /// NOTE: THIS NUMBER IS NOT IN LUMENS AND IS NOT THE SAME UNIT TO UNIT
            /// </summary>
            public int Target
            {
                set { _obj.Target = value; }
            }

            /// <summary>
            /// Sets the Equalizer Averaged Light Feed Back (matched by the light feed back)
            /// NOTE: THIS NUMBER IS NOT IN LUMENS AND IS NOT THE SAME UNIT TO UNIT
            /// </summary>
            public int LightOutput
            {
                set { _obj.LightOutput = value; }
            }

            /// <summary>
            /// Sets the Equalizer Driver Output
            /// </summary>
            public double PowerOutput
            {
                set { _obj.PowerOutput = value; }
            }

            /// <summary>
            /// Build the new EqualizerObject
            /// </summary>
            /// <returns></returns>
            public EqualizerObject Build()
            {
                return _obj;
            }
        }
    }
}
