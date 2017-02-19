using SCHOTT.Core.Communication.Ethernet;
using SCHOTT.Core.Extensions;
using SCHOTT.Core.Threading;
using SCHOTT.Core.Utilities;
using SCHOTT.CVLS.Communications;
using SCHOTT.CVLS.Enums;
using SCHOTT.CVLS.Ethernet.Binary.Enums;
using SCHOTT.CVLS.Ethernet.Binary.Workers;
using SCHOTT.CVLS.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace SCHOTT.CVLS.Ethernet.Binary
{
    /// <summary>
    /// The BinarySocket implementation of the SCHOTT AsyncSocket, using the IThreadInterface. This class 
    /// allows the user to communicate with the BinarySocket of the CVLS units.
    /// </summary>
    public class BinarySocket : AsyncSocket, IThreadInterface
    {
        /// <summary>
        /// Enum definition of LoginPermissions for a given level
        /// </summary>
        public enum LoginPermissions
        {
            /// <summary>
            /// User is not logged in with sufficent privileges 
            /// </summary>
            Disabled,

            /// <summary>
            /// User logged in as a higher level, or login is not required
            /// </summary>
            Allowed,

            /// <summary>
            /// User is logged in as this requirement level
            /// </summary>
            LoggedIn
        }
        
        /// <summary>
        /// Class to update user on current login levels.
        /// </summary>
        public class LoginUpdateArgs
        {
            /// <summary>
            /// Admin Permissions of the current user. Defines if Admin level controls are accessable.
            /// </summary>
            public LoginPermissions Admin { get; private set; }

            /// <summary>
            /// User Permissions of the current user. Defines if User level controls are accessable.
            /// </summary>
            public LoginPermissions User { get; private set; }

            /// <summary>
            /// Create a new LoginUpdateArgs object.
            /// </summary>
            /// <param name="admin"></param>
            /// <param name="user"></param>
            public LoginUpdateArgs(LoginPermissions admin, LoginPermissions user)
            {
                Admin = admin;
                User = user;
            }

            /// <summary>
            /// Function to test for equality between two ConnectionUpdateArgs objects.
            /// </summary>
            /// <param name="newArgs">ConnectionUpdateArgs object to compare against.</param>
            /// <returns>True = Objects are equal, False = Objects differ</returns>
            public bool IsEqual(LoginUpdateArgs newArgs)
            {
                return Admin == newArgs.Admin && User == newArgs.User;
            }

            /// <summary>
            /// Function to set one object based on the parameters of another.
            /// </summary>
            /// <param name="newArgs">The object to copy.</param>
            public void SetFrom(LoginUpdateArgs newArgs)
            {
                Admin = newArgs.Admin;
                User = newArgs.User;
            }
        }

        /// <summary>
        /// Register to recieve updates on login level
        /// </summary>
        /// <param name="context">The context in which to run the update action.</param>
        /// <param name="action">The lambda expression to execute on updates.</param>
        public void RegisterLoginUpdate(MessageBroker.MessageContext context, Action<LoginUpdateArgs> action)
        {
            MessageBroker.Register("LoginUpdate", context, action);
        }

        private readonly LoginUpdateArgs _lastLoginUpdateArgs = new LoginUpdateArgs(LoginPermissions.Disabled, LoginPermissions.Disabled);
        private void RunLoginUpdate(LoginUpdateArgs args)
        {
            if (_lastLoginUpdateArgs.IsEqual(args))
                return;

            _lastLoginUpdateArgs.SetFrom(args);
            MessageBroker.RunActions("LoginUpdate", args);
        }

        /// <summary>
        /// Register to recieve updates to the controls interface.
        /// </summary>
        /// <param name="context">The context in which to run the update action.</param>
        /// <param name="action">The lambda expression to execute on updates.</param>
        public void RegisterControlsUpdate(MessageBroker.MessageContext context, Action<ControlsObject> action)
        {
            MessageBroker.Register("ControlsUpdate", context, action);
        }

        private void RunControlsUpdate(ControlsObject args)
        {
            MessageBroker.RunActions("ControlsUpdate", args);
        }

        /// <summary>
        /// Register to recieve updates to the status interface.
        /// </summary>
        /// <param name="context">The context in which to run the update action.</param>
        /// <param name="action">The lambda expression to execute on updates.</param>
        public void RegisterStatusUpdate(MessageBroker.MessageContext context, Action<StatusObject> action)
        {
            MessageBroker.Register("StatusUpdate", context, action);
        }

        private void RunStatusUpdate(StatusObject args)
        {
            MessageBroker.RunActions("StatusUpdate", args);
        }

        /// <summary>
        /// Register to recieve messages from the socket.
        /// </summary>
        /// <param name="context">The context in which to run the update action.</param>
        /// <param name="action">The lambda expression to execute on updates.</param>
        public void RegisterSocketMessage(MessageBroker.MessageContext context, Action<string> action)
        {
            MessageBroker.Register("SocketMessage", context, action);
        }

        private void RunSocketMessage(string message)
        {
            MessageBroker.RunActions("SocketMessage", message);
        }

        /// <summary>
        /// A CVLS datapoint for the user to consume
        /// </summary>
        public class DataPoint
        {
            /// <summary>
            /// The index count from the CVLS unit. Counts through 5000 indexes per second.
            /// </summary>
            public double CounterIndex { get; set; }

            /// <summary>
            /// The value to report.
            /// </summary>
            public double Value { get; set; }
        }

        /// <summary>
        /// Options for the user to receive data points from
        /// </summary>
        public enum DataPointOptions
        {
            /// <summary>
            /// How full the BinarySocket buffer is.
            /// </summary>
            BufferFullness = 2,

            /// <summary>
            /// The status of the fault output
            /// </summary>
            FaultOutput,

            /// <summary>
            /// The temperature of the LED
            /// </summary>
            LedTemperature,

            /// <summary>
            /// The temperature of the Driver PCB
            /// </summary>
            BoardTemperature,

            /// <summary>
            /// The speed of the cooling fan (in RPM)
            /// </summary>
            FanSpeedRpm,

            /// <summary>
            /// The light feed back value of the system. This number is not a calibrated 
            /// to lumens and can vary unit to unit for the same lumens output.
            /// </summary>
            LightFeedBack,

            /// <summary>
            /// The averaged light feed back value that the equalizer uses.
            /// </summary>
            EqualizerLightFeedBack,

            /// <summary>
            /// The output controlled by the equalizer
            /// </summary>
            EqualizerOutputPower,

            /// <summary>
            /// The status of the equalizer
            /// </summary>
            EqualizerStatus,

            /// <summary>
            /// The voltage being output on the 5V Ref pin of the multiport connector
            /// </summary>
            VoltageMonitor5V,

            /// <summary>
            /// The input voltage to the system.
            /// </summary>
            VoltageMonitor24V,

            /// <summary>
            /// The position of the front knob
            /// </summary>
            FrontKnob,

            /// <summary>
            /// The state of the front switch
            /// </summary>
            FrontSwitch,

            /// <summary>
            /// The value of the analog channel 1 input on the multiport connector
            /// </summary>
            MultiportAnalogCh1,

            /// <summary>
            /// The value of the analog channel 2 input on the multiport connector
            /// </summary>
            MultiportAnalogCh2,

            /// <summary>
            /// The value of the analog channel 3 input on the multiport connector
            /// </summary>
            MultiportAnalogCh3,

            /// <summary>
            /// The value of the analog channel 4 input on the multiport connector
            /// </summary>
            MultiportAnalogCh4,

            /// <summary>
            /// The value of the digital channel 1 input on the multiport connector
            /// </summary>
            MultiportDigitalCh1,

            /// <summary>
            /// The value of the digital channel 2 input on the multiport connector
            /// </summary>
            MultiportDigitalCh2,

            /// <summary>
            /// The value of the digital channel 3 input on the multiport connector
            /// </summary>
            MultiportDigitalCh3,

            /// <summary>
            /// The value of the digital channel 4 input on the multiport connector
            /// </summary>
            MultiportDigitalCh4
        }

        /// <summary>
        /// Register to recieve updates to the graph interfaces.
        /// </summary>
        /// <param name="dataPointOptions">Which graph point to subscribe too.</param>
        /// <param name="context">The context in which to run the update action.</param>
        /// <param name="action">The lambda expression to execute on updates.</param>
        public void RegisterDataPointUpdate(DataPointOptions dataPointOptions, MessageBroker.MessageContext context, Action<DataPoint> action)
        {
            MessageBroker.Register(dataPointOptions.ToString(), context, action);
        }

        private void RunDataPointUpdate(ushort command, DataPoint args)
        {
            MessageBroker.RunActions(((DataPointOptions)command).ToString(), args);
        }

        /// <summary>
        /// Class to update the user on an unknown packet
        /// </summary>
        public class UnknownPacketArgs
        {
            /// <summary>
            /// The unknown packet received
            /// </summary>
            public List<byte> Packet { get; private set; }

            /// <summary>
            /// Create a new UnknownPacketArgs object.
            /// </summary>
            /// <param name="packet">The data of the unknown packet</param>
            public UnknownPacketArgs(List<byte> packet)
            {
                Packet = packet.CloneList();
            }
        }

        /// <summary>
        /// Register to receive contents of unknown packets.
        /// </summary>
        /// <param name="context">The context in which to run the update action.</param>
        /// <param name="action">The lambda expression to execute on updates.</param>
        public void RegisterUnknownCommand(MessageBroker.MessageContext context, Action<UnknownPacketArgs> action)
        {
            MessageBroker.Register("UnknownCommand", context, action);
        }

        private void RunUnknownCommand(UnknownPacketArgs args)
        {
            MessageBroker.RunActions("UnknownCommand", args);
        }
        
        /// <summary>
        /// Class to allow the user to upload INI files to the connected CVLS unit.
        /// </summary>
        public IniUploader IniUploader;

        /// <summary>
        /// Class to allow the user to download INI files from the connected CVLS unit.
        /// </summary>
        public IniDownloader IniDownloader;

        /// <summary>
        /// Class to allow the user to download Log files from the connected CVLS unit.
        /// </summary>
        public LogDownloader LogDownloader;

        /// <summary>
        /// Class to allow the user to upload firmware to the connected CVLS unit.
        /// </summary>
        public FirmwareUploader FirmwareUploader;

        private readonly string _threadName;
        private int _applicationClosingCount;
        private bool _waitingOnFirmware;
        private string _firmwareString = "";
        private SettingsObject _settingsObject;
        private bool _waitingOnSettings;

        private readonly List<byte> _socketInFifo = new List<byte>();

        private readonly Timer _keepAliveTimer;
        private ushort _keepAliveMissedCount;

        /// <summary>
        /// Create a new BinarySocket 
        /// </summary>
        /// <param name="threadName">The name to report in closing operations for this binary socket.</param>
        /// <param name="closingWorker">The closing worker to add this binary socket too.</param>
        public BinarySocket(string threadName, ClosingWorker closingWorker)
        {
            closingWorker?.AddThread(this);

            _threadName = threadName;

            IniUploader = new IniUploader(this, "INI Uploader", null);
            IniDownloader = new IniDownloader(this, "INI Downloader", null);
            LogDownloader = new LogDownloader(this, "Log Downloader", null);
            FirmwareUploader = new FirmwareUploader(this, "Customer Firmware Uploader", null);

            _keepAliveTimer = new Timer();
            _keepAliveTimer.Elapsed += KeepAliveTimer_Elapsed;
            _keepAliveTimer.Interval = 100;
        }

        #region Events

        private void KeepAliveTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // send keep alive command
            SendBinaryCommand(CommandSets.System, (ushort)SystemCommands.SystemKeepalive, true);

            _keepAliveMissedCount++;
            if (_keepAliveMissedCount >= 10)
            {
                Disconnect();
            }
        }

        #endregion
        
        #region Internal Functions

        private enum Encoding
        {
            Start = 0x12,
            Stop = 0x13,
            Esc = 0x7D
        }

        // adds Encoding.START, STOP, and Encoding.ESC characters to the dataToEncode byte array
        private static void CommandEncode(ref List<byte> command)
        {
            // esc data for message
            for (var i = 0; i < command.Count; i++)
            {
                if (command[i] != (byte) Encoding.Start && command[i] != (byte) Encoding.Esc &&
                    command[i] != (byte) Encoding.Stop)
                    continue;

                command.Insert(i++, (byte) Encoding.Esc);
                command[i] ^= 0x80;
            }

            // add start byte
            command.Insert(0, (byte) Encoding.Start);

            // add stop byte
            command.Add((byte) Encoding.Stop);
        }

        private void KeepAlivePulse()
        {
            _keepAliveMissedCount = 0;
        }

        private void ClearFifOtoPointer(ref int workingPointer)
        {
            _socketInFifo.RemoveRange(0, workingPointer);
            workingPointer = 0;
        }

        private void CommandDecoderWorker()
        {
            var command = new List<byte>();
            var isEscapedChar = false;
            var workingPointer = 0;
            var waitingOnStart = true;

            if (_socketInFifo.Count == 0)
                return;

            while (workingPointer != _socketInFifo.Count)
            {
                var workingbyte = _socketInFifo[workingPointer++];

                // remove any extra bits while waiting for a start byte
                if (waitingOnStart && workingbyte != (byte) Encoding.Start)
                {
                    ClearFifOtoPointer(ref workingPointer);
                    continue;
                }

                switch (workingbyte)
                {
                    case (byte) Encoding.Start:
                        command.Clear();
                        waitingOnStart = false;
                        break;

                    case (byte) Encoding.Esc:
                        isEscapedChar = true;
                        break;

                    case (byte) Encoding.Stop:
                        // check to make sure that command is correct length
                        if (command.Count >= 6)
                        {
                            var instruction = new BinaryCommand(command.GetRange(0, 4));
                            if (instruction.DataLength + 6 == command.Count)
                            {
                                // compute the checksum
                                var checksum = Checksums.Fletcher16(command.ToArray(), instruction.DataLength + 4);

                                // validate checksum
                                if ((command[command.Count - 2] == checksum[0]) &&
                                    (command[command.Count - 1] == checksum[1]))
                                {
                                    CommandParse(command);
                                    ClearFifOtoPointer(ref workingPointer);
                                    waitingOnStart = true;
                                }
                            }
                        }
                        break;

                    default:
                        if (isEscapedChar)
                        {
                            isEscapedChar = false;
                            command.Add((byte) (workingbyte ^ 0x80));
                        }
                        else
                        {
                            command.Add(workingbyte);
                        }
                        break;
                }
            }
        }

        private void CommandParse(List<byte> command)
        {
            var instruction = new BinaryCommand(command.GetRange(0, 4));

            switch (instruction.CommandType)
            {
                #region SystemCommands

                case (byte) CommandSets.System:

                    switch (instruction.CommandSet)
                    {
                        case CommandSets.System:
                            int i;
                            switch ((SystemCommands) instruction.Command)
                            {
                                #region None Functions

                                case SystemCommands.SystemReportStatus:
                                    const int count = 52;
                                    if (instruction.DataLength < count ||
                                        instruction.DataLength != count + command[count + 3])
                                        return;

                                    ParseLoginLevel(command.GetRange(4, 2));

                                    var statusBuilder = new StatusObject.Builder();
                                    i = 6;

                                    #region Status Label Updates

                                    statusBuilder.TemperatureLed = new TemperatureObject.Builder
                                    {
                                        Temperature = DataConversions.ConvertListToUint16(command.GetRange(i, 2)) * 0.01 - 40.0,
                                        Status = (StatusIndicators)command[i + 2],
                                        ThermistorStatus = (StatusIndicators)command[i + 3]
                                    }.Build();
                                    i += 4;
                                    
                                    statusBuilder.TemperatureBoard = new TemperatureObject.Builder
                                    {
                                        Temperature = DataConversions.ConvertListToUint16(command.GetRange(i, 2)) * 0.01 - 40.0,
                                        Status = (StatusIndicators)command[i + 2],
                                        ThermistorStatus = (StatusIndicators)command[i + 3]
                                    }.Build();
                                    i += 4;

                                    statusBuilder.VoltageRefOut = new VoltageObject.Builder
                                    {
                                        Voltage = DataConversions.ConvertListToUint16(command.GetRange(i, 2)) * 0.01,
                                        Status = (StatusIndicators)command[i + 2]
                                    }.Build();
                                    i += 3;

                                    statusBuilder.VoltageInput = new VoltageObject.Builder
                                    {
                                        Voltage = DataConversions.ConvertListToUint16(command.GetRange(i, 2)) * 0.01,
                                        Status = (StatusIndicators)command[i + 2]
                                    }.Build();
                                    i += 3;

                                    statusBuilder.Fan = new FanStatusObject.Builder
                                    {
                                        Speed = DataConversions.ConvertListToUint16(command.GetRange(i, 2)),
                                        Status = (StatusIndicators)command[i + 2]
                                    }.Build();
                                    i += 3;

                                    var systemBuilder = new SystemObject.Builder
                                    {
                                        SystemMode = (SystemMode)command[i++]
                                    };

                                    statusBuilder.Equalizer = new EqualizerStatusObject.Builder
                                    {
                                        Mode = (EqualizerStatus)command[i++],
                                        Status = (StatusIndicators)command[i++]
                                    }.Build();

                                    systemBuilder.LightFeedBack = DataConversions.ConvertListToUint16(command.GetRange(i, 2));
                                    i += 2;

                                    systemBuilder.LastCommandSource = (CommandSource)command[i++];
                                    systemBuilder.UserMode = (UserMode)command[i++];
                                    systemBuilder.KnobMode = (KnobControl)command[i++];
                                    systemBuilder.Time = new TimeObject(DataConversions.ConvertListToUint32(command.GetRange(i, 4)));
                                    statusBuilder.System = systemBuilder.Build();
                                    i += 4;

                                    statusBuilder.Memory = new MemoryObject.Builder
                                    {
                                        CustomerSettings = DataConversions.ConvertListToUint32(command.GetRange(i, 4)),
                                        FactorySettings = DataConversions.ConvertListToUint32(command.GetRange(i + 4, 4)),
                                        Firmware = DataConversions.ConvertListToUint32(command.GetRange(i + 8, 4)),
                                        Exceptions = DataConversions.ConvertListToUint32(command.GetRange(i + 12, 4))
                                    }.Build();
                                    i += 16;

                                    var serial = DataConversions.ConvertListToUint32(command.GetRange(i, 4));
                                    i += 4;

                                    var model =
                                        System.Text.Encoding.UTF8.GetString(
                                            command.GetRange(i + 1, command[i]).ToArray());

                                    statusBuilder.Identification = new IdentificationObject.Builder
                                    {
                                        Serial = serial,
                                        Model = model,
                                        SerialFull = $"{model}:{serial:000000}"
                                    }.Build();

                                    #endregion

                                    RunStatusUpdate(statusBuilder.Build());
                                    break;

                                case SystemCommands.SystemReportControls:
                                    if (instruction.DataLength != 100)
                                        return;

                                    var controlsBuilder = new ControlsObject.Builder();
                                    i = 4;

                                    #region LED Settings

                                    var ledBuilder = new LedObject.Builder
                                    {
                                        DemoMode = command[i++] != 0,
                                        TriggerMode = command[i++] != 0 ? TriggerModes.Combined : TriggerModes.Independent,
                                        KnobMode = (KnobControl)command[i++],
                                        ChannelMode = command[i++] != 0 ? ChannelModes.Single : ChannelModes.Quad,
                                        Enable = command[i++] != 0
                                    };

                                    ledBuilder.Power = DataConversions.ConvertListToUint16(command.GetRange(i, 2)) * 0.1;
                                    i += 2;

                                    for (var j = 1; j <= 4; j++)
                                    {
                                        var continuousStrobeChannelBuilder = new LedChannelObject.Builder();

                                        continuousStrobeChannelBuilder.Enabled = command[i++] != 0;
                                        continuousStrobeChannelBuilder.Power = DataConversions.ConvertListToUint16(command.GetRange(i, 2)) * 0.1;
                                        i += 2;
                                        continuousStrobeChannelBuilder.ShutdownPolarity = command[i++] != 0
                                            ? SignalPolarity.ActiveHigh
                                            : SignalPolarity.ActiveLow;

                                        ledBuilder.Channel((Channels) j, continuousStrobeChannelBuilder.Build());
                                    }

                                    controlsBuilder.Led = ledBuilder.Build();

                                    #endregion

                                    #region Continuous Strobe

                                    var continuousStrobeBuilder = new ContinuousStrobeObject.Builder
                                    {
                                        Enable = command[i++] != 0,
                                        ChannelMode = (ChannelModes)command[i++]
                                    };

                                    continuousStrobeBuilder.Frequency = DataConversions.ConvertListToUint16(command.GetRange(i, 2));
                                    i += 2;

                                    for (var j = 1; j <= 4; j++)
                                    {
                                        var continuousStrobeChannelBuilder = new ContinuousStrobeChannelObject.Builder();

                                        continuousStrobeChannelBuilder.DutyCycle = DataConversions.ConvertListToUint16(command.GetRange(i, 2)) * 0.1;
                                        i += 2;
                                        continuousStrobeChannelBuilder.PhaseShift = DataConversions.ConvertListToUint16(command.GetRange(i, 2)) * 0.1;
                                        i += 2;
                                        continuousStrobeChannelBuilder.WavePolarity = command[i++] != 0 ? SignalPolarity.ActiveHigh : SignalPolarity.ActiveLow;

                                        continuousStrobeBuilder.Channel((Channels)j, continuousStrobeChannelBuilder.Build());
                                    }

                                    controlsBuilder.ContinuousStrobe = continuousStrobeBuilder.Build();

                                    #endregion

                                    #region Triggered Strobe

                                    var triggeredStrobeBuilder = new TriggeredStrobeObject.Builder
                                    {
                                        Enable = command[i++] != 0,
                                        TriggerMode = command[i++] != 0 ? TriggerModes.Combined : TriggerModes.Independent,
                                        ChannelMode = command[i++] != 0 ? ChannelModes.Single : ChannelModes.Quad
                                    };

                                    for (var j = 1; j <= 4; j++)
                                    {
                                        var triggeredStrobeChannelBuilder = new TriggeredStrobeChannelObject.Builder();

                                        triggeredStrobeChannelBuilder.Delay = (int)DataConversions.ConvertListToUint32(command.GetRange(i, 4));
                                        i += 4;
                                        triggeredStrobeChannelBuilder.OnTime = (int)DataConversions.ConvertListToUint32(command.GetRange(i, 4));
                                        i += 4;
                                        triggeredStrobeChannelBuilder.EdgeType = command[i++] != 0
                                            ? EdgeTypes.FallingEdge
                                            : EdgeTypes.RisingEdge;

                                        triggeredStrobeBuilder.Channel((Channels)j, triggeredStrobeChannelBuilder.Build());
                                    }

                                    controlsBuilder.TriggeredStrobe = triggeredStrobeBuilder.Build();
                                    
                                    #endregion

                                    #region Equalizer Settings

                                    var equalizerBuilder = new EqualizerObject.Builder();

                                    equalizerBuilder.Enable = command[i++] != 0;
                                    equalizerBuilder.Delay = DataConversions.ConvertListToUint16(command.GetRange(i, 2));
                                    i += 2;
                                    equalizerBuilder.Target = DataConversions.ConvertListToUint16(command.GetRange(i, 2));
                                    i += 2;
                                    equalizerBuilder.LightOutput = DataConversions.ConvertListToUint16(command.GetRange(i, 2));
                                    i += 2;
                                    equalizerBuilder.PowerOutput = DataConversions.ConvertListToUint16(command.GetRange(i, 2)) * 0.1;
                                    i += 2;

                                    controlsBuilder.Equalizer = equalizerBuilder.Build();

                                    #endregion

                                    #region Fan Settings

                                    var fanBuilder = new FanObject.Builder();

                                    fanBuilder.ManualOverride = command[i++] != 0;
                                    fanBuilder.Speed = DataConversions.ConvertListToUint16(command.GetRange(i, 2)) * 0.1;
                                    i += 2;
                                    fanBuilder.TargetTemperature = DataConversions.ConvertListToUint16(command.GetRange(i, 2)) * 0.1;

                                    controlsBuilder.Fan = fanBuilder.Build();

                                    #endregion

                                    RunControlsUpdate(controlsBuilder.Build());
                                    break;

                                case SystemCommands.SystemReportBuffer:
                                case SystemCommands.SystemReportFault:
                                case SystemCommands.SystemReportEqualizerStatus:
                                case SystemCommands.SystemReportFrontSwitch:
                                case SystemCommands.SystemReportMultiportDigitalCh1:
                                case SystemCommands.SystemReportMultiportDigitalCh2:
                                case SystemCommands.SystemReportMultiportDigitalCh3:
                                case SystemCommands.SystemReportMultiportDigitalCh4:
                                    if (instruction.DataLength != 3)
                                        return;

                                    RunDataPointUpdate(instruction.Command, new DataPoint
                                    {
                                        CounterIndex = DataConversions.ConvertListToUint16(command.GetRange(4, 2)),
                                        Value = command[6]
                                    });
                                    break;

                                case SystemCommands.SystemReportLedTemp:
                                case SystemCommands.SystemReportBoardTemp:
                                    if (instruction.DataLength != 4)
                                        return;

                                    RunDataPointUpdate(instruction.Command, new DataPoint
                                    {
                                        CounterIndex = DataConversions.ConvertListToUint16(command.GetRange(4, 2)),
                                        Value = DataConversions.ConvertListToUint16(command.GetRange(6, 2)) * 0.01 - 40.0
                                    });
                                    break;

                                case SystemCommands.SystemReportFanSpeed:
                                    if (instruction.DataLength != 4)
                                        return;

                                    RunDataPointUpdate(instruction.Command, new DataPoint
                                    {
                                        CounterIndex = DataConversions.ConvertListToUint16(command.GetRange(4, 2)),
                                        Value = DataConversions.ConvertListToUint16(command.GetRange(6, 2)) * 0.001
                                    });
                                    break;

                                case SystemCommands.SystemReportLightFeedBack:
                                case SystemCommands.SystemReportEqualizerLightFeedBack:
                                    if (instruction.DataLength != 4)
                                        return;

                                    RunDataPointUpdate(instruction.Command, new DataPoint
                                    {
                                        CounterIndex = DataConversions.ConvertListToUint16(command.GetRange(4, 2)),
                                        Value = DataConversions.ConvertListToUint16(command.GetRange(6, 2))
                                    });
                                    break;

                                case SystemCommands.SystemReportEqualizerOutputPower:
                                case SystemCommands.SystemReport5VMonitor:
                                case SystemCommands.SystemReport24VMonitor:
                                case SystemCommands.SystemReportFrontKnob:
                                case SystemCommands.SystemReportMultiportAnalogCh1:
                                case SystemCommands.SystemReportMultiportAnalogCh2:
                                case SystemCommands.SystemReportMultiportAnalogCh3:
                                case SystemCommands.SystemReportMultiportAnalogCh4:
                                    if (instruction.DataLength != 4)
                                        return;

                                    RunDataPointUpdate(instruction.Command, new DataPoint
                                    {
                                        CounterIndex = DataConversions.ConvertListToUint16(command.GetRange(4, 2)),
                                        Value = DataConversions.ConvertListToUint16(command.GetRange(6, 2)) * 0.01
                                    });
                                    break;

                                case SystemCommands.SystemDisconnect:
                                    Disconnect();
                                    RunSocketMessage("Client forcefully disconnected!");
                                    break;

                                case SystemCommands.SystemKeepalive:
                                    KeepAlivePulse();
                                    break;

                                case SystemCommands.SystemMessageString:
                                    RunSocketMessage(System.Text.Encoding.UTF8.GetString(
                                                command.GetRange(4, instruction.DataLength).ToArray()));
                                    break;

                                case SystemCommands.SystemLoginRequest:
                                    RunSocketMessage("Please Log In!");
                                    break;

                                case SystemCommands.SystemLoginSuccessful:
                                    //CrossThreadDialogs.MessageBoxNonBlocking(new DialogConfiguration
                                    //{
                                    //    Message = System.Text.Encoding.UTF8.GetString(command.GetRange(4, instruction.DataLength).ToArray()),
                                    //    Title = "Binary Socket Message"
                                    //});
                                    break;

                                case SystemCommands.SystemLoginFailed:
                                    RunSocketMessage("Login Failed! Incorrect Username or Password.");
                                    break;

                                case SystemCommands.SystemFirmwareVersion:
                                    _firmwareString = System.Text.Encoding.UTF8.GetString(
                                        command.GetRange(4, instruction.DataLength).ToArray());
                                    _waitingOnFirmware = false;
                                    break;

                                #endregion

                                default:
                                    break;
                            }
                            break;

                        case CommandSets.Admin:
                            switch ((AdminCommands) instruction.Command)
                            {
                                #region Admin Functions

                                case AdminCommands.AdminFirmware:
                                    FirmwareUploader.ReceiveData(
                                        DataConversions.ConvertListToUint16(command.GetRange(4, 2)),
                                        command.GetRange(6, instruction.DataLength - 2));
                                    break;

                                case AdminCommands.AdminLogsRead:
                                    LogDownloader.ReceiveData(
                                        DataConversions.ConvertListToUint16(command.GetRange(4, 2)),
                                        command.GetRange(6, instruction.DataLength - 2));
                                    break;

                                case AdminCommands.AdminLogsCount:
                                    LogDownloader.SetPageCount((int)DataConversions.ConvertListToUint32(command.GetRange(4, 4)));
                                    break;

                                case AdminCommands.AdminConfigExport:
                                    IniDownloader.ReceiveData(
                                        DataConversions.ConvertListToUint16(command.GetRange(4, 2)),
                                        command.GetRange(6, instruction.DataLength - 2));
                                    break;

                                case AdminCommands.AdminConfigExportCount:
                                    IniDownloader.SetPageCount((int)DataConversions.ConvertListToUint32(command.GetRange(4, 4)));
                                    break;

                                case AdminCommands.AdminConfigImport:
                                    IniUploader.ReceiveData(DataConversions.ConvertListToUint16(command.GetRange(4, 2)),
                                        command.GetRange(6, instruction.DataLength - 2));
                                    break;

                                case AdminCommands.AdminConfigImportComplete:
                                    if (command[4] == 0)
                                    {
                                        // import had errors, query the system for the logs
                                        SendBinaryCommand(CommandSets.Admin,
                                            (ushort) AdminCommands.AdminConfigImportLogRead, true);
                                    }
                                    else
                                    {
                                        // import successful
                                        IniUploader.ReceiveUploadSuccess();
                                    }
                                    break;

                                case AdminCommands.AdminConfigImportLogRead:
                                    IniUploader.ReceiveUploadLogs(command.GetRange(4, instruction.DataLength));
                                    break;

                                case AdminCommands.AdminSettingsGet:
                                    i = 4;
                                    _settingsObject = new SettingsObject();

                                    #region Parse Settings

                                    try
                                    {
                                        _settingsObject.GeneralLoginTimeoutEnable = command[i++] == 1;
                                        _settingsObject.GeneralLoginTimeoutMinutes = command[i++];
                                        _settingsObject.GeneralRequireUser = command[i++] == 1;
                                        _settingsObject.GeneralRequireAdmin = command[i++] == 1;
                                        _settingsObject.GeneralAllowSavePassword = command[i++] == 1;
                                        _settingsObject.GeneralLockoutFront = command[i++] == 1;
                                        _settingsObject.GeneralLockoutMultiport = command[i++] == 1;

                                        _settingsObject.NetworkHostname = System.Text.Encoding.UTF8.GetString(
                                            command.GetRange(i + 1, command[i]).ToArray());
                                        i += command[i] + 1;

                                        _settingsObject.NetworkDhcpEnabled = command[i++] == 1;

                                        _settingsObject.NetworkIpAddress = System.Text.Encoding.UTF8.GetString(
                                            command.GetRange(i + 1, command[i]).ToArray());
                                        i += command[i] + 1;

                                        _settingsObject.NetworkDhcpIpAddress = System.Text.Encoding.UTF8.GetString(
                                            command.GetRange(i + 1, command[i]).ToArray());
                                        i += command[i] + 1;

                                        _settingsObject.NetworkSubnetMask = System.Text.Encoding.UTF8.GetString(
                                            command.GetRange(i + 1, command[i]).ToArray());
                                        i += command[i] + 1;

                                        _settingsObject.NetworkDhcpSubnetMask = System.Text.Encoding.UTF8.GetString(
                                            command.GetRange(i + 1, command[i]).ToArray());
                                        i += command[i] + 1;

                                        _settingsObject.NetworkGateway = System.Text.Encoding.UTF8.GetString(
                                            command.GetRange(i + 1, command[i]).ToArray());
                                        i += command[i] + 1;

                                        _settingsObject.NetworkDhcpGateway = System.Text.Encoding.UTF8.GetString(
                                            command.GetRange(i + 1, command[i]).ToArray());
                                        i += command[i] + 1;

                                        _settingsObject.NetworkPrimaryDns = System.Text.Encoding.UTF8.GetString(
                                            command.GetRange(i + 1, command[i]).ToArray());
                                        i += command[i] + 1;

                                        _settingsObject.NetworkDhcpPrimaryDns = System.Text.Encoding.UTF8.GetString(
                                            command.GetRange(i + 1, command[i]).ToArray());
                                        i += command[i] + 1;

                                        _settingsObject.NetworkSecondaryDns = System.Text.Encoding.UTF8.GetString(
                                            command.GetRange(i + 1, command[i]).ToArray());
                                        i += command[i] + 1;

                                        _settingsObject.NetworkDhcpSecondaryDns = System.Text.Encoding.UTF8.GetString(
                                            command.GetRange(i + 1, command[i]).ToArray());
                                        i += command[i] + 1;

                                        _settingsObject.SocketsLegacyEnable = command[i++] == 1;
                                        _settingsObject.SocketsLegacyPort = DataConversions.ConvertListToUint16(command.GetRange(i, 2));
                                        i += 2;

                                        _settingsObject.SocketsBinaryEnable = command[i++] == 1;
                                        _settingsObject.SocketsBinaryPort = DataConversions.ConvertListToUint16(command.GetRange(i, 2));
                                        i += 2;

                                        _settingsObject.UartBaudrate = (BaudRates)command[i++];
                                        _settingsObject.UartStopBits = (StopBits)command[i++];
                                        _settingsObject.UartParity = (Parity)command[i];
                                    }
                                    catch
                                    {
                                        _settingsObject = null;
                                    }

                                    #endregion

                                    _waitingOnSettings = false;
                                    break;

                                #endregion

                                default:
                                    break;
                            }
                            break;

                        default:
                            break;
                    }
                    break;

                #endregion

                default:
                    RunUnknownCommand(new UnknownPacketArgs(command));
                    break;
            }

        }

        /// <summary>
        /// Function to be overridden by derived classes. Processing of data should be done here.
        /// </summary>
        /// <param name="data">The data received from the socket.</param>
        protected override void ProcessReceivedData(List<byte> data)
        {
            _socketInFifo.AddRange(data);
            CommandDecoderWorker();
        }

        private void ParseLoginLevel(IList<byte> data)
        {
            if (data[0] > 2 || data[1] > 2)
                return;
            
            // login changed
            RunLoginUpdate(new LoginUpdateArgs((LoginPermissions)data[0], (LoginPermissions)data[1]));
        }

        #endregion

        #region External Functions

        /// <summary>
        /// The current permission status of admin controls
        /// </summary>
        public LoginPermissions AdminPermissions => _lastLoginUpdateArgs.Admin;

        /// <summary>
        /// The current permission status of user controls
        /// </summary>
        public LoginPermissions UserPermissions => _lastLoginUpdateArgs.User;

        /// <summary>
        /// Create a BinarySocket Connection
        /// </summary>
        /// <param name="address">Address to connect too</param>
        /// <param name="port">Port to connect too</param>
        /// <param name="connectionTimeoutMilliseconds">Timeout of connection attempt</param>
        /// <returns>Returns status of connection</returns>
        public new ConnectionStatus Connect(string address = "192.168.0.2", string port = "5000",
            int connectionTimeoutMilliseconds = 500)
        {
            _keepAliveMissedCount = 0;
            _keepAliveTimer.Start();
            return base.Connect(address, port, connectionTimeoutMilliseconds);
        }

        /// <summary>
        /// Disconnect from the BinarySocket
        /// </summary>
        public override void Disconnect()
        {
            _keepAliveTimer.Stop();
            base.Disconnect();
        }

        /// <summary>
        /// Begin shutdown of BinarySocket and request status.
        /// </summary>
        /// <returns>Readiness of BinarySocket and all children to shutdown.</returns>
        public ClosingInfo ShutdownReady()
        {
            var closingInfo = new ClosingInfo();

            // disconnect the ethernet socket
            Disconnect();

            // poke each thread then see if it is complete
            closingInfo.ChildInfo.Add(IniUploader.ShutdownReady());
            closingInfo.ChildInfo.Add(IniDownloader.ShutdownReady());
            closingInfo.ChildInfo.Add(LogDownloader.ShutdownReady());
            closingInfo.ChildInfo.Add(FirmwareUploader.ShutdownReady());

            // update ClosingInfo
            if (closingInfo.ChildInfo.Any(x => x.ShutdownReady != true))
            {
                // we have children still waiting
                closingInfo.ShutdownReady = false;
                closingInfo.Message = ThreadInfo.MessageStatus($"Closing Thread ({_threadName}): Waiting",
                    _applicationClosingCount++);
                foreach (var info in closingInfo.ChildInfo.Where(x => x.ShutdownReady == false))
                {
                    closingInfo.Message +=
                        $"{Environment.NewLine}  {info.Message.Replace(Environment.NewLine, $"{Environment.NewLine}  ")}";
                }
            }
            else
            {
                closingInfo.ShutdownReady = true;
                closingInfo.Message = $"Closing Thread ({_threadName}): Done!";
            }

            // return readiness to shutdown
            return closingInfo;
        }

        /// <summary>
        /// Login to the BinarySocket
        /// </summary>
        public bool Login(string userName, string password)
        {
            if (!IsConnected)
            {
                RunSocketMessage("Please connect to a unit before trying to log in!");
                return false;
            }

            SendBinaryCommand(CommandSets.System, (ushort) SystemCommands.SystemLoginUsername,
                true, System.Text.Encoding.ASCII.GetBytes(userName).ToList());

            SendBinaryCommand(CommandSets.System, (ushort) SystemCommands.SystemLoginPassword,
                true, System.Text.Encoding.ASCII.GetBytes(password).ToList());

            return true;
        }

        /// <summary>
        /// Logout of the BinarySocket
        /// </summary>
        public void Logout()
        {
            SendBinaryCommand(CommandSets.System, (ushort) SystemCommands.SystemLogout, true);
        }
        
        /// <summary>
        /// Send a command to the BinarySocket
        /// </summary>
        /// <param name="binaryCommand">The binary command to send</param>
        /// <param name="dataPayload">The data to send with the command</param>
        public void SendBinaryCommand(BinaryCommand binaryCommand, List<byte> dataPayload = null)
        {
            var data = new List<byte>();
            // make sure payload is not null
            if (dataPayload == null)
                dataPayload = new List<byte>();

            // update payload length
            binaryCommand.DataLength = (ushort)dataPayload.Count;

            // add instruction to packet
            data.AddRange(binaryCommand.GetCommandList());

            // add payload to packet
            data.AddRange(dataPayload);

            // add checksum to packet
            data.AddRange(Checksums.Fletcher16(data.ToArray(), (ushort)dataPayload.Count + 4));

            // encode the packet
            CommandEncode(ref data);

            SendData(data.ToArray());
        }

        /// <summary>
        /// Send a command to the BinarySocket
        /// </summary>
        /// <param name="commandSet">Which command set to send the command from</param>
        /// <param name="command">Which command in the command set to send</param>
        /// <param name="writeAccess">Set to True if command needs write access</param>
        /// <param name="dataPayload">The data to send with the command</param>
        public void SendBinaryCommand(CommandSets commandSet, ushort command, bool writeAccess, List<byte> dataPayload = null)
        {
            SendBinaryCommand(new BinaryCommand(commandSet, command, writeAccess), dataPayload);
        }

        /// <summary>
        /// Get the firmware of the unit connected to this BinarySocket
        /// </summary>
        /// <returns>Firmware String</returns>
        public string GetFirmware()
        {
            _waitingOnFirmware = true;
            _firmwareString = "";
            SendBinaryCommand(CommandSets.System, (ushort)SystemCommands.SystemFirmwareVersion, false);

            var timeout = DateTime.Now.AddMilliseconds(100);
            while (_waitingOnFirmware && DateTime.Now < timeout)
            {
                TimeFunctions.Wait(10);
            }

            return _firmwareString;
        }

        /// <summary>
        /// Get the SettingsObject of the unit connected to this BinarySocket
        /// </summary>
        /// <returns>The current SettingsObject</returns>
        public SettingsObject GetSettings()
        {
            _waitingOnSettings = true;
            _settingsObject = null;

            // querry values
            SendBinaryCommand(CommandSets.Admin, (ushort)AdminCommands.AdminSettingsGet, true);

            var timeout = DateTime.Now.AddMilliseconds(100);
            while (_waitingOnSettings && DateTime.Now < timeout)
            {
                TimeFunctions.Wait(10);
            }

            return _settingsObject;
        }

        #endregion

    }

}
