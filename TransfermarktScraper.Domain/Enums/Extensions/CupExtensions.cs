using System;

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
                Cup.None => string.Empty,
                _ => throw new ArgumentException($"Error in {nameof(CupExtensions)}.{nameof(ToString)}: {cup} is not a valid {nameof(Cup)}."),
            };
        }

        /// <summary>
        /// Converts a string representation of a <see cref="Cup"/> to its corresponding enum value.
        /// </summary>
        /// <param name="cupString">The string representation of the <see cref="Cup"/>.</param>
        /// <returns>The corresponding <see cref="Cup"/> enum value.</returns>
        public static Cup FromString(string cupString)
        {
            cupString = cupString.ToLower();

            return cupString switch
            {
                "domestic cup" => Cup.DomesticCup,
                "domestic super cup" => Cup.DomesticSuperCup,
                "international" => Cup.International,
                "" => Cup.None,
                _ => throw new ArgumentException($"Error in {nameof(CupExtensions)}.{nameof(FromString)}: {cupString} is not a valid {nameof(Cup)} string."),
            };
        }
    }
}
