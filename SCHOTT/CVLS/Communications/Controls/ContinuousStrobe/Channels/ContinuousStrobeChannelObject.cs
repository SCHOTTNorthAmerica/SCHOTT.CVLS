using SCHOTT.CVLS.Enums;

namespace SCHOTT.CVLS.Communications
{
    /// <summary>
    /// ContinuousStrobeChannelObject section of the CVLS Legacy Protocol
    /// </summary>
    public class ContinuousStrobeChannelObject
    {
        /// <summary>
        /// Returns the duty cycle (0-100%, rounded to the nearest 0.1% resolution).
        /// </summary>
        public double DutyCycle { get; private set; }

        /// <summary>
        /// Returns the phase shift (0-100%, rounded to the nearest 0.1% resolution).
        /// </summary>
        public double PhaseShift { get; private set; }

        /// <summary>
        /// Returns the wave polarity
        /// </summary>
        public SignalPolarity WavePolarity { get; private set; }

        /// <summary>
        /// Fluent Builder class for the ContinuousStrobeChannelObject
        /// </summary>
        public class Builder
        {
            private readonly ContinuousStrobeChannelObject _obj = new ContinuousStrobeChannelObject();

            /// <summary>
            /// Sets the duty cycle
            /// </summary>
            public double DutyCycle
            {
                set { _obj.DutyCycle = value; }
            }

            /// <summary>
            /// Sets the phase shift
            /// </summary>
            public double PhaseShift
            {
                set { _obj.PhaseShift = value; }
            }
            
            /// <summary>
            /// Sets the wave polarity
            /// </summary>
            public SignalPolarity WavePolarity
            {
                set { _obj.WavePolarity = value; }
            }

            /// <summary>
            /// Build the new ContinuousStrobeChannelObject
            /// </summary>
            /// <returns></returns>
            public ContinuousStrobeChannelObject Build()
            {
                return _obj;
            }
        }
    }
}
