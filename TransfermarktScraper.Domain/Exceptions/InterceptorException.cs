using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace TransfermarktScraper.Domain.Exceptions
{
    /// <summary>
    /// Represents an exception that occurs when the interceptor to retrieve the Competitions from the Countries fails.
    /// </summary>
    public class InterceptorException : LoggedException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptorException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InterceptorException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Logs an error message and returns a <see cref="InterceptorException"/> instance.
        /// </summary>
        /// <param name="methodName">The method where the error occurred.</param>
        /// <param name="className">The class where the error occurred.</param>
        /// <param name="message">The error message.</param>
        /// <param name="logger">The logger instance to record the error.</param>
        /// <param name="filePath">The file path where the error was logged.</param>
        /// <param name="callerLineNumber">The line number where the <see cref="InterceptorException"/> is called.</param>
        /// <returns>A new instance of <see cref="InterceptorException"/>.</returns>
        public static InterceptorException LogError(
            string methodName,
            string className,
            string message,
            ILogger logger,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            var logMessage = FormatLogMessage(methodName, className, message, true, default, default, filePath, callerLineNumber);

            if (logger != null)
            {
                logger.LogError(logMessage);
            }

            return new InterceptorException(logMessage);
        }
    }
}
