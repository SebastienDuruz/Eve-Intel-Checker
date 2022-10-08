/// Author : Sébastien Duruz
/// Date : 25.09.2022

using NAudio.Wave;
using System.Data.Common;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading;

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
        /// The audio file to play
        /// </summary>
        private AudioFileReader AudioFile { get; set; }

        /// <summary>
        /// SoundPlayer for Windows platform
        /// </summary>
        private SoundPlayer WinSoundPlayer { get; set; }

        /// <summary>
        /// SoundPlayer for Mac platform using NAudio
        /// </summary>
        private WaveOutEvent MacSoundPlayer { get; set; }

        /// <summary>
        /// Custom Constructor
        /// </summary>
        /// <param name="soundPath">The path of the sound to use</param>
        public CustomSoundPlayer(string soundPath)
        {
            OperatingSystem = new OperatingSystemSelector();
            AudioFile = new AudioFileReader(soundPath);

            // Select the correct Soundplayer for the current OperatingSystem
            switch(OperatingSystem.CurrentOS)
            {
                case OperatingSystemSelector.OperatingSystemType.Mac:
                    MacSoundPlayer = new WaveOutEvent();
                    MacSoundPlayer.Init(AudioFile);
                    break;
                case OperatingSystemSelector.OperatingSystemType.Windows:
                    WinSoundPlayer = new SoundPlayer(soundPath);
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
                    MacSoundPlayer.Play();
                    break;
            }
        }
    }
}
