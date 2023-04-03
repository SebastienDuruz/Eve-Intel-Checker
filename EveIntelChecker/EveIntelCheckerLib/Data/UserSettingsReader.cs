using EveIntelCheckerLib.Models;
using Newtonsoft.Json;
using System;
using System.IO;

namespace EveIntelCheckerLib.Data
{
    /// <summary>
    /// Class UserSettingsReader
    /// </summary>
    public class UserSettingsReader
    {
        /// <summary>
        /// File path of the userSettings file
        /// </summary>
        private string FilePath { get; }

        /// <summary>
        /// Objects that contains the settings values
        /// </summary>
        public UserSettings UserSettingsValues { get; set; }

        /// <summary>
        /// Custom Constructor
        /// </summary>
        /// <param name="identifier">The prefix to identify the setting file</param>
        public UserSettingsReader(string identifier)
        {
            if(!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EveIntelChecker")))
            {
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EveIntelChecker"));
            }
            
            FilePath = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EveIntelChecker"), $"userSettings{identifier}.json");
            
            ReadUserSettings();
        }

        /// <summary>
        /// Read the userSettings json file, create it if not exists
        /// Load the content into UserSettings Object
        /// </summary>
        public void ReadUserSettings()
        {
            if(File.Exists(FilePath))
            {
                try
                {
                    UserSettingsValues = JsonConvert.DeserializeObject<UserSettings>(File.ReadAllText(FilePath));
                }
                catch (Exception ex)
                {
                    // Reset the settings by recreating a file
                    WriteUserSettings();
                    UserSettingsValues = new UserSettings();
                }
            }
            else
            {
                UserSettingsValues = new UserSettings();
                WriteUserSettings();
            }
        }

        /// <summary>
        /// Write the UserSettings object to json file
        /// </summary>
        public void WriteUserSettings()
        {
            try
            {
                File.WriteAllText(FilePath, JsonConvert.SerializeObject(UserSettingsValues));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
