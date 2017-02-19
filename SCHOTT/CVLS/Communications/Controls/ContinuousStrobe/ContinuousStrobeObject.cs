using SCHOTT.CVLS.Enums;
using System.Collections.Generic;
using System.Linq;

namespace SCHOTT.CVLS.Communications
{
    /// <summary>
    /// ContinuousStrobeObject section of the CVLS Legacy Protocol
    /// </summary>
    public class ContinuousStrobeObject
    {
        private readonly ContinuousStrobeChannelObject[] _channels = new ContinuousStrobeChannelObject[4];

        /// <summary>
        /// Returns the ContinuousStrobeChannelObject for Channel 1
        /// </summary>
        public ContinuousStrobeChannelObject Ch1 => _channels[0];

        /// <summary>
        /// Returns the ContinuousStrobeChannelObject for Channel 2
        /// </summary>
        public ContinuousStrobeChannelObject Ch2 => _channels[1];

        /// <summary>
        /// Returns the ContinuousStrobeChannelObject for Channel 3
        /// </summary>
        public ContinuousStrobeChannelObject Ch3 => _channels[2];

        /// <summary>
        /// Returns the ContinuousStrobeChannelObject for Channel 4
        /// </summary>
        public ContinuousStrobeChannelObject Ch4 => _channels[3];

        /// <summary>
        /// Returns the ContinuousStrobeChannelObject for the selected channel
        /// </summary>
        /// <param name="selectedChannel">The channel to return.</param>
        /// <returns>selected channel object</returns>
        public ContinuousStrobeChannelObject Channel(Enums.Channels selectedChannel)
        {
            return _channels[(int)selectedChannel - 1];
        }

        /// <summary>
        /// Returns a list of ContinuousStrobeChannelObject for all 4 channels
        /// </summary>
        /// <returns>List of LedChannelObjects</returns>
        public List<ContinuousStrobeChannelObject> ChannelList()
        {
            return _channels.ToList();
        }

        /// <summary>
        /// Get/Set the Continuous Strobe Enable status
        /// </summary>
        public bool Enable { get; private set; }

        /// <summary>
        /// Get/Set the Continuous Strobe channel mode
        /// </summary>
        public ChannelModes ChannelMode { get; private set; }

        /// <summary>
        /// Get/Set the Continuous Strobe Frequency
        /// </summary>
        public int Frequency { get; private set; }

        /// <summary>
        /// Fluent Builder class for the ContinuousStrobeChannelObject
        /// </summary>
        public class Builder
        {
            private readonly ContinuousStrobeObject _obj = new ContinuousStrobeObject();

            /// <summary>
            /// Sets channel 1
            /// </summary>
            public ContinuousStrobeChannelObject Ch1
            {
                set { _obj._channels[0] = value; }
            }

            /// <summary>
            /// Sets channel 2
            /// </summary>
            public ContinuousStrobeChannelObject Ch2
            {
                set { _obj._channels[1] = value; }
            }

            /// <summary>
            /// Sets channel 3
            /// </summary>
            public ContinuousStrobeChannelObject Ch3
            {
                set { _obj._channels[2] = value; }
            }

            /// <summary>
            /// Sets channel 4
            /// </summary>
            public ContinuousStrobeChannelObject Ch4
            {
                set { _obj._channels[3] = value; }
            }

            /// <summary>
            /// Sets the ContinuousStrobeChannelObject for the selected channel
            /// </summary>
            /// <param name="selectedChannel">The channel to assign the ContinuousStrobeChannelObject to.</param>
            /// <param name="channelObject">The ContinuousStrobeChannelObject to assign.</param>
            public void Channel(Enums.Channels selectedChannel, ContinuousStrobeChannelObject channelObject)
            {
                _obj._channels[(int)selectedChannel - 1] = channelObject;
            }

            /// <summary>
            /// Sets the Continuous Strobe Enable status
            /// </summary>
            public bool Enable
            {
                set { _obj.Enable = value; }
            }

            /// <summary>
            /// Sets the Continuous Strobe channel mode
            /// </summary>
            public ChannelModes ChannelMode
            {
                set { _obj.ChannelMode = value; }
            }

            /// <summary>
            /// Sets the Continuous Strobe Frequency
            /// </summary>
            public int Frequency
            {
                set { _obj.Frequency = value; }
            }
            
            /// <summary>
            /// Build the new ContinuousStrobeObject
            /// </summary>
            /// <returns></returns>
            public ContinuousStrobeObject Build()
            {
                return _obj;
            }
        }
    }
}
