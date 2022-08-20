using ElectronNET.API;
using ElectronNET.API.Entities;

namespace EveIntelChecker.ElectronApp
{
    public static class ElectronHandler
    {
        private static BrowserWindow AppMainWindow { get; set; }

        public async static Task CreateElectronWindow()
        {
            AppMainWindow = await Electron.WindowManager.CreateWindowAsync(
                new BrowserWindowOptions()
                {
                    Width = 1300,
                    Height = 1000,
                    Resizable = false,
                    Maximizable = false,
                    AutoHideMenuBar = true,
                    Title = "Eve IntelChecker",
                });

            AppMainWindow.Show();

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
