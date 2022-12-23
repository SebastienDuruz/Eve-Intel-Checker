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
            AppMainWindow = await Electron.WindowManager.CreateWindowAsync(
                new BrowserWindowOptions()
                {
                    AutoHideMenuBar = true,
                    AlwaysOnTop = UserSettingsReader.Instance.UserSettingsValues.WindowIsTopMost,
                    MinHeight = 100,
                    Height = (int)UserSettingsReader.Instance.UserSettingsValues.WindowHeight,
                    MinWidth = 180,
                    Width = (int)UserSettingsReader.Instance.UserSettingsValues.WindowWidth,
                    Title = "Eve Intel Checker",
                });

            // Add events to mainWindow
            AppMainWindow.OnReadyToShow += () => AppMainWindow.Show();
            AppMainWindow.OnClosed += () => CloseApplication();
        }

        public async static void ReloadMainWindow()
        {
            if (AppMainWindow != null)
            {
                AppMainWindow.Reload();

                // Resolve an issue where the focus is lost and user need to focus other window to be able to use inputs
                AppMainWindow.Minimize();
                AppMainWindow.Restore();
            }
        }

        /// <summary>
        /// Save the application dimensions before closing the app
        /// </summary>
        public static void CloseApplication()
        {
            int[] windowSize = AppMainWindow.GetSizeAsync().Result;
            UserSettingsReader.Instance.UserSettingsValues.WindowWidth = windowSize[0];
            UserSettingsReader.Instance.UserSettingsValues.WindowHeight = windowSize[1];
            UserSettingsReader.Instance.WriteUserSettings();
            Electron.App.Quit();
        }
    }
}
