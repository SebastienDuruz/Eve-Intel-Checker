﻿using EveIntelCheckerLib.Data;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using System.Windows;
using System.Windows.Input;

namespace EveIntelChecker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Reader for the Settings json file
        /// </summary>
        private UserSettingsReader SettingsReader { get; set; }
        
        /// <summary>
        /// SecondaryWindow object
        /// </summary>
        private SecondaryWindow SecondaryWindow { get; set; } = null;
        
        /// <summary>
        /// Default Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            SettingsReader = new UserSettingsReader("_1");

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
            if (SecondaryWindow != null)
            {
                SecondaryWindow.Close();    
            }
            
            SettingsReader.UserSettingsValues.WindowWidth = this.Width;
            SettingsReader.UserSettingsValues.WindowHeight = this.Height;
            SettingsReader.UserSettingsValues.WindowTop = this.Top;
            SettingsReader.UserSettingsValues.WindowLeft = this.Left;
            SettingsReader.WriteUserSettings();
        }

        private void MainWindowOnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.T)
            {
                if (SecondaryWindow == null || !SecondaryWindow.IsVisible)
                {
                    // Create and show the new window
                    SecondaryWindow = new SecondaryWindow();
                    SecondaryWindow.Show();
                }
                
                // Set e.Handled to true to indicate that the event has been handled
                e.Handled = true;
            }
        }
    }
}
