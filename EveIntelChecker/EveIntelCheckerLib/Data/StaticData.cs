using System;

namespace EveIntelCheckerLib.Data;

public static class StaticData
{
    public static int ApplicationPort { get; set; } = 3969;

    public enum LogLevel
    {
        Critical,
        Error,
        Warning,
        Info
    }

    public static void Log(LogLevel logLevel, string message)
    {
        Console.WriteLine($"[{DateTime.Now}] [{logLevel.ToString()}] {message}");
    }
}