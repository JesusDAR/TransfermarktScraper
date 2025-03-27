using System;

namespace TransfermarktScraper.Domain.Utils
{
    /// <summary>
    /// Provides util methods to manage height values.
    /// </summary>
    public class HeightUtils
    {
        /// <summary>
        /// Converts a height string (e.g., "1,91m") to centimeters as integer (e.g., 191).
        /// </summary>
        /// <param name="height">Height string in format "X,XXm".</param>
        /// <returns>Height in centimeters.</returns>
        public static int ConvertToInt(string height)
        {
            var numericPart = height.Trim().TrimEnd('m', 'M');

            numericPart = numericPart.Replace(',', '.');

            if (double.TryParse(numericPart, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double meters))
            {
                return (int)(meters * 100);
            }

            throw new FormatException($"Error in {nameof(HeightUtils)}.{nameof(ConvertToInt)}: money format {height} not valid.");
        }

        /// <summary>
        /// Converts height in centimeters (e.g., 191) to standard format string (e.g., "1,91m").
        /// </summary>
        /// <param name="height">Height in centimeters.</param>
        /// <returns>Formatted height string.</returns>
        public static string ConvertToString(int height)
        {
            double meters = height / 100.0;
            var metersString = meters.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + "m";
            return metersString;
        }
    }
}