using SCHOTT.Core.Extensions;
using SCHOTT.Core.StateMachine;
using SCHOTT.Core.Threading;
using SCHOTT.CVLS.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SCHOTT.CVLS.Serial.Workers
{
    /// <summary>
    /// Class to allow the user to upload firmware to the connected CVLS unit.
    /// </summary>
    public class FirmwareUploaderSerial : ThreadedTransferMachine
    {
        private readonly CVLSThreadedComPort _comPortRef;

        #region Variables for FirmwareUploaderSerial

        private const int PageSize = 256;
        private const int PagesTransmitted = 4;

        private readonly List<byte> _workingFile = new List<byte>();
        private readonly List<byte> _payload = new List<byte>();
        private int _pointer;
        private int _readLength;
        private ushort _payloadPage;

        private readonly string _command = "&@f";

        #endregion

        /// <summary>
        /// Create a new FirmwareUploaderSerial
        /// </summary>
        /// <param name="comPort">The ThreadedComPort to attach this FirmwareUploaderSerial too</param>
        /// <param name="threadName">The name to report in closing operations for this binary socket.</param>
        /// <param name="closingWorker">The closing worker to add this binary socket too.</param>
        public FirmwareUploaderSerial(CVLSThreadedComPort comPort, string threadName, ClosingWorker closingWorker) : base(false, threadName, closingWorker)
        {
            // hook up to the comPort
            _comPortRef = comPort;
        }

        #region Internal Functions

        private bool Initialize(Stream firmwareStream)
        {
            _missedPage = 0;
            _missedPageCount = 0;

            if (_comPortRef.Protocol?.FirmwareVersion < 1.14)
            {
                // Must be connected to a unit with firmware 1.14 or later to upload firmware!
                SetTransferState(TransferState.FailedInvalidFirmware);
                return false;
            }

            _workingFile.Clear();
            _workingFile.AddRange(firmwareStream.ToByteArray());

            // calculate page size
            SetPageCount((int)Math.Ceiling(_workingFile.Count/(double)PageSize));

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

            if (_payloadPage != _currentPage)
            {
                // calculate read length
                _readLength = PageSize * PagesTransmitted;
                if (_pointer + _readLength > _workingFile.Count)
                    _readLength = _workingFile.Count - _pointer;

                // load the array into a list with _page information
                _payload.Clear();
                _payload.AddRange(DataConversions.ConvertUInt16ToList(_currentPage));
                _payload.AddRange(_workingFile.GetRange(_pointer, _readLength));
                _payload.AddRange(Checksums.Fletcher16(_payload));

                // add escapes to the payload
                _payload.EscapeList();

                // add the command to the front of the payload
                _payload.InsertRange(0, Encoding.ASCII.GetBytes(_command));

                // update the housekeeping variables
                _payloadPage = _currentPage;
            }

            // send the data
            var returnString = _comPortRef.SendCommand(_payload, 1).FirstOrDefault();

            if (returnString?.Contains($"{_command}{_currentPage},{_readLength}") == true)
            {
                // housekeeping variables
                _pointer += _readLength;
                _currentPage += PagesTransmitted;

                // we had a good transfer, determine next step
                if (_pointer == _workingFile.Count)
                {
                    // we are at the end of the file, so tell the system we are done
                    _payload.Clear();
                    _payload.AddRange(DataConversions.ConvertUInt16ToList(0xFFFF));
                    _payload.AddRange(Checksums.Fletcher16(_payload));

                    // add escapes to the payload
                    _payload.EscapeList();

                    // add the command to the front of the payload
                    _payload.InsertRange(0, Encoding.ASCII.GetBytes(_command));

                    // send command
                    _comPortRef.SendCommand(_payload, 1);

                    // let the user know
                    SetTransferState(TransferState.Succeeded);

                    // move to end
                    MachineFunctions.JumpToLast(currentStep);
                    return StepReturn.JumpCommandUsed;
                }

                // not done with file, so send the next packet
                return StepReturn.RepeatStep;
            }

            // switch to deal with packet error types
            switch (returnString)
            {
                case "&@f!c":
                    // checksum error
                    break;

                case "&@f!w":
                    // flash write error
                    break;

                case "&@f!r":
                    // rebooting unit
                    break;

                case "":
                    // lost connection
                    break;

                default:
                    break;
            }

            // process the missed packet count
            return ProcessMissedPage(_currentPage);
        }
        
        #endregion
    }

}
