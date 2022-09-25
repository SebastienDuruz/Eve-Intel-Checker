/// Author : Sébastien Duruz
/// Date : 25.09.2022

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
    /// Class MultiPlatformSoundPlayer
    /// </summary>
    public class MultiPlatformSoundPlayer
    {
        /// <summary>
        /// Possible OS values
        /// </summary>
        private enum OperatingSystemType
        {
            Windows,
            Mac,
            Linux
        }

        private OperatingSystemType OperatingSystem { get; set; }

        /// <summary>
        /// The path of the sound to play
        /// </summary>
        private string SoundPath { get; set; }

        /// <summary>
        /// SoundPlayer for Windows platform
        /// </summary>
        private SoundPlayer WinSoundPlayer { get; set; }

        /// <summary>
        /// Custom Constructor
        /// </summary>
        /// <param name="soundPath">The path of the sound to use</param>
        public MultiPlatformSoundPlayer(string soundPath)
        {
            SoundPath = soundPath;

            // Create the correct instance for current OS
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                OperatingSystem = OperatingSystemType.Mac;
                throw new NotImplementedException("Mac sound not implemented");
            }
            else if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                OperatingSystem = OperatingSystemType.Windows;
                WinSoundPlayer = new SoundPlayer(soundPath);
            }
            else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                OperatingSystem |= OperatingSystemType.Linux;
                throw new NotImplementedException("Linux sound not implemented");
            }
        }

        /// <summary>
        /// Play the sound selected with corresponding player
        /// </summary>
        public void PlaySound()
        {
            switch(OperatingSystem)
            {
                case OperatingSystemType.Windows:
                    WinSoundPlayer.Play();
                    break;
                case OperatingSystemType.Mac:
                    break;
                case OperatingSystemType.Linux:
                    break;
            }
        }
    }
}
