using SCHOTT.Core.Communication;
using SCHOTT.Core.Communication.Serial;
using SCHOTT.CVLS.Communications;
using System;
using System.Linq;

namespace SCHOTT.CVLS.Serial
{
    /// <summary>
    /// CVLSComPort extenstion class to add simplified connection methods for CVLS units.
    /// </summary>
    public class CVLSComPort : ComPortBase , ILegacyProtocol
    {
        /// <summary>
        /// Protocol object to allow easy access of CVLS functions
        /// </summary>
        public LegacyProtocol Protocol { get; }

        /// <summary>
        /// Protocol object to allow easy access of CVLS functions, echoing all com traffic to the message function.
        /// </summary>
        public LegacyProtocol ProtocolEcho { get; }

        #region Initialization Functions

        /// <summary>
        /// Initialize a CVLSComPort using the supplied parameters.
        /// </summary>
        /// <param name="portName">The port name to connect too.</param>
        /// <param name="portParameters">The parameters to use when setting up the CVLSComPort</param>
        public CVLSComPort(string portName, ComParameters portParameters) : base(portName, portParameters)
        {
            Protocol = new LegacyProtocol(this);
            ProtocolEcho = new LegacyProtocol(this, true);
        }

        #endregion

        #region External Functions
        
        /// <summary>
        /// Extension to test if a connected CVLSComPort is a CVLS USB connection.
        /// </summary>
        /// <returns>True = CVLSComPort is CVLS USB connection, False otherwise</returns>
        public bool IsUsb()
        {
            var ports = ComPortInfo.GetDescriptions();
            var port = ports.FirstOrDefault(p => p.Port == PortName);
            return port?.Name.Contains("SCHOTT CV-LS") == true;
        }

        /// <summary>
        /// Port capability for high speed.
        /// </summary>
        public new bool IsHighSpeed()
        {
            return IsUsb();
        }

        #endregion

        #region Static Functions

        /// <summary>
        /// Find and connect to any CVLS unit with the given parameters.
        /// </summary>
        /// <param name="cvlsPortType">Select the CVLSPortType, can use multiple flags</param>
        public static CVLSComPort AutoConnectComPort(CVLSPortType cvlsPortType)
        {
            var parameters = ComParameters();
            var comPorts = ComPortInfo.GetDescriptions();

            if (ComMode(cvlsPortType) == ThreadedComPortBase.ConnectionMode.SelectionRule)
                comPorts = comPorts.Where(SelectionRule(cvlsPortType)).ToList();

            return comPorts.Any() ? AutoConnectComPort<CVLSComPort>(comPorts.Select(p => p.Port).ToList(), parameters) : null;
        }

        /// <summary>
        /// Find and connect to any CVLS unit with the given parameters.
        /// </summary>
        /// <param name="cvlsPortType">Select the CVLSPortType, can use multiple flags</param>
        /// <param name="serialNumber">The serial number to connect too</param>
        public static CVLSComPort AutoConnectComPort(CVLSPortType cvlsPortType, int serialNumber)
        {
            var parameters = ComParameters(serialNumber);
            var comPorts = ComPortInfo.GetDescriptions();

            if (ComMode(cvlsPortType) == ThreadedComPortBase.ConnectionMode.SelectionRule)
                comPorts = comPorts.Where(SelectionRule(cvlsPortType)).ToList();

            return comPorts.Any() ? AutoConnectComPort<CVLSComPort>(comPorts.Select(p => p.Port).ToList(), parameters) : null;
        }

        /// <summary>
        /// Find and connect to any CVLS unit with the given parameters.
        /// </summary>
        /// <param name="portName">The port to connect to in format 'COM#'</param>
        public static CVLSComPort AutoConnectComPort(string portName)
        {
            var parameters = ComParameters();
            var comPorts = ComPortInfo.GetDescriptions().Where(p => p.Port == portName).ToList();

            return comPorts.Any() ? AutoConnectComPort<CVLSComPort>(comPorts.Select(p => p.Port).ToList(), parameters) : null;
        }

        /// <summary>
        /// Default ComParameters to use for CVLS units.
        /// </summary>
        /// <returns>new ComParameter object</returns>
        public static ComParameters ComParameters()
        {
            return new ComParameters { Command = "&z", ExpectedResponce = "&z", EndPrompt = "MULTILINECOMPLETE" };
        }

        /// <summary>
        /// Default ComParameters to use for CVLS units.
        /// </summary>
        /// <param name="serialNumber">The serial number to connect to</param>
        /// <returns>new ComParameter object</returns>
        public static ComParameters ComParameters(int serialNumber)
        {
            return new ComParameters { Command = "&z", ExpectedResponce = $"&z{serialNumber:000000}", EndPrompt = "MULTILINECOMPLETE" };
        }

        /// <summary>
        /// Extension to give the ComPortBase.ConnectionMode for a given CVLS CVLSPortType
        /// </summary>
        /// <param name="cvlsPortType">The CVLSPortType to use.</param>
        /// <returns>ComPortBase.ConnectionMode to pass into ComPortBase</returns>
        public static ThreadedComPortBase.ConnectionMode ComMode(CVLSPortType cvlsPortType)
        {
            switch (cvlsPortType)
            {
                case CVLSPortType.Usb:
                case CVLSPortType.Rs232:
                    return ThreadedComPortBase.ConnectionMode.SelectionRule;

                default:
                    return ThreadedComPortBase.ConnectionMode.AnyCom;
            }
        }

        /// <summary>
        /// Extension to give the SelectionRule for a given CVLS CVLSPortType
        /// </summary>
        /// <param name="cvlsPortType">The CVLSPortType to use.</param>
        /// <returns>SelectionRule to pass into ComPortBase</returns>
        public static Func<ComPortInfo, bool> SelectionRule(CVLSPortType cvlsPortType)
        {
            switch (cvlsPortType)
            {
                case CVLSPortType.Usb:
                    return port => port.Name.Contains("SCHOTT CV-LS");

                case CVLSPortType.Rs232:
                    return port => !port.Name.Contains("SCHOTT CV-LS");

                default:
                    return null;
            }
        }

        #endregion

    }
}
