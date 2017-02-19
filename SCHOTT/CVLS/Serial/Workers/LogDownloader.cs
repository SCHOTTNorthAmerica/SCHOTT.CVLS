using SCHOTT.Core.Extensions;
using SCHOTT.Core.StateMachine;
using SCHOTT.Core.Threading;
using SCHOTT.CVLS.Utilities;
using System.Collections.Generic;

namespace SCHOTT.CVLS.Serial.Workers
{
    /// <summary>
    /// Class to allow the user to download Log files from the connected CVLS unit.
    /// </summary>
    public class LogDownloaderSerial : ThreadedTransferMachine
    {
        private readonly CVLSThreadedComPort _comPortRef;

        #region Variables for TransferWorker

        private readonly List<LogData> _logs = new List<LogData>();

        #endregion

        /// <summary>
        /// Create a new LogDownloader
        /// </summary>
        /// <param name="comPort">The ThreadedComPort to attach this LogDownloaderSerial too</param>
        /// <param name="threadName">The name to report in closing operations for this binary socket.</param>
        /// <param name="closingWorker">The closing worker to add this binary socket too.</param>
        public LogDownloaderSerial(CVLSThreadedComPort comPort, string threadName, ClosingWorker closingWorker) : base(false, threadName, closingWorker)
        {
            // hook up to the comPort
            _comPortRef = comPort;
        }

        #region Internal Functions

        private bool Initialize()
        {
            SetPageCount(256);

            _missedPage = 0;
            _missedPageCount = 0;

            if (_comPortRef.Protocol?.FirmwareVersion < 1.14)
            {
                // Must be connected to a unit with firmware 1.14 or later to download logs!
                SetTransferState(TransferState.FailedInvalidFirmware);
                return false;
            }

            // request update on counts
            int pageCount = _comPortRef.Protocol?.Diagnostics.GetLogCount() ?? -1;
            if (pageCount != -1)
                SetPageCount(pageCount);

            _logs.Clear();
            _currentPage = 0;

            SetTransferState(TransferState.Running);
            return true;
        }

        #endregion

        #region External Functions

        /// <summary>
        /// Start the transfer process
        /// </summary>
        /// <returns>The current transfer status object.</returns>
        public TransferStatus Start()
        {
            // make sure the worker is stopped and initialized for a new transfer
            if (!Stop() || !Initialize())
                return CreateTransferStatus();

            // start the transfer
            if (base.Start())
                return CreateTransferStatus();

            // let the user know
            SetTransferState(TransferState.FailedStart);

            // return the status
            return CreateTransferStatus();
        }

        /// <summary>
        /// A function to download all logs and wait for completion.
        /// </summary>
        /// <param name="logData">The INI file text will be output in this string.</param>
        /// <param name="timeoutSeconds">Number of seconds to try the download before canceling.</param>
        /// <returns>The current transfer status object.</returns>
        public TransferStatus GetAllLogs(ref List<LogData> logData, int timeoutSeconds = 5)
        {
            logData.Clear();

            if (Start().TransferState > TransferState.Succeeded)
            {
                // failed to start
                return CreateTransferStatus();
            }

            // wait for complete or timeout
            WaitForCompleteOrTimeout(timeoutSeconds);

            // return the text we have with the status
            return GetCurrentLogs(ref logData);
        }

        /// <summary>
        /// Gets the current logs and lets the user know if the download is successfull.
        /// </summary>
        /// <param name="logData">A list of the logs retreived</param>
        /// <returns>The current transfer status object.</returns>
        public TransferStatus GetCurrentLogs(ref List<LogData> logData)
        {
            // clear the storage list, then add the current logs to it
            logData.Clear();
            logData.AddRange(_logs.CloneList());

            // return the current status
            return CreateTransferStatus();
        }

        #endregion

        #region StateMachine Functions (Add new steps here)

        protected override bool StateMachine_TransferPacket(StepDefinition currentStep)
        {
            // check for connection
            if (!_comPortRef.IsConnected)
            {
                SetTransferState(TransferState.FailedConnection);
                MachineFunctions.JumpToLast(currentStep);
                return StepReturn.JumpCommandUsed;
            }

            // update the users with % complete
            RunPercentUpdate();
            
            LogData log = null;
            if (_comPortRef.Protocol?.Diagnostics.GetLog(_currentPage, out log) == true)
            {
                if (log != null)
                {
                    // good log, add to list
                    _currentPage++;
                    _logs.Add(log);
                }
                else
                {
                    // all logs gathered
                    SetTransferState(TransferState.Succeeded);

                    // end the downloader
                    MachineFunctions.JumpToLast(currentStep);
                    return StepReturn.JumpCommandUsed;
                }
            }

            // we had a bad log, try again
            return ProcessMissedPage(_currentPage);
        }

        #endregion

    }

}
