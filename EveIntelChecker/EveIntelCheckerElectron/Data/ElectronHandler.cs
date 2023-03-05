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
        private static BrowserWindow MainWindow { get; set; }
        
        /// <summary>
        /// Secondary App Window
        /// </summary>
        private static BrowserWindow SecondaryWindow { get; set; }

        /// <summary>
        /// Create the Electron Window
        /// </summary>
        /// <returns>Results of the task</returns>
        public async static Task CreateElectronWindow()
        {
            Electron.ReadAuth();
            MainSettingsReader = new UserSettingsReader("1_");
            SecondarySettingsReader = new UserSettingsReader("2_");

            MainWindow = await Electron.WindowManager.CreateWindowAsync(
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
            MainWindow.OnReadyToShow += () => MainWindow.Show();
            MainWindow.OnClosed += () => CloseApplication();
            
            Electron.GlobalShortcut.Register("CommandOrControl+T", async () =>
            {
                if (SecondaryWindow == null)
                {
                    SecondaryWindow = await Electron.WindowManager.CreateWindowAsync(
                        new BrowserWindowOptions()
                        {
                            AutoHideMenuBar = true,
                            AlwaysOnTop = SecondarySettingsReader.UserSettingsValues.WindowIsTopMost,
                            MinHeight = 100,
                            Height = (int)SecondarySettingsReader.UserSettingsValues.WindowHeight,
                            MinWidth = 180,
                            Width = (int)SecondarySettingsReader.UserSettingsValues.WindowWidth,
                            Title = "Eve Intel Checker - Secondary",
                            Parent = MainWindow
                        });
                    SecondaryWindow.LoadURL("/secondary");
                    SecondaryWindow.Show();
                }
            });
        }

        /// <summary>
        /// Save the application dimensions before closing the app
        /// </summary>
        public static void CloseApplication()
        {
            int[] mainWindowSize = MainWindow.GetSizeAsync().Result;
            int[] mainWindowPosition = MainWindow.GetPositionAsync().Result;
            MainSettingsReader.UserSettingsValues.WindowWidth = mainWindowSize[0];
            MainSettingsReader.UserSettingsValues.WindowHeight = mainWindowSize[1];
            MainSettingsReader.WriteUserSettings();
            
            int[] secondaryWindowSize = MainWindow.GetSizeAsync().Result;
            int[] secondaryWindowPosition = MainWindow.GetPositionAsync().Result;
            SecondarySettingsReader.UserSettingsValues.WindowWidth = secondaryWindowSize[0];
            SecondarySettingsReader.UserSettingsValues.WindowHeight = secondaryWindowSize[1];
            SecondarySettingsReader.WriteUserSettings();
            Electron.App.Quit();
        }
    }
}
