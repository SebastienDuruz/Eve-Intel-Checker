namespace EveIntelCheckerLib.Models
{
    /// <summary>
    /// Class UserSettings
    /// </summary>
    public class UserSettings
    {
        /// <summary>
        /// Last file opened by the application
        /// </summary>
        public string LastFileName { get; set; } = "";

        /// <summary>
        /// The last system selected by the user (only valid system)
        /// </summary>
        public string LastSelectedSystem { get; set; } = "";

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
        public double WindowWidth { get; set; } = 170;

        /// <summary>
        /// Height of the main Window
        /// </summary>
        public double WindowHeight { get; set; } = 300;

        /// <summary>
        /// Compact mode is the default display mode, if set to false display a Node graph, not currently used
        /// </summary>
        public bool CompactMode { get; set; } = true;

        /// <summary>
        /// Using an alternate color sheme for accessibility usage
        /// </summary>
        public bool AccessibleTheme { get; set; } = false;

        /// <summary>
        /// Set the theme to Light/Dark
        /// </summary>
        public bool DarkMode { get; set; } = true;
    }
}
