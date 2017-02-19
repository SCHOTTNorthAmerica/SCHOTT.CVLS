using SCHOTT.CVLS.Ethernet.Binary.Enums;
using System.Collections.Generic;
using SCHOTT.Core.Extensions;

namespace SCHOTT.CVLS.Ethernet.Binary
{
    /// <summary>
    /// The command structure for the CVLS Binary Socket
    /// </summary>
    public class BinaryCommand
    {
        /// <summary>
        /// Command type to be used.
        /// </summary>
        public ushort CommandType { get; set; }

        /// <summary>
        /// Flag for if the command is writing or just reading data
        /// </summary>
        public bool WriteAccess { get; set; }

        /// <summary>
        /// Command set to be used.
        /// </summary>
        public CommandSets CommandSet { get; set; }

        /// <summary>
        /// Which command is being used
        /// </summary>
        public ushort Command { get; set; }

        /// <summary>
        /// The length of the data section of the command
        /// </summary>
        public ushort DataLength { get; set; }
        
        /// <summary>
        /// Create a command for the BinarySocket
        /// </summary>
        /// <param name="commandList">List of Bytes to convert to a command</param>
        public BinaryCommand(List<byte> commandList)
        {
            ParseCommand(commandList);
        }

        /// <summary>
        /// Create a command for the BinarySocket
        /// </summary>
        /// <param name="commandSet">Select the CommandSet</param>
        /// <param name="command">Select the Command from the CommandSet</param>
        public BinaryCommand(CommandSets commandSet, ushort command)
        {
            CommandSet = commandSet;
            Command = command;
        }

        /// <summary>
        /// Create a command for the BinarySocket
        /// </summary>
        /// <param name="commandSet">Select the CommandSet</param>
        /// <param name="command">Select the Command from the CommandSet</param>
        /// <param name="writeAccess">Bool to indicate if the command is writing a setting to the unit.</param>
        public BinaryCommand(CommandSets commandSet, ushort command, bool writeAccess)
        {
            WriteAccess = writeAccess;
            CommandSet = commandSet;
            Command = command;
        }

        /// <summary>
        /// Parse the list of bytes into a command structure
        /// </summary>
        /// <param name="data">List of Bytes to parse</param>
        /// <returns>Command structure</returns>
        public bool ParseCommand(List<byte> data)
        {
            if (data.Count != 4)
                return false;

            var tempCommand = (ushort)((data[0] << 8) + data[1]);
            Command = (ushort)(tempCommand & 0x7FF);
            CommandSet = (CommandSets)((tempCommand & 0x1FFF) >> 11);
            WriteAccess = tempCommand.CheckBit(13);
            CommandType = (ushort)((tempCommand & 0xC000) >> 14);
            DataLength = (ushort)((data[2] << 8) + data[3]);

            return true;
        }
        
        /// <summary>
        /// Gets the command structure as a List of Bytes.
        /// </summary>
        /// <returns>List of Bytes representing the command</returns>
        public List<byte> GetCommandList()
        {
            var tempCommand = GetCommand();

            var returnCommand = new List<byte>
            {
                (byte) (tempCommand >> 8),
                (byte) tempCommand,
                (byte) (DataLength >> 8),
                (byte) DataLength
            };

            return returnCommand;
        }

        private ushort GetCommand()
        {
            return (ushort)((ushort)(CommandType << 14) + (ushort)((WriteAccess ? 1 : 0) << 13) +
                (ushort)(((ushort)CommandSet & 0x3) << 11) + (Command & 0x7FF));
        }

        /// <summary>
        /// Convert the command to a string format for comparison to other commands.
        /// </summary>
        /// <returns>Command String</returns>
        public string GetCommandString()
        {
            return $"BinaryCommand({GetCommand():X4})";
        }

    }

}
