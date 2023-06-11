namespace Drogecode.Knrm.Oefenrooster.Client.Helpers;

public class DebugHelper
{
    public static bool LogToConsole { get; set; } = true;
    /// <summary>
    /// Only write to console when debugging is true;
    /// </summary>
    /// <param name="message"></param>
    public static void WriteLine(string message)
    {
        if (!LogToConsole) return;
        Console.WriteLine(message);
    }
    /// <summary>
    /// Only write exception to console when debugging is true;
    /// </summary>
    /// <param name="exception">Exception to log</param>
    public static void WriteLine(Exception exception)
    {
        if (!LogToConsole) return;
        Console.WriteLine(exception);
    }
}
