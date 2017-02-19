using SCHOTT.Core.Communication;
using SCHOTT.Core.Communication.Serial;
using SCHOTT.Core.Extensions;
using SCHOTT.Core.Threading;
using SCHOTT.CVLS.Communications;
using SCHOTT.CVLS.Serial.Workers;
using System.Collections.Generic;

namespace SCHOTT.CVLS.Serial
{
    /// <summary>
    /// CVLS Implimentation of a ThreadedComPortBase. This port type should be used when the unit connection
    /// state is unknown. Functions are provided to be notified of connections and status changes. When using
    /// a static configuration, consider using the CVLSComPort classes instead.
    /// </summary>
    public class CVLSThreadedComPort : ThreadedComPortBase, ILegacyProtocol
    {
        /// <summary>
        /// Class to allow the user to upload firmware to the connected CVLS unit.
        /// </summary>
        public FirmwareUploaderSerial FirmwareUploader;

        /// <summary>
        /// Class to allow the user to download exception logs from the connected CVLS unit.
        /// </summary>
        public LogDownloaderSerial LogDownloader;

        /// <summary>
        /// Class to allow the user to upload INI files to the connected CVLS unit.
        /// </summary>
        public IniUploaderSerial IniUploader;

        /// <summary>
        /// Class to allow the user to download INI files from the connected CVLS unit.
        /// </summary>
        public IniDownloaderSerial IniDownloader;
        
        #region Function Overrides for Derived Class

        /// <summary>
        /// Allows access to the CurrentConnection
        /// </summary>
        public new CVLSComPort CurrentConnection => (CVLSComPort)base.CurrentConnection;

        /// <summary>
        /// AutoConnect function called by the CVLSThreadedComPort class. A derived class can override this function
        /// to return a different derived type of CVLSComPort for the connect function. This allows for extension of
        /// the CVLSThreadedComPort.
        /// </summary>
        /// <param name="portsToCheck">Which ports to check for a connection.</param>
        /// <param name="portParameters">Which parameters to use when checking ports.</param>
        /// <returns></returns>
        protected override ComPortBase AutoConnectComPort(List<string> portsToCheck, ComParameters portParameters)
        {
            return ComPortBase.AutoConnectComPort<CVLSComPort>(portsToCheck, portParameters);
        }

        #endregion

        #region Initialization Functions

        /// <summary>
        /// Create a CVLSThreadedComPort for a CVLS unit. This port type should be used when the unit connection
        /// state is unknown. Functions are provided to be notified of connections and status changes. When using
        /// a static configuration, consider using the CVLSComPort classes instead.
        /// </summary>
        /// <param name="threadName">Name of the thread.</param>
        /// <param name="closingWorker">The ClosingWorker to add this thread too</param>
        /// <param name="cvlsPortType">Select the CVLSPortType, can use multiple flags</param>
        public CVLSThreadedComPort(string threadName, ClosingWorker closingWorker, CVLSPortType cvlsPortType)
            : base(threadName, closingWorker, CVLSComPort.ComParameters(), CVLSComPort.ComMode(cvlsPortType), CVLSComPort.SelectionRule(cvlsPortType))
        {
            InitializeFirmwareUploader();
        }

        /// <summary>
        /// Create a CVLSThreadedComPort for a CVLS unit. This port type should be used when the unit connection
        /// state is unknown. Functions are provided to be notified of connections and status changes. When using
        /// a static configuration, consider using the CVLSComPort classes instead.
        /// </summary>
        /// <param name="threadName">Name of the thread.</param>
        /// <param name="closingWorker">The ClosingWorker to add this thread too</param>
        /// <param name="cvlsPortType">Select the CVLSPortType, can use multiple flags</param>
        /// <param name="serialNumber">The serial number to connect too</param>
        public CVLSThreadedComPort(string threadName, ClosingWorker closingWorker, CVLSPortType cvlsPortType, int serialNumber)
            : base(threadName, closingWorker, CVLSComPort.ComParameters(serialNumber), CVLSComPort.ComMode(cvlsPortType), CVLSComPort.SelectionRule(cvlsPortType))
        {
            InitializeFirmwareUploader();
        }

