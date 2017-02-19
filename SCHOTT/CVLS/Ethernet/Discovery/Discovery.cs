using SCHOTT.Core.Utilities;
using SCHOTT.CVLS.Communications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace SCHOTT.CVLS.Ethernet.Discovery
{
    /// <summary>
    /// Class to discover CVLS units on the network.
    /// </summary>
    public class Discovery
    {
        /// <summary>
        /// The MessageBroker for the Discovery class to use.
        /// </summary>
        private readonly MessageBroker _messageBroker = new MessageBroker();

        private struct UdpState
        {
            public System.Net.IPEndPoint Ep;
            public UdpClient UdpClient;
        }

        private UdpState _globalUdp;

        /// <summary>
        /// Allows the user to register for Unit Discovery Updates from the network.
        /// </summary>
        /// <param name="context">Allows the user to specify how the update should arrive for syncing with GUI applications.</param>
        /// <param name="action">The lambda expression to execute on discovery events.</param>
        public void RegisterDiscoveredUnit(MessageBroker.MessageContext context, Action<DiscoveryObject> action)
        {
            _messageBroker.Register("DiscoveredUnit", context, action);
        }

        private void RunDiscoveredUnit(DiscoveryObject args)
        {
            _messageBroker.RunActions("DiscoveredUnit", args);
        }

        /// <summary>
        /// Allows the user to register for Listening Status Updates from the Discovery Object.
        /// </summary>
        /// <param name="context">Allows the user to specify how the update should arrive for syncing with GUI applications.</param>
        /// <param name="action">The lambda expression to execute on discovery events.</param>
        public void RegisterListeningStatusUpdate(MessageBroker.MessageContext context, Action<bool> action)
        {
            _messageBroker.Register("ListeningStatusUpdate", context, action);
        }

        private void RunListeningStatusUpdate(bool args)
        {
            _messageBroker.RunActions("ListeningStatusUpdate", args);
        }

        /// <summary>
        /// Enable the Listener
        /// </summary>
        /// <returns>True = Listening for units, False otherwise</returns>
        public bool ListenerStart()
        {
            if (IsConnected)
                return true;

            try
            {
                _globalUdp.UdpClient = new UdpClient();
                //GlobalUDP.EP = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("255.255.255.255"), 30303);
                var bindEp = new System.Net.IPEndPoint(System.Net.IPAddress.Any, 30303);
                var discoverMsg = Encoding.ASCII.GetBytes("Discovery: Who is out there?");

                // Set the local UDP port to listen on
                _globalUdp.UdpClient.Client.Bind(bindEp);

                // Enable the transmission of broadcast packets without having them be received by ourself
                _globalUdp.UdpClient.EnableBroadcast = true;
                _globalUdp.UdpClient.MulticastLoopback = false;

                // Configure ourself to receive discovery responses
                WaitForData(_globalUdp);

                // Transmit the discovery request message
                _globalUdp.UdpClient.Send(discoverMsg, discoverMsg.Length, new System.Net.IPEndPoint(System.Net.IPAddress.Parse("255.255.255.255"), 30303));
            }
            catch
            {
                ListenerStop();
                return false;
            }
            RunListeningStatusUpdate(IsConnected);
            return true;
        }

        /// <summary>
        /// Stop the Listener
        /// </summary>
        public void ListenerStop()
        {
            if (_globalUdp.UdpClient == null)
                return;

            _globalUdp.UdpClient.Client.Close();
            _globalUdp.UdpClient.Close();

            RunListeningStatusUpdate(IsConnected);
        }

        /// <summary>
        /// Returns if listening for units
        /// </summary>
        public bool IsConnected => _globalUdp.UdpClient?.Client?.IsBound == true;

        /// <summary>
        /// Poll the network for devices
        /// </summary>
        /// <returns>True = Pinged the network for units, False = Couldn't start the listener</returns>
        public bool DiscoverDevices()
        {
            try
            {
                // try to bind the udp port if it isn't yet
                if (!IsConnected && !ListenerStart())
                    return false;

                // Try to send the discovery request message
                var discoverMsg = Encoding.ASCII.GetBytes("Discovery: Who is out there?");
                _globalUdp.UdpClient.Send(discoverMsg, discoverMsg.Length, new System.Net.IPEndPoint(System.Net.IPAddress.Parse("255.255.255.255"), 30303));
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void WaitForData(UdpState myUdp)
        {
            try
            {
                // Configure the UdpClient class to accept more messages, if they arrive
                myUdp.UdpClient.BeginReceive(ReceiveCallback, myUdp);
            }
            catch
            {
                /* if the socket is already closed this will throw a ObjectDisposedException */
            }
        }

        private enum FieldTypes
        {
            AnnounceFieldTruncated = 0x01,
            AnnounceFieldMacAddr,
            AnnounceFieldMacType,
            AnnounceFieldHostName,
            AnnounceFieldIpv4Address,
            AnnounceFieldIpv6Unicast,
            AnnounceFieldIpv6Multicast,
            AnnounceFieldIpv6DefaultRouter,
            AnnounceFieldIpv6DefaultGateway,
            AnnounceFieldSerial,
            AnnounceFieldModel,
            AnnounceFieldFirmware,
            AnnounceFieldStatus
        }

        private static string ParseByteArray(IList<byte> byteArray, int length, string delimiter, string format = "")
        {
            var temp = string.Empty;
            if (byteArray.Count < length)
                return temp;

            for (var i = 0; i < length; i++)
            {
                if (i != 0)
                    temp += delimiter;

                temp += byteArray[i].ToString(format);
            }
            return temp;
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            var myUdp = (UdpState)ar.AsyncState;

            var tokenListCollection = new List<List<byte>>();
            var tokenList = new List<byte>();

            var discoveryObjectBuilder = new DiscoveryObject.Builder();

            try
            {
                // Obtain the UDP message body and convert it to a string, with remote IP address attached as well
                var incData = myUdp.UdpClient.EndReceive(ar, ref myUdp.Ep);
                var macFound = false;

                #region decode data

                if (incData[0] < 0x0F)
                {
                    // split up the tokens
                    for (var i = 0; i < incData.Length; i++)
                    {
                        if (incData[i] == '\r' && incData[i + 1] == '\n')
                        {
                            // found end of token, advance past the second end char
                            i++;

                            // add token to parse list
                            tokenListCollection.Add(tokenList);
                            tokenList = new List<byte>();
                        }
                        else
                        {
                            // add the byto to the token
                            tokenList.Add(incData[i]);
                        }
                    }

                    foreach (var token in tokenListCollection)
                    {
                        switch ((FieldTypes)token[0])
                        {
                            case FieldTypes.AnnounceFieldTruncated:
                                break;

                            case FieldTypes.AnnounceFieldMacAddr:
                                var macAddress = ParseByteArray(token.Skip(1).ToList(), 6, ":", "X2");
                                discoveryObjectBuilder.MacAddress = macAddress;

                                if (macAddress.Length > 0)
                                    macFound = true;

                                break;

                            case FieldTypes.AnnounceFieldMacType:
                                if (!macFound)
                                    break;

                                discoveryObjectBuilder.MacType = Encoding.UTF8.GetString(token.Skip(1).ToArray());
                                break;

                            case FieldTypes.AnnounceFieldHostName:
                                if (!macFound)
                                    break;

                                discoveryObjectBuilder.HostName = Encoding.UTF8.GetString(token.Skip(1).ToArray()).Trim();
                                break;

                            case FieldTypes.AnnounceFieldIpv4Address:
                                discoveryObjectBuilder.IpAddress = ParseByteArray(token.Skip(1).ToList(), 4, ".");
                                break;

                            case FieldTypes.AnnounceFieldIpv6Unicast:
                                discoveryObjectBuilder.Ipv6UAddress = ParseByteArray(token.Skip(1).ToList(), 16, ":");
                                break;

                            case FieldTypes.AnnounceFieldIpv6Multicast:
                                discoveryObjectBuilder.Ipv6MAddress = ParseByteArray(token.Skip(1).ToList(), 16, ":");
                                break;

                            case FieldTypes.AnnounceFieldIpv6DefaultRouter:
                                discoveryObjectBuilder.Ipv6Router = ParseByteArray(token.Skip(1).ToList(), 16, ":");
                                break;

                            case FieldTypes.AnnounceFieldIpv6DefaultGateway:
                                discoveryObjectBuilder.Ipv6Gateway = ParseByteArray(token.Skip(1).ToList(), 16, ":");
                                break;

                            case FieldTypes.AnnounceFieldSerial:
                                if (!macFound)
                                    break;

                                discoveryObjectBuilder.Serial = Encoding.UTF8.GetString(token.Skip(1).ToArray());
                                break;

                            case FieldTypes.AnnounceFieldModel:
                                if (!macFound)
                                    break;

                                discoveryObjectBuilder.Model = Encoding.UTF8.GetString(token.Skip(1).ToArray());
                                break;

                            case FieldTypes.AnnounceFieldFirmware:
                                if (!macFound)
                                    break;

                                discoveryObjectBuilder.Firmware = Encoding.UTF8.GetString(token.Skip(1).ToArray());
                                break;

                            case FieldTypes.AnnounceFieldStatus:
                                if (!macFound)
                                    break;

                                var rawStatus = Encoding.UTF8.GetString(token.Skip(1).ToArray());
                                var tokens = rawStatus.Split(',').ToArray();

                                discoveryObjectBuilder.Status = rawStatus;
                                var statusObjectBuilder = new StatusObject.Builder();

                                var i = 0;
                                if (tokens.Length == 24)
                                {
                                    statusObjectBuilder.TemperatureLed = new TemperatureObject.Builder
                                    {
                                        Temperature = double.Parse(tokens[i++]),
                                        Status = (Enums.StatusIndicators)int.Parse(tokens[i++]),
                                        ThermistorStatus = (Enums.StatusIndicators)int.Parse(tokens[i++])
                                    }.Build();

                                    statusObjectBuilder.TemperatureBoard = new TemperatureObject.Builder
                                    {
                                        Temperature = double.Parse(tokens[i++]),
                                        Status = (Enums.StatusIndicators)int.Parse(tokens[i++]),
                                        ThermistorStatus = (Enums.StatusIndicators)int.Parse(tokens[i++])
                                    }.Build();

                                    statusObjectBuilder.VoltageRefOut = new VoltageObject.Builder
                                    {
                                        Voltage = double.Parse(tokens[i++]),
                                        Status = (Enums.StatusIndicators)int.Parse(tokens[i++])
                                    }.Build();

                                    statusObjectBuilder.VoltageInput = new VoltageObject.Builder
                                    {
                                        Voltage = double.Parse(tokens[i++]),
                                        Status = (Enums.StatusIndicators)int.Parse(tokens[i++])
                                    }.Build();

                                    statusObjectBuilder.Fan = new FanStatusObject.Builder
                                    {
                                        Speed = int.Parse(tokens[i++]),
                                        Status = (Enums.StatusIndicators)int.Parse(tokens[i++])
                                    }.Build();

                                    var systemBuilder = new SystemObject.Builder
                                    {
                                        SystemMode = (Enums.SystemMode)int.Parse(tokens[i++])
                                    };

                                    statusObjectBuilder.Equalizer = new EqualizerStatusObject.Builder
                                    {
                                        Mode = (Enums.EqualizerStatus)int.Parse(tokens[i++]),
                                        Status = (Enums.StatusIndicators)int.Parse(tokens[i++])
                                    }.Build();

                                    systemBuilder.LightFeedBack = int.Parse(tokens[i++]);
                                    systemBuilder.LastCommandSource = (Enums.CommandSource)int.Parse(tokens[i++]);
                                    systemBuilder.UserMode = (Enums.UserMode)int.Parse(tokens[i++]);
                                    systemBuilder.KnobMode = (Enums.KnobControl)int.Parse(tokens[i++]);
                                    systemBuilder.Time = new TimeObject(uint.Parse(tokens[i++]));
                                    statusObjectBuilder.System = systemBuilder.Build();

                                    statusObjectBuilder.Memory = new MemoryObject.Builder
                                    {
                                        CustomerSettings = uint.Parse(tokens[i++]),
                                        FactorySettings = uint.Parse(tokens[i++]),
                                        Firmware = uint.Parse(tokens[i++]),
                                        Exceptions = uint.Parse(tokens[i])
                                    }.Build();

                                    statusObjectBuilder.Identification = new IdentificationObject.Builder
                                    {
                                        Firmware = discoveryObjectBuilder.Build().Firmware,
                                        Model = discoveryObjectBuilder.Build().Model,
                                        Serial = uint.Parse(discoveryObjectBuilder.Build().Serial),
                                        SerialFull = $"{discoveryObjectBuilder.Build().Model}:{discoveryObjectBuilder.Build().Serial}"
                                    }.Build();

                                    discoveryObjectBuilder.StatusObject = statusObjectBuilder.Build();
                                }
                                break;

                            default:
                                break;
                        }
                    }
                }

                #endregion

                if (IsReady(discoveryObjectBuilder.Build()))
                {
                    // Write the received UDP message text to the listbox in a thread-safe manner
                    RunDiscoveredUnit(discoveryObjectBuilder.Build());
                }

            }
            catch
            {
                /* if the socket is already closed this will throw a ObjectDisposedException */
            }

            WaitForData(myUdp);
        }

        /// <summary>
        /// Do we have the minimum amount of data to report a unit.
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public bool IsReady(DiscoveryObject unit)
        {
            return unit.MacAddress != null && unit.IpAddress != "" && unit.MacAddress != "";
        }

    }


}
