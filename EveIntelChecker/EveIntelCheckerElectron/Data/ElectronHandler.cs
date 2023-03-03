/// Author : Sébastien Duruz
/// Date : 04.10.2022

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
        private static UserSettingsReader MainSettingsReader { get; set; }
        
        /// <summary>
        /// Reader for the Settings json file for SecondaryWindow
        /// </summary>
        private static UserSettingsReader SecondarySettingsReader { get; set; }
        
        /// <summary>
        /// Main App Window
        /// </summary>
        private static BrowserWindow AppMainWindow { get; set; }

        /// <summary>
        /// Create the Electron Window
        /// </summary>
        /// <returns>Results of the task</returns>
        public async static Task CreateElectronWindow()
        {
            Electron.ReadAuth();
            MainSettingsReader = new UserSettingsReader("_1");
            SecondarySettingsReader = new UserSettingsReader("_2");

            AppMainWindow = await Electron.WindowManager.CreateWindowAsync(
                new BrowserWindowOptions()
                {
                    AutoHideMenuBar = true,
                    AlwaysOnTop = MainSettingsReader.UserSettingsValues.WindowIsTopMost,
                    MinHeight = 100,
                    Height = (int)MainSettingsReader.UserSettingsValues.WindowHeight,
                    MinWidth = 180,
                    Width = (int)MainSettingsReader.UserSettingsValues.WindowWidth,
                    Title = "Eve Intel Checker",
                });
            
            // Add events to mainWindow
            AppMainWindow.OnReadyToShow += () => AppMainWindow.Show();
            AppMainWindow.OnClosed += () => CloseApplication();
        }

        

        /// <summary>
        /// Save the application dimensions before closing the app
        /// </summary>
        public static void CloseApplication()
        {
            int[] windowSize = AppMainWindow.GetSizeAsync().Result;
            int[] windowPosition = AppMainWindow.GetPositionAsync().Result;
            MainSettingsReader.UserSettingsValues.WindowWidth = windowSize[0];
            MainSettingsReader.UserSettingsValues.WindowHeight = windowSize[1];
            MainSettingsReader.WriteUserSettings();
            Electron.App.Quit();
        }
    }
}
