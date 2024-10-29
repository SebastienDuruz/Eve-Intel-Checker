using System;
using System.IO;

namespace EveIntelCheckerLib.Data
{
    /// <summary>
    /// Class LogsWriter
    /// </summary>
    public class LogsWriter
    {
        /// <summary>
        /// File path
        /// </summary>
        private string LogFile { get; set; }

        /// <summary>
        /// The logLevel
        /// </summary>
        public enum LogLevel
        {
            Info,
            Warn,
            Error
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public LogsWriter()
        {
            if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EveIntelChecker")))
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EveIntelChecker"));

            LogFile = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EveIntelChecker"), $"logs.txt");
        }

        /// <summary>
        /// Log a new entry to the logfile
        /// </summary>
        /// <param name="logLvl"></param>
        /// <param name="message"></param>
        public void Write(LogLevel logLvl, string message)
        {
            File.AppendAllText(LogFile, $"[{DateTime.Now}] {logLvl.ToString().ToUpper()} - {message}\n");
        }
    }
}
