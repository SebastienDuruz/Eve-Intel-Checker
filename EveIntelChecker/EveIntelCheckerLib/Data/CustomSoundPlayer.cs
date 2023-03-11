using NetCoreAudio;
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
        /// Last Window that played sound
        /// </summary>
        private string LastPlayed { get; set; }
        
        /// <summary>
        /// Custom Constructor
        /// </summary>
        /// <param name="soundPath">The path of the sound to use</param>
        public CustomSoundPlayer(string normalSoundPath, string dangerSoundPath)
        {
            DangerAudioFilePath = dangerSoundPath;
            NormalAudioFilePath = normalSoundPath;
            SoundPlayer = new Player();
            LastPlayed = "NONE";
        }

        /// <summary>
        /// Play the sound selected with corresponding player
        /// </summary>
        /// <param name="isDanger">Play Danger notification or normal notification</param>
        /// <param name="sender">_1 or _2 (Primary or Secondary window ?)</param>
        /// <param name="volume">Volume applied to the notification</param>
        public async Task PlaySound(bool isDanger, string sender = "FORCE", int volume = -1)
        {
            if(volume != -1)
                SoundPlayer.SetVolume((byte)volume);
            
            // Sound is already playing by the same window
            if (SoundPlayer.Playing && sender == LastPlayed && sender != "FORCE") return;

            LastPlayed = sender;
            
            // All good, play the sound
            SoundPlayer.Play(isDanger ? DangerAudioFilePath : NormalAudioFilePath);
        }
    }
}
