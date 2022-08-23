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
                    Resizable = true,
                    Maximizable = true,
                    AutoHideMenuBar = true,
                    Title = "Eve IntelChecker",
                });

            AppMainWindow.Show();

            // Add events to mainWindow
            AppMainWindow.OnClosed += () => Electron.App.Quit();
        }
    }
}
