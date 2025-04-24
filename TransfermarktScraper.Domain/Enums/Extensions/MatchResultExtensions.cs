using TransfermarktScraper.Domain.Exceptions;

namespace TransfermarktScraper.Domain.Enums.Extensions
{
    /// <summary>
    /// Extension methods for the <see cref="MatchResult"/> enum.
    /// </summary>
    public static class MatchResultExtensions
    {
        /// <summary>
        /// Converts a <see cref="MatchResult"/> enum value to its corresponding string representation.
        /// </summary>
        /// <param name="result">The <see cref="MatchResult"/> enum value.</param>
        /// <returns>A user-friendly string representation of the <see cref="MatchResult"/>.</returns>
        public static string ToString(this MatchResult result)
        {
            return result switch
            {
                MatchResult.Win => "Win",
                MatchResult.Loss => "Loss",
                MatchResult.Draw => "Draw",
                MatchResult.Unknown => "Unknown",
                _ => HandleUnsupportedEnum(result),
            };
        }

        /// <summary>
        /// Converts a string representation of a <see cref="MatchResult"/> to its corresponding enum value.
        /// </summary>
        /// <param name="resultString">The string representation of the match result.</param>
        /// <returns>The corresponding <see cref="MatchResult"/> enum value.</returns>
        public static MatchResult ToEnum(string resultString)
        {
            resultString = resultString.ToLower().Trim();

            return resultString switch
            {
                "win" => MatchResult.Win,
                "loss" => MatchResult.Loss,
                "draw" => MatchResult.Draw,
                "unknown" => MatchResult.Unknown,
                _ => HandleUnsupportedString(resultString),
            };
        }

        /// <summary>
        /// Handles unsupported values of the <see cref="MatchResult"/> enum.
        /// Logs a warning and returns an empty string.
        /// </summary>
        /// <param name="result">The unsupported enum value.</param>
        /// <returns>An empty string.</returns>
        private static string HandleUnsupportedEnum(MatchResult result)
        {
            var message = $"Unsupported enum value: {result}";
            EnumException.LogWarning(nameof(ToString), nameof(MatchResultExtensions), message);
            return string.Empty;
        }

        /// <summary>
        /// Handles unsupported string inputs when parsing a <see cref="MatchResult"/>.
        /// Logs a warning and returns <see cref="MatchResult.Unknown"/>.
        /// </summary>
        /// <param name="resultString">The unsupported string value.</param>
        /// <returns><see cref="MatchResult.Unknown"/> enum value.</returns>
        private static MatchResult HandleUnsupportedString(string resultString)
        {
            var message = $"Unsupported string value: {resultString}";
            EnumException.LogWarning(nameof(ToEnum), nameof(MatchResultExtensions), message);
            return MatchResult.Unknown;
        }
    }
}
