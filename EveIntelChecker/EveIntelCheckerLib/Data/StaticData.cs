namespace EveIntelCheckerLib.Data;

/// <summary>
/// Class StaticData
/// </summary>
public static class StaticData
{
    /// <summary>
    /// The port used by the application
    /// </summary>
    public static int ApplicationPort { get; set; } = 3969;

    /// <summary>
    /// The interval between each log file read
    /// </summary>
    public static int ReadLogInterval { get; set; } = 1000;

    /// <summary>
    /// Name of the application
    /// </summary>
    public static string ApplicationName { get; set; } = "EveIntelChecker";

    /// <summary>
    /// Name of the default log file with extension
    /// </summary>
    public static string ApplicationLogsName { get; set; } = "logs.txt";

    /// <summary>
    /// Name of the folder that contains the copied Eve logs files
    /// </summary>
    public static string EvelogsCopyFolderName { get; set; } = "evelogs_copies";

    /// <summary>
    /// LogLevel Enum
    /// </summary>
    public enum LogLevel
    {
        Critical,
        Error,
        Warning,
        Info
    }
}