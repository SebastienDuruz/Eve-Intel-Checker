/// Author : Sébastien Duruz
/// Date : 04.10.2022

using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EveIntelCheckerLib.Data
{
    /// <summary>
    /// Class OperatingSystemSelector
    /// </summary>
    public class OperatingSystemSelector
    {
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
        public OperatingSystemSelector()
        {
            // Set the correct parameter for OS
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                CurrentOS = OperatingSystemType.Mac;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                CurrentOS = OperatingSystemType.Windows;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                CurrentOS = OperatingSystemType.Linux;
        }
    }
}
