/// Author : Sébastien Duruz
/// Date : 13.10.2022

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
        /// Padlock for thread safe Singleton operations
        /// </summary>
        private static readonly object _padLock = new object();

        /// <summary>
        /// Singleton instance
        /// </summary>
        private static UserSettingsReader _instance = null;

        /// <summary>
        /// File path of the userSettings file
        /// </summary>
        private string _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "userSettings.json");

        /// <summary>
        /// Objects that contains the settings values
        /// </summary>
        public UserSettings UserSettingsValues { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        private UserSettingsReader()
        {
            ReadUserSettings();
        }

        /// <summary>
        /// Get the singleton instance
        /// </summary>
        public static UserSettingsReader Instance
        {
            get
            {
                lock (_padLock)
                {
                    if (_instance == null)
                        _instance = new UserSettingsReader();
                    return _instance;
                }
            }
        }

        /// <summary>
        /// Read the userSettings json file, create it if not exists
        /// Load the content into UserSettings Object
        /// </summary>
        public void ReadUserSettings()
        {
            if(File.Exists(_filePath))
            {
                try
                {
                    UserSettingsValues = JsonConvert.DeserializeObject<UserSettings>(File.ReadAllText(_filePath));
                }
                catch (Exception)
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
                File.WriteAllText(_filePath, JsonConvert.SerializeObject(UserSettingsValues));
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
