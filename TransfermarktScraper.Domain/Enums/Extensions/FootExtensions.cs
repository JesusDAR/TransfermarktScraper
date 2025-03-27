using System;

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
                Foot.Right => "right",
                Foot.Left => "left",
                Foot.Unknown => "unknown",
                _ => throw new ArgumentException($"Error in {nameof(FootExtensions)}.{nameof(ToString)}: {foot} is not a valid {nameof(Foot)}."),
            };
        }

        /// <summary>
        /// Converts a string representation of a <see cref="Foot"/> to its corresponding enum value.
        /// </summary>
        /// <param name="footString">The string representation of the <see cref="Foot"/>.</param>
        /// <returns>The corresponding <see cref="Foot"/> enum value.</returns>
        public static Foot FromString(string footString)
        {
            footString = footString.ToLower();

            return footString switch
            {
                "right" => Foot.Right,
                "left" => Foot.Left,
                "unknown" => Foot.Unknown,
                _ => throw new ArgumentException($"Error in {nameof(FootExtensions)}.{nameof(FromString)}: {footString} is not a valid {nameof(Foot)} string."),
            };
        }
    }
}
