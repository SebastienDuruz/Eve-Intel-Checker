using System.Collections.Generic;
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
        private List<Player> SoundPlayers { get; set; }
        
        /// <summary>
        /// FilePath of the danger audio file
        /// </summary>
        private string DangerAudioFilePath { get; set; }

        /// <summary>
        /// FilePath of the normal audio file
        /// </summary>
        private string NormalAudioFilePath { get; set; }
        
        /// <summary>
        /// FilePath of the danger audio file
        /// </summary>
        private string DangerAudioFilePath2 { get; set; }

        /// <summary>
        /// FilePath of the normal audio file
        /// </summary>
        private string NormalAudioFilePath2 { get; set; }
        
        /// <summary>
        /// Last Window that played sound
        /// </summary>
        private string LastPlayed { get; set; }
        
        /// <summary>
        /// Custom Constructor
        /// </summary>
        /// <param name="soundPath">The path of the sound to use</param>
        public CustomSoundPlayer(string normalSoundPath, string dangerSoundPath, string normalSoundPath2, string dangerSoundPath2)
        {
            DangerAudioFilePath = dangerSoundPath;
            NormalAudioFilePath = normalSoundPath;
            DangerAudioFilePath2 = dangerSoundPath2;
            NormalAudioFilePath2 = normalSoundPath2;
            
            SoundPlayers = new List<Player>() { new Player(), new Player(), new Player(), new Player(), new Player(), new Player() };
            LastPlayed = "NONE";
        }

        /// <summary>
        /// Play the sound selected with corresponding player
        /// </summary>
        /// <param name="isDanger">Play Danger notification or normal notification</param>
        /// <param name="sender">_1 or _2 (Primary or Secondary window ?)</param>
        /// <param name="volume">Volume applied to the notification</param>
        public async Task PlaySound(bool isDanger, string sender, int volume = -1)
        {
            int index = sender switch
            {
                "_1" => 0,
                "_2" => 3
            };

            if (volume != -1)
            {
                SoundPlayers[index].SetVolume((byte)volume);
                SoundPlayers[index+1].SetVolume((byte)volume);
                SoundPlayers[index+2].SetVolume((byte)volume);
            }

            if (sender == "_1")
            {
                if (!SoundPlayers[index].Playing)
                    SoundPlayers[index].Play(isDanger ? DangerAudioFilePath : NormalAudioFilePath);
                else if (!SoundPlayers[index + 1].Playing)
                    SoundPlayers[index + 1].Play(isDanger ? DangerAudioFilePath : NormalAudioFilePath);
                else if (!SoundPlayers[index + 2].Playing)
                    SoundPlayers[index + 1].Play(isDanger ? DangerAudioFilePath : NormalAudioFilePath);
                else 
                    SoundPlayers[index].Play(isDanger ? DangerAudioFilePath : NormalAudioFilePath);
            }
            else
            {
                if (!SoundPlayers[index].Playing)
                    SoundPlayers[index].Play(isDanger ? DangerAudioFilePath2 : NormalAudioFilePath2);
                else if (!SoundPlayers[index + 1].Playing)
                    SoundPlayers[index + 1].Play(isDanger ? DangerAudioFilePath2 : NormalAudioFilePath2);
                else if (!SoundPlayers[index + 2].Playing)
                    SoundPlayers[index + 1].Play(isDanger ? DangerAudioFilePath2 : NormalAudioFilePath2);
                else 
                    SoundPlayers[index].Play(isDanger ? DangerAudioFilePath2 : NormalAudioFilePath2);
            }
        }
    }
}
