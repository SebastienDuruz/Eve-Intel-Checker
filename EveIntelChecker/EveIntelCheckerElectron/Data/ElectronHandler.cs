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
                    MinHeight = 100,
                    MaxHeight = 1200,
                    Height = 300,
                    MinWidth = 200,
                    MaxWidth = 200,
                    Width = 200,
                    Title = "PI2"
                });

            AppMainWindow.OnReadyToShow += () => AppMainWindow.Show();

            // Add events to mainWindow
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
