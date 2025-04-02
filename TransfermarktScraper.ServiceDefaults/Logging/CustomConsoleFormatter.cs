using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

/// <summary>
/// A custom console formatter for logging, which applies ANSI color formatting
/// to log levels and structures the log output with timestamps.
/// </summary>
public class CustomConsoleFormatter : ConsoleFormatter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomConsoleFormatter"/> class.
    /// </summary>
    public CustomConsoleFormatter()
        : base("custom")
    {
    }

    /// <summary>
    /// Writes a log entry to the console, applying ANSI color formatting based on log level.
    /// </summary>
    /// <typeparam name="TState">The type of the log state.</typeparam>
    /// <param name="logEntry">The log entry containing the log level, message, and exception.</param>
    /// <param name="scopeProvider">Provides access to log scopes.</param>
    /// <param name="textWriter">The writer to which the log entry is written.</param>
    public override void Write<TState>(
        in LogEntry<TState> logEntry,
        IExternalScopeProvider? scopeProvider,
        TextWriter textWriter)
    {
        string logLevel = logEntry.LogLevel switch
        {
            LogLevel.Information => "\u001b[32minfo:\u001b[0m ",
            LogLevel.Warning => "\u001b[33mwarn:\u001b[0m ",
            LogLevel.Error => "\u001b[31merror:\u001b[0m ",
            LogLevel.Critical => "\u001b[35mcritical:\u001b[0m ",
            LogLevel.Debug => "\u001b[36mdebug:\u001b[0m ",
            LogLevel.Trace => "\u001b[90mtrace:\u001b[0m ",
            _ => "\u001b[37mlog:\u001b[0m "
        };

        string message = logEntry.State?.ToString() ?? string.Empty;

        using var utf8Writer = new StreamWriter(Console.OpenStandardOutput(), Encoding.UTF8);
        utf8Writer.WriteLine($"{logLevel}[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}");
        utf8Writer.Flush();
    }
}