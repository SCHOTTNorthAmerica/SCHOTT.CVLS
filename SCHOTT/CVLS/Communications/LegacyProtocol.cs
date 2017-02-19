using SCHOTT.Core.Communication;

namespace SCHOTT.CVLS.Communications
{
    /// <summary>
    /// CVLS Wrapper for the ITextProtocol Interface. 
    /// Purely used to seperate CVLS extension functions from SCHOTT.Core functions.
    /// </summary>
    public interface ILegacyProtocol : ITextProtocol
    {
    }

    /// <summary>
    /// Class to give easy access to the CVLS Legacy Protocol
    /// </summary>
    public class LegacyProtocol
    {
        private readonly ILegacyProtocol _port;
        private readonly bool _echoComTraffic;

        /// <summary>
        /// Unit Controls
        /// </summary>
        public readonly ControlsProtocol Controls;

        /// <summary>
        /// Unit Status
        /// </summary>
        public readonly StatusProtocol Status;

        /// <summary>
        /// Unit Diagnostics
        /// </summary>
        public readonly DiagnosticsProtocol Diagnostics;

        /// <summary>
        /// Unit Configuration
        /// </summary>
        public readonly ConfigurationProtocol Configurations;

        /// <summary>
        /// Creates a new LegacyProtocol for the given port
        /// </summary>
        /// <param name="port">The port for the LegacyProtocol to use.</param>
        /// <param name="echoComTraffic">When True, the protocol object will echo com traffic to the subscribed message functions.</param>
        public LegacyProtocol(ILegacyProtocol port, bool echoComTraffic = false)
        {
            _port = port;
            _echoComTraffic = echoComTraffic;
            Controls = new ControlsProtocol(port, echoComTraffic);
            Status = new StatusProtocol(port, echoComTraffic);
            Diagnostics = new DiagnosticsProtocol(port, echoComTraffic);
            Configurations = new ConfigurationProtocol(port, echoComTraffic);
        }
        
        /// <summary>
        /// Process the Firmware Version of the currently connected unit.
        /// </summary>
        public double FirmwareVersion
        {
            get
            {
                string workingLine;

                // request firmware
                if (!_port.SendCommandSingleTest("&f", "&f", out workingLine, true))
                    return 0;

                // see if we can get the firmware token
                var tokens = workingLine.Split(' ');
                if (tokens.Length <= 0)
                    return 0;

                // see if we can parse a number from the token
                double firmware;
                if (!double.TryParse(tokens[0], out firmware))
                    return 0;

                return firmware;
            }
        }
        
    }
}
