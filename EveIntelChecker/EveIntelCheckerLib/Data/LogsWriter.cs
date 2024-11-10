using System;
using System.IO;

namespace EveIntelCheckerLib.Data
{
    /// <summary>
    /// Class LogsWriter
    /// </summary>
    public sealed class LogsWriter
    {
        /// <summary>
        /// File path
        /// </summary>
        private static string LogFile { get; set; }

        /// <summary>
        /// Singleton instance of the class
        /// </summary>
        private static LogsWriter _instance = null;

        /// <summary>
        /// Empty object used for simple thread safety
        /// </summary>
        private static readonly object _padlock = new object();


        /// <summary>
        /// Constructor
        /// </summary>
        private LogsWriter()
        {
            if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), StaticData.ApplicationName)))
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), StaticData.ApplicationName));

            LogFile = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), StaticData.ApplicationName), StaticData.ApplicationLogsName);
        }

        /// <summary>
        /// Instance accessor
        /// </summary>
        public static LogsWriter Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                        _instance = new LogsWriter();
                    return _instance;
                }
            }
        }

        /// <summary>
        /// Log a new entry to the logfile
        /// </summary>
        /// <param name="logLvl"></param>
        /// <param name="message"></param>
        public void Log(StaticData.LogLevel logLvl, string message)
        {
            File.AppendAllText(LogFile, $"[{DateTime.Now}] {logLvl.ToString().ToUpper()} - {message}\n");
        }
    }
}
