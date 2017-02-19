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
    /// Class to allow the user to upload INI files to the connected CVLS unit.
    /// </summary>
    public class IniUploaderSerial : ThreadedTransferMachine
    {
        private readonly CVLSThreadedComPort _comPortRef;

        #region Variables for INIUploadWorker

        private const int PageSize = 1024;
        private const int PagesTransmitted = 1;

        private readonly List<byte> _workingFile = new List<byte>();
        private readonly List<byte> _payload = new List<byte>();
        private readonly List<byte> _receivedData = new List<byte>();
        private int _pointer;
        private int _readLength;
        private int _payloadPage;

        private const string Command = "&@u";

        #endregion

        /// <summary>
        /// Create a new IniUploader
        /// </summary>
        /// <param name="comPort">The ThreadedComPort to attach this IniUploaderSerial too</param>
        /// <param name="threadName">The name to report in closing operations for this binary socket.</param>
        /// <param name="closingWorker">The closing worker to add this binary socket too.</param>
        public IniUploaderSerial(CVLSThreadedComPort comPort, string threadName, ClosingWorker closingWorker) : base(false, threadName, closingWorker)
        {
            // hook up to the comPort
            _comPortRef = comPort;
        }

        #region Internal Functions

        private bool Initialize(Stream firmwareStream)
        {
            _missedPage = 0;
            _missedPageCount = 0;

            if (!_comPortRef.IsConnected)
            {
                SetTransferState(TransferState.FailedConnection);
                return false;
            }

            _workingFile.Clear();
            _workingFile.AddRange(firmwareStream.ToByteArray());

            // calculate _currentPage size
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

            var payloadLength = Math.Min(_workingFile.Count - _pointer, 1024);

            if (_payloadPage != _currentPage)
            {
                // calculate read length
                _readLength = PageSize * PagesTransmitted;
                if (_pointer + _readLength > _workingFile.Count)
                    _readLength = _workingFile.Count - _pointer;

                // load the array into a list with _currentPage information
                _payload.Clear();
                _payload.AddRange(DataConversions.ConvertUInt16ToList(_currentPage));
                _payload.AddRange(_workingFile.GetRange(_pointer, _readLength));
                _payload.AddRange(Checksums.Fletcher16(_payload));

                // add escapes to the payload
                _payload.EscapeList();

                // add the command to the front of the payload
                _payload.InsertRange(0, Encoding.ASCII.GetBytes(Command));

                // update the housekeeping variables
                _payloadPage = _currentPage;
            }

            // send the data
            var returnString = _comPortRef.SendCommand(_payload, 1).FirstOrDefault();

            if (returnString?.Contains($"{Command}{_currentPage},{payloadLength}") == true)
            {
                // housekeeping variables
                _pointer += payloadLength;
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
                    _payload.InsertRange(0, Encoding.ASCII.GetBytes(Command));

                    // send command
                    var returnStrings = _comPortRef.SendCommand(_payload, 5000);

                    if (returnStrings.Count > 0)
                    {
                        if (returnStrings[0].Contains("&@us"))
                        {
                            // upload successfull
                            SetTransferState(TransferState.Succeeded);

                            // move to end
                            MachineFunctions.JumpToLast(currentStep);
                            return StepReturn.JumpCommandUsed;
                        }

                        if (returnStrings[0].Contains("&@ue"))
                        {
                            // file error
                            returnStrings[0] = returnStrings[0].Replace("&@ue", "");
                            returnStrings.RemoveRange(returnStrings.Count - 2, 2);
                            var substring = string.Join("\r\n", returnStrings.ToArray());
                            SetTransferState(TransferState.FailedInvalidFile, $"INI Upload Failed! The INI file had the following errors:{Environment.NewLine}{substring}");

                            // move to end
                            MachineFunctions.JumpToLast(currentStep);
                            return StepReturn.JumpCommandUsed;
                        }
                    }
                    else
                    {
                        SetTransferState(TransferState.Failed, "Unable to parse INI file, unknown error!");

                        // move to end
                        MachineFunctions.JumpToLast(currentStep);
                        return StepReturn.JumpCommandUsed;
                    }

                }

                // next loop
                return StepReturn.RepeatStep;
            }

            // switch to deal with packet error types
            switch (returnString)
            {
                case "&@u!c":
                    SetTransferState(_currentTransferState, "Checksum Error!");
                    break;

                case "&@u!w":
                    SetTransferState(_currentTransferState, "Data Processing Error");
                    break;

                case "&@u!s":
                    SetTransferState(_currentTransferState, "Upload Complete");
                    break;

                case "&@u!e":
                    SetTransferState(_currentTransferState, "Upload Error");
                    break;

                case "":
                    SetTransferState(_currentTransferState, "Lost Connection");
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
