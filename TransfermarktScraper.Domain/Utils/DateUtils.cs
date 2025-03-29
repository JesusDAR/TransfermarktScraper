using System;
using System.Globalization;

namespace TransfermarktScraper.Domain.Utils
{
    /// <summary>
    /// Provides util methods to manage date values.
    /// </summary>
    public static class DateUtils
    {
        /// <summary>
        /// Converts a string representation of a date to a DateTime object.
        /// </summary>
        /// <param name="date">The date string to convert (expected format: "MMM d, yyyy" e.g., "Jun 26, 1999").</param>
        /// <returns>A DateTime object representing the parsed date.</returns>
        public static DateTime? ConvertToDateTime(string date)
        {
            if (string.IsNullOrWhiteSpace(date))
            {
                return null;
            }

            var dateTime = DateTime.ParseExact(
                date,
                "MMM d, yyyy",
                CultureInfo.InvariantCulture);

            return dateTime;
        }
    }
}
