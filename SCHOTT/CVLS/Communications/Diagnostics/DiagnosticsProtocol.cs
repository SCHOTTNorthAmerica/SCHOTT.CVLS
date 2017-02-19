using SCHOTT.CVLS.Utilities;
using System;
using System.Collections.Generic;

namespace SCHOTT.CVLS.Communications
{
    /// <summary>
    /// DiagnosticsProtocol section of the CVLS Legacy Protocol
    /// </summary>
    public class DiagnosticsProtocol
    {
        private readonly ILegacyProtocol _port;
        private readonly bool _echoComTraffic;

        /// <summary>
        /// Creates a new DiagnosticsProtocol for the given port
        /// </summary>
        /// <param name="port">The port for the protocol to use.</param>
        /// <param name="echoComTraffic">When True, the protocol object will echo com traffic to the subscribed message functions.</param>
        public DiagnosticsProtocol(ILegacyProtocol port, bool echoComTraffic = false)
        {
            _port = port;
            _echoComTraffic = echoComTraffic;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ClearLogs()
        {
            string workingLine;
            return _port.SendCommandSingleTest("&o3", "&o3", out workingLine, echoComTraffic: _echoComTraffic);
        }

        /// <summary>
        /// Gets the number of exceptions currently logged on the system.
        /// </summary>
        /// <returns>Current number of exceptions logged, -1 if there is a com error</returns>
        public int GetLogCount()
        {
            string workingLine;
            if (_port.SendCommandSingleTest("&@e?", "&@e", out workingLine, true, "", _echoComTraffic))
            {
                int value;
                if (int.TryParse(workingLine, out value))
                {
                    return value;
                }
            }
            return -1;
        }

        /// <summary>
        /// Gets the log from the given index.
        /// </summary>
        /// <param name="index">The index to retrieve the log from. Range 0-255</param>
        /// <param name="log">The log from the given index, null if log at location doesn't exist, or there is a comm error.</param>
        /// <returns>True if log command was successfull, false otherwise.</returns>
        public bool GetLog(uint index, out LogData log)
        {
            log = null;

            string workingLine;
            if (_port.SendCommandSingleTest($"&@e{index}", "&@e", out workingLine, true, "", _echoComTraffic))
            {
                uint returnIndex, exception, time;
                var tempArray = workingLine.Split(new[] { ',' }, 4);

                if (tempArray.Length > 1 && uint.TryParse(tempArray[0], out returnIndex) && returnIndex == index &&
                   uint.TryParse(tempArray[1], out exception) && uint.TryParse(tempArray[2], out time))
                {
                    // we have a good log, parse out the rest of it
                    log = new LogData(exception, time, tempArray[3]);
                }

                // command was successfull, so return true
                return true;
            }

            // command failed, return false
            return false;
        }

        /// <summary>
        /// Retrieve the logs from the unit
        /// </summary>
        /// <param name="logData">A list of the logs retreived</param>
        /// <param name="timeoutSeconds">How long to wait before stopping the logs download.</param>
        /// <returns>True if data is ready, false if there were errors</returns>
        public bool GetAllLogs(ref List<LogData> logData, int timeoutSeconds = 5)
        {
            var timeout = DateTime.Now.AddSeconds(timeoutSeconds);
            uint i = 0;
            var failCount = 0;

            logData.Clear();

            while (DateTime.Now < timeout)
            {
                LogData log;
                if (GetLog(i, out log))
                {
                    // good command, clear failCount
                    failCount = 0;

                    if (log != null)
                    {
                        // good log, add to list
                        i++;
                        logData.Add(log);
                    }
                    else
                    {
                        // all logs gathered
                        return true;
                    }
                }
                else
                {
                    // failed command
                    failCount++;

                    if (failCount > 5)
                    {
                        // failed too many times
                        return false;
                    }
                }
            }
            
            // timeout
            return false;
        }
    }
}
