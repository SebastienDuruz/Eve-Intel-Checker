namespace EveIntelCheckerLib.Models
{
    /// <summary>
    /// Class ChatLogFile
    /// </summary>
    public class ChatLogFile
    {
        /// <summary>
        /// Folder of the logFile
        /// </summary>
        public string LogFileFolder { get; set; } = string.Empty;

        /// <summary>
        /// Folder of the copy logFile
        /// </summary>
        public string CopyLogFileFolder { get; set; } = string.Empty;

        /// <summary>
        /// The full name of the log file
        /// </summary>
        public string LogFileFullPath { get; set; } = string.Empty;

        /// <summary>
        /// The full name of the copy logFile
        /// </summary>
        public string CopyLogFileFullPath { get; set; } = string.Empty;

        /// <summary>
        /// The short name of the log file (name of the chat channel)
        /// </summary>
        public string LogFileShortName { get; set; } = string.Empty;

        /// <summary>
        /// The last message sent writed on the logFile
        /// </summary>
        public string LastLogFileMessage { get; set; } = string.Empty;

        /// <summary>
        /// The last time new message has been read from the logFile
        /// </summary>
        public string LastLogFileRead { get; set; } = string.Empty;
    }
}
