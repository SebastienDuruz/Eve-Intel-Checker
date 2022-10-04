/// Author : Sébastien Duruz
/// Date : 25.09.2022

using System.Data.Common;
using System.Media;
using System.Runtime.InteropServices;

namespace EveIntelCheckerLib.Data
{
    /// <summary>
    /// Class MultiPlatformSoundPlayer
    /// </summary>
    public class CustomSoundPlayer
    {
        /// <summary>
        /// The current operating system specific settings
        /// </summary>
        private OperatingSystemSelector OperatingSystem { get; set; }

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
        public CustomSoundPlayer(string soundPath)
        {
            OperatingSystem = new OperatingSystemSelector();
            SoundPath = soundPath;

            // Create the correct instance for current OS
            if (OperatingSystem.CurrentOS == OperatingSystemSelector.OperatingSystemType.Mac)
            {
                // TODO : Implement sounds
            }
            else if(OperatingSystem.CurrentOS == OperatingSystemSelector.OperatingSystemType.Windows)
            {
                WinSoundPlayer = new System.Media.SoundPlayer(soundPath);
            }
        }

        /// <summary>
        /// Play the sound selected with corresponding player
        /// </summary>
        public void PlaySound()
        {
            switch(OperatingSystem.CurrentOS)
            {
                case OperatingSystemSelector.OperatingSystemType.Windows:
                    WinSoundPlayer.Play();
                    break;
                case OperatingSystemSelector.OperatingSystemType.Mac:
                    // TODO : implement player
                    break;
            }
        }
    }
}
