using System;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace TransfermarktScraper.Domain.Exceptions
{
    /// <summary>
    /// Represents an exception that occurs when interacting with the data layer.
    /// Provides methods to format a message that will be logged by other exception implementations.
    /// </summary>
    public class DatabaseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DatabaseException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Logs an error message and returns a <see cref="DatabaseException"/> instance.
        /// </summary>
        /// <param name="methodName">The method where the error occurred.</param>
        /// <param name="className">The class where the error occurred.</param>
        /// <param name="message">The error message.</param>
        /// <param name="logger">The logger instance to record the error.</param>
        /// <param name="filePath">The file path where the error was logged.</param>
        /// <param name="callerLineNumber">The line number where the <see cref="DatabaseException"/> is called.</param>
        /// <returns>A new instance of <see cref="DatabaseException"/>.</returns>
        public static DatabaseException LogError(
            string methodName,
            string className,
            string message,
            ILogger logger,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            var fileName = !string.IsNullOrEmpty(filePath) ? Path.GetFileName(filePath) : "UnknownFile";

            var logMessage = $"Error in {className}.{methodName} (File: {fileName}, Caller line: {callerLineNumber}) - Message: {message}";

            logger.LogError(logMessage);
            return new DatabaseException(message);
        }
    }
}
