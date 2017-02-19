using SCHOTT.Core.StateMachine;
using SCHOTT.Core.Threading;
using SCHOTT.CVLS.Utilities;

namespace SCHOTT.CVLS.Serial.Workers
{
    /// <summary>
    /// Class to allow the user to download INI files from the connected CVLS unit.
    /// </summary>
    public class IniDownloaderSerial : ThreadedTransferMachine
    {
        private readonly CVLSThreadedComPort _comPortRef;

        #region Variables for DownloadWorker

        private string _fileText;

        #endregion

        /// <summary>
        /// Create a new IniDownloader
        /// </summary>
        /// <param name="comPort">The ThreadedComPort to attach this IniDownloaderSerial too</param>
        /// <param name="threadName">The name to report in closing operations for this binary socket.</param>
        /// <param name="closingWorker">The closing worker to add this binary socket too.</param>
        public IniDownloaderSerial(CVLSThreadedComPort comPort, string threadName, ClosingWorker closingWorker) : base(false, threadName, closingWorker)
        {
            // hook up to the comPort
            _comPortRef = comPort;
        }

        #region Internal Functions

        private bool Initialize()
        {
            // we start assuming the origional page count
            SetPageCount(5);

            _missedPage = 0;
            _missedPageCount = 0;

            if (!_comPortRef.IsConnected)
            {
                SetTransferState(TransferState.FailedConnection);
                return false;
            }

            if (_comPortRef.Protocol?.FirmwareVersion < 1.14)
            {
                // Must be connected to a unit with firmware 1.14 or later to download logs!
                SetTransferState(TransferState.FailedInvalidFirmware);
                return false;
            }

            // request update on counts
            int pageCount = _comPortRef.Protocol?.Configurations.GetIniPageCount() ?? -1;
            if (pageCount != -1)
                SetPageCount(pageCount);

            _fileText = "";
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
        /// A function to download a INI file representing the current unit settings.
        /// </summary>
        /// <param name="fileText">The INI file text will be output in this string.</param>
        /// <param name="timeoutSeconds">Number of seconds to try the download before canceling.</param>
        /// <returns>The current transfer status object.</returns>
        public TransferStatus GetFull(out string fileText, int timeoutSeconds = 5)
        {
            fileText = "";

            if (Start().TransferState > TransferState.Succeeded)
            {
                // failed to start
                return CreateTransferStatus();
            }

            // wait for complete or timeout
            WaitForCompleteOrTimeout(timeoutSeconds);

            // return the text we have with the status
            return GetCurrent(out fileText);
        }

        /// <summary>
        /// Gets the current INI text and lets the user know if the download is successfull.
        /// </summary>
        /// <param name="fileText">The text of the INI received so far.</param>
        /// <returns>The current transfer status object.</returns>
        public TransferStatus GetCurrent(out string fileText)
        {
            fileText = _fileText;

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

            var returnStrings = _comPortRef.SendCommand($"&@i{_currentPage}", 5000);

            // check for response
            if (returnStrings.Count > 0 && returnStrings[0].Contains($"&@i{_currentPage},"))
            {
                if (returnStrings[0].Contains($"&@i{_currentPage},MULTILINECOMPLETE"))
                {
                    // ini downloaded
                    SetTransferState(TransferState.Succeeded);

                    // end the downloader
                    MachineFunctions.JumpToLast(currentStep);
                    return StepReturn.JumpCommandUsed;
                }

                // we have data
                returnStrings[0] = returnStrings[0].Replace($"&@i{_currentPage},", "");
                returnStrings.RemoveRange(returnStrings.Count - 2, 2);
                _fileText += string.Join("\r\n", returnStrings.ToArray());
                _currentPage++;
                return StepReturn.RepeatStep;
            }

            // deal with any errors
            switch (returnStrings[0])
            {
                case "&@i!o":
                    SetTransferState(_currentTransferState, "Buffer overflow!");
                    break;

                case "&@i!v":
                    SetTransferState(_currentTransferState, "Invalid Page Number!");
                    break;

                default:
                    break;
            }

            // we had a bad log, try again
            return ProcessMissedPage(_currentPage);
        }

        #endregion

    }

}
