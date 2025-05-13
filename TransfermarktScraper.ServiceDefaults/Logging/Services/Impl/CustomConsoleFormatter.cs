using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using TransfermarktScraper.ServiceDefaults.Logging.Services.Impl;
using TransfermarktScraper.ServiceDefaults.Logging.Services.Interfaces;

/// <summary>
/// A custom console formatter for logging, which applies ANSI color formatting
/// to log levels and structures the log output with timestamps.
/// It also sends log messages asynchronously to a SignalR hub.
/// </summary>
public class CustomConsoleFormatter : ConsoleFormatter, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private IHubContext<LogHub>? _hubContext;
    private ILogStorageService? _logStorage;
    private bool _initialized = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomConsoleFormatter"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to resolve and inject services.</param>
    public CustomConsoleFormatter(IServiceProvider serviceProvider)
        : base("custom")
    {
        _serviceProvider = serviceProvider;
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
        var logLevel = logEntry.LogLevel switch
        {
            LogLevel.Information => "\u001b[32minfo:\u001b[0m ",
            LogLevel.Warning => "\u001b[33mwarn:\u001b[0m ",
            LogLevel.Error => "\u001b[31merror:\u001b[0m ",
            LogLevel.Critical => "\u001b[35mcritical:\u001b[0m ",
            LogLevel.Debug => "\u001b[36mdebug:\u001b[0m ",
            LogLevel.Trace => "\u001b[90mtrace:\u001b[0m ",
            _ => "\u001b[37mlog:\u001b[0m "
        };

        var message = logEntry.State?.ToString() ?? string.Empty;

        using var utf8Writer = new StreamWriter(Console.OpenStandardOutput(), Encoding.UTF8);

        var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";

        utf8Writer.WriteLine(string.Concat(logLevel, logMessage));
        utf8Writer.Flush();

        if (!_initialized)
        {
            _hubContext = _serviceProvider.GetService<IHubContext<LogHub>>();
            _logStorage = _serviceProvider.GetService<ILogStorageService>();
            _initialized = true;
        }

        _logStorage?.AddLog(logMessage);

        _hubContext?.Clients.All.SendAsync("ReceiveLog", logMessage).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _hubContext = null;
    }
}