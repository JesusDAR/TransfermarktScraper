using System.Globalization;
using TransfermarktScraper.Domain.Exceptions;

namespace TransfermarktScraper.BLL.Utils
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
        public static DateTime? ConvertToDateTime(string? date)
        {
            try
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
            catch (Exception ex)
            {
                var message = $"Parsing string date: {date} to DateTime failed.";
                throw UtilException.LogWarning(nameof(ConvertToDateTime), nameof(DateUtils), message, default, default, ex);
            }
        }

        /// <summary>
        /// Converts a DateTime object to a string in the format "MMM d, yyyy" (e.g., "Jun 26, 1999").
        /// </summary>
        /// <param name="date">The DateTime object to convert.</param>
        /// <returns>A string representing the formatted date.</returns>
        public static string? ConvertToString(DateTime? date)
        {
            if (!date.HasValue)
            {
                return null;
            }

            return date.Value.ToString("MMM d, yyyy", CultureInfo.InvariantCulture);
        }
    }
}
