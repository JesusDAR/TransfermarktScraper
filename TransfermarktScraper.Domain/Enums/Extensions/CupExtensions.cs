using TransfermarktScraper.Domain.Exceptions;

namespace TransfermarktScraper.Domain.Enums.Extensions
{
    /// <summary>
    /// Extension methods for the <see cref="Cup"/> enum.
    /// </summary>
    public static class CupExtensions
    {
        /// <summary>
        /// Converts a <see cref="Cup"/> enum value to its corresponding string representation.
        /// </summary>
        /// <param name="cup">The <see cref="Cup"/> enum value.</param>
        /// <returns>A user friendly string representation of the <see cref="Cup"/>.</returns>
        public static string ToString(this Cup cup)
        {
            return cup switch
            {
                Cup.DomesticCup => "Domestic Cup",
                Cup.DomesticSuperCup => "Domestic Super Cup",
                Cup.International => "International",
                Cup.Unknown => "Unknown",
                Cup.None => string.Empty,
                _ => HandleUnsupportedEnum(cup),
            };
        }

        /// <summary>
        /// Converts a string representation of a <see cref="Cup"/> to its corresponding enum value.
        /// </summary>
        /// <param name="cupString">The string representation of the <see cref="Cup"/>.</param>
        /// <returns>The corresponding <see cref="Cup"/> enum value.</returns>
        public static Cup ToEnum(string cupString)
        {
            cupString = cupString.ToLower();

            return cupString switch
            {
                "domestic cup" => Cup.DomesticCup,
                "domestic super cup" => Cup.DomesticSuperCup,
                "international" => Cup.International,
                "unknown" => Cup.Unknown,
                "" => Cup.None,
                _ => HandleUnsupportedString(cupString),
            };
        }

        /// <summary>
        /// Handles unsupported values of the <see cref="Cup"/> enum.
        /// Logs a warning indicating an unexpected enum value and returns an empty string.
        /// </summary>
        /// <param name="cup">The unsupported enum value.</param>
        /// <returns>An empty string.</returns>
        private static string HandleUnsupportedEnum(Cup cup)
        {
            var message = $"Unsupported enum value: {cup}";
            EnumException.LogWarning(nameof(ToString), nameof(CupExtensions), message);
            return ToString(Cup.Unknown);
        }

        /// <summary>
        /// Handles unsupported string of the <see cref="Cup"/> enum.
        /// Logs a warning indicating an unexpected string and returns an empty string.
        /// </summary>
        /// <param name="cupString">The unsupported string.</param>
        /// <returns>The <see cref="Cup.Unknown"/> enum value.</returns>
        private static Cup HandleUnsupportedString(string cupString)
        {
            var message = $"Unsupported string value: {cupString}";
            EnumException.LogWarning(nameof(ToEnum), nameof(CupExtensions), message);
            return Cup.Unknown;
        }
    }
}
