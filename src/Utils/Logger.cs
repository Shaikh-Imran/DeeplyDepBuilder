using System;

namespace CodersTea.DeeplyDep.Utils;

public static class Logger
{
    public static bool isTraceEnabled { get; set; } = false;
    public static void Info(string message)
    {
        Log("INFO", message, ConsoleColor.Cyan);
    }

    public static void Error(string message, Exception? ex = null)
    {
        var fullMessage = ex == null ? message : $"{message}\n{ex}";
        Log("ERROR", fullMessage, ConsoleColor.Red);
    }

    public static void Warning(string message)
    {
        Log("WARNING", message, ConsoleColor.Yellow);
    }

    public static void Trace(string message)
    {
        if(isTraceEnabled)
            Log("TRACE", message, ConsoleColor.DarkGray);
    }

    private static void Log(string level, string message, ConsoleColor color)
    {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            Console.WriteLine($"[{timestamp}] [{level}] {message}");
            Console.ForegroundColor = originalColor;
    }
}
