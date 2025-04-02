using System;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace TransfermarktScraper.Domain.Exceptions
{
    /// <summary>
    /// Represents an exception that occurs because of the scraping process.
    /// Provides logging capabilities for warnings and errors, including metadata like file name and line number.
    /// </summary>
    public class ScrapingException : LoggedException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScrapingException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ScrapingException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrapingException"/> class with a specified error message and inner exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public ScrapingException(string message, Exception innerException)
            : base(message, innerException)
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
        /// <param name="callerLineNumber">The line number where the <see cref="ScrapingException"/> is called.</param>
        /// <returns>A new instance of <see cref="ScrapingException"/>.</returns>
        public static ScrapingException LogWarning(
            string methodName,
            string className,
            string message,
            string page,
            ILogger logger,
            Exception? exception = null,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            var logMessage = FormatLogMessage(methodName, className, message, false, page, exception, filePath, callerLineNumber);

            logger.LogWarning(logMessage);
            return new ScrapingException(logMessage);
        }

        /// <summary>
        /// Logs an error message with exception details and returns a <see cref="ScrapingException"/> instance.
        /// Captures additional stack trace information.
        /// </summary>
        /// <param name="methodName">The method where the error occurred.</param>
        /// <param name="className">The class where the error occurred.</param>
        /// <param name="message">The error message.</param>
        /// <param name="page">The page currently being scraped.</param>
        /// <param name="logger">The logger instance to record the error.</param>
        /// <param name="exception">The exception that was thrown.</param>
        /// <param name="filePath">The file path where the error was logged.</param>
        /// <param name="callerLineNumber">The line number where the <see cref="ScrapingException"/> is called.</param>
        /// <returns>A new instance of <see cref="ScrapingException"/>.</returns>
        public static ScrapingException LogError(
            string methodName,
            string className,
            string message,
            string page,
            ILogger logger,
            Exception? exception = null,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            var logMessage = FormatLogMessage(methodName, className, message, true, page, exception, filePath, callerLineNumber);

            logger.LogError(logMessage);
            return new ScrapingException(logMessage);
        }
    }
}
