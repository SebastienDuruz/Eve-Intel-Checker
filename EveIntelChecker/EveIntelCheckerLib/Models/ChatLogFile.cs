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
        public string LogFileFolder { get; set; } = "";

        /// <summary>
        /// Folder of the copy logFile
        /// </summary>
        public string CopyLogFileFolder { get; set; } = "";

        /// <summary>
        /// The full name of the log file
        /// </summary>
        public string LogFileFullName { get; set; } = "";

        /// <summary>
        /// The short name of the log file (name of the chat channel)
        /// </summary>
        public string LogFileShortName { get; set; } = "";

        /// <summary>
        /// The full name of the copy logFile
        /// </summary>
        public string CopyLogFileFullName { get; set; } = "";

        /// <summary>
        /// The last message sent writed on the logFile
        /// </summary>
        public string LastLogFileMessage { get; set; } = "";

        /// <summary>
        /// The last time new message has been read from the logFile
        /// </summary>
        public string LastLogFileRead { get; set; } = ""; 
    }
}
