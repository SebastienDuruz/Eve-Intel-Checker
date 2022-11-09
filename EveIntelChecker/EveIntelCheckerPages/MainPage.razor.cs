﻿/// Autor : Sébastien Duruz
/// Date : 17.09.2022
/// Description : Backend of the main page

using EveIntelCheckerLib.Data;
using EveIntelCheckerLib.Models;
using EveIntelCheckerLib.Models.Database;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading;
using System.Threading.Tasks;

namespace EveIntelCheckerPages
{
    /// <summary>
    /// Classe MainPage
    /// </summary>
    public partial class MainPage
    {
        /// <summary>
        /// The selected system (root)
        /// </summary>
        private MapSolarSystem _selectedSystem;

        /// <summary>
        /// Property of the _selectedSytem attribute
        /// </summary>
        private MapSolarSystem SelectedSystem { get { return _selectedSystem; } set { if (value != null) { _selectedSystem = value; } BuildSystems(); } }

        /// <summary>
        /// List of System to check
        /// </summary>
        private List<IntelSystem> IntelSystems { get; set; } = new List<IntelSystem>();

        /// <summary>
        /// Does a LogFile is currently loaded
        /// </summary>
        private bool LogFileLoaded { get; set; }

        /// <summary>
        /// Timer for reading the chat log file
        /// </summary>
        private Timer? ReadFileTimer { get; set; }

        /// <summary>
        /// The informations about current chat LogFile
        /// </summary>
        private ChatLogFile ChatLogFile { get; set; }

        /// <summary>
        /// Object that contains the current OS run by the user
        /// </summary>
        private OperatingSystemSelector OperatingSystem { get; set; }

        /// <summary>
        /// SoundPlayer for alert trigger
        /// </summary>
        private EveIntelCheckerLib.Data.CustomSoundPlayer SoundPlayer { get; set; }

        /// <summary>
        /// Mud componant for selecting the root system
        /// </summary>
        private MudAutocomplete<MapSolarSystem> SolarSystemSelector { get; set; } = new MudAutocomplete<MapSolarSystem>();

        /// <summary>
        /// The color for chat log file selector button
        /// </summary>
        private Color FileIconColor { get; set; } = Color.Error;

        /// <summary>
        /// Classes for IntelSystem displayed as red
        /// </summary>
        private string RedClasses { get; set; } = " mud-error-text";

        /// <summary>
        /// Classes for IntelSystem displayed as orange
        /// </summary>
        private string OrangeClasses { get; set; } = " mud-warning-text";

        /// <summary>
        /// Classes for IntelSystem displayed as clear
        /// </summary>
        private string ClearClasses { get; set; } = "d-flex justify-space-around align-center flex-grow-1 gap-8";

        /// <summary>
        /// Set to true if Settings panel is open
        /// </summary>
        private bool SettingsPageOpened { get; set; } = false;

        /// <summary>
        /// Custom theme for MudBlazor
        /// </summary>
        MudTheme CustomTheme = new MudTheme()
        {
            Typography = new Typography()
            {
                Default = new Default()
                {
                    FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" }
                }
            }
        };

        /// <summary>
        /// After the component as been Initialized
        /// </summary>
        /// <returns>Result of the task</returns>
        protected override async Task OnInitializedAsync()
        {
            LogFileLoaded = false;
            OperatingSystem = new OperatingSystemSelector();
            SetChatLogFile();
            LoadUserSettingsLastLog();

            SoundPlayer = new EveIntelCheckerLib.Data.CustomSoundPlayer("notification.wav");

            // Read chat log file each sec
            ReadFileTimer = new Timer(async (object? stateInfo) =>
            {
                await ReadLogFile();
            }, new AutoResetEvent(false), 1000, 1000);
        }

        /// <summary>
        /// Select the chat log file
        /// </summary>
        /// <param name="e">args of the event caller</param>
        private void SelectFile(InputFileChangeEventArgs e)
        {
            IBrowserFile logFile = null;

            // Only the last selected file will be used
            foreach (var file in e.GetMultipleFiles())
            {
                logFile = file;
            }

            if (logFile != null && logFile.ContentType == "text/plain")
            {
                // Update chat logs values
                ChatLogFile.LogFileFullName = logFile.Name;
                ChatLogFile.CopyLogFileFullName = BuildCopyFromFullName(ChatLogFile.LogFileFullName);
                ChatLogFile.LogFileShortName = ExtractShortNameFromFullName(ChatLogFile.LogFileFullName);
                FileIconColor = Color.Success;

                // Update the settings file
                UserSettingsReader.UserSettingsValues.LastFileName = ChatLogFile.LogFileFullName;
                UserSettingsReader.WriteUserSettings();
                LogFileLoaded = true;
            }
            else
            {
                FileIconColor = Color.Error;
                LogFileLoaded = false;
                SetChatLogFile();
            }
        }

