using EveIntelCheckerLib.Data;
using EveIntelCheckerLib.Models;
using EveIntelCheckerLib.Models.Database;
using EveIntelCheckerLib.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace EveIntelCheckerPages
{
    /// <summary>
    /// Classe MainPage
    /// </summary>
    public partial class MainPage : IAsyncDisposable
    {
        /// <summary>
        /// Window Suffix parameter
        /// </summary>
        [Parameter]
        public string? WindowSpecificSuffix { get; set; }

        /// <summary>
        /// SettingsReader object
        /// </summary>
        [Parameter]
        public UserSettingsReader? SettingsReader { get; set; }

        /// <summary>
        /// Log tailer service
        /// </summary>
        [Inject]
        public ILogFileReader LogFileReader { get; set; } = default!;

        /// <summary>
        /// Intel message processor
        /// </summary>
        [Inject]
        public IIntelMessageProcessor IntelMessageProcessor { get; set; } = default!;

        /// <summary>
        /// Map data builder
        /// </summary>
        [Inject]
        public IMapDataBuilder MapDataBuilder { get; set; } = default!;

        /// <summary>
        /// The selected system (root)
        /// </summary>
        private MapSolarSystem? _selectedSystem;

        /// <summary>
        /// Property of the _selectedSytem attribute
        /// </summary>
        private MapSolarSystem? SelectedSystem
        {
            get { return _selectedSystem; }
            set
            {
                _selectedSystem = value;
                FireAndForget(BuildSystemsAsync(), "BuildSystems");
            }
        }

        /// <summary>
        /// List of System to check
        /// </summary>
        private List<IntelSystem> IntelSystems { get; set; } = new List<IntelSystem>();

        /// <summary>
        /// Does a LogFile is currently loaded
        /// </summary>
        private bool LogFileLoaded { get; set; }

        /// <summary>
        /// The informations about current chat LogFile
        /// </summary>
        private ChatLogFile ChatLogFile { get; set; } = new ChatLogFile();

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
        private bool SettingsPageOpened { get; set; }

        /// <summary>
        /// Define if a settings as been changed by user (recreate or not the Systems list)
        /// </summary>
        private bool SettingsChanged { get; set; }

        /// <summary>
        /// Object that contains the build data ready to be used by JS (building the map)
        /// </summary>
        private MapData MapDataState { get; set; } = MapData.Empty;

        /// <summary>
        /// Set to true if settings panel just closed
        /// </summary>
        private bool MapRebuildRequired { get; set; }

        /// <summary>
        /// Main theme for MudBlazor
        /// </summary>
        private readonly MudTheme _mainTheme = new ()
        {
            PaletteDark = new PaletteDark()
            {
                Primary = "#007ea7",
                Background = "#1c1c1c",
                BackgroundGray = "#1c1c1c",
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
                Surface = "#1c1c1c",
            },
            Typography = new Typography()
            {
                Default = new DefaultTypography()
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

            if (SettingsReader == null)
                SettingsReader = new UserSettingsReader("web");

            LogFileReader.LineRead += OnLogLineRead;
            LogFileReader.Tick += OnLogTick;

            SetDefaultChatLogFileFolders();
            LoadUserSettingsLastLog();
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
                await LogFileReader.StartAsync(StaticData.ReadLogInterval);
                UpdateLogReaderState();

                await SoundPlayer.SetPlayersVolume(SettingsReader!.UserSettingsValues.NotificationVolume);
            }

            if (SettingsReader is { UserSettingsValues.CompactMode: false })
            {
                if (firstRender || MapRebuildRequired)
                {
                    await JsRuntime.InvokeVoidAsync("buildMap", [MapDataState.Nodes, MapDataState.Links]);

                    // Reset the value, avoiding rebuild at every rendering
                    MapRebuildRequired = false;
                }
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// Log tailer new line handler
        /// </summary>
        private void OnLogLineRead(object? sender, string line)
        {
            FireAndForget(InvokeAsync(() => HandleLogLineAsync(line)), "HandleLogLine");
        }

        /// <summary>
        /// Log tailer tick handler
        /// </summary>
        private void OnLogTick(object? sender, EventArgs e)
        {
            FireAndForget(InvokeAsync(CheckNewLogFile), "CheckNewLogFile");
        }

        /// <summary>
        /// Handle a new log line
        /// </summary>
        private async Task HandleLogLineAsync(string line)
        {
            if (!LogFileLoaded || IntelSystems.Count == 0 || SettingsReader == null)
                return;

            if (line == ChatLogFile.LastLogFileMessage)
                return;

            ChatLogFile.LastLogFileMessage = line;

            IntelMessageResult result = IntelMessageProcessor.Process(line, IntelSystems, SettingsReader.UserSettingsValues);
            foreach (IntelNotification notification in result.Notifications)
                PlayNotificationSound(notification.IsDanger);

            if (!string.IsNullOrWhiteSpace(result.NewRedSystemName))
            {
                LogsWriter.Instance.Log(StaticData.LogLevel.Info, $"New trigger in : {result.NewRedSystemName}");

                if (!SettingsReader.UserSettingsValues.CompactMode)
                {
                    MapDataState = MapDataBuilder.Build(IntelSystems);
                    await JsRuntime.InvokeVoidAsync("setData", new Object[] { MapDataState.Nodes });
                }
                else
                {
                    StateHasChanged();
                }
            }

            await ExtractTimeFromMessageAsync(ChatLogFile.LastLogFileMessage);
        }

        /// <summary>
        /// Update log reader enable state
        /// </summary>
        private void UpdateLogReaderState()
        {
            string? desiredPath = LogFileLoaded && !string.IsNullOrWhiteSpace(ChatLogFile.LogFileFullPath)
                ? ChatLogFile.LogFileFullPath
                : null;
            if (!string.Equals(LogFileReader.FilePath, desiredPath, StringComparison.Ordinal))
                LogFileReader.SetFilePath(desiredPath);

            LogFileReader.SetEnabled(LogFileLoaded && IntelSystems.Count > 0);
        }

        /// <summary>
        /// Fire and forget helper that logs exceptions
        /// </summary>
        private static void FireAndForget(Task task, string context)
        {
            _ = task.ContinueWith(t =>
            {
                if (t.Exception != null)
                    LogsWriter.Instance.Log(StaticData.LogLevel.Error, $"{context} failed: {t.Exception.GetBaseException().Message}");
            }, TaskScheduler.Default);
        }

        /// <summary>
        /// Load the userSettings last logfile at the initial start
        /// </summary>
        private void LoadUserSettingsLastLog()
        {
            // Start by reading the UserSettings
            if (SettingsReader != null)
            {
                SettingsReader.ReadUserSettings();

                // If a filename is found
                if (!string.IsNullOrWhiteSpace(SettingsReader.UserSettingsValues.LastLogFile))
                {
                    // Chat-log file exists
                    if (File.Exists(SettingsReader.UserSettingsValues.LastLogFile))
                    {
                        ChatLogFile.LogFileFullPath = SettingsReader.UserSettingsValues.LastLogFile;
                        ChatLogFile.LogFileShortName = ExtractShortNameFromFullPath(ChatLogFile.LogFileFullPath);
                        ChatLogFile.CopyLogFileFullPath = BuildCopyPathFromFullPath(ChatLogFile.LogFileFullPath);
                        ChatLogFile.LogFileFolder = Path.GetDirectoryName(ChatLogFile.LogFileFullPath) ?? string.Empty;
                        ChatLogFile.CopyLogFileFolder = SettingsReader.CopyLogFolderPath;
                        FileIconColor = Color.Success;
                        LogFileLoaded = true;
                        UpdateLogReaderState();
                    }
                    else
                    {
                        LogFileLoaded = false;
                        FileIconColor = Color.Error;
                        UpdateLogReaderState();
                    }
                }

                // Select the system if it exists in the DB
                if (!string.IsNullOrWhiteSpace(SettingsReader.UserSettingsValues.LastSelectedSystem)
                    && EveStaticDatabase.Instance.SolarSystems.Exists(x =>
                        x.SolarSystemName == SettingsReader.UserSettingsValues.LastSelectedSystem))
                {
                    SolarSystemSelector.Value = EveStaticDatabase.Instance.SolarSystems.Where(x =>
                        x.SolarSystemName == SettingsReader.UserSettingsValues.LastSelectedSystem).First();
                    SolarSystemSelector.Text = SettingsReader.UserSettingsValues.LastSelectedSystem;
                    SelectedSystem = SolarSystemSelector.Value;
                }
            }
        }

        /// <summary>
        /// SearchSystem event
        /// </summary>
        /// <param name="value">System name to search</param>
        /// <param name="cancellationToken"></param>
        /// <returns>DB object with sytem informations</returns>
        private static async Task<IEnumerable<MapSolarSystem>> SearchSystem(string value, CancellationToken cancellationToken = new ())
        {
            if (string.IsNullOrWhiteSpace(value))
                return [];
            
            return await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                List<MapSolarSystem> systems = EveStaticDatabase.Instance.SolarSystems
                    .Where(x =>
                        x.SolarSystemName.Contains(value, StringComparison.InvariantCultureIgnoreCase) ||
                        x.SolarSystemID.ToString().Contains(value, StringComparison.InvariantCultureIgnoreCase))
                    .ToList();

                return systems.AsEnumerable();
            }, cancellationToken);
        }

        /// <summary>
        /// Build the list of systems to display
        /// </summary>
        private async Task BuildSystemsAsync()
        {
            // Check the value before doing anything
            if (SelectedSystem == null)
                return;

            // Build the list of systems
            IntelSystems = EveStaticDatabase.Instance.BuildSystemsList(SelectedSystem, SettingsReader!.UserSettingsValues.SystemsDepth);

            MapDataState = MapDataBuilder.Build(IntelSystems);
            if (!SettingsReader.UserSettingsValues.CompactMode && !SettingsPageOpened)
                await JsRuntime.InvokeVoidAsync("buildMap", [MapDataState.Nodes, MapDataState.Links]);

            // Update the userSettings with new selected system
            SettingsReader.UserSettingsValues.LastSelectedSystem = SelectedSystem.SolarSystemName;
            SettingsReader.WriteUserSettings();
            UpdateLogReaderState();
        }

        /// <summary>
        /// Reset the triggers counter to 0
        /// </summary>
        /// <returns>Result of the Task</returns>
        private async Task ResetTriggers()
        {
            foreach (IntelSystem system in IntelSystems)
            {
                system.TriggerCounter = 0;
                system.IsRed = false;
            }

            MapDataState = MapDataBuilder.Build(IntelSystems);
            if (!SettingsReader!.UserSettingsValues.CompactMode)
                await JsRuntime.InvokeVoidAsync("setData", new Object[] { MapDataState.Nodes });
        }

        /// <summary>
        /// Resize the size and position of the starmap
        /// </summary>
        /// <returns>Result of the Task</returns>
        private async Task ResizeMap()
        {
            await JsRuntime.InvokeVoidAsync("buildMap", new Object[] { MapDataState.Nodes, MapDataState.Links });
        }

        /// <summary>
        /// Update the root system to correspond with selection
        /// </summary>
        /// <param name="system">The system to define as root</param>
        /// <returns>Result of the Task</returns>
        private void UpdateRootSystem(IntelSystem system)
        {
            SelectedSystem = EveStaticDatabase.Instance.SolarSystems.FirstOrDefault(x => x.SolarSystemName == system.SystemName);
            SolarSystemSelector.Text = SelectedSystem!.SolarSystemName;
        }

        /// <summary>
        /// Check if a new chatlog file has been created by the game.
        /// Change the chatlog file information if needed
        /// </summary>
        /// <returns>True if changed, false if nothing changed</returns>
        private void CheckNewLogFile()
        {
            if (ChatLogFile.LogFileFolder != "" && ChatLogFile.LogFileShortName != "" && LogFileLoaded)
            {
                try
                {
                    // Get the logfiles corresponding to the selected chat file
                    List<string> chatLogFiles = Directory
                        .GetFiles(ChatLogFile.LogFileFolder, $"{ChatLogFile.LogFileShortName}*.txt").ToList();
                    string[] splitedCurrent = ChatLogFile.LogFileFullPath.Split("_");

                    // Replace the logfile by the most recent if it's not the current that is used
                    foreach (string chatLogFile in chatLogFiles)
                    {
                        // Only check different files
                        if (ChatLogFile.LogFileFullPath != chatLogFile)
                        {
                            string[] splitedToCheck = chatLogFile.Split("_");

                            // FileName is not valid
                            if (splitedToCheck.Length < 4)
                                continue;

                            // Must be same client ID
                            if (splitedCurrent[3] != splitedToCheck[3])
                                continue;

                            // Creation date is too low
                            if (long.Parse(splitedToCheck[1]) < long.Parse(splitedCurrent[1]))
                                continue;

                            // same creation date but hour is too low
                            if (splitedToCheck[1] == splitedCurrent[1] &&
                                long.Parse(splitedToCheck[2]) < long.Parse(splitedCurrent[2]))
                                continue;

                            // NEW FILE DETECTED do the necessary changes
                            ChatLogFile.LogFileFullPath = chatLogFile;
                            ChatLogFile.LogFileShortName = ExtractShortNameFromFullPath(ChatLogFile.LogFileFullPath);
                            ChatLogFile.CopyLogFileFullPath = BuildCopyPathFromFullPath(ChatLogFile.LogFileFullPath);
                            ChatLogFile.LogFileFolder = Path.GetDirectoryName(chatLogFile) ?? ChatLogFile.LogFileFolder;
                            ChatLogFile.CopyLogFileFolder = SettingsReader?.CopyLogFolderPath ?? ChatLogFile.CopyLogFileFolder;
                            UpdateLogReaderState();

                            // Set the file to settings
                            SettingsReader!.UserSettingsValues.LastLogFile = ChatLogFile.LogFileFullPath;
                            SettingsReader.WriteUserSettings();
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogsWriter.Instance.Log(StaticData.LogLevel.Warning, ex.Message);
                }
            }
        }

        /// <summary>
        /// Play a sound on new trigger
        /// </summary>
        /// <returns>Result of the Task</returns>
        private void PlayNotificationSound(bool isDanger)
        {
            if (string.IsNullOrWhiteSpace(WindowSpecificSuffix))
                return;

            // Secondary Window, play sound only if Window is opened at the moment of the sound trigger
            if ((WindowSpecificSuffix.Equals("_2") && ElectronHandler.SecondaryWindowOpened) || WindowSpecificSuffix.Equals("_1"))
                FireAndForget(SoundPlayer.PlaySound(isDanger, WindowSpecificSuffix), "PlaySound");
        }

        /// <summary>
        /// Extract the time when new Log line has been detected
        /// </summary>
        /// <param name="message">The message from where to extract the time</param>
        /// <returns>Time if on the expected format, empty if not</returns>
        private Task ExtractTimeFromMessageAsync(string message)
        {
            try
            {
                // Only if message have correct format
                if (message.Contains("[") && message.Contains("]"))
                {
                    string time = message.Split("[")[1];
                    time = time.Split("]")[0];
                    ChatLogFile.LastLogFileRead = time.Split(" ")[2];

                    StateHasChanged();
                }
            }
            catch (Exception)
            {
                // Nothing to do
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Extract the chat name from a filename
        /// </summary>
        /// <param name="fileName">The full file path</param>
        /// <returns>Extracted chat name or empty if not in correct format</returns>
        private string ExtractShortNameFromFullPath(string fileName)
        {
            try
            {
                string chatName = Path.GetFileName(fileName).Split("_")[0];
                return chatName;
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// Build the copy filePath from full filepath
        /// </summary>
        /// <param name="filePath">The filepath to use</param>
        /// <returns>Copy filename</returns>
        private string BuildCopyPathFromFullPath(string filePath)
        {
            string filename = Path.GetFileName(filePath);
            return Path.Combine(SettingsReader!.CopyLogFolderPath, $"Copy{WindowSpecificSuffix}{filename}");
        }

        /// <summary>
        /// Reset the ChatLogFile to default with folder values
        /// </summary>
        private void SetDefaultChatLogFileFolders()
        {
            ChatLogFile = new ChatLogFile();

            ChatLogFile.LogFileFolder = SettingsReader!.UserSettingsValues.LogFilesFolder;
            ChatLogFile.CopyLogFileFolder = SettingsReader.CopyLogFolderPath;
            UpdateLogReaderState();
        }
        #region Settings
        /// <summary>
        /// Open or Close the Settings panel, save the settings if settings panel as been closed
        /// </summary>
        private void OpenCloseSettingsPanel()
        {
            SettingsPageOpened = !SettingsPageOpened;
            if (!SettingsPageOpened)
            {
                // Required to rebuild the map
                MapRebuildRequired = true;

                // Apply changes
                SettingsReader!.WriteUserSettings();
                if (_selectedSystem != null && SettingsChanged)
                {
                    FireAndForget(BuildSystemsAsync(), "BuildSystems");
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
            SettingsReader!.UserSettingsValues.CompactMode = newValue;
            SettingsReader.WriteUserSettings();
        }

        /// <summary>
        /// Update the value of TopMost
        /// </summary>
        /// <param name="newValue">The new value to be applied</param>
        private void TopMostChanged(bool newValue)
        {
            SettingsReader!.UserSettingsValues.WindowIsTopMost = newValue;
            SettingsReader.WriteUserSettings();
        }
        
        private void ClearResetCounterChanged(bool newValue)
        {
            SettingsReader!.UserSettingsValues.ClearResetCounter = newValue;
            SettingsReader.WriteUserSettings();
        }

        /// <summary>
        /// Update the value of SystemsDepth
        /// </summary>
        /// <param name="newValue">The new value to be applied</param>
        private void SystemsDepthChanged(int newValue)
        {
            SettingsReader!.UserSettingsValues.SystemsDepth = newValue;
            SettingsChanged = true;
            SettingsReader.WriteUserSettings();
        }

        /// <summary>
        /// Update the value of DangerNotification
        /// </summary>
        /// <param name="newValue">The new value to be applied</param>
        private void DangerNotificationChanged(int newValue)
        {
            SettingsReader!.UserSettingsValues.DangerNotification = newValue;
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
            SettingsReader!.UserSettingsValues.IgnoreNotification = newValue;
            if (SettingsReader.UserSettingsValues.DangerNotification > newValue)
                SettingsReader.UserSettingsValues.DangerNotification = newValue;
            SettingsReader.WriteUserSettings();
        }

        /// <summary>
        /// Update the value of NotificationVolume
        /// </summary>
        /// <param name="newValue">The new value to be applied</param>
        private async Task NotificationVolumeChanged(int newValue)
        {
            SettingsReader!.UserSettingsValues.NotificationVolume = newValue;
            SettingsReader.WriteUserSettings();
            await SoundPlayer.SetPlayersVolume(SettingsReader.UserSettingsValues.NotificationVolume);
            if (!string.IsNullOrWhiteSpace(WindowSpecificSuffix))
                await SoundPlayer.PlaySound(true, WindowSpecificSuffix);
        }

        #endregion Settings

        /// <summary>
        /// Task for closing the application
        /// </summary>
        private async Task CloseApplication()
        {
            await LogFileReader.StopAsync();
            await ElectronHandler.CloseMainWindow();
        }

        /// <summary>
        /// Open an URL into the default browser
        /// </summary>
        /// <param name="url">URL to open</param>
        private void OpenURL(string url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            else
                Process.Start(url);
        }

        /// <summary>
        /// Open the log file selected by the user
        /// </summary>
        /// <returns></returns>
        private async Task OpenLogFile()
        {
            string logFileFullPath = await ElectronHandler.OpenFileDialog();
            this.SetLogFile(logFileFullPath);
        }

        /// <summary>
        /// From the path of the log file, build the necessary information to use a logfile
        /// </summary>
        /// <param name="logFileFullPath"></param>
        private void SetLogFile(string logFileFullPath)
        {
            if (logFileFullPath != string.Empty)
            {
                // Update chat logs values
                ChatLogFile.LogFileFullPath = logFileFullPath; // Full name with folder path
                ChatLogFile.CopyLogFileFullPath = BuildCopyPathFromFullPath(Path.GetFileName(logFileFullPath));
                ChatLogFile.LogFileShortName = ExtractShortNameFromFullPath(Path.GetFileName(logFileFullPath));
                ChatLogFile.LogFileFolder = Path.GetDirectoryName(logFileFullPath) ?? string.Empty;
                ChatLogFile.CopyLogFileFolder = SettingsReader?.CopyLogFolderPath ?? string.Empty;
                FileIconColor = Color.Success;

                // Update the settings file
                if (SettingsReader != null)
                {
                    SettingsReader.UserSettingsValues.LastLogFile = logFileFullPath;
                    SettingsReader.UserSettingsValues.LogFilesFolder = Path.GetDirectoryName(logFileFullPath)!;
                    SettingsReader.WriteUserSettings();
                }

                LogFileLoaded = true;
                UpdateLogReaderState();
            }
            else
            {

                FileIconColor = Color.Error;
                LogFileLoaded = false;
                SetDefaultChatLogFileFolders();
                UpdateLogReaderState();
            }
        }

        /// <summary>
        /// Dispose async to stop background tasks
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            LogFileReader.LineRead -= OnLogLineRead;
            LogFileReader.Tick -= OnLogTick;
            await LogFileReader.StopAsync();
        }
    }
}
