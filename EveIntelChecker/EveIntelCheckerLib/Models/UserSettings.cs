﻿namespace EveIntelCheckerLib.Models
{
    /// <summary>
    /// Class UserSettings
    /// </summary>
    public class UserSettings
    {
        /// <summary>
        /// Last file opened by the application
        /// </summary>
        public string LastLogFile { get; set; } = string.Empty;

        /// <summary>
        /// Folder that contains the log files (set each time user load an Eve logs file
        /// </summary>
        public string LogFilesFolder { get; set; } = string.Empty;

        /// <summary>
        /// The last system selected by the user (only valid system)
        /// </summary>
        public string LastSelectedSystem { get; set; } = string.Empty;

        /// <summary>
        /// The numbers of jumps to take around the main system
        /// </summary>
        public int SystemsDepth { get; set; } = 5;

        /// <summary>
        /// The numbers of jumps to notifiate with Danger notification
        /// </summary>
        public int DangerNotification { get; set; } = 2;

        /// <summary>
        /// The numbers of jumps to ignore the base notification
        /// </summary>
        public int IgnoreNotification { get; set; } = 5;

        /// <summary>
        /// The Volume of the notification
        /// </summary>
        public int NotificationVolume { get; set; } = 100;

        /// <summary>
        /// Width of the main Window
        /// </summary>
        public int WindowWidth { get; set; } = 170;

        /// <summary>
        /// Height of the main Window
        /// </summary>
        public int WindowHeight { get; set; } = 300;

        /// <summary>
        /// Default TOP position of the main Window
        /// </summary>
        public int WindowTop { get; set; } = 100;

        /// <summary>
        /// Default LEFT position of the main Window
        /// </summary>
        public int WindowLeft { get; set; } = 100;

        /// <summary>
        /// Window is TopMost ? (default -> true)
        /// </summary>
        public bool WindowIsTopMost { get; set; } = true;

        /// <summary>
        /// Compact mode is the default display mode, if set to false display a Node graph
        /// </summary>
        public bool CompactMode { get; set; } = true;

        /// <summary>
        /// Using an alternate color scheme for accessibility usage
        /// </summary>
        public bool AccessibleTheme { get; set; } = false;

        /// <summary>
        /// Set the theme to Light/Dark
        /// </summary>
        public bool DarkMode { get; set; } = true;
    }
}
