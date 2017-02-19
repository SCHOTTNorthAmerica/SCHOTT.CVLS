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
    /// Class to allow the user to upload firmware to the connected CVLS unit.
    /// </summary>
    public class FirmwareUploader : ThreadedTransferMachine
    {
        private readonly BinarySocket _binarySocketRef;

        #region Variables for FirmwareUploader

        private const int PageSize = 256;
        private const int PagesTransmitted = 4;

        private readonly List<byte> _workingFile = new List<byte>();
        private readonly List<byte> _payload = new List<byte>();
        private readonly List<byte> _receivedData = new List<byte>();
        private int _pointer;
        private int _readLength;
        private int _payloadPage;

        private BinaryCommand _writeCommand = new BinaryCommand(CommandSets.Admin, (ushort)AdminCommands.AdminFirmware, true);
        private BinaryCommand _bootloaderCommand = new BinaryCommand(CommandSets.Admin, (ushort)AdminCommands.AdminFirmwareLoad, true);

        #endregion

        /// <summary>
        /// Create a new FirmwareUploader
        /// </summary>
        /// <param name="binarySocket">The BinarySocket to attach this FirmwareUploader too</param>
        /// <param name="threadName">The name to report in closing operations for this binary socket.</param>
        /// <param name="closingWorker">The closing worker to add this binary socket too.</param>
        public FirmwareUploader(BinarySocket binarySocket, string threadName, ClosingWorker closingWorker) : base(true, threadName, closingWorker)
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

            // check the embedded firmware size against the actuall firmware size
            if (_workingFile.Count > 16 && DataConversions.ConvertListToUint32(_workingFile.GetRange(12, 4)) != _workingFile.Count)
            {
                // Firmware file size does not match header!
                SetTransferState(TransferState.FailedInvalidFile);
                return false;
            }

            _pointer = 0;
            _currentPage = 0;
            _payloadPage = 0xFFFF;

            SetTransferState(TransferState.Running);
            return true;
        }

        #endregion

        #region External Functions

        /// <summary>
        /// Change the commands that will used by this firmware uploader
        /// </summary>
        /// <param name="writeCommand"></param>
        /// <param name="bootloaderCommand"></param>
        public void ChangeCommands(BinaryCommand writeCommand, BinaryCommand bootloaderCommand)
        {
            _writeCommand = writeCommand;
            _bootloaderCommand = bootloaderCommand;
        }

        /// <summary>
        /// Start the transfer process
        /// </summary>
        /// <param name="firmwareStream">The firmware to stream to the CVLS unit. 
        /// Firmware streams can be accessed using embedded resources like: Start(CustomerFirmware.Streams["1.14"])</param>
        /// <returns>The current transfer status object.</returns>
        public TransferStatus Start(Stream firmwareStream)
        {
            // make sure the worker is stopped and initialized for a new transfer
            if (!Stop() || !Initialize(firmwareStream))
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
        /// Process the received data from the attached BinarySocket
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

                // determine next step
                if (_pointer == _workingFile.Count)
                {
                    // we have reached the end of the file, so tell the system we are done
                    _binarySocketRef.SendBinaryCommand(_bootloaderCommand);

                    // end state machine
                    SetTransferState(TransferState.Succeeded);
                    Stop();
                    return;
                }

                // not done with file, send the next page
                MachineFunctions.JumpToStep("Transfer Packet", WorkerStateMachine);
                return;
            }

            // process the missed packet count
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
            _binarySocketRef.SendBinaryCommand(_writeCommand, _payload);

            // continue to next
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
