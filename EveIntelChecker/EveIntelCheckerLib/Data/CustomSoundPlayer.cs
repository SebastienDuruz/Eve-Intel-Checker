/// Author : Sébastien Duruz
/// Date : 25.09.2022

using NetCoreAudio;
using System.Media;

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
            AudioFilePath = soundPath;

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
