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
    }
}