        /// <summary>
        /// Load the userSettings last logfile at the initial start
        /// </summary>
        private void LoadUserSettingsLastLog()
        {
            // Start by reading the UserSettings
            UserSettingsReader.ReadUserSettings();

            // If a filename is found
            if(!String.IsNullOrWhiteSpace(UserSettingsReader.UserSettingsValues.LastFileName))
            {
                // Chatlog file exists
                if(File.Exists(Path.Combine(ChatLogFile.LogFileFolder, UserSettingsReader.UserSettingsValues.LastFileName)))
                {
                    ChatLogFile.LogFileFullName = UserSettingsReader.UserSettingsValues.LastFileName;
                    ChatLogFile.LogFileShortName = ExtractShortNameFromFullName(ChatLogFile.LogFileFullName);
                    ChatLogFile.CopyLogFileFullName = BuildCopyFromFullName(ChatLogFile.LogFileFullName);
                    FileIconColor = Color.Success;
                    LogFileLoaded = true;
                }
                else
                {
                    LogFileLoaded = false;
                    FileIconColor = Color.Error;
                }
            }

            // Select the system if it exists in the DB
            if(!String.IsNullOrWhiteSpace(UserSettingsReader.UserSettingsValues.LastSelectedSystem) 
                && EveStaticDb.SolarSystems.Exists(x => x.SolarSystemName == UserSettingsReader.UserSettingsValues.LastSelectedSystem))
            {
                SolarSystemSelector.Value = EveStaticDb.SolarSystems.Where(x => x.SolarSystemName == UserSettingsReader.UserSettingsValues.LastSelectedSystem).First();
                SolarSystemSelector.Text = UserSettingsReader.UserSettingsValues.LastSelectedSystem;
                SelectedSystem = SolarSystemSelector.Value;
            }
        }

