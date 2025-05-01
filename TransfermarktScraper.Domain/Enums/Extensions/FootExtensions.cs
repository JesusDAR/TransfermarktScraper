using TransfermarktScraper.Domain.Exceptions;

namespace TransfermarktScraper.Domain.Enums.Extensions
{
    /// <summary>
    /// Extension methods for the <see cref="Foot"/> enum.
    /// </summary>
    public static class FootExtensions
    {
        /// <summary>
        /// Converts a <see cref="Foot"/> enum value to its corresponding string representation.
        /// </summary>
        /// <param name="foot">The <see cref="Foot"/> enum value.</param>
        /// <returns>A user friendly string representation of the <see cref="Foot"/>.</returns>
        public static string ToString(this Foot foot)
        {
            return foot switch
            {
                Foot.Right => "Right",
                Foot.Left => "Left",
                Foot.Unknown => "Unknown",
                _ => HandleUnsupportedEnum(foot),
            };
        }

        /// <summary>
        /// Converts a string representation of a <see cref="Foot"/> to its corresponding enum value.
        /// </summary>
        /// <param name="footString">The string representation of the <see cref="Foot"/>.</param>
        /// <returns>The corresponding <see cref="Foot"/> enum value.</returns>
        public static Foot ToEnum(string footString)
        {
            footString = footString.ToLower().Trim();

            return footString switch
            {
                "right" => Foot.Right,
                "left" => Foot.Left,
                "unknown" => Foot.Unknown,
                _ => HandleUnsupportedString(footString),
            };
        }

        /// <summary>
        /// Handles unsupported values of the <see cref="Foot"/> enum.
        /// Logs a warning indicating an unexpected enum value and returns an empty string.
        /// </summary>
        /// <param name="foot">The unsupported enum value.</param>
        /// <returns>An empty string.</returns>
        private static string HandleUnsupportedEnum(Foot foot)
        {
            var message = $"Unsupported enum value: {foot}";
            EnumException.LogWarning(nameof(ToString), nameof(FootExtensions), message);
            return string.Empty;
        }

        /// <summary>
        /// Handles unsupported string of the <see cref="Foot"/> enum.
        /// Logs a warning indicating an unexpected string and returns an empty string.
        /// </summary>
        /// <param name="footString">The unsupported string.</param>
        /// <returns>The <see cref="Foot.Unknown"/> enum value.</returns>
        private static Foot HandleUnsupportedString(string footString)
        {
            var message = $"Unsupported string value: {footString}";
            EnumException.LogWarning(nameof(ToEnum), nameof(FootExtensions), message);
            return Foot.Unknown;
        }
    }
}
