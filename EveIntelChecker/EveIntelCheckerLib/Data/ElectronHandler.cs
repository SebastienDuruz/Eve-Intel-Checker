﻿using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.VisualBasic.FileIO;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EveIntelCheckerLib.Data
{
    /// <summary>
    /// Class ElectronHandler
    /// </summary>
    public static class ElectronHandler
    {
        /// <summary>
        /// Reader for the Settings json file for MainWindow
        /// </summary>
        public static UserSettingsReader MainSettingsReader { get; set; }

        /// <summary>
        /// Reader for the Settings json file for SecondaryWindow
        /// </summary>
        public static UserSettingsReader SecondarySettingsReader { get; set; }

        /// <summary>
        /// Main App Window
        /// </summary>
        private static BrowserWindow MainWindow { get; set; }

        /// <summary>
        /// Secondary App Window
        /// </summary>
        private static BrowserWindow SecondaryWindow { get; set; }

        /// <summary>
        /// State of the Secondary Window
        /// </summary>
        public static bool SecondaryWindowOpened { get; set; }

        /// <summary>
        /// State of the instance of Secondary Window
        /// </summary>
        private static bool SecondaryWindowInstanced { get; set; }

        /// <summary>
        /// Set to true if this is the first render
        /// </summary>
        private static bool IsFirstShow { get; set; } = true;

        /// <summary>
        /// Setup the settings corresponding to the Platform EveIntelChecker as been launched on
        /// </summary>
        /// <returns>result of the task</returns>
        public static bool SetupSettings()
        {
            try
            {
                MainSettingsReader = new UserSettingsReader("_1");
                SecondarySettingsReader = new UserSettingsReader("_2");
            }
            catch (Exception ex)
            {
                LogsWriter.Instance.Log(StaticData.LogLevel.Error, $"Failed to create instances for windows settings : {ex.Message}");
                return false;
            }

            return true;

            // Setup the corresponding chatlog folder for the required platform
            //if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            //    LogFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\EVE\\logs\\Chatlogs\\";
            //else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            //    LogFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/Documents/EVE/logs/Chatlogs/";
            //else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            //{
            //    // No default folder for linux
            //    // TODO : Detect if Eve is installed somewhere ?
            //}
        }

        /// <summary>
        /// Create the Electron Window
        /// </summary>
        /// <returns>Results of the task</returns>
        public static async Task CreateElectronWindow()
        {
            await ValidateApplicationPosition();
            SecondaryWindowOpened = false;
            SecondaryWindowInstanced = false;

            MainWindow = await Electron.WindowManager.CreateWindowAsync(
                new BrowserWindowOptions()
                {
                    Icon = "appIcon.ico",
                    AutoHideMenuBar = true,
                    Frame = false,
                    UseContentSize = true,
                    Focusable = true,
                    AlwaysOnTop = MainSettingsReader.UserSettingsValues.WindowIsTopMost,
                    MinHeight = 100,
                    Height = MainSettingsReader.UserSettingsValues.WindowHeight,
                    MinWidth = 210,
                    Width = MainSettingsReader.UserSettingsValues.WindowWidth,
                    X = MainSettingsReader.UserSettingsValues.WindowLeft,
                    Y = MainSettingsReader.UserSettingsValues.WindowTop,
                    Title = "Eve Intel Checker",
                });

            // Clear cache to prevent old JS file to not update
            await MainWindow.WebContents.Session.ClearCacheAsync();

            // Add events to mainWindow
            MainWindow.OnReadyToShow += MainWindowOnOnReadyToShow;
            MainWindow.OnBlur += () => MainWindow.SetAlwaysOnTop(MainSettingsReader.UserSettingsValues.WindowIsTopMost);
            MainWindow.SetAlwaysOnTop(MainSettingsReader.UserSettingsValues.WindowIsTopMost);

            LogsWriter.Instance.Log(StaticData.LogLevel.Info, "Application started");
        }

        /// <summary>
        /// OnReadyToShow Event
        /// </summary>
        private static void MainWindowOnOnReadyToShow()
        {
            MainWindow.Show();
            if (IsFirstShow)
            {
                MainWindow.Reload();
                IsFirstShow = false;
            }
        }

        /// <summary>
        /// Save the application dimensions before closing the app
        /// </summary>
        public static async void CloseMainWindow()
        {
            // Save the current state of the mainWindow
            int[] mainWindowSize = await MainWindow.GetSizeAsync();
            int[] mainWindowPosition = await MainWindow.GetPositionAsync();
            MainSettingsReader.UserSettingsValues.WindowWidth = mainWindowSize[0] - 14;
            MainSettingsReader.UserSettingsValues.WindowHeight = mainWindowSize[1] - 8;
            MainSettingsReader.UserSettingsValues.WindowLeft = mainWindowPosition[0] + 7;
            MainSettingsReader.UserSettingsValues.WindowTop = mainWindowPosition[1];
            MainSettingsReader.WriteUserSettings();

            // Save and close the secondary window if has been opened
            if (SecondaryWindowInstanced)
            {
                await SaveSecondaryWindowSettings();
                SecondaryWindow.Close();
            }

            // Close the windows before exiting the app
            MainWindow.Close();
            Electron.App.Quit();

            LogsWriter.Instance.Log(StaticData.LogLevel.Info, "Application closed");
        }

        /// <summary>
        /// Save the secondary window dimensions and positions
        /// </summary>
        private static async Task SaveSecondaryWindowSettings()
        {
            // Save the current state of the secondaryWindow
            int[] secondaryContentSize = await SecondaryWindow.GetSizeAsync();
            int[] secondaryWindowPosition = await SecondaryWindow.GetPositionAsync();
            SecondarySettingsReader.UserSettingsValues.WindowWidth = secondaryContentSize[0] - 14;
            SecondarySettingsReader.UserSettingsValues.WindowHeight = secondaryContentSize[1] - 8;
            SecondarySettingsReader.UserSettingsValues.WindowLeft = secondaryWindowPosition[0] + 7;
            SecondarySettingsReader.UserSettingsValues.WindowTop = secondaryWindowPosition[1];
            SecondarySettingsReader.WriteUserSettings();
        }

        /// <summary>
        /// Hide or Show the secondary window, automaticaly create the window if it does not exists yet
        /// </summary>
        public static async Task HideAndShowSecondaryWindow()
        {
            if (SecondaryWindowOpened)
            {
                await SaveSecondaryWindowSettings();
                SecondaryWindow.Hide();
                SecondaryWindowOpened = false;
            }
            else
            {
                if (SecondaryWindowInstanced)
                {
                    SecondaryWindow.Show();
                    SecondaryWindowOpened = true;
                }
                else
                {
                    SecondaryWindow = await Electron.WindowManager.CreateWindowAsync(
                        new BrowserWindowOptions()
                        {
                            AutoHideMenuBar = true,
                            Frame = false,
                            UseContentSize = true,
                            Focusable = true,
                            AlwaysOnTop = SecondarySettingsReader.UserSettingsValues.WindowIsTopMost,
                            MinHeight = 100,
                            Height = SecondarySettingsReader.UserSettingsValues.WindowHeight,
                            MinWidth = 210,
                            Width = SecondarySettingsReader.UserSettingsValues.WindowWidth,
                            X = SecondarySettingsReader.UserSettingsValues.WindowLeft,
                            Y = SecondarySettingsReader.UserSettingsValues.WindowTop,
                        });
                    SecondaryWindow.LoadURL($"http://localhost:{3969}/secondary");

                    SecondaryWindow.OnReadyToShow += () => SecondaryWindow.Show();
                    SecondaryWindow.OnBlur += () => SecondaryWindow.SetAlwaysOnTop(SecondarySettingsReader.UserSettingsValues.WindowIsTopMost);
                    SecondaryWindowInstanced = true;
                    SecondaryWindowOpened = true;
                    SecondaryWindow.SetAlwaysOnTop(SecondarySettingsReader.UserSettingsValues.WindowIsTopMost);
                }
            }
        }

        /// <summary>
        /// Validate that the application is on the bounds of the displays
        /// <returns>True if valid position, false if not</returns>
        /// </summary>
        private static async Task<bool> ValidateApplicationPosition()
        {
            bool mainWindowPositionIsValid = false;
            bool secondaryWindowPositionIsValid = false;
            Display[] displays = await Electron.Screen.GetAllDisplaysAsync();

            // check Windows positions
            foreach (Display display in displays)
            {
                if (display.Bounds.X <= MainSettingsReader.UserSettingsValues.WindowLeft
                    && display.Bounds.X + display.Bounds.Width >= MainSettingsReader.UserSettingsValues.WindowLeft
                    && display.Bounds.Y <= MainSettingsReader.UserSettingsValues.WindowHeight
                    && display.Bounds.Y + display.Bounds.Height >= MainSettingsReader.UserSettingsValues.WindowTop)
                    mainWindowPositionIsValid = true;

                if (display.Bounds.X <= SecondarySettingsReader.UserSettingsValues.WindowLeft
                    && display.Bounds.X + display.Bounds.Width >= SecondarySettingsReader.UserSettingsValues.WindowLeft
                    && display.Bounds.Y <= SecondarySettingsReader.UserSettingsValues.WindowHeight
                    && display.Bounds.Y + display.Bounds.Height >= SecondarySettingsReader.UserSettingsValues.WindowTop)
                    secondaryWindowPositionIsValid = true;
            }

            // Reset the position values for MainWindow
            if (!mainWindowPositionIsValid)
            {
                MainSettingsReader.UserSettingsValues.WindowLeft = 100;
                MainSettingsReader.UserSettingsValues.WindowTop = 100;
                MainSettingsReader.WriteUserSettings();
            }

            // Reset the position values for SecondaryWindow
            if (!secondaryWindowPositionIsValid)
            {
                SecondarySettingsReader.UserSettingsValues.WindowLeft = 100;
                SecondarySettingsReader.UserSettingsValues.WindowTop = 100;
                SecondarySettingsReader.WriteUserSettings();
            }

            // Return the result
            return mainWindowPositionIsValid && secondaryWindowPositionIsValid;
        }

        /// <summary>
        /// Open dialog to select a log file
        /// </summary>
        /// <returns></returns>
        public static async Task<string> OpenFileDialog()
        {
            // Set the default path, if not already set or exists -> default is documents
            string defaultPath = MainSettingsReader.UserSettingsValues.LogFilesFolder == "" ? SpecialDirectories.MyDocuments : MainSettingsReader.UserSettingsValues.LogFilesFolder;
            if(!Directory.Exists(defaultPath))
                defaultPath = SpecialDirectories.MyDocuments;

            string[] files = await Electron.Dialog.ShowOpenDialogAsync(MainWindow, new OpenDialogOptions()
            {
                DefaultPath = defaultPath,
                Properties = [OpenDialogProperty.openFile],
                Filters = [new FileFilter { Name = "Text Files", Extensions = new[] { "txt" }}],
            });


            if (files.Length > 0)
            {
                LogsWriter.Instance.Log(StaticData.LogLevel.Info, $"Logfile path to load : {files[0]}");
                return files[0];
            }

            LogsWriter.Instance.Log(StaticData.LogLevel.Info, "Logfile path is empty");
            return string.Empty;
        }
    }
}
