using SCHOTT.Core.Extensions;
using SCHOTT.CVLS.Serial.Workers;
using SCHOTT.CVLS.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SCHOTT.CVLS.Communications
{
    /// <summary>
    /// ConfigurationProtocol section of the CVLS Legacy Protocol
    /// </summary>
    public class ConfigurationProtocol
    {
        private readonly ILegacyProtocol _port;
        private readonly bool _echoComTraffic;

        /// <summary>
        /// Creates a new ConfigurationProtocol for the given port
        /// </summary>
        /// <param name="port">The port for the protocol to use.</param>
        /// <param name="echoComTraffic">When True, the protocol object will echo com traffic to the subscribed message functions.</param>
        public ConfigurationProtocol(ILegacyProtocol port, bool echoComTraffic = false)
        {
            _port = port;
            _echoComTraffic = echoComTraffic;
        }

        /// <summary>
        /// Saves the current settings to unit memory
        /// </summary>
        /// <returns>True if save is successful, false if failed.</returns>
        public bool SettingsSave()
        {
            string workingLine;
            return _port.SendCommandSingleTest("&s", "&s", out workingLine, echoComTraffic: _echoComTraffic);
        }

        /// <summary>
        /// Overwrites current settings with the last saved settings in the unit.
        /// </summary>
        /// <returns>True if restore is successful, false if failed.</returns>
        public bool SettingsRestore()
        {
            string workingLine;
            return _port.SendCommandSingleTest("&t", "&t", out workingLine, echoComTraffic: _echoComTraffic);
        }

        /// <summary>
        /// Overwrites current settings with the factory settings in the unit.
        /// </summary>
        /// <returns>True if restore is successful, false if failed.</returns>
        public bool SettingsRestoreFactory()
        {
            string workingLine;
            return _port.SendCommandSingleTest("&o", "&o", out workingLine, echoComTraffic: _echoComTraffic);
        }

        /// <summary>
        /// Overwrites current settings with the factory settings in the unit, but leaves any custom network settings as they are.
        /// </summary>
        /// <returns>True if restore is successful, false if failed.</returns>
        public bool SettingsRestoreFactoryPreservingNetworkSettings()
        {
            string workingLine;
            return _port.SendCommandSingleTest("&o2", "&o2", out workingLine, echoComTraffic: _echoComTraffic);
        }

        /// <summary>
        /// Gets the pages to download for an INI file.
        /// </summary>
        /// <returns>Pages to download for an INI file, -1 if there is a com error</returns>
        public int GetIniPageCount()
        {
            string workingLine;
            if (_port.SendCommandSingleTest("&@i?", "&@i", out workingLine, true, "", _echoComTraffic))
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
        /// A function to download a INI file representing the current unit settings.
        /// </summary>
        /// <param name="iniText">The INI file text will be output in this string.</param>
        /// <param name="message">A string containing any status messages from the uploader.</param>
        /// <param name="timeoutSeconds">Number of seconds to try the download before canceling.</param>
        /// <returns>True if iniText contains complete INI contents, false otherwise.</returns>
        public bool DownloadIni(out string iniText, out string message, int timeoutSeconds = 5)
        {
            message = "";
            iniText = "";

            var page = 0;
            var timeout = DateTime.Now.AddSeconds(timeoutSeconds);
            
            while (true)
            {
                if (timeout < DateTime.Now)
                {
                    message = "Download Timeout!";
                    return false;
                }

                var returnStrings = _port.SendCommand($"&@i{page}", 5000);

                // check for response
                if (returnStrings.Count > 0 && returnStrings[0].Contains($"&@i{page},"))
                {
                    if (returnStrings[0].Contains($"&@i{page},MULTILINECOMPLETE"))
                    {
                        // download successfull
                        return true;
                    }

                    // we have data
                    returnStrings[0] = returnStrings[0].Replace($"&@i{page},", "");
                    returnStrings.RemoveRange(returnStrings.Count - 2, 2);
                    iniText += string.Join("\r\n", returnStrings.ToArray());
                    page++;
                    continue;
                }

                // deal with any errors
                switch (returnStrings[0])
                {
                    case "&@i!o":
                        message = "Buffer Overflow!";
                        break;

                    case "&@i!v":
                        message = "Invalid Page Number!";
                        break;

                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// A function to upload an INI file to a unit for quick configuration.
        /// </summary>
        /// <param name="stream">The stream to upload to the unit.</param>
        /// <param name="message">A string containing any status messages from the uploader.</param>
        /// <param name="timeoutSeconds">Number of seconds to try the upload before canceling.</param>
        /// <returns>True if stream was uploaded successfully, false otherwise.</returns>
        public bool UploadIni(Stream stream, out string message, int timeoutSeconds = 5)
        {
            message = "";

            // make a local copy of the stream to upload
            var workingFile = new List<byte>();
            var payload = new List<byte>();
            workingFile.AddRange(stream.ToByteArray());

            var timeout = DateTime.Now.AddSeconds(timeoutSeconds);
            var pointer = 0;
            ushort page = 0;
            ushort missedPage = 0;
            var missedPageCount = 0;
            var retransmit = false;
            const string command = "&@u";

            while (true)
            {
                if (timeout < DateTime.Now)
                {
                    message = "Download Timeout!";
                    return false;
                }

                var payloadLength = Math.Min(workingFile.Count - pointer, 1024);

                if (!retransmit)
                {
                    // load the array into a list with page information
                    payload.Clear();
                    payload.AddRange(DataConversions.ConvertUInt16ToList(page));
                    payload.AddRange(workingFile.GetRange(pointer, payloadLength));
                    payload.AddRange(Checksums.Fletcher16(payload));

                    // add escapes to the payload
                    payload.EscapeList();

                    // add the command to the front of the payload
                    payload.InsertRange(0, Encoding.ASCII.GetBytes(command));
                }

                // send the data
                retransmit = false;
                var returnString = _port.SendCommand(payload, 1).FirstOrDefault();

                if (returnString?.Contains($"{command}{page},{payloadLength}") == true)
                {
                    // housekeeping variables
                    pointer += payloadLength;
                    page++;

                    // we had a good transfer, determine next step
                    if (pointer == workingFile.Count)
                    {
                        // we are at the end of the file, so tell the system we are done
                        payload.Clear();
                        payload.AddRange(DataConversions.ConvertUInt16ToList(0xFFFF));
                        payload.AddRange(Checksums.Fletcher16(payload));

                        // add escapes to the payload
                        payload.EscapeList();

                        // add the command to the front of the payload
                        payload.InsertRange(0, Encoding.ASCII.GetBytes(command));

                        // send command
                        var returnStrings = _port.SendCommand(payload, 5000);

                        if (returnStrings.Count > 0)
                        {
                            if (returnStrings[0].Contains("&@us"))
                            {
                                // upload successfull
                                message = "INI Upload Successfull!";
                                return true;
                            }

                            if (returnStrings[0].Contains("&@ue"))
                            {
                                // file error
                                returnStrings[0] = returnStrings[0].Replace("&@ue", "");
                                returnStrings.RemoveRange(returnStrings.Count - 2, 2);
                                var substring = string.Join("\r\n", returnStrings.ToArray());
                                message = $"INI Upload Failed! The INI file had the following errors:{Environment.NewLine}{substring}";
                                return false;
                            }
                        }
                        else
                        {
                            message = "Unable to parse INI file, unknown error!";
                            return false;
                        }
                    }

                    // next loop
                    continue;
                }

                // switch to deal with packet error types
                switch (returnString)
                {
                    case "&@u!c":
                        message = "Checksum Error!";
                        break;

                    case "&@u!w":
                        message = "Data processing error!";
                        break;

                    case "&@u!s":
                        message = "Upload Complete!";
                        break;

                    case "&@u!e":
                        message = "Upload Error!";
                        break;

                    case "":
                        message = "Lost Connection!";
                        break;

                    default:
                        break;
                }

                // process the missed packet count
                if (page == missedPage)
                {
                    missedPageCount++;
                    if (missedPageCount > 5)
                    {
                        // upload failed
                        return false;
                    }
                }
                else
                {
                    // missed a different page, so reset counter
                    missedPage = page;
                    missedPageCount = 1;
                }

                // retransmit page
                retransmit = true;
            }
        }
    }
}
