using System;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace TransfermarktScraper.Domain.Exceptions
{
    /// <summary>
    /// Represents an exception that occurs in the enum extension classes when assigning values.
    /// Provides methods to format a message that will be logged by other exception implementations.
    /// </summary>
    public class EnumException : LoggedException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public EnumException(string message)
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
        /// <param name="callerLineNumber">The line number where the <see cref="EnumException"/> is called.</param>
        /// <returns>A new instance of <see cref="EnumException"/>.</returns>
        public static EnumException LogWarning(
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

            return new EnumException(logMessage);
        }

        /// <summary>
        /// Logs an error message with exception details and returns a <see cref="ScrapingException"/> instance.
        /// Captures additional stack trace information.
        /// </summary>
        /// <param name="page">The page currently being scraped.</param>
        /// <param name="methodName">The method where the error occurred.</param>
        /// <param name="className">The class where the error occurred.</param>
        /// <param name="message">The error message.</param>
        /// <param name="logger">The logger instance to record the error.</param>
        /// <param name="exception">The exception that was thrown.</param>
        /// <param name="filePath">The file path where the error was logged.</param>
        /// <param name="callerLineNumber">The line number where the <see cref="ScrapingException"/> is called.</param>
        /// <returns>A new instance of <see cref="ScrapingException"/>.</returns>
        public static EnumException LogError(
            string page,
            string methodName,
            string className,
            string message,
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

            return new EnumException(logMessage);
        }
    }
}
