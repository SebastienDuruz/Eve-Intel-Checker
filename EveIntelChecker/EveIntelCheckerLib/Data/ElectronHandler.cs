using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.Extensions.Caching.Memory;
using System.Windows;

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
        /// Create the Electron Window
        /// </summary>
        /// <returns>Results of the task</returns>
        public static async Task CreateElectronWindow()
        {
            MainSettingsReader = new UserSettingsReader("_1");
            SecondarySettingsReader = new UserSettingsReader("_2");
            
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
            
            // Add events to mainWindow
            MainWindow.OnReadyToShow += () => MainWindow.Show();
            MainWindow.OnBlur += () => MainWindow.SetAlwaysOnTop(MainSettingsReader.UserSettingsValues.WindowIsTopMost);
            MainWindow.SetAlwaysOnTop(MainSettingsReader.UserSettingsValues.WindowIsTopMost);
            
            // Check if shortcuts are required
            if (MainSettingsReader.UserSettingsValues.UseKeyboardShortcuts)
                Electron.GlobalShortcut.Register("CommandOrControl+T", async () =>
                {
                    await HideAndShowSecondaryWindow();
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

            // Save and close the secondary window if has been opened
            if (SecondaryWindowInstanced)
            {
                await SaveSecondaryWindowSettings();
                SecondaryWindow.Close();                
            }
            
            // Close the windows before exiting the app
            MainWindow.Close();
            Electron.App.Exit();
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
                            Height = (int)SecondarySettingsReader.UserSettingsValues.WindowHeight,
                            MinWidth = 210,
                            Width = (int)SecondarySettingsReader.UserSettingsValues.WindowWidth,
                            X = (int)SecondarySettingsReader.UserSettingsValues.WindowLeft,
                            Y = (int)SecondarySettingsReader.UserSettingsValues.WindowTop,
                        });
                    SecondaryWindow.LoadURL("http://localhost:8001/secondary");

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
            return mainWindowPositionIsValid && secondaryWindowPositionIsValid ? true : false;
        }
    }
}
