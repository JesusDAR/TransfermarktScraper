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
            var moneyArray = money.Where(char.IsLetterOrDigit).ToArray();
            return new string(moneyArray);
        }

        /// <summary>
        /// Converts a monetary string with abbreviations (k, m, bn) into an integer.
        /// </summary>
        /// <param name="money">The monetary string (e.g., "10k", "5m", "3bn").</param>
        /// <returns>The numeric value as an integer.</returns>
        /// <exception cref="FormatException">Thrown when the input format is invalid.</exception>
        public static int ToInt(string money)
        {
            money = money.Trim().ToLower();

            if (money.EndsWith("k", StringComparison.OrdinalIgnoreCase))
            {
                string numericPart = money.Substring(0, money.Length - 1);

                if (int.TryParse(numericPart, out int result))
                {
                    return result * 1000;
                }
            }
            else if (money.EndsWith("m", StringComparison.OrdinalIgnoreCase))
            {
                string numericPart = money.Substring(0, money.Length - 1);

                if (int.TryParse(numericPart, out int result))
                {
                    return result * 1_000_000;
                }
            }
            else if (money.EndsWith("bn", StringComparison.OrdinalIgnoreCase))
            {
                string numericPart = money.Substring(0, money.Length - 2);

                if (int.TryParse(numericPart, out int result))
                {
                    return result * 1_000_000_000;
                }
            }
            else
            {
                if (int.TryParse(money, out int result))
                {
                    return result;
                }
            }

            throw new FormatException($"Error in {nameof(MoneyUtils)}.{nameof(ToInt)}: money format no valid.");
        }

        /// <summary>
        /// Converts an integer monetary value into an abbreviated format (k, m, bn).
        /// </summary>
        /// <param name="money">The monetary value as an integer.</param>
        /// <returns>A formatted string representing the value in an abbreviated format.</returns>
        public static string ToAbb(int money)
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
    }
}
