/// Author : Sébastien Duruz
/// Date : 25.09.2022

using NetCoreAudio;

namespace EveIntelCheckerLib.Data
{
    /// <summary>
    /// Class MultiPlatformSoundPlayer
    /// </summary>
    public class CustomSoundPlayer
    {
        /// <summary>
        /// SoundPlayer using NAudio
        /// </summary>
        private Player SoundPlayer { get; set; }

        /// <summary>
        /// FilePath of the danger audio file
        /// </summary>
        private string DangerAudioFilePath { get; set; }

        /// <summary>
        /// FilePath of the normal audio file
        /// </summary>
        private string NormalAudioFilePath { get; set; }

        /// <summary>
        /// The current volume applied to the sound player
        /// </summary>
        private byte CurrentVolume { get; set; }

        /// <summary>
        /// Custom Constructor
        /// </summary>
        /// <param name="soundPath">The path of the sound to use</param>
        public CustomSoundPlayer(string normalSoundPath, string dangerSoundPath)
        {
            DangerAudioFilePath = dangerSoundPath;
            NormalAudioFilePath = normalSoundPath;
            CurrentVolume = 100;
            SoundPlayer = new Player();
        }

        /// <summary>
        /// Play the sound selected with corresponding player
        /// </summary>
        /// <param name="isDanger">Play Danger notification or normal notification</param>
        /// <param name="volume">Volume applied to the notification</param>
        public async void PlaySound(bool isDanger, int volume = -1)
        {
            if(volume != CurrentVolume && volume != -1)
            {
                // Currently only working on Windows, issue with mac -> Sound always reset master
                if (OperatingSystemSelector.Instance.CurrentOS != OperatingSystemSelector.OperatingSystemType.Mac)
                    await SoundPlayer.SetVolume((byte)volume);
                CurrentVolume = (byte)volume;
            }

            if (isDanger)
                SoundPlayer.Play(DangerAudioFilePath);
            else
                SoundPlayer.Play(NormalAudioFilePath);
        }
    }
}
