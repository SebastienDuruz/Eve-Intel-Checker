using EveIntelCheckerLib.Data;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using System.Windows;

namespace EveIntelChecker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            this.Width = UserSettingsReader.Instance.UserSettingsValues.WindowWidth;
            this.Height = UserSettingsReader.Instance.UserSettingsValues.WindowHeight;
            this.Topmost = UserSettingsReader.Instance.UserSettingsValues.WindowIsTopMost;
            this.Top = UserSettingsReader.Instance.UserSettingsValues.WindowTop;
            this.Left = UserSettingsReader.Instance.UserSettingsValues.WindowLeft;

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddWpfBlazorWebView();
            serviceCollection.AddMudServices();
            Resources.Add("services", serviceCollection.BuildServiceProvider());
        }

        /// <summary>
        /// Event that appends on window closing
        /// Save the current window size
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">args collection</param>
        private void WindowOnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UserSettingsReader userSettings = UserSettingsReader.Instance;
            userSettings.UserSettingsValues.WindowWidth = this.Width;
            userSettings.UserSettingsValues.WindowHeight = this.Height;
            userSettings.UserSettingsValues.WindowTop = this.Top;
            userSettings.UserSettingsValues.WindowLeft = this.Left;
            userSettings.WriteUserSettings();
        }
    }
}
