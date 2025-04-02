using System;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace TransfermarktScraper.Domain.Exceptions
{
    /// <summary>
    /// Represents an exception that occurs during the utils operations.
    /// Provides methods to format a message that will be logged by other exception implementations.
    /// </summary>
    public class UtilException : LoggedException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UtilException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public UtilException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Logs a warning message including metadata such as class, method, file, and line number.
        /// </summary>
        /// <param name="methodName">The method where the warning occurred.</param>
        /// <param name="className">The class where the warning occurred.</param>
        /// <param name="message">The warning message.</param>
        /// <param name="page">The page currently being scraped.</param>
        /// <param name="logger">The logger instance to record the message.</param>
        /// <param name="exception">The exception that was thrown.</param>
        /// <param name="filePath">The file path where the warning was logged.</param>
        /// <param name="callerLineNumber">The line number where the <see cref="UtilException"/> is called.</param>
        /// <returns>A new instance of <see cref="UtilException"/>.</returns>
        public static UtilException LogWarning(
            string methodName,
            string className,
            string message,
            string? page = null,
            ILogger? logger = null,
            Exception? exception = null,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            var logMessage = FormatLogMessage(methodName, className, message, false, page, exception, filePath, callerLineNumber);

            if (logger != null)
            {
                logger.LogWarning(logMessage);
            }

            return new UtilException(logMessage);
        }

        /// <summary>
        /// Logs an error message with exception details and returns a <see cref="UtilException"/> instance.
        /// Captures additional stack trace information.
        /// </summary>
        /// <param name="methodName">The method where the error occurred.</param>
        /// <param name="className">The class where the error occurred.</param>
        /// <param name="message">The error message.</param>
        /// <param name="page">The page currently being scraped.</param>
        /// <param name="logger">The logger instance to record the error.</param>
        /// <param name="exception">The exception that was thrown.</param>
        /// <param name="filePath">The file path where the error was logged.</param>
        /// <param name="callerLineNumber">The line number where the <see cref="UtilException"/> is called.</param>
        /// <returns>A new instance of <see cref="UtilException"/>.</returns>
        public static UtilException LogError(
            string methodName,
            string className,
            string message,
            string? page = null,
            ILogger? logger = null,
            Exception? exception = null,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            var logMessage = FormatLogMessage(methodName, className, message, true, page, exception, filePath, callerLineNumber);

            if (logger != null)
            {
                logger.LogError(logMessage);
            }

            return new UtilException(logMessage);
        }
    }
}
