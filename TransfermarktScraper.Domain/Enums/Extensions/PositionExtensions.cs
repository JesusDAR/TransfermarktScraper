using System;

namespace TransfermarktScraper.Domain.Enums.Extensions
{
    /// <summary>
    /// Extension methods for the <see cref="Position"/> enum.
    /// </summary>
    public static class PositionExtensions
    {
        /// <summary>
        /// Converts a <see cref="Position"/> enum value to its corresponding string representation.
        /// </summary>
        /// <param name="position">The <see cref="Position"/> enum value.</param>
        /// <returns>A user friendly string representation of the <see cref="Position"/>.</returns>
        public static string ToString(this Position position)
        {
            return position switch
            {
                Position.Goalkeeper => "Goalkeeper",
                Position.CentreBack => "Centre-Back",
                Position.LeftBack => "Left-Back",
                Position.RightBack => "Right-Back",
                Position.DefensiveMidfield => "Defensive Midfield",
                Position.CentralMidfield => "Central Midfield",
                Position.AttackingMidfield => "Attacking Midfield",
                Position.LeftWinger => "Left Winger",
                Position.RightWinger => "Right Winger",
                Position.CentreForward => "Centre-Forward",
                Position.Unknown => "Unknown",
                _ => throw new ArgumentException($"Error in {nameof(PositionExtensions)}.{nameof(ToString)}: {position} is not a valid {nameof(Position)}."),
            };
        }

        /// <summary>
        /// Converts a string representation of a <see cref="Position"/> to its corresponding enum value.
        /// </summary>
        /// <param name="positionString">The string representation of the <see cref="Position"/>.</param>
        /// <returns>The corresponding <see cref="Position"/> enum value.</returns>
        public static Position FromString(string positionString)
        {
            positionString = positionString.ToLower();

            return positionString switch
            {
                "goalkeeper" => Position.Goalkeeper,
                "centre-back" => Position.CentreBack,
                "left-back" => Position.LeftBack,
                "right-back" => Position.RightBack,
                "defensive midfield" => Position.DefensiveMidfield,
                "central midfield" => Position.CentralMidfield,
                "attacking midfield" => Position.AttackingMidfield,
                "left winger" => Position.LeftWinger,
                "right winger" => Position.RightWinger,
                "centre-forward" => Position.CentreForward,
                "unknown" => Position.Unknown,
                _ => throw new ArgumentException($"Error in {nameof(PositionExtensions)}.{nameof(FromString)}: {positionString} is not a valid {nameof(Position)} string."),
            };
        }
    }
}
