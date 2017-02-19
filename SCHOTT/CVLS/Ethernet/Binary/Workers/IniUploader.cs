using SCHOTT.Core.Extensions;
using SCHOTT.Core.StateMachine;
using SCHOTT.Core.Threading;
using SCHOTT.CVLS.Ethernet.Binary.Enums;
using SCHOTT.CVLS.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SCHOTT.CVLS.Ethernet.Binary.Workers
{
    /// <summary>
    /// Class to allow the user to upload INI files to the connected CVLS unit.
    /// </summary>
    public class IniUploader : ThreadedTransferMachine
    {
        private readonly BinarySocket _binarySocketRef;

        #region Variables for INIUploadWorker

        private const int PageSize = 1024;
        private const int PagesTransmitted = 1;

        private readonly List<byte> _workingFile = new List<byte>();
        private readonly List<byte> _payload = new List<byte>();
        private readonly List<byte> _receivedData = new List<byte>();
        private int _pointer;
        private int _readLength;
        private int _payloadPage;

        #endregion

        /// <summary>
        /// Create a new IniUploader
        /// </summary>
        /// <param name="binarySocket">The BinarySocket to attach this IniUploader too</param>
        /// <param name="threadName">The name to report in closing operations for this binary socket.</param>
        /// <param name="closingWorker">The closing worker to add this binary socket too.</param>
        public IniUploader(BinarySocket binarySocket, string threadName, ClosingWorker closingWorker) : base(true, threadName, closingWorker)
        {
            // hook up to the BinarySocket
            _binarySocketRef = binarySocket;
        }

        #region Internal Functions
        
        private bool Initialize(Stream firmwareStream)
        {
            _missedPage = 0;
            _missedPageCount = 0;

            if (!_binarySocketRef.IsConnected)
            {
                SetTransferState(TransferState.FailedConnection);
                return false;
            }

            _workingFile.Clear();
            _workingFile.AddRange(firmwareStream.ToByteArray());

            // calculate page size
            SetPageCount((int)Math.Ceiling(_workingFile.Count / (double)PageSize));

            _pointer = 0;
            _currentPage = 0;
            _payloadPage = 0xFFFF;

            SetTransferState(TransferState.Running);
            return true;
        }

        #endregion

        #region External Functions

        /// <summary>
        /// Start the INI upload process
        /// </summary>
        /// <param name="stream">The INI file to stream to the CVLS unit</param>
        /// <returns>The current transfer status object.</returns>
        public TransferStatus Start(Stream stream)
        {
            // make sure the worker is stopped and initialized for a new transfer
            if (!Stop() || !Initialize(stream))
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
        /// Start the INI upload process and wait for completion
        /// </summary>
        /// <param name="iniFile">The file to stream to the CVLS unit</param>
        /// <param name="timeoutSeconds">How long to wait before stopping the firmware upload. Normal upload time is ~1 seconds.</param>
        /// <returns>True if upload succeeded. False if upload timed out.</returns>
        public TransferStatus UploadFull(Stream iniFile, int timeoutSeconds = 5)
        {
            if (Start(iniFile).TransferState > TransferState.Succeeded)
            {
                // failed to start
                return CreateTransferStatus();
            }

            // wait for complete or timeout
            WaitForCompleteOrTimeout(timeoutSeconds);

            // return the status
            return CreateTransferStatus();
        }

        /// <summary>
        /// Process the data received by the attached BinarySocket
        /// </summary>
        /// <param name="page">Which page was returned</param>
        /// <param name="data">The data that was written to the page</param>
        public void ReceiveData(ushort page, List<byte> data)
        {
            _receivedData.Clear();
            _receivedData.AddRange(DataConversions.ConvertUInt16ToList(page));
            _receivedData.AddRange(data.CloneList());

            if (_payload.SequenceEqual(_receivedData))
            {
                // housekeeping variables
                _pointer += _readLength;
                _currentPage += PagesTransmitted;

                // send the next page
                MachineFunctions.JumpToStep("Transfer Packet", WorkerStateMachine);
                return;
            }

            // process the missed packet count
            ProcessMissedPage(_currentPage);
        }

        /// <summary>
        /// Flag upload as successfull
        /// </summary>
        public void ReceiveUploadSuccess()
        {
            SetTransferState(TransferState.Succeeded);
            MachineFunctions.JumpToLast(WorkerStateMachine);
        }

        /// <summary>
        /// Process Logs from the upload
        /// </summary>
        /// <param name="data"></param>
        public void ReceiveUploadLogs(List<byte> data)
        {
            SetTransferState(TransferState.FailedInvalidFile, $"INI Upload Failed! The INI file had the following errors:{Environment.NewLine}{System.Text.Encoding.UTF8.GetString(data.ToArray())}");
            MachineFunctions.JumpToLast(WorkerStateMachine);
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

            if (_pointer < _workingFile.Count)
            {
                if (_payloadPage != _currentPage)
                {
                    // calculate read length
                    _readLength = PageSize * PagesTransmitted;
                    if (_pointer + _readLength > _workingFile.Count)
                        _readLength = _workingFile.Count - _pointer;

                    // load the array into a list with page information
                    _payload.Clear();
                    _payload.AddRange(DataConversions.ConvertUInt16ToList(_currentPage));
                    _payload.AddRange(_workingFile.GetRange(_pointer, _readLength));

                    // update the housekeeping variables
                    _payloadPage = _currentPage;
                }

                // send the data
                _binarySocketRef.SendBinaryCommand(CommandSets.Admin, (ushort)AdminCommands.AdminConfigImport, true, _payload);
            }
            else
            {
                _binarySocketRef.SendBinaryCommand(CommandSets.Admin, (ushort)AdminCommands.AdminConfigImportComplete, true);
            }

            // wait for packet
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
