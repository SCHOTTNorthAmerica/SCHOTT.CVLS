using SCHOTT.CVLS.Enums;
using System.Collections.Generic;
using System.Linq;

namespace SCHOTT.CVLS.Communications
{
    /// <summary>
    /// LedObject section of the CVLS Legacy Protocol
    /// </summary>
    public class LedObject
    {
        private readonly LedChannelObject[] _ledChannels = new LedChannelObject[4];

        /// <summary>
        /// Returns the LedChannelObject for Channel 1
        /// </summary>
        public LedChannelObject Ch1 => _ledChannels[0];

        /// <summary>
        /// Returns the LedChannelObject for Channel 2
        /// </summary>
        public LedChannelObject Ch2 => _ledChannels[1];

        /// <summary>
        /// Returns the LedChannelObject for Channel 3
        /// </summary>
        public LedChannelObject Ch3 => _ledChannels[2];

        /// <summary>
        /// Returns the LedChannelObject for Channel 4
        /// </summary>
        public LedChannelObject Ch4 => _ledChannels[3];

        /// <summary>
        /// Returns the LedChannelObject for the selected channel
        /// </summary>
        /// <param name="selectedChannel">The channel to return.</param>
        /// <returns>selected channel object</returns>
        public LedChannelObject Channel(Enums.Channels selectedChannel)
        {
            return _ledChannels[(int)selectedChannel - 1];
        }

        /// <summary>
        /// Returns a list of LedChannelObject for all 4 channels
        /// </summary>
        /// <returns>List of LedChannelObjects</returns>
        public List<LedChannelObject> ChannelList()
        {
            return _ledChannels.ToList();
        }

        /// <summary>
        /// Returns the demo mode enabled status
        /// </summary>
        public bool DemoMode { get; private set; }

        /// <summary>
        /// Returns the trigger functionality of the standard mode
        /// </summary>
        public TriggerModes TriggerMode { get; private set; }

        /// <summary>
        /// Returns the knob control mode of the unit
        /// </summary>
        public KnobControl KnobMode { get; private set; }

        /// <summary>
        /// Returns the channel mode of the standard mode
        /// </summary>
        public ChannelModes ChannelMode { get; private set; }

        /// <summary>
        /// Returns the Common Output Enable status
        /// </summary>
        public bool Enable { get; private set; }

        /// <summary>
        /// Returns the Common Output Power
        /// </summary>
        public double Power { get; private set; }

        /// <summary>
        /// Fluent Builder class for the LedChannelObject
        /// </summary>
        public class Builder
        {
            private readonly LedObject _obj = new LedObject();

            /// <summary>
            /// Sets channel 1
            /// </summary>
            public LedChannelObject Ch1
            {
                set { _obj._ledChannels[0] = value; }
            }

            /// <summary>
            /// Sets channel 2
            /// </summary>
            public LedChannelObject Ch2
            {
                set { _obj._ledChannels[1] = value; }
            }

            /// <summary>
            /// Sets channel 3
            /// </summary>
            public LedChannelObject Ch3
            {
                set { _obj._ledChannels[2] = value; }
            }

            /// <summary>
            /// Sets channel 4
            /// </summary>
            public LedChannelObject Ch4
            {
                set { _obj._ledChannels[3] = value; }
            }

            /// <summary>
            /// Sets the LedChannelObject for the selected channel
            /// </summary>
            /// <param name="selectedChannel">The channel to assign the LedChannelObject to.</param>
            /// <param name="channelObject">The LedChannelObject to assign.</param>
            public void Channel(Enums.Channels selectedChannel, LedChannelObject channelObject)
            {
                _obj._ledChannels[(int)selectedChannel - 1] = channelObject;
            }

            /// <summary>
            /// Sets the demo mode enabled status
            /// </summary>
            public bool DemoMode
            {
                set { _obj.DemoMode = value; }
            }

            /// <summary>
            /// Sets the trigger functionality of the standard mode
            /// </summary>
            public TriggerModes TriggerMode
            {
                set { _obj.TriggerMode = value; }
            }

            /// <summary>
            /// Sets the knob control mode of the unit
            /// </summary>
            public KnobControl KnobMode
            {
                set { _obj.KnobMode = value; }
            }

            /// <summary>
            /// Sets the channel mode of the standard mode
            /// </summary>
            public ChannelModes ChannelMode
            {
                set { _obj.ChannelMode = value; }
            }

            /// <summary>
            /// Sets the Common Output Enable status
            /// </summary>
            public bool Enable
            {
                set { _obj.Enable = value; }
            }

            /// <summary>
            /// Sets the Common Output Power
            /// </summary>
            public double Power
            {
                set
                {
                    _obj.Power = value;
                }
            }

            /// <summary>
            /// Build the new LedObject
            /// </summary>
            /// <returns></returns>
            public LedObject Build()
            {
                return _obj;
            }
        }
    }
}
