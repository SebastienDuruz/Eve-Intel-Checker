using EveIntelCheckerLib.Data;
using EveIntelCheckerLib.Models;
using EveIntelCheckerLib.Models.Database;
using EveIntelCheckerLib.Models.Map;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.AspNetCore.Components;
using static System.GC;
using Colors = MudBlazor.Colors;

namespace EveIntelCheckerPages
{
    /// <summary>
    /// Classe MainPage
    /// </summary>
    public partial class MainPage
    {
        /// <summary>
        /// Window Suffix parameter
        /// </summary>
        [Parameter] 
        public string?  WindowSpecificSuffix { get; set; }

        /// <summary>
        /// SettingsReader object
        /// </summary>
        [Parameter]
        public UserSettingsReader SettingsReader { get; set; }
        
        /// <summary>
        /// The selected system (root)
        /// </summary>
        private MapSolarSystem? _selectedSystem;
        
        /// <summary>
        /// Property of the _selectedSytem attribute
        /// </summary>
        private MapSolarSystem SelectedSystem { get { return _selectedSystem; } set { _selectedSystem = value; BuildSystems(); } }

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
        private static Timer? ReadFileTimer { get; set; }

        /// <summary>
        /// The informations about current chat LogFile
        /// </summary>
        private ChatLogFile ChatLogFile { get; set; }

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
        /// Define if a settings as been changed by user (recreate or not the Systems list)
        /// </summary>
        private bool SettingsChanged { get; set; } = false;

        /// <summary>
        /// Object that contains the build data ready to be used by JS (building the map)
        /// </summary>
        private (MapNode[], MapLink[]) MapSystemsData { get; set; }

        /// <summary>
        /// Set to true if settings panel just closed
        /// </summary>
        private bool MapRebuildRequired { get; set; } = false;

        /// <summary>
        /// Main theme for MudBlazor
        /// </summary>
        private MudTheme _mainTheme = new MudTheme()
        {
            PaletteDark = new PaletteDark()
            {
                Primary = "#007ea7",
                Background = "#1c1c1c",
                AppbarBackground = "#1c1c1c",
                DrawerBackground = "#1c1c1c",
                Divider = "#FFFFFF",
                AppbarText = "#FFFFFF",
                DrawerText = "#FFFFFF",
                White = "#FFFFFF",
                Dark = "#1c1c1c",
                DarkDarken = "#1c1c1c",
                DarkLighten = "#1c1c1c",
                OverlayDark = "#1c1c1c",
                GrayDark = "#1c1c1c",
                GrayDarker = "#1c1c1c",
                BackgroundGrey = "#1c1c1c",
                Surface = "#1c1c1c",
            },
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
        protected override void OnInitialized()
        {
            LogFileLoaded = false;
            SetDefaultChatLogFileFolders();
            LoadUserSettingsLastLog();
        }

        /// <summary>
        /// Handler for logfile reading process
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="e">Event Args</param>
        private void ReadLogHandler(object source, ElapsedEventArgs e)
        {
            ReadLogFile();
            Collect();
        }

        /// <summary>
        /// Execute JS routines after render is done
        /// </summary>
        /// <param name="firstRender">Is first render</param>
        /// <returns>result of the task</returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                // Read chat log file each sec
                ReadFileTimer = new Timer(1000);

                ReadFileTimer.Elapsed += ReadLogHandler;
                ReadFileTimer.AutoReset = true;
                ReadFileTimer.Enabled = true;
                ReadFileTimer.Start();
            }
            
            if (SettingsReader is { UserSettingsValues.CompactMode: false })
            {
                if(firstRender || MapRebuildRequired)
                {
                    JsRuntime.InvokeVoidAsync("buildMap", new Object[] { MapSystemsData.Item1, MapSystemsData.Item2 });

                    // Reset the value, avoiding rebuild at every rendering
                    MapRebuildRequired = false;
                }
            }
            await base.OnAfterRenderAsync(firstRender);
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

            if (logFile is { ContentType: "text/plain" })
            {
                // Update chat logs values
                ChatLogFile.LogFileFullName = logFile.Name;
                ChatLogFile.CopyLogFileFullName = BuildCopyFromFullName(ChatLogFile.LogFileFullName);
                ChatLogFile.LogFileShortName = ExtractShortNameFromFullName(ChatLogFile.LogFileFullName);
                FileIconColor = Color.Success;

                // Update the settings file
                SettingsReader.UserSettingsValues.LastFileName = ChatLogFile.LogFileFullName;
                SettingsReader.WriteUserSettings();
                
                LogFileLoaded = true;
            }
            else
            {
                FileIconColor = Color.Error;
                LogFileLoaded = false;
                SetDefaultChatLogFileFolders();
            }
        }

