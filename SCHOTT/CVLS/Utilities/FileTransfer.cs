using SCHOTT.Core.StateMachine;
using SCHOTT.Core.Threading;
using SCHOTT.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SCHOTT.CVLS.Utilities
{
    /// <summary>
    /// The current file state
    /// </summary>
    public enum TransferState
    {
        /// <summary>
        /// Transfer is currently in process
        /// </summary>
        Running,

        /// <summary>
        /// Transfer is stopped
        /// </summary>
        Succeeded,

        /// <summary>
        /// Transfer failed
        /// </summary>
        Failed,

        /// <summary>
        /// Transfer lost too many packets
        /// </summary>
        FailedLostPackets,

        /// <summary>
        /// Transfer timed out
        /// </summary>
        FailedTimeOut,

        /// <summary>
        /// File format is not valid
        /// </summary>
        FailedInvalidFile,

        /// <summary>
        /// No connection to unit
        /// </summary>
        FailedConnection,
        
        /// <summary>
        /// The current firmware does not support this feature
        /// </summary>
        FailedInvalidFirmware,

        /// <summary>
        /// Failed to initialize the new transfer
        /// </summary>
        FailedInitialize,

        /// <summary>
        /// Unable to stop the current transfer
        /// </summary>
        FailedStop,

        /// <summary>
        /// Unable to start the transfer
        /// </summary>
        FailedStart
    }
    
    /// <summary>
    /// Class that holds information on socket transferes
    /// </summary>
    public class TransferStatus
    {
        /// <summary>
        /// Total bytes in the requested transfer.
        /// </summary>
        public int BytesTotal { get; private set; }

        /// <summary>
        /// Total bytes in the requested transfer.
        /// </summary>
        public int BytesTransfered { get; private set; }

        /// <summary>
        /// A percentage conversion of the total data in the requested transfer.
        /// </summary>
        public int PercentTransfered { get; private set; }

        /// <summary>
        /// The current state of the transfer
        /// </summary>
        public TransferState TransferState { get; private set; }

        /// <summary>
        /// The current state of the transfer
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Create a TransferStatus object.
        /// </summary>
        /// <param name="bytesTotal">Total bytes in the requested transfer.</param>
        /// <param name="bytesTransfered">Total bytes sent in the requested transfer.</param>
        /// <param name="percentTransfered">A percentage conversion of the total data sent in the requested transfer.</param>
        /// <param name="transferState">The current state of the transfer</param>
        /// <param name="message">A message to display with the update</param>
        public TransferStatus(int bytesTotal, int bytesTransfered, int percentTransfered, TransferState transferState, string message = "")
        {
            BytesTotal = bytesTotal;
            BytesTransfered = bytesTransfered;
            PercentTransfered = percentTransfered;
            TransferState = transferState;
            Message = message;

            if (message.Length > 0)
                return;

            switch (transferState)
            {
                case TransferState.Failed:
                    Message = "Unknown Failure";
                    break;

                case TransferState.Running:
                    Message = "Currently transfer pages.";
                    break;

                case TransferState.Succeeded:
                    Message = "Transfer successfully completed.";
                    break;

                case TransferState.FailedLostPackets:
                    Message = "Too many missed packets.";
                    break;

                case TransferState.FailedTimeOut:
                    Message = "Too many packet timeouts.";
                    break;

                case TransferState.FailedInvalidFile:
                    Message = "File is not the correct format.";
                    break;

                case TransferState.FailedConnection:
                    Message = "Unable to connect to unit.";
                    break;

                case TransferState.FailedInvalidFirmware:
                    Message = "Firmware does not support this feature, please upgrade the unit firmware.";
                    break;

                case TransferState.FailedInitialize:
                    Message = "Unable to initialize transfer.";
                    break;

                case TransferState.FailedStop:
                    Message = "Unable to stop current transfer.";
                    break;

                case TransferState.FailedStart:
                    Message = "Unable to start current transfer.";
                    break;

                default:
                    break;
            }
        }
    }

    /// <summary>
    /// A class to provide common functions for a ThreadedTransferMachine
    /// </summary>
    public class ThreadedTransferMachine : ThreadedStateMachine
    {
        /// <summary>
        /// The internal MessageBroker for this FirmwareUploader
        /// </summary>
        private readonly MessageBroker _messageBroker = new MessageBroker();

        /// <summary>
        /// Register to updates of the Firmware uploader.
        /// </summary>
        /// <param name="context">The context in which to run the update action.</param>
        /// <param name="action">The lambda expression to execute on updates.</param>
        public void RegisterPercentUpdate(MessageBroker.MessageContext context, Action<TransferStatus> action)
        {
            _messageBroker.Register("PercentUpdate", context, action);
        }

        protected void RunPercentUpdate()
        {
            _messageBroker.RunActions("PercentUpdate", CreateTransferStatus());
        }

        #region Variables for TransferWorker

        protected TransferState _currentTransferState = TransferState.Succeeded;
        protected string _currentTransferMessage = "";
        protected ushort _currentPage;
        protected int _missedPage;
        protected int _missedPageCount;
        protected int _pageCount;
        protected double _pageCountInv;

        #endregion

        /// <summary>
        /// Create a new LogDownloader
        /// </summary>
        /// <param name="isAsyncTransfer">Determines if this machine will be set up asyncronus transfers.</param>
        /// <param name="threadName">The name to report in closing operations for this binary socket.</param>
        /// <param name="closingWorker">The closing worker to add this binary socket too.</param>
        protected ThreadedTransferMachine(bool isAsyncTransfer, string threadName, ClosingWorker closingWorker) : base(threadName)
        {
            // add thread to the closing worker
            closingWorker?.AddThread(this);

            // initialize the StateMachine
            WorkerState = new StateDefinition();

            if (isAsyncTransfer)
            {
                MachineFunctions.InitializeStates(WorkerStateMachine = new StepDefinition(threadName, true, WorkerState)
                {
                    #region StateMachine
                    SubSteps = new List<StepDefinition>
                    {
                        new StepDefinition("Transfer Packet") { Delegate = StateMachine_TransferPacket, DelayTimeMs = 500 },
                        new StepDefinition("Packet Timeout") { Delegate = StateMachine_PacketTimeout, DelayTimeMs = 0 },
                        new StepDefinition("Complete") { Delegate = StateMachine_Complete, DelayTimeMs = 200 }
                    }
                    #endregion // StateMachine
                });
            }
            else
            {
                MachineFunctions.InitializeStates(WorkerStateMachine = new StepDefinition(threadName, true, WorkerState)
                {
                    #region StateMachine
                    SubSteps = new List<StepDefinition>
                    {
                        new StepDefinition("Transfer Packet") { Delegate = StateMachine_TransferPacket, DelayTimeMs = 0 },
                        new StepDefinition("Complete") { Delegate = StateMachine_Complete, DelayTimeMs = 200 }
                    }
                    #endregion // StateMachine
                });
            }

            // start the StateMachine in the Complete state
            MachineFunctions.JumpToLast(WorkerStateMachine);

            // initialize ThreadingBase
            WorkerThreads = new List<ThreadInfo>
            {
                new ThreadInfo(new Thread(Worker), "State Machine", threadName, WorkerStateMachine)
            };
        }

        #region Internal Functions

        protected void SetTransferState(TransferState state, string message = "")
        {
            _currentTransferMessage = message;
            _currentTransferState = state;
            RunPercentUpdate();
        }

        protected void WaitForCompleteOrTimeout(int timeoutSeconds)
        {
            var timeout = DateTime.Now.AddSeconds(timeoutSeconds);
            while (_currentTransferState == TransferState.Running)
            {
                if (DateTime.Now > timeout)
                {
                    // timed out
                    SetTransferState(TransferState.FailedTimeOut);
                    Stop();
                    break;
                }

                TimeFunctions.Wait(50);
            }
        }

        protected bool ProcessMissedPage(int page)
        {
            if (page == _missedPage)
            {
                _missedPageCount++;
            }
            else
            {
                _missedPage = page;
                _missedPageCount = 1;
            }

            // decide how to proceed
            if (_missedPageCount <= 5)
            {
                // request packet again
                MachineFunctions.JumpToStep("Transfer Packet", WorkerStateMachine);
                return StepReturn.JumpCommandUsed;
            }

            // failed the upload, jump to end
            SetTransferState(TransferState.FailedLostPackets);

            MachineFunctions.JumpToLast(WorkerStateMachine);
            return StepReturn.JumpCommandUsed;
        }

        protected TransferStatus CreateTransferStatus()
        {
            return new TransferStatus(_pageCount, _currentPage, (int)(_currentPage * _pageCountInv), _currentTransferState, _currentTransferMessage);
        }

        #endregion

        #region External Functions

        /// <summary>
        /// Gets the current transfer status of this Firmware Uploader object 
        /// </summary>
        /// <returns>The current TransferStatus</returns>
        public TransferStatus TransferStatus => CreateTransferStatus();

        /// <summary>
        /// Updates the page count for reporting callbacks
        /// </summary>
        /// <param name="count">Sets the number of pages to expect.</param>
        public void SetPageCount(int count)
        {
            _pageCount = count;

            if (count > 0)
                _pageCountInv = 100.0 / _pageCount;
        }

        /// <summary>
        /// Stop the worker thread.
        /// </summary>
        /// <param name="timeoutMilliseconds">Number of milliseconds to wait for state machine to Stop.</param>
        /// <returns>True if state machine is stoped, false otherwise.</returns>
        public new bool Stop(int timeoutMilliseconds = 50)
        {
            if (base.Stop(timeoutMilliseconds))
                return true;

            SetTransferState(TransferState.FailedStop);
            return false;
        }

        #endregion

        #region StateMachine Functions (Add new steps here)

        protected virtual bool StateMachine_TransferPacket(StepDefinition currentStep)
        {
            return StepReturn.ContinueToNext;
        }

        protected virtual bool StateMachine_PacketTimeout(StepDefinition currentStep)
        {
            return StepReturn.ContinueToNext;
        }

        protected bool StateMachine_Complete(StepDefinition currentStep)
        {
            if (IsFirstPass(currentStep))
            {
                currentStep.FailCount++;

                if (_currentTransferState == TransferState.Running)
                    SetTransferState(TransferState.Failed);
            }

            if (WorkerState.ThreadClosing)
            {
                WorkerState.StateMachineComplete = true;
            }
            return StepReturn.RepeatStep;
        }

        #endregion

    }

}
