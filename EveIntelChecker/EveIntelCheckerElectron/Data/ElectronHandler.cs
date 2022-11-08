/// Author : Sébastien Duruz
/// Date : 04.10.2022

using ElectronNET.API;
using ElectronNET.API.Entities;

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
                    AlwaysOnTop = true,
                    MinHeight = 100,
                    Height = 300,
                    MinWidth = 180,
                    Width = 180,
                    Title = "Eve Intel Checker"
                });

            // Add events to mainWindow
            AppMainWindow.OnReadyToShow += () => AppMainWindow.Show();
            AppMainWindow.OnClosed += () => Electron.App.Quit();
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
    }
}
