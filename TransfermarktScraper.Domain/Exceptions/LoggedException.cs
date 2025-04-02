using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace TransfermarktScraper.Domain.Exceptions
{
    /// <summary>
    /// Base class for exceptions with built-in logging capabilities
    /// </summary>
    public abstract class LoggedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoggedException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public LoggedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggedException"/> class with a specified error message and inner exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public LoggedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Formats the message to be logged.
        /// </summary>
        /// <param name="methodName">The method where the error occurred.</param>
        /// <param name="className">The class where the error occurred.</param>
        /// <param name="message">The error message.</param>
        /// <param name="isError">Value to check whether the log is for error or warning.</param>
        /// <param name="page">The page currently being scraped.</param>
        /// <param name="exception">The exception that was thrown.</param>
        /// <param name="filePath">The file path where the error was logged.</param>
        /// <param name="callerLineNumber">The line number where the <see cref="ScrapingException"/> is called.</param>
        /// <returns>A new instance of <see cref="ScrapingException"/>.</returns>
        public static string FormatLogMessage(
            string methodName,
            string className,
            string message,
            bool isError,
            string? page = null,
            Exception? exception = null,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            StackTrace? stackTrace = null;

            if (exception != null)
            {
                stackTrace = new StackTrace(exception, true);
            }

            var frame = stackTrace?.GetFrame(0); // First line of the error
            var errorLineNumber = frame?.GetFileLineNumber() ?? 0;

            var fileName = !string.IsNullOrEmpty(filePath) ? Path.GetFileName(filePath) : "UnknownFile";

            var logMessage = string.Empty;

            logMessage =
                $"\n{(isError ? "Error" : "Warning")} in {className}.{methodName}\n" +
                $"{(!string.IsNullOrWhiteSpace(page) ? string.Concat("Page: ", page) : string.Empty)}\n" +
                $"File: {fileName},\n" +
                $"Caller line: {callerLineNumber}\n" +
                $"{(errorLineNumber != 0 ? string.Concat("Error line: ", errorLineNumber, "\n") : string.Empty)}" +
                $"Message: {message}\n" +
                $"{(!string.IsNullOrWhiteSpace(exception?.Message) ? string.Concat("StackTraceMessage => {\n", exception.Message.Replace("\n", " "), "\n}\n") : string.Empty)}";

            return logMessage;
        }
    }
}