        /// <summary>
        /// Create a CVLSThreadedComPort for a CVLS unit. This port type should be used when the unit connection
        /// state is unknown. Functions are provided to be notified of connections and status changes. When using
        /// a static configuration, consider using the CVLSComPort classes instead.
        /// </summary>
        /// <param name="threadName">Name of the thread.</param>
        /// <param name="closingWorker">The ClosingWorker to add this thread too</param>
        /// <param name="portName">The port to connect to in format 'COM#'</param>
        public CVLSThreadedComPort(string threadName, ClosingWorker closingWorker, string portName)
            : base(threadName, closingWorker, CVLSComPort.ComParameters(), ConnectionMode.SelectionRule, port => port.Port == portName)
        {
            InitializeFirmwareUploader();
        }

        #endregion

        #region External Functions

        /// <summary>
        /// Protocol object to allow easy access of CVLS functions.
        /// NOTE: This can return null when not connected to a CVLSComPort.
        /// </summary>
        public LegacyProtocol Protocol => CurrentConnection?.Protocol;

        /// <summary>
        /// Protocol object to allow easy access of CVLS functions, echoing all com traffic to the message function.
        /// NOTE: This can return null when not connected to a CVLSComPort.
        /// </summary>
        public LegacyProtocol ProtocolEcho => CurrentConnection?.ProtocolEcho;

        /// <summary>
        /// Port capability for high speed.
        /// </summary>
        /// <returns>True = CVLSComPort is CVLS USB connection, False otherwise</returns>
        public new bool IsHighSpeed()
        {
            return CurrentConnection?.IsHighSpeed() == true;
        }

        /// <summary>
        /// Change the connection method of the CVLSThreadedComPort.
        /// </summary>
        /// <param name="cvlsPortType">Select the CVLSPortType, can use multiple flags</param>
        public void ChangeMode(CVLSPortType cvlsPortType)
        {
            ConnectionParameters.CopyFrom(CVLSComPort.ComParameters());
            base.ChangeMode(CVLSComPort.ComMode(cvlsPortType), CVLSComPort.SelectionRule(cvlsPortType));
        }

        /// <summary>
        /// Change the connection method of the CVLSThreadedComPort.
        /// </summary>
        /// <param name="cvlsPortType">Select the CVLSPortType, can use multiple flags</param>
        /// <param name="serialNumber">The serial number to connect too</param>
        public void ChangeMode(CVLSPortType cvlsPortType, int serialNumber)
        {
            ConnectionParameters.CopyFrom(CVLSComPort.ComParameters(serialNumber));
            base.ChangeMode(CVLSComPort.ComMode(cvlsPortType), CVLSComPort.SelectionRule(cvlsPortType));
        }

        /// <summary>
        /// Change the connection method of the CVLSThreadedComPort.
        /// </summary>
        /// <param name="portName">The port to connect to in format 'COM#'</param>
        public void ChangeMode(string portName)
        {
            ConnectionParameters.CopyFrom(CVLSComPort.ComParameters());
            base.ChangeMode(ConnectionMode.SelectionRule, port => port.Port == portName);
        }
        
        /// <summary>
        /// Function to be overridden by derived classes. Processing of data should be done here.
        /// </summary>
        /// <param name="closingInfo">The closing info object to add children too.</param>
        protected override void AddDerivedClosingInfoChildren(ClosingInfo closingInfo)
        {
            // poke each thread then see if it is complete
            closingInfo.ChildInfo.Add(FirmwareUploader.ShutdownReady());
            closingInfo.ChildInfo.Add(LogDownloader.ShutdownReady());
            closingInfo.ChildInfo.Add(IniDownloader.ShutdownReady());
            closingInfo.ChildInfo.Add(IniUploader.ShutdownReady());
        }

        #endregion

        #region Internal Functions

        private void InitializeFirmwareUploader()
        {
            FirmwareUploader = new FirmwareUploaderSerial(this, "Firmware Uploader Serial", null);
            LogDownloader = new LogDownloaderSerial(this, "Log Downloader Serial", null);
            IniUploader = new IniUploaderSerial(this, "INI Uploader Serial", null);
            IniDownloader = new IniDownloaderSerial(this, "INI Downloader Serial", null);
        }

        #endregion
    }
}
