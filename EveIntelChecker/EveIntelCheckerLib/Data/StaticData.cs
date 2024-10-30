using System;

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

    /// <summary>
    /// Log a message from the whole application
    /// </summary>
    /// <param name="logLevel">Loglevel to apply to the message</param>
    /// <param name="message">The message to log</param>
    public static void Log(LogLevel logLevel, string message)
    {
        Console.WriteLine($"[{DateTime.Now}] [{logLevel.ToString()}] {message}");
    }
}