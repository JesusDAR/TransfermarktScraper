using System;
using System.Linq;
using TransfermarktScraper.Domain.Exceptions;

namespace TransfermarktScraper.Domain.Utils
{
    /// <summary>
    /// Provides util methods to manage monetary values.
    /// </summary>
    public static class MoneyUtils
    {
        /// <summary>
        /// Extracts the numeric portion from a monetary string, removing any currency symbols.
        /// </summary>
        /// <param name="money">The monetary string (e.g., "€10k", "$5m", "£3bn").</param>
        /// <returns>The numeric part as a string (e.g., "10k", "5m", "3bn").</returns>
        public static string ExtractNumericPart(string money)
        {
            var moneyArray = money.Where(char.IsLetterOrDigit).ToArray();
            return new string(moneyArray);
        }

        /// <summary>
        /// Converts a monetary string with abbreviations (k, m, bn) into an float.
        /// </summary>
        /// <param name="money">The monetary string (e.g., "10k", "5m", "3bn").</param>
        /// <returns>The numeric value as an float.</returns>
        /// <exception cref="FormatException">Thrown when the input format is invalid.</exception>
        public static float ConvertToFloat(string money)
        {
            money = money.Trim().ToLower();

            try
            {
                if (money.EndsWith("k", StringComparison.OrdinalIgnoreCase))
                {
                    string numericPart = money.Substring(0, money.Length - 1);

                    if (float.TryParse(numericPart, out float result))
                    {
                        return result * 1000;
                    }
                }
                else if (money.EndsWith("m", StringComparison.OrdinalIgnoreCase))
                {
                    string numericPart = money.Substring(0, money.Length - 1);

                    if (float.TryParse(numericPart, out float result))
                    {
                        return result * 1_000_000;
                    }
                }
                else if (money.EndsWith("bn", StringComparison.OrdinalIgnoreCase))
                {
                    string numericPart = money.Substring(0, money.Length - 2);

                    if (float.TryParse(numericPart, out float result))
                    {
                        return result * 1_000_000_000;
                    }
                }
                else
                {
                    if (float.TryParse(money, out float result))
                    {
                        return result;
                    }

                    var message = $"Money abbreviation {money} not found.";
                    throw UtilException.LogWarning(nameof(ConvertToFloat), nameof(MoneyUtils), message);
                }
            }
            catch (Exception ex)
            {
                var message = $"Converting money: {money} to float failed.";
                throw UtilException.LogWarning(nameof(ConvertToFloat), nameof(MoneyUtils), message, default, default, ex);
            }

            return 0;
        }

        /// <summary>
        /// Converts an float monetary value into an abbreviated format (k, m, bn).
        /// </summary>
        /// <param name="money">The monetary value as an float.</param>
        /// <returns>A formatted string representing the value in an abbreviated format.</returns>
        public static string ConvertToString(float money)
        {
            try
            {
                if (money >= 1_000_000_000 && money % 1_000_000_000 == 0)
                {
                    return (money / 1_000_000_000) + "bn";
                }
                else if (money >= 1_000_000 && money % 1_000_000 == 0)
                {
                    return (money / 1_000_000) + "m";
                }
                else if (money >= 1000 && money % 1000 == 0)
                {
                    return (money / 1000) + "k";
                }
                else
                {
                    return money.ToString();
                }
            }
            catch (Exception)
            {
                var message = $"Converting money: {money} to string failed.";
                throw UtilException.LogWarning(nameof(ConvertToString), nameof(MoneyUtils), message);
            }
        }
    }
}
