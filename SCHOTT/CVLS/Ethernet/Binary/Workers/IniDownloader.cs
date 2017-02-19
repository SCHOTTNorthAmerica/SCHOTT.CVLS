using SCHOTT.Core.StateMachine;
using SCHOTT.Core.Threading;
using SCHOTT.CVLS.Ethernet.Binary.Enums;
using SCHOTT.CVLS.Utilities;
using System.Collections.Generic;

namespace SCHOTT.CVLS.Ethernet.Binary.Workers
{
    /// <summary>
    /// Class to allow the user to download INI files from the connected CVLS unit.
    /// </summary>
    public class IniDownloader : ThreadedTransferMachine
    {
        private readonly BinarySocket _binarySocketRef;

        #region Variables for DownloadWorker

        private readonly List<byte> _receivedData = new List<byte>();

        #endregion

        /// <summary>
        /// Create a new IniDownloader
        /// </summary>
        /// <param name="binarySocket">The BinarySocket to attach this IniDownloader too</param>
        /// <param name="threadName">The name to report in closing operations for this binary socket.</param>
        /// <param name="closingWorker">The closing worker to add this binary socket too.</param>
        public IniDownloader(BinarySocket binarySocket, string threadName, ClosingWorker closingWorker) : base(true, threadName, closingWorker)
        {
            // hook up to the BinarySocket
            _binarySocketRef = binarySocket;
        }

        #region Internal Functions

        private bool Initialize()
        {
            // we start assuming the origional page count
            SetPageCount(5);

            _missedPage = 0;
            _missedPageCount = 0;

            if (!_binarySocketRef.IsConnected)
            {
                SetTransferState(TransferState.FailedConnection);
                return false;
            }

            // request update on counts
            _binarySocketRef.SendBinaryCommand(CommandSets.Admin, (ushort)AdminCommands.AdminConfigExportCount, true);

            _receivedData.Clear();
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
        /// <param name="iniText">The text of the INI received so far.</param>
        /// <returns>The current transfer status object.</returns>
        public TransferStatus GetCurrent(out string iniText)
        {
            iniText = System.Text.Encoding.UTF8.GetString(_receivedData.ToArray());

            // return the current status
            return CreateTransferStatus();
        }

        /// <summary>
        /// Process the data received by the attached BinarySocket
        /// </summary>
        /// <param name="page">Which page was returned</param>
        /// <param name="data">The data that was written to the page</param>
        public void ReceiveData(ushort page, List<byte> data)
        {
            if (page == _currentPage)
            {
                if (data.Count > 0)
                {
                    // we have a new page
                    _receivedData.AddRange(data);
                    _currentPage++;
                    MachineFunctions.JumpToStep("Transfer Packet", WorkerStateMachine);
                    return;
                }

                // transfer complete, set state
                SetTransferState(TransferState.Succeeded);

                // end the transfer
                Stop();
                return;
            }

            // we had a bad packet, try again
            ProcessMissedPage(_currentPage);
        }

        #endregion

        #region StateMachine Functions (Add new steps here)

        protected override bool StateMachine_TransferPacket(StepDefinition currentStep)
        {
            // check for connection
            if (!_binarySocketRef.IsConnected)
            {
                SetTransferState(TransferState.FailedConnection);
                MachineFunctions.JumpToLast(currentStep);
                return StepReturn.JumpCommandUsed;
            }

            // update the users with % complete
            RunPercentUpdate();

            _binarySocketRef.SendBinaryCommand(CommandSets.Admin, (ushort)AdminCommands.AdminConfigExport,
                   true, DataConversions.ConvertUInt16ToList(_currentPage));

            return StepReturn.ContinueToNext;
        }

        protected override bool StateMachine_PacketTimeout(StepDefinition currentStep)
        {
            // process the missed packet count
            return ProcessMissedPage(_currentPage);
        }

        #endregion

    }

}
