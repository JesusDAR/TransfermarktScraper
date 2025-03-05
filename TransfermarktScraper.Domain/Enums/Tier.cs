using System;

namespace TransfermarktScraper.Domain.Enums
{
    /// <summary>
    /// Represents different tiers of a competition system.
    /// </summary>
    public enum Tier
    {
        FirstTier = 1,   // Firs division league level
        SecondTier = 2,  // Second division league level
        ThirdTier = 3,   // Third division league level
        YouthLeague = 4  // Youth league level
    }

    /// <summary>
    /// Extension methods for the Tier enum.
    /// </summary>
    public static class TierExtensions
    {
        /// <summary>
        /// Converts a Tier enum value to its corresponding string representation.
        /// </summary>
        /// <param name="tier">The Tier enum value.</param>
        /// <returns>A user-friendly string representation of the Tier.</returns>
        public static string ToString(this Tier tier)
        {
            switch (tier)
            {
                case Tier.FirstTier:
                    return "First Tier";
                case Tier.SecondTier:
                    return "Second Tier";
                case Tier.ThirdTier:
                    return "Third Tier";
                case Tier.YouthLeague:
                    return "Youth League";
                default:
                    throw new ArgumentOutOfRangeException(nameof(tier), tier,
                        $"Error in {nameof(TierExtensions)}: {nameof(tier)} not found");
            }
        }
    }
}
