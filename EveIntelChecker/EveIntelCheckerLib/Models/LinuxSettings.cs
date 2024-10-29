namespace EveIntelCheckerLib.Models;

public class LinuxSettings
{
    /// <summary>
    /// Only for linux users -> Set it to the correct Wine environnment
    /// </summary>
    public string LinuxEveLogFolder { get; set; } = string.Empty;
}