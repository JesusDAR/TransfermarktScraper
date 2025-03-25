using System;

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
            tierString = tierString.ToLower();

            return tierString switch
            {
                "first tier" => Tier.FirstTier,
                "second tier" => Tier.SecondTier,
                "third tier" => Tier.ThirdTier,
                "youth league" => Tier.YouthLeague,
                _ => throw new ArgumentException($"Error in {nameof(TierExtensions)}.{nameof(FromString)}: {tierString} is not a valid {nameof(Tier)} string."),
            };
        }
    }
}
