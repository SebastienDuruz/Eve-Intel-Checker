/// Author : Sébastien Duruz
/// Date : 04.10.2022

using System.Runtime.InteropServices;

namespace EveIntelCheckerLib.Data
{
    /// <summary>
    /// Class OperatingSystemSelector
    /// </summary>
    public class OperatingSystemSelector
    {
        /// <summary>
        /// Padlock for thread safe Singleton operations
        /// </summary>
        private static readonly object _padLock = new object();

        /// <summary>
        /// Singleton instance
        /// </summary>
        private static OperatingSystemSelector _instance = null;

        /// <summary>
        /// Possible OS values
        /// </summary>
        public enum OperatingSystemType
        {
            Windows,
            Mac,
            Linux
        }

        /// <summary>
        /// OS of the host system
        /// </summary>
        public OperatingSystemType CurrentOS { get; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        private OperatingSystemSelector()
        {
            // Set the correct parameter for OS
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                CurrentOS = OperatingSystemType.Mac;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                CurrentOS = OperatingSystemType.Windows;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                CurrentOS = OperatingSystemType.Linux;
        }

        /// <summary>
        /// Get the singleton instance
        /// </summary>
        public static OperatingSystemSelector Instance
        {
            get
            {
                lock (_padLock)
                {
                    if (_instance == null)
                        _instance = new OperatingSystemSelector();
                    return _instance;
                }
            }
        }
    }
}
