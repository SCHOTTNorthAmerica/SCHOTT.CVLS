using SCHOTT.Core.Communication.Ethernet;
using SCHOTT.CVLS.Communications;

namespace SCHOTT.CVLS.Ethernet.Legacy
{
    /// <summary>
    /// CVLS Legacy Socket, Implimentation of a TelnetSocket
    /// </summary>
    public class LegacySocket : TelnetSocket, ILegacyProtocol
    {
        /// <summary>
        /// Protocol object to allow easy access of CVLS functions
        /// </summary>
        public LegacyProtocol Protocol { get; }

        /// <summary>
        /// Protocol object to allow easy access of CVLS functions, echoing all com traffic to the message function.
        /// </summary>
        public LegacyProtocol ProtocolEcho { get; }

        /// <summary>
        /// Create a LegacySocket with the appropriate ComParameters
        /// </summary>
        public LegacySocket()
        {
            ComParameters.Command = "&f";
            ComParameters.ExpectedResponce = "&f";
            ComParameters.EndPrompt = "MULTILINECOMPLETE";
            Protocol = new LegacyProtocol(this);
            ProtocolEcho = new LegacyProtocol(this, true);
        }

        /// <summary>
        /// Connect to a unit.
        /// </summary>
        /// <param name="address">Address of unit</param>
        /// <param name="port">Port of Legacy Socket</param>
        /// <param name="connectionTimeoutMilliseconds">Connection Timeout</param>
        /// <returns>Returns status of connection</returns>
        public new ConnectionStatus Connect(string address = "192.168.0.2", string port = "50811", int connectionTimeoutMilliseconds = 500)
        {
            return base.Connect(address, port, connectionTimeoutMilliseconds);
        }
    }
}
