/// Author : Sébastien Duruz
/// Date : 25.09.2022

using System.Data.Common;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading;
using NetCoreAudio;

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
        /// SoundPlayer for Windows platform
        /// </summary>
        private SoundPlayer WinSoundPlayer { get; set; }

        /// <summary>
        /// SoundPlayer for Mac platform using NAudio
        /// </summary>
        private Player MacSoundPlayer { get; set; }

        /// <summary>
        /// FilePath of the audio file (Used only on MacOS)
        /// </summary>
        private string AudioFilePath { get; set; }

        /// <summary>
        /// Custom Constructor
        /// </summary>
        /// <param name="soundPath">The path of the sound to use</param>
        public CustomSoundPlayer(string soundPath)
        {
            OperatingSystem = new OperatingSystemSelector();

            if(OperatingSystem.CurrentOS == OperatingSystemSelector.OperatingSystemType.Windows)
                AudioFilePath = $"Assets\\{soundPath}";
            else if(OperatingSystem.CurrentOS == OperatingSystemSelector.OperatingSystemType.Mac)
            {
                //AudioFilePath = Path.Combine(Directory.GetCurrentDirectory(), $"bin/Debug/net6.0/Assets/{soundPath}");
                AudioFilePath = $"Assets/{soundPath}";
            }

            // Select the correct Soundplayer for the current OperatingSystem
            switch (OperatingSystem.CurrentOS)
            {
                case OperatingSystemSelector.OperatingSystemType.Mac:
                    MacSoundPlayer = new Player();
                    break;
                case OperatingSystemSelector.OperatingSystemType.Windows:
                    WinSoundPlayer = new SoundPlayer(AudioFilePath);
                    break;
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
                    MacSoundPlayer.Play(AudioFilePath);
                    break;
            }
        }
    }
}
