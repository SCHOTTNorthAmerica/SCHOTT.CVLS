using SCHOTT.CVLS.Communications;

namespace SCHOTT.CVLS.Ethernet.Discovery
{
    /// <summary>
    /// Object contains all information reported by the unit.
    /// </summary>
    public class DiscoveryObject
    {
        /// <summary>
        /// Mac Address of unit
        /// </summary>
        public string MacAddress { get; private set; }

        /// <summary>
        /// Mac Type
        /// </summary>
        public string MacType { get; private set; }

        /// <summary>
        /// Text Host Name
        /// </summary>
        public string HostName { get; private set; }

        /// <summary>
        /// IPv4 Address
        /// </summary>
        public string IpAddress { get; private set; }

        /// <summary>
        /// IPv6 Unicast Address
        /// </summary>
        public string Ipv6UAddress { get; private set; }

        /// <summary>
        /// IPv6 Multicast Address
        /// </summary>
        public string Ipv6MAddress { get; private set; }

        /// <summary>
        /// IPv6 Router Address
        /// </summary>
        public string Ipv6Router { get; private set; }

        /// <summary>
        /// IPv6 Gateway Address
        /// </summary>
        public string Ipv6Gateway { get; private set; }

        /// <summary>
        /// Unit Serial Number
        /// </summary>
        public string Serial { get; private set; }

        /// <summary>
        /// Unit Model
        /// </summary>
        public string Model { get; private set; }

        /// <summary>
        /// Unit Firmware Version
        /// </summary>
        public string Firmware { get; private set; }

        /// <summary>
        /// Raw status dump from unit
        /// </summary>
        public string Status { get; private set; }

        /// <summary>
        /// Converted Status of the unit
        /// </summary>
        public StatusObject StatusObject { get; private set; }

        /// <summary>
        /// Fluent Builder class for the DiscoveryObject
        /// </summary>
        public class Builder
        {
            private readonly DiscoveryObject _obj = new DiscoveryObject();

            /// <summary>
            /// Mac Address of unit
            /// </summary>
            public string MacAddress
            {
                set { _obj.MacAddress = value; }
            }

            /// <summary>
            /// Mac Type
            /// </summary>
            public string MacType
            {
                set { _obj.MacType = value; }
            }

            /// <summary>
            /// Text Host Name
            /// </summary>
            public string HostName
            {
                set { _obj.HostName = value; }
            }

            /// <summary>
            /// IPv4 Address
            /// </summary>
            public string IpAddress
            {
                set { _obj.IpAddress = value; }
            }

            /// <summary>
            /// IPv6 Unicast Address
            /// </summary>
            public string Ipv6UAddress
            {
                set { _obj.Ipv6UAddress = value; }
            }

            /// <summary>
            /// IPv6 Multicast Address
            /// </summary>
            public string Ipv6MAddress
            {
                set { _obj.Ipv6MAddress = value; }
            }

            /// <summary>
            /// IPv6 Router Address
            /// </summary>
            public string Ipv6Router
            {
                set { _obj.Ipv6Router = value; }
            }

            /// <summary>
            /// IPv6 Gateway Address
            /// </summary>
            public string Ipv6Gateway
            {
                set { _obj.Ipv6Gateway = value; }
            }

            /// <summary>
            /// Unit Serial Number
            /// </summary>
            public string Serial
            {
                set { _obj.Serial = value; }
            }

            /// <summary>
            /// Unit Model
            /// </summary>
            public string Model
            {
                set { _obj.Model = value; }
            }

            /// <summary>
            /// Unit Firmware Version
            /// </summary>
            public string Firmware
            {
                set { _obj.Firmware = value; }
            }

            /// <summary>
            /// Raw status dump from unit
            /// </summary>
            public string Status
            {
                set { _obj.Status = value; }
            }

            /// <summary>
            /// Converted Status of the unit
            /// </summary>
            public StatusObject StatusObject
            {
                set { _obj.StatusObject = value; }
            }

            /// <summary>
            /// Build the new DiscoveryObject
            /// </summary>
            /// <returns></returns>
            public DiscoveryObject Build()
            {
                return _obj;
            }
        }

    }

}
