/// Autor : Sébastien Duruz
/// Date : 17.09.2022
/// Description : Backend of the main page

using EveIntelCheckerLib.Models;
using EveIntelCheckerLib.Models.Database;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading.Tasks;

namespace EveIntelChecker
{
    /// <summary>
    /// Class Index
    /// </summary>
    public partial class Index
    {
        /// <summary>
        /// The selected system (root)
        /// </summary>
        private MapSolarSystem _selectedSystem;

        /// <summary>
        /// Property of the _selectedSytem attribute
        /// </summary>
        private MapSolarSystem SelectedSystem { get { return _selectedSystem; } set { if (value != null) { _selectedSystem = value; } BuildSystemsList(); }}

        /// <summary>
        /// Timer for reading the chat log file
        /// </summary>
        private System.Threading.Timer? ReadFileTimer { get; set; }

        /// <summary>
        /// List of System to check
        /// </summary>
        private List<IntelSystem> IntelSystems { get; set; } = new List<IntelSystem>();

        /// <summary>
        /// Path of the log file
        /// </summary>
        private string LogFilePath { get; set; }

        /// <summary>
        /// Path of the log file copy to read
        /// </summary>
        private string CopyLogFilePath { get; set; }

        /// <summary>
        /// The last line of the chat log file
        /// </summary>
        private string LastLogFileLine { get; set; }

        /// <summary>
        /// The last time message has been send to Chat log
        /// </summary>
        private string LastLogFileRead { get; set; } = "";

        /// <summary>
        /// LogFile object
        /// </summary>
        private IBrowserFile LogFile { get; set; }

        /// <summary>
        /// SoundPlayer for alert trigger
        /// </summary>
        private SoundPlayer Player { get; set; } = new SoundPlayer("notification.wav");

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
        private string ClearClasses { get; set; } = "d-flex";

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
            // Read chat log file each sec
            ReadFileTimer = new System.Threading.Timer(async (object? stateInfo) =>
            {
                await ReadLogFile();
            }, new System.Threading.AutoResetEvent(false), 1000, 1000);
        }

        /// <summary>
        /// Select the chat log file
        /// </summary>
        /// <param name="e">args of the event caller</param>
        private void SelectFile(InputFileChangeEventArgs e)
        {
            // Only the last selected file will be used
            foreach (var file in e.GetMultipleFiles())
                LogFile = file;

            if (LogFile != null && LogFile.ContentType == "text/plain")
            {
                FileIconColor = Color.Success;
                LogFilePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\EVE\\logs\\Chatlogs\\{LogFile.Name}";
                CopyLogFilePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Copy_{LogFile.Name}";
            }
            else
            {
                FileIconColor = Color.Error;
                LogFile = null;
                LogFilePath = "";
                CopyLogFilePath = "";
                LastLogFileLine = "";
                LastLogFileRead = "";
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
        private void BuildSystemsList()
        {
            this.IntelSystems = EveStaticDb.BuildSystemsList(SelectedSystem);
        }
        
        /// <summary>
        /// Read the chat log file
        /// </summary>
        /// <returns>Result of the Task</returns>
        private async Task ReadLogFile()
        {
            // User has selected the required 
            if (LogFile != null && IntelSystems.Count > 0)
                if (File.Exists(LogFilePath))
                {
                    if (File.Exists(CopyLogFilePath))
                        File.Delete(CopyLogFilePath);
                    File.Copy(LogFilePath, CopyLogFilePath);
                    string[] lines = await File.ReadAllLinesAsync(CopyLogFilePath);
                    if (lines != null)
                        if (lines[lines.Count() - 1] != LastLogFileLine)
                        {
                            LastLogFileLine = lines[lines.Count() - 1];
                            await CheckSystemProximity();
                            await ExtractTimeFromMessage(LastLogFileLine);
                            await InvokeAsync(() => StateHasChanged());
                        }
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
                if (LastLogFileLine.Contains(intelSystem.SystemName))
                {
                    intelSystem.IsRed = true;
                    await PlayNotificationSound();
                    ++intelSystem.TriggerCounter;
                    newRedSystem = intelSystem.SystemName;
                }

            // If needed reset the last system set to RED
            if(newRedSystem != "")
                foreach(IntelSystem intelSystem in IntelSystems)
                    if(intelSystem.SystemName != newRedSystem)
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
        private async Task UpdateRootSystem(IntelSystem system)
        {
            SelectedSystem = EveStaticDb.SolarSystems.Where(x => x.SolarSystemName == system.SystemName).FirstOrDefault();
            SolarSystemSelector.Text = SelectedSystem.SolarSystemName;
        }

        /// <summary>
        /// Play a sound on new trigger
        /// </summary>
        /// <returns>Result of the Task</returns>
        private async Task PlayNotificationSound()
        {
            Player.Play();
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
                LastLogFileRead = time.Split(" ")[2];
                await InvokeAsync(() => StateHasChanged());
            }
            catch(Exception)
            {
                
            }
        }
    }
}
