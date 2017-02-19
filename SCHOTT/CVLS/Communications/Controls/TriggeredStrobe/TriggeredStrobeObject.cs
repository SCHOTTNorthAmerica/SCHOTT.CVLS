using SCHOTT.CVLS.Enums;
using System.Collections.Generic;
using System.Linq;

namespace SCHOTT.CVLS.Communications
{
    /// <summary>
    /// TriggeredStrobeObject section of the CVLS Legacy Protocol
    /// </summary>
    public class TriggeredStrobeObject
    {
        private readonly TriggeredStrobeChannelObject[] _channels = new TriggeredStrobeChannelObject[4];

        /// <summary>
        /// Returns the TriggeredStrobeChannelObject for Channel 1
        /// </summary>
        public TriggeredStrobeChannelObject Ch1 => _channels[0];

        /// <summary>
        /// Returns the TriggeredStrobeChannelObject for Channel 2
        /// </summary>
        public TriggeredStrobeChannelObject Ch2 => _channels[1];

        /// <summary>
        /// Returns the TriggeredStrobeChannelObject for Channel 3
        /// </summary>
        public TriggeredStrobeChannelObject Ch3 => _channels[2];

        /// <summary>
        /// Returns the TriggeredStrobeChannelObject for Channel 4
        /// </summary>
        public TriggeredStrobeChannelObject Ch4 => _channels[3];

        /// <summary>
        /// Returns the TriggeredStrobeChannelObject for the selected channel
        /// </summary>
        /// <param name="selectedChannel">The channel to return.</param>
        /// <returns>selected channel object</returns>
        public TriggeredStrobeChannelObject Channel(Enums.Channels selectedChannel)
        {
            return _channels[(int)selectedChannel - 1];
        }

        /// <summary>
        /// Returns a list of TriggeredStrobeChannelObject for all 4 channels
        /// </summary>
        /// <returns>List of LedChannelObjects</returns>
        public List<TriggeredStrobeChannelObject> ChannelList()
        {
            return _channels.ToList();
        }

        /// <summary>
        /// Get/Set the Triggered Strobe Enable status
        /// </summary>
        public bool Enable { get; private set; }

        /// <summary>
        /// Get/Set the Triggered Strobe trigger mode
        /// </summary>
        public TriggerModes TriggerMode { get; private set; }

        /// <summary>
        /// Get/Set the Triggered Strobe channel mode
        /// </summary>
        public ChannelModes ChannelMode { get; private set; }
        
        /// <summary>
        /// Fluent Builder class for the TriggeredStrobeChannelObject
        /// </summary>
        public class Builder
        {
            private readonly TriggeredStrobeObject _obj = new TriggeredStrobeObject();

            /// <summary>
            /// Sets channel 1
            /// </summary>
            public TriggeredStrobeChannelObject Ch1
            {
                set { _obj._channels[0] = value; }
            }

            /// <summary>
            /// Sets channel 2
            /// </summary>
            public TriggeredStrobeChannelObject Ch2
            {
                set { _obj._channels[1] = value; }
            }

            /// <summary>
            /// Sets channel 3
            /// </summary>
            public TriggeredStrobeChannelObject Ch3
            {
                set { _obj._channels[2] = value; }
            }

            /// <summary>
            /// Sets channel 4
            /// </summary>
            public TriggeredStrobeChannelObject Ch4
            {
                set { _obj._channels[3] = value; }
            }

            /// <summary>
            /// Sets the TriggeredStrobeChannelObject for the selected channel
            /// </summary>
            /// <param name="selectedChannel">The channel to assign the TriggeredStrobeChannelObject to.</param>
            /// <param name="channelObject">The TriggeredStrobeChannelObject to assign.</param>
            public void Channel(Enums.Channels selectedChannel, TriggeredStrobeChannelObject channelObject)
            {
                _obj._channels[(int)selectedChannel - 1] = channelObject;
            }

            /// <summary>
            /// Sets the Triggered Strobe Enable status
            /// </summary>
            public bool Enable
            {
                set { _obj.Enable = value; }
            }

            /// <summary>
            /// Sets the Triggered Strobe trigger mode
            /// </summary>
            public TriggerModes TriggerMode
            {
                set { _obj.TriggerMode = value; }
            }

            /// <summary>
            /// Sets the Triggered Strobe channel mode
            /// </summary>
            public ChannelModes ChannelMode
            {
                set { _obj.ChannelMode = value; }
            }

            /// <summary>
            /// Build the new TriggeredStrobeObject
            /// </summary>
            /// <returns></returns>
            public TriggeredStrobeObject Build()
            {
                return _obj;
            }
        }
    }
}
