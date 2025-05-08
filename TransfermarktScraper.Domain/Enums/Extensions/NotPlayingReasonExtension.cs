using TransfermarktScraper.Domain.Exceptions;

namespace TransfermarktScraper.Domain.Enums.Extensions
{
    /// <summary>
    /// Extension methods for the <see cref="NotPlayingReason"/> enum.
    /// </summary>
    public static class NotPlayingReasonExtension
    {
        /// <summary>
        /// Converts a <see cref="NotPlayingReason"/> enum value to its corresponding string representation.
        /// </summary>
        /// <param name="reason">The <see cref="NotPlayingReason"/> enum value.</param>
        /// <returns>A user-friendly string representation of the <see cref="NotPlayingReason"/>.</returns>
        public static string ToString(this NotPlayingReason reason)
        {
            return reason switch
            {
                NotPlayingReason.OnTheBench => "On the Bench",
                NotPlayingReason.NotInSquad => "Not in Squad",
                NotPlayingReason.Injured => "Injured",
                NotPlayingReason.RedCardSuspension => "Red Card Suspension",
                NotPlayingReason.Other => "Other",
                NotPlayingReason.None => "None",
                _ => HandleUnsupportedEnum(reason),
            };
        }

        /// <summary>
        /// Converts a string representation of a <see cref="NotPlayingReason"/> to its corresponding enum value.
        /// </summary>
        /// <param name="reasonString">The string representation of the reason.</param>
        /// <returns>The corresponding <see cref="NotPlayingReason"/> enum value.</returns>
        public static NotPlayingReason ToEnum(string reasonString)
        {
            reasonString = reasonString.ToLower().Trim();

            return reasonString switch
            {
                "on the bench" => NotPlayingReason.OnTheBench,
                "not in squad" => NotPlayingReason.NotInSquad,
                "injured" => NotPlayingReason.Injured,
                "red card suspension" => NotPlayingReason.RedCardSuspension,
                "other" => NotPlayingReason.Other,
                "none" => NotPlayingReason.None,
                _ => HandleUnsupportedString(reasonString),
            };
        }

        /// <summary>
        /// Handles unsupported enum values for <see cref="NotPlayingReason"/>.
        /// </summary>
        /// <param name="reason">The unsupported enum value.</param>
        /// <returns>An empty string.</returns>
        private static string HandleUnsupportedEnum(NotPlayingReason reason)
        {
            var message = $"Unsupported enum value: {reason}";
            EnumException.LogWarning(nameof(ToString), nameof(NotPlayingReasonExtension), message);
            return string.Empty;
        }

        /// <summary>
        /// Handles unsupported string values when parsing to <see cref="NotPlayingReason"/>.
        /// </summary>
        /// <param name="reasonString">The unsupported string value.</param>
        /// <returns><see cref="NotPlayingReason.None"/> as the default fallback.</returns>
        private static NotPlayingReason HandleUnsupportedString(string reasonString)
        {
            var message = $"Unsupported string value: {reasonString}";
            EnumException.LogWarning(nameof(ToEnum), nameof(NotPlayingReasonExtension), message);
            return NotPlayingReason.None;
        }
    }
}
