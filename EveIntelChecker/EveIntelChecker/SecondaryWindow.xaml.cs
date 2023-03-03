using EveIntelCheckerLib.Data;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using System.Windows;

namespace EveIntelChecker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SecondaryWindow : Window
    {
        /// <summary>
        /// Reader for the Settings json file
        /// </summary>
        private UserSettingsReader SettingsReader { get; set; }
        
        /// <summary>
        /// Default Constructor
        /// </summary>
        public SecondaryWindow()
        {
            InitializeComponent();

            SettingsReader = new UserSettingsReader("_2");

            this.Width = SettingsReader.UserSettingsValues.WindowWidth;
            this.Height = SettingsReader.UserSettingsValues.WindowHeight;
            this.Topmost = SettingsReader.UserSettingsValues.WindowIsTopMost;
            this.Top = SettingsReader.UserSettingsValues.WindowTop;
            this.Left = SettingsReader.UserSettingsValues.WindowLeft;

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddWpfBlazorWebView();
            serviceCollection.AddMudServices();
            serviceCollection.AddSingleton(SettingsReader);
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
            SettingsReader.UserSettingsValues.WindowWidth = this.Width;
            SettingsReader.UserSettingsValues.WindowHeight = this.Height;
            SettingsReader.UserSettingsValues.WindowTop = this.Top;
            SettingsReader.UserSettingsValues.WindowLeft = this.Left;
            SettingsReader.WriteUserSettings();
        }
    }
}
