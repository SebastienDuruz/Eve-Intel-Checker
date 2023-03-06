/// Author : Sébastien Duruz
/// Date : 04.10.2022

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
        public async static Task CreateElectronWindow()
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
                    AlwaysOnTop = MainSettingsReader.UserSettingsValues.WindowIsTopMost,
                    MinHeight = 100,
                    Height = (int)MainSettingsReader.UserSettingsValues.WindowHeight,
                    MinWidth = 180,
                    Width = (int)MainSettingsReader.UserSettingsValues.WindowWidth,
                    X = (int)MainSettingsReader.UserSettingsValues.WindowLeft,
                    Y = (int)MainSettingsReader.UserSettingsValues.WindowTop,
                    Title = "Eve Intel Checker",
                });
            
            // Add events to mainWindow
            MainWindow.OnReadyToShow += () => MainWindow.Show();
            MainWindow.OnClose += () => CloseMainWindow();
            
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
            int[] mainWindowSize = await MainWindow.GetSizeAsync();
            int[] mainWindowPosition = await MainWindow.GetPositionAsync();
            MainSettingsReader.UserSettingsValues.WindowWidth = mainWindowSize[0];
            MainSettingsReader.UserSettingsValues.WindowHeight = mainWindowSize[1];
            MainSettingsReader.UserSettingsValues.WindowLeft = mainWindowPosition[0];
            MainSettingsReader.UserSettingsValues.WindowTop = mainWindowPosition[1];
            MainSettingsReader.WriteUserSettings();
            
            MainWindow.Destroy();
            SecondaryWindow.Destroy();
            Electron.App.Quit();
        }

        /// <summary>
        /// Save the secondary window dimensions and positions before closing the window
        /// </summary>
        private static async void CloseSecondaryWindow()
        {
            int[] secondaryWindowSize = await SecondaryWindow.GetSizeAsync();
            int[] secondaryWindowPosition = await SecondaryWindow.GetPositionAsync();
            SecondarySettingsReader.UserSettingsValues.WindowWidth = secondaryWindowSize[0];
            SecondarySettingsReader.UserSettingsValues.WindowHeight = secondaryWindowSize[1];
            SecondarySettingsReader.UserSettingsValues.WindowLeft = secondaryWindowPosition[0];
            SecondarySettingsReader.UserSettingsValues.WindowTop = secondaryWindowPosition[1];
            SecondarySettingsReader.WriteUserSettings();
        }

        public static async void HideAndShowSecondaryWindow()
        {
            if (SecondaryWindowOpened)
            {
                CloseSecondaryWindow();
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
                            Closable = false,
                            Frame = false,
                            AlwaysOnTop = SecondarySettingsReader.UserSettingsValues.WindowIsTopMost,
                            MinHeight = 100,
                            Height = (int)SecondarySettingsReader.UserSettingsValues.WindowHeight,
                            MinWidth = 180,
                            Width = (int)SecondarySettingsReader.UserSettingsValues.WindowWidth,
                            X = (int)SecondarySettingsReader.UserSettingsValues.WindowLeft,
                            Y = (int)SecondarySettingsReader.UserSettingsValues.WindowTop,
                            Title = "Eve Intel Checker - Secondary"
                        });
                    SecondaryWindow.LoadURL("http://localhost:8001/secondary");
                    SecondaryWindow.OnReadyToShow += () => SecondaryWindow.Show();
                    SecondaryWindow.OnClose += () => CloseSecondaryWindow();
                    SecondaryWindowInstanced = true;
                    SecondaryWindowOpened = true;
                }
            }
        }
    }
}
