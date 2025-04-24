using System;
using TransfermarktScraper.Domain.Exceptions;

namespace TransfermarktScraper.Domain.Enums.Extensions
{
    /// <summary>
    /// Extension methods for the <see cref="Tier"/> enum.
    /// </summary>
    public static class TierExtensions
    {
        /// <summary>
        /// Converts a <see cref="Tier"/> enum value to its corresponding string representation.
        /// </summary>
        /// <param name="tier">The <see cref="Tier"/> enum value.</param>
        /// <returns>A user friendly string representation of the <see cref="Tier"/>.</returns>
        public static string ToString(this Tier tier)
        {
            return tier switch
            {
                Tier.FirstTier => "First Tier",
                Tier.SecondTier => "Second Tier",
                Tier.ThirdTier => "Third Tier",
                Tier.YouthLeague => "Youth League",
                Tier.Unknown => "Unknown Tier",
                Tier.None => string.Empty,
                _ => HandleUnsupportedEnum(tier),
            };
        }

        /// <summary>
        /// Converts a string representation of a <see cref="Tier"/> to its corresponding enum value.
        /// </summary>
        /// <param name="tierString">The string representation of the <see cref="Tier"/>.</param>
        /// <returns>The corresponding <see cref="Tier"/> enum value.</returns>
        public static Tier ToEnum(string tierString)
        {
            tierString = tierString.ToLower().Trim();

            if (tierString.Contains("first tier"))
            {
                return Tier.FirstTier;
            }

            if (tierString.Contains("second tier"))
            {
                return Tier.SecondTier;
            }

            if (tierString.Contains("third tier"))
            {
                return Tier.ThirdTier;
            }

            if (tierString.Contains("youth league"))
            {
                return Tier.YouthLeague;
            }

            if (!string.IsNullOrEmpty(tierString))
            {
                return Tier.Unknown;
            }

            return HandleUnsupportedString(tierString);
        }

        /// <summary>
        /// Handles unsupported values of the <see cref="Tier"/> enum.
        /// Logs a warning indicating an unexpected enum value and returns an empty string.
        /// </summary>
        /// <param name="tier">The unsupported enum value.</param>
        /// <returns>An empty string.</returns>
        private static string HandleUnsupportedEnum(Tier tier)
        {
            var message = $"Unsupported enum value: {tier}";
            EnumException.LogWarning(nameof(ToString), nameof(TierExtensions), message);
            return ToString(Tier.Unknown);
        }

        /// <summary>
        /// Handles unsupported string of the <see cref="Tier"/> enum.
        /// Logs a warning indicating an unexpected string and returns an empty string.
        /// </summary>
        /// <param name="tierString">The unsupported string.</param>
        /// <returns>The <see cref="Tier.Unknown"/> enum value.</returns>
        private static Tier HandleUnsupportedString(string tierString)
        {
            var message = $"Unsupported string value: {tierString}";
            EnumException.LogWarning(nameof(ToEnum), nameof(TierExtensions), message);
            return Tier.Unknown;
        }
    }
}
