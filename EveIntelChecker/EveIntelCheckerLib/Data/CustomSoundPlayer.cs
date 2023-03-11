/// Author : Sébastien Duruz
/// Date : 25.09.2022

using NetCoreAudio;
using System.IO;
using System.Threading.Tasks;

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
        /// Check if a sound is currently playing
        /// </summary>
        private bool IsPlaying { get; set; }

        /// <summary>
        /// Custom Constructor
        /// </summary>
        /// <param name="soundPath">The path of the sound to use</param>
        public CustomSoundPlayer(string normalSoundPath, string dangerSoundPath)
        {
            DangerAudioFilePath = dangerSoundPath;
            NormalAudioFilePath = normalSoundPath;
            SoundPlayer = new Player();
            IsPlaying = false;
        }

        /// <summary>
        /// Play the sound selected with corresponding player
        /// </summary>
        /// <param name="isDanger">Play Danger notification or normal notification</param>
        /// <param name="volume">Volume applied to the notification</param>
        public async Task<bool> PlaySound(bool isDanger, int volume = -1)
        {
            if(volume != -1)
                SoundPlayer.SetVolume((byte)volume);
            
            // Sound is already playing
            if (IsPlaying) 
                return false;
            
            // All good, play the sound
            IsPlaying = true;
            if (isDanger)
                await SoundPlayer.Play(DangerAudioFilePath);
            else
                await SoundPlayer.Play(NormalAudioFilePath);
            IsPlaying = false;

            return true;
        }
    }
}
