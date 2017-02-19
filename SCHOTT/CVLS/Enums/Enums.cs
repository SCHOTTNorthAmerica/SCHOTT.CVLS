namespace SCHOTT.CVLS.Enums
{
    /// <summary>
    /// Defines the various command interfaces of the CVLS
    /// </summary>
    public enum CommandSource
    {
        /// <summary>
        /// The front knob is in control of light output
        /// </summary>
        Front,

        /// <summary>
        /// The rear multiport is in control of light output
        /// </summary>
        Multiport,

        /// <summary>
        /// The digital commands are in control of light output, last command recieved over RS232
        /// </summary>
        Rs232,

        /// <summary>
        /// The digital commands are in control of light output, last command recieved over LegacySocket
        /// </summary>
        LegacySocket,

        /// <summary>
        /// The digital commands are in control of light output, last command recieved over USB
        /// </summary>
        Usb,

        /// <summary>
        /// The digital commands are in control of light output, last command recieved over Web Page
        /// </summary>
        Web,

        /// <summary>
        /// The digital commands are in control of light output, last command recieved over BinarySocket
        /// </summary>
        BinarySocket,

        /// <summary>
        /// A return when there is a com error
        /// </summary>
        ComError
    }

    /// <summary>
    /// Defines the user operation mode. Defaults to Home.
    /// </summary>
    public enum UserMode
    {
        /// <summary>
        /// Home mode limits the output to 75% of max, but uses the minimum fan speed to reduce noise.
        /// </summary>
        Home,

        /// <summary>
        /// Industrial mode allows 100% output, but sets the fan to 50% speed to provide better cooling for the LED. 
        /// </summary>
        Industrial,

        /// <summary>
        /// Heavy Industrial mode allows 100% light output, and uses the maximum fan speed to provide better cooling for the LED. 
        /// </summary>
        HeavyIndustrial,

        /// <summary>
        /// A return when there is a com error
        /// </summary>
        ComError
    }

    /// <summary>
    /// Defines which channel is being controlled by the front knob.
    /// </summary>
    public enum KnobControl
    {
        /// <summary>
        /// Controls the common channel, this is selected by default on single channel units.
        /// </summary>
        Common,

        /// <summary>
        /// Controls channel 1 of the light source in Quad Channel Mode. This is the default setting on RGBW units and controls the red channel.
        /// </summary>
        Ch1,

        /// <summary>
        /// Controls channel 2 of the light source in Quad Channel Mode. This is the green channel in the RGBW units.
        /// </summary>
        Ch2,

        /// <summary>
        /// Controls channel 3 of the light source in Quad Channel Mode. This is the blue channel in the RGBW units.
        /// </summary>
        Ch3,

        /// <summary>
        /// Controls channel 4 of the light source in Quad Channel Mode. This is the white channel in the RGBW units.
        /// </summary>
        Ch4,

        /// <summary>
        /// This puts the unit in demo mode which will cycle all color combinations on RGBW units.
        /// </summary>
        Demo,

        /// <summary>
        /// A return when there is a com error
        /// </summary>
        ComError
    }

    /// <summary>
    /// Status indicator options
    /// </summary>
    public enum StatusIndicators
    {
        /// <summary>
        /// Item is off
        /// </summary>
        Off,

        /// <summary>
        /// Item is in full compliance
        /// </summary>
        Good,

        /// <summary>
        /// Item is nearing an operational limit
        /// </summary>
        Warning,

        /// <summary>
        /// Item has an error, generally will prevent the LED from turning on
        /// </summary>
        Error,

        /// <summary>
        /// Item is just for information purposes
        /// </summary>
        Info,

        /// <summary>
        /// A return when there is a com error
        /// </summary>
        ComError
    }

    /// <summary>
    /// Status of the equalizer
    /// </summary>
    public enum EqualizerStatus
    {
        /// <summary>
        /// Equalizer is not being able to control the light output. If the LED is turned off, this is the state that will be reported.
        /// </summary>
        Unstable = 0,

        /// <summary>
        /// Equalizer is maintaining the light output
        /// </summary>
        Locked = 1,

        /// <summary>
        /// Equalizer is in delay state, will begin once the delay time has elapsed
        /// </summary>
        Initializing = 2,

        /// <summary>
        /// The light has fallen below the control range, this may lead to instability
        /// </summary>
        LightLow = 4,

        /// <summary>
        /// The light has risen above the control range, this may lead to instability
        /// </summary>
        LightHigh = 6,

        /// <summary>
        /// The light is to low to maintain stability
        /// </summary>
        OverRange = 8,

        /// <summary>
        /// The light is to high to maintain stability
        /// </summary>
        UnderRange = 10,

        /// <summary>
        /// A return when there is a com error
        /// </summary>
        ComError
    }

    /// <summary>
    /// Defines the operation mode of the unit
    /// </summary>
    public enum SystemMode
    {
        /// <summary>
        /// LED is off
        /// </summary>
        Off,

        /// <summary>
        /// LED is in constant mode. This is the default setting of all units.
        /// </summary>
        Constant,

        /// <summary>
        /// Triggered strobe mode is active, LED will wait for trigger input
        /// </summary>
        TriggeredStrobe,

        /// <summary>
        /// Continuous strobe mode is active
        /// </summary>
        ContinuousStrobe,

        /// <summary>
        /// Equalizer is active
        /// </summary>
        Equalizer,

        /// <summary>
        /// Demo mode is active
        /// </summary>
        Demo,

        /// <summary>
        /// A return when there is a com error
        /// </summary>
        ComError
    }

    /// <summary>
    /// Frequency of GraphPoint updates
    /// </summary>
    public enum PollerFrequencies
    {
#pragma warning disable 1591
        Off,
        Poller5000Hz,
        Poller2500Hz,
        Poller1250Hz,
        Poller1000Hz,
        Poller625Hz,
        Poller500Hz,
        Poller250Hz,
        Poller200Hz,
        Poller125Hz,
        Poller100Hz,
        Poller50Hz,
        Poller40Hz,
        Poller25Hz,
        Poller20Hz,
        Poller10Hz,
        Poller8Hz,
        Poller5Hz,
        Poller4Hz,
        Poller2Hz,
        Poller1Hz
#pragma warning restore 1591
    }

    /// <summary>
    /// UART BaudRates setting
    /// </summary>
    public enum BaudRates
    {
#pragma warning disable 1591
        Baud110,
        Baud300,
        Baud600,
        Baud1200,
        Baud2400,
        Baud4800,
        Baud9600,
        Baud14400,
        Baud19200,
        Baud38400,
        Baud57600,
        Baud115200,
        Baud230400,
        Baud460800,
        Baud921600
#pragma warning restore 1591
    }

    /// <summary>
    /// UART Parity setting
    /// </summary>
    public enum Parity
    {
#pragma warning disable 1591
        None,
        Even,
        Odd
#pragma warning restore 1591
    }

    /// <summary>
    /// UART StopBits setting
    /// </summary>
    public enum StopBits
    {
#pragma warning disable 1591
        Bits1,
        Bits2
#pragma warning restore 1591
    }

    /// <summary>
    /// Channel speicification used in Legacy Functions
    /// </summary>
    public enum Channels
    {
        /// <summary>
        /// Channel 1
        /// </summary>
        Ch1 = 1,

        /// <summary>
        /// Channel 2
        /// </summary>
        Ch2,

        /// <summary>
        /// Channel 3
        /// </summary>
        Ch3,

        /// <summary>
        /// Channel 4
        /// </summary>
        Ch4
    }

    /// <summary>
    /// Defines the behavior of logic signals
    /// </summary>
    public enum SignalPolarity
    {
        /// <summary>
        /// The signal is active when logic low
        /// </summary>
        ActiveLow,

        /// <summary>
        /// The signal is active when logic high
        /// </summary>
        ActiveHigh
    }

    /// <summary>
    /// Defines the behavior of logic signals
    /// </summary>
    public enum EdgeTypes
    {
        /// <summary>
        /// Detect a Falling Edge
        /// </summary>
        FallingEdge,

        /// <summary>
        /// Detect a Rising Edge
        /// </summary>
        RisingEdge,
    }

    /// <summary>
    /// Defines how the driver will control the installed LED
    /// </summary>
    public enum ChannelModes
    {
        /// <summary>
        /// LED is a quad channel device.
        /// </summary>
        Quad,

        /// <summary>
        /// LED is a single channel device.
        /// </summary>
        Single
    }

    /// <summary>
    /// Defines modes for LED Trigger Pins
    /// </summary>
    public enum TriggerModes
    {
        /// <summary>
        /// Each channel input is independent
        /// </summary>
        Independent,

        /// <summary>
        /// Channel 1 input is used to trigger all channels
        /// </summary>
        Combined
    }

}
