using SCHOTT.CVLS.Enums;

namespace SCHOTT.CVLS.Communications
{
    /// <summary>
    /// Object to hold current system settings
    /// </summary>
    public class SettingsObject
    {
        /// <summary>
        /// Enable login timeouts
        /// </summary>
        public bool GeneralLoginTimeoutEnable { get; set; }

        /// <summary>
        /// Timeout in minutes
        /// </summary>
        public int GeneralLoginTimeoutMinutes { get; set; }

        /// <summary>
        /// Require a login for user controls
        /// </summary>
        public bool GeneralRequireUser { get; set; }

        /// <summary>
        /// Require a login for admin controls
        /// </summary>
        public bool GeneralRequireAdmin { get; set; }

        /// <summary>
        /// Allow browsers to save the username/password when logging in
        /// </summary>
        public bool GeneralAllowSavePassword { get; set; }

        /// <summary>
        /// Lockout front controls
        /// </summary>
        public bool GeneralLockoutFront { get; set; }

        /// <summary>
        /// Lockout rear controls
        /// </summary>
        public bool GeneralLockoutMultiport { get; set; }

        /// <summary>
        /// Hostname of the unit
        /// </summary>
        public string NetworkHostname { get; set; }

        /// <summary>
        /// Enable DHCP on the unit, will default to static settings on timeout
        /// </summary>
        public bool NetworkDhcpEnabled { get; set; }

        /// <summary>
        /// Static IP address
        /// </summary>
        public string NetworkIpAddress { get; set; }

        /// <summary>
        /// Current DHCP address
        /// </summary>
        public string NetworkDhcpIpAddress { get; set; }

        /// <summary>
        /// Static Subnet Mask
        /// </summary>
        public string NetworkSubnetMask { get; set; }

        /// <summary>
        /// Current DHCP Subnet Mask
        /// </summary>
        public string NetworkDhcpSubnetMask { get; set; }

        /// <summary>
        /// Static Gateway
        /// </summary>
        public string NetworkGateway { get; set; }

        /// <summary>
        /// Current DHCP Gateway
        /// </summary>
        public string NetworkDhcpGateway { get; set; }

        /// <summary>
        /// Static Primary Dns 
        /// </summary>
        public string NetworkPrimaryDns { get; set; }

        /// <summary>
        /// Current DHCP Primary Dns 
        /// </summary>
        public string NetworkDhcpPrimaryDns { get; set; }

        /// <summary>
        /// Static Secondary Dns
        /// </summary>
        public string NetworkSecondaryDns { get; set; }

        /// <summary>
        /// Current DHCP Secondary Dns 
        /// </summary>
        public string NetworkDhcpSecondaryDns { get; set; }

        /// <summary>
        /// Enable the LegacySocket
        /// </summary>
        public bool SocketsLegacyEnable { get; set; }

        /// <summary>
        /// Defines the port of the LegacySocket
        /// </summary>
        public int SocketsLegacyPort { get; set; }


        /// <summary>
        /// Enable the BinarySocket
        /// </summary>
        public bool SocketsBinaryEnable { get; set; }

        /// <summary>
        /// Defines the port of the BinarySocket
        /// </summary>
        public int SocketsBinaryPort { get; set; }

        /// <summary>
        /// The UART Baud Rate
        /// </summary>
        public BaudRates UartBaudrate { get; set; }

        /// <summary>
        /// The UART Stop Bits
        /// </summary>
        public StopBits UartStopBits { get; set; }

        /// <summary>
        /// The UART Port Parity
        /// </summary>
        public Parity UartParity { get; set; }
        
    }
}
