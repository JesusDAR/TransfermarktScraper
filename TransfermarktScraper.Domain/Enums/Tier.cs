using System;

namespace TransfermarktScraper.Domain.Enums
{
    /// <summary>
    /// Represents different tiers of a competition system.
    /// </summary>
    public enum Tier
    {
        /// <summary>
        /// First division league level.
        /// </summary>
        FirstTier = 1,

        /// <summary>
        /// Second division league level.
        /// </summary>
        SecondTier = 2,

        /// <summary>
        /// Third division league level.
        /// </summary>
        ThirdTier = 3,

        /// <summary>
        /// Youth league level.
        /// </summary>
        YouthLeague = 4,
    }

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
                _ => throw new ArgumentException($"Error in {nameof(TierExtensions)}.{nameof(ToString)}: {tier} is not a valid {nameof(Tier)}."),
            };
        }

        /// <summary>
        /// Converts a string representation of a <see cref="Tier"/> to its corresponding enum value.
        /// </summary>
        /// <param name="tierString">The string representation of the <see cref="Tier"/>.</param>
        /// <returns>The corresponding <see cref="Tier"/> enum value.</returns>
        public static Tier FromString(string tierString)
        {
            return tierString switch
            {
                "First Tier" => Tier.FirstTier,
                "Second Tier" => Tier.SecondTier,
                "Third Tier" => Tier.ThirdTier,
                "Youth League" => Tier.YouthLeague,
                _ => throw new ArgumentException($"Error in {nameof(TierExtensions)}.{nameof(FromString)}: {tierString} is not a valid {nameof(Tier)} string."),
            };
        }
    }
}