        /// <summary>
        /// SearchSystem event
        /// </summary>
        /// <param name="value">System name to search</param>
        /// <returns>DB object with sytem informations</returns>
        private async Task<IEnumerable<MapSolarSystem>> SearchSystem(string value)
        {
            if (string.IsNullOrEmpty(value))
                return new List<MapSolarSystem>();
            return EveStaticDb.SolarSystems.Where(x => x.SolarSystemName.Contains(value, StringComparison.InvariantCultureIgnoreCase) || x.SolarSystemID.ToString().Contains(value, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        /// <summary>
        /// Build the list of systems to display
        /// </summary>
        private void BuildSystems()
        {
            // Build the list of systems
            IntelSystems = EveStaticDb.BuildSystemsList(SelectedSystem, UserSettingsReader.UserSettingsValues.SystemsDepth);

            // Update the userSettings with new selected system
            UserSettingsReader.UserSettingsValues.LastSelectedSystem = SelectedSystem.SolarSystemName;
            UserSettingsReader.WriteUserSettings();
        }

        /// <summary>
        /// Read the chat log file
        /// </summary>
        /// <returns>Result of the Task</returns>
        private async Task ReadLogFile()
        {
            // User has selected the required
            if (LogFileLoaded && IntelSystems.Count > 0)
            {
                // File exists (Read the file)
                if (File.Exists($"{ChatLogFile.LogFileFolder}{ChatLogFile.LogFileFullName}"))
                {
                    File.Copy($"{ChatLogFile.LogFileFolder}{ChatLogFile.LogFileFullName}", $"{ChatLogFile.CopyLogFileFolder}{ChatLogFile.CopyLogFileFullName}", true);

                    // Execute the main process by reading last line of the logfile
                    string[] lines = await File.ReadAllLinesAsync($"{ChatLogFile.CopyLogFileFolder}{ChatLogFile.CopyLogFileFullName}");
                    if (lines != null)
                        if (lines[lines.Count() - 1] != ChatLogFile.LastLogFileMessage)
                        {
                            ChatLogFile.LastLogFileMessage = lines[lines.Count() - 1];
                            await CheckSystemProximity();
                            await ExtractTimeFromMessage(ChatLogFile.LastLogFileMessage);
                            await InvokeAsync(() => StateHasChanged());
                        }
                }

                // Check for new chatlog file
                await CheckNewLogFile();
            }
        }

        /// <summary>
        /// Check if the last chat log file contains a system to check
        /// </summary>
        /// <returns>Result of the task</returns>
        private async Task CheckSystemProximity()
        {
            string newRedSystem = "";

            // Check if message contains a system set to be checked
            foreach (IntelSystem intelSystem in IntelSystems)
                if (ChatLogFile.LastLogFileMessage.Contains(intelSystem.SystemName))
                {
                    intelSystem.IsRed = true;
                    await PlayNotificationSound();
                    ++intelSystem.TriggerCounter;
                    newRedSystem = intelSystem.SystemName;
                }

            // If needed reset the last system set to RED
            if (newRedSystem != "")
                foreach (IntelSystem intelSystem in IntelSystems)
                    if (intelSystem.SystemName != newRedSystem)
                        intelSystem.IsRed = false;

            await InvokeAsync(() => StateHasChanged());
        }

        /// <summary>
        /// Reset the triggers counter to 0
        /// </summary>
        /// <returns>Result of the Task</returns>
        private async Task ResetTriggers()
        {
            if (IntelSystems != null)
                foreach (IntelSystem system in IntelSystems)
                {
                    system.TriggerCounter = 0;
                    system.IsRed = false;
                }
        }

        /// <summary>
        /// Update the root system to correspond with selection
        /// </summary>
        /// <param name="system">The system to define as root</param>
        /// <returns>Result of the Task</returns>
        private void UpdateRootSystem(IntelSystem system)
        {
            SelectedSystem = EveStaticDb.SolarSystems.Where(x => x.SolarSystemName == system.SystemName).FirstOrDefault();
            SolarSystemSelector.Text = SelectedSystem.SolarSystemName;
        }

        /// <summary>
        /// Check if a new chatlog file has been created by the game.
        /// Change the chatlog file information if needed
        /// </summary>
        /// <returns>True if changed, false if nothing changed</returns>
        private async Task CheckNewLogFile()
        {
            if(ChatLogFile.LogFileFolder != "" && ChatLogFile.LogFileShortName != "" && LogFileLoaded)
            {
                // Get the logfiles corresponding to the selected chat file
                List<string> chatLogFiles = Directory.GetFiles(ChatLogFile.LogFileFolder, $"{ChatLogFile.LogFileShortName}*.txt").ToList();
                string[] splitedCurrent = ChatLogFile.LogFileFullName.Split("_");

                // Replace the logfile by the most recent if it's not the current that is used
                foreach (string chatLogFile in chatLogFiles)
                {
                    // Only check different files
                    if($"{ChatLogFile.LogFileFolder}{ChatLogFile.LogFileFullName}" != chatLogFile)
                    {
                        string[] splitedToCheck = chatLogFile.Split("_");

                        // Must be same client ID
                        if (splitedCurrent[3] != splitedToCheck[3])
                            continue;

                        // Creation date is too low
                        if (long.Parse(splitedToCheck[1]) < long.Parse(splitedCurrent[1]))
                            continue;

                        // same creation date but hour is too low
                        if (splitedToCheck[1] == splitedCurrent[1] && long.Parse(splitedToCheck[2]) < long.Parse(splitedCurrent[2]))
                            continue;

                        // NEW FILE DETECTED do the necessary changes
                        ChatLogFile.LogFileFullName = chatLogFile.Split("\\").Last();
                        ChatLogFile.LogFileShortName = ExtractShortNameFromFullName(ChatLogFile.LogFileFullName);
                        ChatLogFile.CopyLogFileFullName = BuildCopyFromFullName(ChatLogFile.LogFileFullName);

                        // Set the file to settings
                        UserSettingsReader.UserSettingsValues.LastFileName = ChatLogFile.LogFileFullName;
                        UserSettingsReader.WriteUserSettings();
                    }
                }
            }
        }

        /// <summary>
        /// Play a sound on new trigger
        /// </summary>
        /// <returns>Result of the Task</returns>
        private async Task PlayNotificationSound()
        {
            SoundPlayer.PlaySound();
        }

        /// <summary>
        /// Extract the time when new Log line has been detected
        /// </summary>
        /// <param name="message">The message from where to extract the time</param>
        /// <returns>Time if on the expected format, empty if not</returns>
        private async Task ExtractTimeFromMessage(string message)
        {
            try
            {
                string time = message.Split("[")[1];
                time = time.Split("]")[0];
                ChatLogFile.LastLogFileRead = time.Split(" ")[2];
                await InvokeAsync(() => StateHasChanged());
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Extract the chat name from a filename
        /// </summary>
        /// <param name="fileName">The full filename</param>
        /// <returns>Extracted chat name or empty if not in correct format</returns>
        private string ExtractShortNameFromFullName(string fileName)
        {
            try
            {
                string chatName = fileName.Split("_")[0];
                return chatName;
            }
            catch(Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// Build the copy filename from full filename
        /// </summary>
        /// <param name="fileName">The filename to use</param>
        /// <returns>Copy filename</returns>
        private string BuildCopyFromFullName(string fileName)
        {
            return $"Copy_{fileName}";
        }

        /// <summary>
        /// Reset the ChatLogFile to default with folder values
        /// </summary>
        private void SetChatLogFile()
        {
            ChatLogFile = new ChatLogFile();
            if (OperatingSystem.CurrentOS == OperatingSystemSelector.OperatingSystemType.Windows)
            {
                ChatLogFile.LogFileFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\EVE\\logs\\Chatlogs\\";
                ChatLogFile.CopyLogFileFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\";
            }
            else if (OperatingSystem.CurrentOS == OperatingSystemSelector.OperatingSystemType.Mac)
            {
                ChatLogFile.LogFileFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/Documents/EVE/logs/Chatlogs/";
                ChatLogFile.CopyLogFileFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/";
            }
        }

        /// <summary>
        /// Open or Close the Settings panel, save the settings if settings panel as been closed
        /// </summary>
        private void OpenCloseSettingsPanel()
        {
            SettingsPageOpened = !SettingsPageOpened;
            if (!SettingsPageOpened)
            {
                UserSettingsReader.WriteUserSettings();
                if (_selectedSystem != null)
                    BuildSystems();
            }
        }
    }
}
