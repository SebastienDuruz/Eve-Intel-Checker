﻿/// Author : Sébastien Duruz
/// Date : 04.10.2022

using System;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using EveIntelCheckerLib.Data;

namespace EveIntelCheckerElectron.Data
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
        private static bool SecondaryWindowOpened { get; set; }
        
        /// <summary>
        /// State of the instance of Secondary Window
        /// </summary>
        private static bool SecondaryWindowInstanced { get; set; }

        /// <summary>
        /// Create the Electron Window
        /// </summary>
        /// <returns>Results of the task</returns>
        public static async Task CreateElectronWindow()
        {
            Electron.ReadAuth();
            MainSettingsReader = new UserSettingsReader("_1");
            SecondarySettingsReader = new UserSettingsReader("_2");
            SecondaryWindowOpened = false;
            SecondaryWindowInstanced = false;

            MainWindow = await Electron.WindowManager.CreateWindowAsync(
                new BrowserWindowOptions()
                {
                    AutoHideMenuBar = true,
                    Frame = false,
                    UseContentSize = true,
                    Focusable = true,
                    AlwaysOnTop = MainSettingsReader.UserSettingsValues.WindowIsTopMost,
                    MinHeight = 100,
                    Height = (int)MainSettingsReader.UserSettingsValues.WindowHeight,
                    MinWidth = 210,
                    Width = (int)MainSettingsReader.UserSettingsValues.WindowWidth,
                    X = (int)MainSettingsReader.UserSettingsValues.WindowLeft,
                    Y = (int)MainSettingsReader.UserSettingsValues.WindowTop,
                    Title = "Eve Intel Checker",
                });
            
            // Add events to mainWindow
            MainWindow.OnReadyToShow += () => MainWindow.Show();
            MainWindow.OnFocus += () =>
                MainWindow.SetAlwaysOnTop(MainSettingsReader.UserSettingsValues.WindowIsTopMost);
            MainWindow.SetAlwaysOnTop(MainSettingsReader.UserSettingsValues.WindowIsTopMost);

            if (MainSettingsReader.UserSettingsValues.UseKeyboardShortcuts)
                Electron.GlobalShortcut.Register("CommandOrControl+T", async () =>
                {
                    HideAndShowSecondaryWindow();
                });
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
            
            // Close the windows before exiting the app
            MainWindow.Close();
            if(SecondaryWindowInstanced)
                SecondaryWindow.Close();
        }

        /// <summary>
        /// Save the secondary window dimensions and positions before closing the window
        /// </summary>
        public static async void CloseSecondaryWindow()
        {
            // Save the current state of the secondaryWindow
            int[] secondaryContentSize = await SecondaryWindow.GetSizeAsync();
            int[] secondaryWindowPosition = await SecondaryWindow.GetPositionAsync();
            SecondarySettingsReader.UserSettingsValues.WindowWidth = secondaryContentSize[0] - 14;
            SecondarySettingsReader.UserSettingsValues.WindowHeight = secondaryContentSize[1] - 8;
            SecondarySettingsReader.UserSettingsValues.WindowLeft = secondaryWindowPosition[0] + 7;
            SecondarySettingsReader.UserSettingsValues.WindowTop = secondaryWindowPosition[1];
            SecondarySettingsReader.WriteUserSettings();

            // Reset the states values before closing the secondaryWindow
            SecondaryWindowInstanced = false;
            SecondaryWindowOpened = false;
            SecondaryWindow.Close();
            
        }

        public static async void HideAndShowSecondaryWindow()
        {
            if (SecondaryWindowOpened)
            {
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
                            Height = (int)SecondarySettingsReader.UserSettingsValues.WindowHeight,
                            MinWidth = 210,
                            Width = (int)SecondarySettingsReader.UserSettingsValues.WindowWidth,
                            X = (int)SecondarySettingsReader.UserSettingsValues.WindowLeft,
                            Y = (int)SecondarySettingsReader.UserSettingsValues.WindowTop,
                            Parent = MainWindow
                        });
                    SecondaryWindow.LoadURL("http://localhost:8001/secondary");
                    SecondaryWindow.OnReadyToShow += () => SecondaryWindow.Show();
                    SecondaryWindow.OnFocus += () =>
                        SecondaryWindow.SetAlwaysOnTop(MainSettingsReader.UserSettingsValues.WindowIsTopMost);
                    SecondaryWindowInstanced = true;
                    SecondaryWindowOpened = true;
                    SecondaryWindow.SetAlwaysOnTop(SecondarySettingsReader.UserSettingsValues.WindowIsTopMost);
                }
            }
        }
    }
}
