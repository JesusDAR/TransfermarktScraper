using TransfermarktScraper.Domain.Exceptions;

namespace TransfermarktScraper.BLL.Utils
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
            var cleaned = money.Replace("€", string.Empty).Replace("$", string.Empty).Trim();

            var result = new string(cleaned.Where(c =>
                char.IsDigit(c) || c == '.' || c == ',' || c == 'm' || c == 'k' || c == 'b' || c == 'n').ToArray());

            return result.Replace(",", ".");
        }

        /// <summary>
        /// Converts a monetary string with abbreviations (k, m, bn) into an float.
        /// </summary>
        /// <param name="money">The monetary string (e.g., "10k", "5m", "3bn").</param>
        /// <returns>The numeric value as an float.</returns>
        /// <exception cref="UtilException">Thrown when the input format is invalid.</exception>
        public static float ConvertToFloat(string? money)
        {
            if (string.IsNullOrWhiteSpace(money))
            {
                return default;
            }

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
                else if (money.EndsWith("bn", StringComparison.OrdinalIgnoreCase)
                    || money.EndsWith("b", StringComparison.OrdinalIgnoreCase))
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
        public static string? ConvertToString(float? money)
        {
            try
            {
                if (money == null)
                {
                    return string.Empty;
                }

                decimal preciseValue = (decimal)money.Value;

                if (preciseValue >= 1_000_000_000m)
                {
                    return (preciseValue / 1_000_000_000m).ToString("0.##") + "bn";
                }
                else if (preciseValue >= 1_000_000m)
                {
                    return (preciseValue / 1_000_000m).ToString("0.##") + "m";
                }
                else if (preciseValue >= 1000m)
                {
                    return (preciseValue / 1000m).ToString("0.##") + "k";
                }
                else
                {
                    return preciseValue.ToString("0.##");
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
