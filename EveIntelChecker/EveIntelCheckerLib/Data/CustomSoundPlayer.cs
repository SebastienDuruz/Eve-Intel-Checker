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
        private SoundPlayer DangerSoundPlayer { get; set; }

        /// <summary>
        /// SoundPlayer for Windows platform
        /// </summary>
        private SoundPlayer NormalSoundPlayer { get; set; }

        /// <summary>
        /// SoundPlayer for Mac platform using NAudio
        /// </summary>
        private Player MacSoundPlayer { get; set; }

        /// <summary>
        /// FilePath of the danger audio file (Used only on MacOS)
        /// </summary>
        private string DangerAudioFilePath { get; set; }

        /// <summary>
        /// FilePath of the normal audio file (Used only on MacOS)
        /// </summary>
        private string NormalAudioFilePath { get; set; }

        /// <summary>
        /// Custom Constructor
        /// </summary>
        /// <param name="soundPath">The path of the sound to use</param>
        public CustomSoundPlayer(string normalSoundPath, string dangerSoundPath)
        {
            OperatingSystem = new OperatingSystemSelector();
            DangerAudioFilePath = dangerSoundPath;
            NormalAudioFilePath = normalSoundPath;

            // Select the correct Soundplayer for the current OperatingSystem
            switch (OperatingSystem.CurrentOS)
            {
                case OperatingSystemSelector.OperatingSystemType.Mac:
                    MacSoundPlayer = new Player();
                    break;
                case OperatingSystemSelector.OperatingSystemType.Windows:
                    DangerSoundPlayer = new SoundPlayer(DangerAudioFilePath);
                    NormalSoundPlayer = new SoundPlayer(NormalAudioFilePath);
                    break;
            }
        }

        /// <summary>
        /// Play the sound selected with corresponding player
        /// </summary>
        public void PlaySound(bool isDanger)
        {
            switch(OperatingSystem.CurrentOS)
            {
                case OperatingSystemSelector.OperatingSystemType.Windows:
                    if (isDanger)
                        DangerSoundPlayer.Play();
                    else
                        NormalSoundPlayer.Play();
                    break;
                case OperatingSystemSelector.OperatingSystemType.Mac:
                    if (isDanger)
                        MacSoundPlayer.Play(DangerAudioFilePath);
                    else
                        MacSoundPlayer.Play(NormalAudioFilePath);
                    break;
            }
        }
    }
}