        /// <summary>
        /// Load the userSettings last logfile at the initial start
        /// </summary>
        private void LoadUserSettingsLastLog()
        {
            // Start by reading the UserSettings
            SettingsReader.ReadUserSettings();

            // If a filename is found
            if(!string.IsNullOrWhiteSpace(SettingsReader.UserSettingsValues.LastFileName))
            {
                // Chat-log file exists
                if(File.Exists(Path.Combine(ChatLogFile.LogFileFolder, SettingsReader.UserSettingsValues.LastFileName)))
                {
                    ChatLogFile.LogFileFullName = SettingsReader.UserSettingsValues.LastFileName;
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
            if(!string.IsNullOrWhiteSpace(SettingsReader.UserSettingsValues.LastSelectedSystem) 
                && EveStaticDatabase.Instance.SolarSystems.Exists(x => x.SolarSystemName == SettingsReader.UserSettingsValues.LastSelectedSystem))
            {
                SolarSystemSelector.Value = EveStaticDatabase.Instance.SolarSystems.Where(x => x.SolarSystemName == SettingsReader.UserSettingsValues.LastSelectedSystem).First();
                SolarSystemSelector.Text = SettingsReader.UserSettingsValues.LastSelectedSystem;
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
            return EveStaticDatabase.Instance.SolarSystems.Where(x => x.SolarSystemName.Contains(value, StringComparison.InvariantCultureIgnoreCase) || x.SolarSystemID.ToString().Contains(value, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        /// <summary>
        /// Build the list of systems to display
        /// </summary>
        private async void BuildSystems()
        {
            // Check the value before doing anything
            if (SelectedSystem == null) return;
            
            // Build the list of systems
            IntelSystems = EveStaticDatabase.Instance.BuildSystemsList(SelectedSystem, SettingsReader.UserSettingsValues.SystemsDepth);

            MapSystemsData = BuildMapNodes();
            if(!SettingsReader.UserSettingsValues.CompactMode)
                JsRuntime.InvokeVoidAsync("buildMap", new Object[] { MapSystemsData.Item1, MapSystemsData.Item2 });

            // Update the userSettings with new selected system
            SettingsReader.UserSettingsValues.LastSelectedSystem = SelectedSystem.SolarSystemName;
            SettingsReader.WriteUserSettings();
        }

        /// <summary>
        /// Read the chat log file
        /// </summary>
        /// <returns>Result of the Task</returns>
        private async void ReadLogFile()
        {
            // User has selected the required
            if (LogFileLoaded && IntelSystems.Count > 0)
            {
                // File exists (Read the file)
                if (File.Exists($"{ChatLogFile.LogFileFolder}{ChatLogFile.LogFileFullName}"))
                {
                    try
                    {
                        File.Copy($"{ChatLogFile.LogFileFolder}{ChatLogFile.LogFileFullName}",
                            $"{ChatLogFile.CopyLogFileFolder}{ChatLogFile.CopyLogFileFullName}", true);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[{DateTime.Now}] [[{WindowSpecificSuffix}]] <- {ex.Message} ->\n{ex.Source}\n{ex.Data}\n{ex.InnerException}");
                    }

                    try
                    {
                        // Execute the main process by reading last line of the logfile
                        IEnumerable<string> lines = File.ReadLines($"{ChatLogFile.CopyLogFileFolder}{ChatLogFile.CopyLogFileFullName}");
                        if (lines != null)
                            if (lines.Last() != ChatLogFile.LastLogFileMessage)
                            {
                                ChatLogFile.LastLogFileMessage = lines.Last();
                                await CheckSystemProximity();
                                await ExtractTimeFromMessage(ChatLogFile.LastLogFileMessage);
                            }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[{DateTime.Now}] [[{WindowSpecificSuffix}]] <- {ex.Message} ->\n{ex.Source}\n{ex.Data}\n{ex.InnerException}");
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
                    // Play specific sounds if needed by the user settings
                    if (intelSystem.Jumps < SettingsReader.UserSettingsValues.IgnoreNotification &&
                        intelSystem.Jumps <= SettingsReader.UserSettingsValues.DangerNotification)
                    {
                        await PlayNotificationSound(true);
                        Console.WriteLine($"{DateTime.Now} Playing Sound for {WindowSpecificSuffix}");
                    }
                    else if (intelSystem.Jumps < SettingsReader.UserSettingsValues.IgnoreNotification &&
                             intelSystem.Jumps > SettingsReader.UserSettingsValues.DangerNotification)
                    {
                        await PlayNotificationSound(false);
                        Console.WriteLine($"{DateTime.Now} Playing Sound for {WindowSpecificSuffix}");
                    }
                    ++intelSystem.TriggerCounter;
                    newRedSystem = intelSystem.SystemName;
                }

            // If needed reset the last system set to RED
            if (newRedSystem != "")
            {
                foreach (IntelSystem intelSystem in IntelSystems)
                    if (intelSystem.SystemName != newRedSystem)
                        intelSystem.IsRed = false;

                // rebuild the systems data for StarMap
                if(!SettingsReader.UserSettingsValues.CompactMode)
                {
                    MapSystemsData = BuildMapNodes();
                    JsRuntime.InvokeVoidAsync("setData", new Object[] { MapSystemsData.Item1 });
                }
            }
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

            MapSystemsData = BuildMapNodes();
            if (!SettingsReader.UserSettingsValues.CompactMode)
                JsRuntime.InvokeVoidAsync("setData", new Object[] { MapSystemsData.Item1 });
        }

        /// <summary>
        /// Resize the size and position of the starmap
        /// </summary>
        /// <returns>Result of the Task</returns>
        private async Task ResizeMap()
        {
            JsRuntime.InvokeVoidAsync("buildMap", new Object[] { MapSystemsData.Item1, MapSystemsData.Item2 });
        }

        /// <summary>
        /// Update the root system to correspond with selection
        /// </summary>
        /// <param name="system">The system to define as root</param>
        /// <returns>Result of the Task</returns>
        private void UpdateRootSystem(IntelSystem system)
        {
            SelectedSystem = EveStaticDatabase.Instance.SolarSystems.FirstOrDefault(x => x.SolarSystemName == system.SystemName);
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

                        //await InvokeAsync(() => StateHasChanged());

                        // Set the file to settings
                        SettingsReader.UserSettingsValues.LastFileName = ChatLogFile.LogFileFullName;
                        SettingsReader.WriteUserSettings();
                    }
                }
            }
        }

        /// <summary>
        /// Play a sound on new trigger
        /// </summary>
        /// <returns>Result of the Task</returns>
        private async Task PlayNotificationSound(bool isDanger)
        {
            // Secondary Window, play sound only if Window is opened at the moment of the sound trigger
            if((WindowSpecificSuffix.Equals("_2") && ElectronHandler.SecondaryWindowOpened) || WindowSpecificSuffix.Equals("_1"))
                await SoundPlayer.PlaySound(isDanger, WindowSpecificSuffix, SettingsReader.UserSettingsValues.NotificationVolume);
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
                // Only if message have correct format
                if(message.Contains("[") && message.Contains("]"))
                {
                    string time = message.Split("[")[1];
                    time = time.Split("]")[0];
                    ChatLogFile.LastLogFileRead = time.Split(" ")[2];

                    await InvokeAsync(StateHasChanged);
                }
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
            return $"Copy{WindowSpecificSuffix}{fileName}";
        }

        /// <summary>
        /// Reset the ChatLogFile to default with folder values
        /// </summary>
        private void SetDefaultChatLogFileFolders()
        {
            ChatLogFile = new ChatLogFile();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                ChatLogFile.LogFileFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\EVE\\logs\\Chatlogs\\";
                ChatLogFile.CopyLogFileFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\EveIntelChecker\\";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                ChatLogFile.LogFileFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/Documents/EVE/logs/Chatlogs/";
                ChatLogFile.CopyLogFileFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/EveIntelChecker/";
            }
        }

        /// <summary>
        /// Open or Close the Settings panel, save the settings if settings panel as been closed
        /// </summary>
        private async Task OpenCloseSettingsPanel()
        {
            SettingsPageOpened = !SettingsPageOpened;
            if (!SettingsPageOpened)
            {
                // Required to rebuild the map
                MapRebuildRequired = true;

                // Apply changes
                SettingsReader.WriteUserSettings();
                if (_selectedSystem != null && SettingsChanged)
                {
                    BuildSystems();
                    SettingsChanged = false;
                }
            }
        }

        /// <summary>
        /// Update the value of compactMode
        /// </summary>
        /// <param name="newValue">The new value to be applied</param>
        private void CompactModeChanged(bool newValue)
        {
            SettingsReader.UserSettingsValues.CompactMode = newValue;
            SettingsReader.WriteUserSettings();
        }

        /// <summary>
        /// Update the value of TopMost
        /// </summary>
        /// <param name="newValue">The new value to be applied</param>
        private void TopMostChanged(bool newValue)
        {
            SettingsReader.UserSettingsValues.WindowIsTopMost = newValue;
            SettingsReader.WriteUserSettings();
        }
        
        /// <summary>
        /// Update the value of UseKeyboardShortcuts
        /// </summary>
        /// <param name="newValue">The new value to be applied</param>
        private void UseKeyboardShortcutsChanged(bool newValue)
        {
            SettingsReader.UserSettingsValues.UseKeyboardShortcuts = newValue;
            SettingsReader.WriteUserSettings();
        }

        /// <summary>
        /// Update the value of SystemsDepth
        /// </summary>
        /// <param name="newValue">The new value to be applied</param>
        private void SystemsDepthChanged(int newValue)
        {
            SettingsReader.UserSettingsValues.SystemsDepth = newValue;
            SettingsChanged = true;
            SettingsReader.WriteUserSettings();
        }

        /// <summary>
        /// Update the value of DangerNotification
        /// </summary>
        /// <param name="newValue">The new value to be applied</param>
        private void DangerNotificationChanged(int newValue)
        {
            SettingsReader.UserSettingsValues.DangerNotification = newValue;
            if (SettingsReader.UserSettingsValues.IgnoreNotification < newValue)
                SettingsReader.UserSettingsValues.IgnoreNotification = newValue;
            SettingsReader.WriteUserSettings();
        }

        /// <summary>
        /// Update the value of IgnoreNotification
        /// </summary>
        /// <param name="newValue">The new value to be applied</param>
        private void IgnoreNotificationChanged(int newValue)
        {
            SettingsReader.UserSettingsValues.IgnoreNotification = newValue;
            if (SettingsReader.UserSettingsValues.DangerNotification > newValue)
                SettingsReader.UserSettingsValues.DangerNotification = newValue;
            SettingsReader.WriteUserSettings();
        }

        /// <summary>
        /// Update the value of NotificationVolume
        /// </summary>
        /// <param name="newValue">The new value to be applied</param>
        private async void NotificationVolumeChanged(int newValue)
        {
            SettingsReader.UserSettingsValues.NotificationVolume = newValue;
            SettingsReader.WriteUserSettings();
            await SoundPlayer.PlaySound(true, WindowSpecificSuffix, SettingsReader.UserSettingsValues.NotificationVolume);
        }

        /// <summary>
        /// Build the data required by the Javascript map
        /// </summary>
        /// <returns></returns>
        private (MapNode[], MapLink[]) BuildMapNodes()
        {
            MapNode[] mapNodes = new MapNode[IntelSystems.Count];
            List<MapLink> mapLinks = new List<MapLink>();

            // Build nodes with Id starting by 1
            for (int i = 0; i < IntelSystems.Count; ++i)
            {
                mapNodes[i] = new MapNode
                {
                    Color =
                    {
                        Background = "#1c1c1cff"
                    }
                };

                if (IntelSystems[i].IsRed)
                    mapNodes[i].Color.Background = "#ff3f5fff";
                else if (IntelSystems[i].TriggerCounter > 0)
                    mapNodes[i].Color.Background = "#ff9800ff";

                if (IntelSystems[i].Jumps == 0)
                {
                    mapNodes[i].Shape = "ellipse";
                    mapNodes[i].BorderWidth = 2;
                }

                mapNodes[i].Font.Multi = true;
                mapNodes[i].Label = $"{IntelSystems[i].SystemName}\n<code>J:{IntelSystems[i].Jumps} T:{IntelSystems[i].TriggerCounter}</code>";
                mapNodes[i].Id = i + 1;
                mapNodes[i].Region = IntelSystems[i].SystemDomainName;
                mapNodes[i].System = IntelSystems[i].SystemName;
            }

            foreach (IntelSystem system in IntelSystems)
            {
                foreach (long link in system.ConnectedSytemsId)
                {
                    MapLink systemLink = new MapLink();

                    try
                    {
                        systemLink.From = mapNodes.FirstOrDefault(x => x.Label.Contains(system.SystemName)).Id;
                        IntelSystem systemToConnect = IntelSystems.FirstOrDefault(x => x.SystemId == link);

                        // Only if system is still on the generation
                        if (systemToConnect != null)
                        {
                            systemLink.To = mapNodes.FirstOrDefault(x => x.Label.Contains(systemToConnect.SystemName)).Id;
                            if (!mapLinks.Exists(x => x.From == systemLink.From && x.To == systemLink.To) &&
                                !mapLinks.Exists(x => x.From == systemLink.To && x.To == systemLink.From))
                                mapLinks.Add(systemLink);
                        }
                    }
                    catch
                    {
                    }
                }
            }

            return (mapNodes, mapLinks.ToArray());
        }
        
        /// <summary>
        /// Task for closing the application
        /// </summary>
        private static async Task CloseApplication()
        {
            ReadFileTimer.Stop();
            ReadFileTimer.Close();
            ElectronHandler.CloseMainWindow();
        }

        /// <summary>
        /// Open an URL into the default browser
        /// </summary>
        /// <param name="url">URL to open</param>
        private async Task OpenURL(string url)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? true : false
            });
        }
    }
}