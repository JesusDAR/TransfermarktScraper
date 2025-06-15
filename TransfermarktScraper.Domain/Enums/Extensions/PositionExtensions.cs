using TransfermarktScraper.Domain.Exceptions;

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
                Position.RightMidfield => "Right Midfield",
                Position.Striker => "Striker",
                Position.Midfielder => "Midfielder",
                Position.Unknown => "Unknown",
                _ => HandleUnsupportedEnum(position),
            };
        }

        /// <summary>
        /// Converts a string representation of a <see cref="Position"/> to its corresponding enum value.
        /// </summary>
        /// <param name="positionString">The string representation of the <see cref="Position"/>.</param>
        /// <returns>The corresponding <see cref="Position"/> enum value.</returns>
        public static Position ToEnum(string positionString)
        {
            positionString = positionString.ToLower().Trim();

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
                "right midfield" => Position.RightMidfield,
                "striker" => Position.Striker,
                "midfielder" => Position.Midfielder,
                "unknown" => Position.Unknown,
                _ => HandleUnsupportedString(positionString),
            };
        }

        /// <summary>
        /// Handles unsupported values of the <see cref="Position"/> enum.
        /// Logs a warning indicating an unexpected enum value and returns an empty string.
        /// </summary>
        /// <param name="position">The unsupported enum value.</param>
        /// <returns>An empty string.</returns>
        private static string HandleUnsupportedEnum(Position position)
        {
            var message = $"Unsupported enum value: {position}";
            EnumException.LogWarning(nameof(ToString), nameof(PositionExtensions), message);
            return string.Empty;
        }

        /// <summary>
        /// Handles unsupported string of the <see cref="Position"/> enum.
        /// Logs a warning indicating an unexpected string and returns an empty string.
        /// </summary>
        /// <param name="positionString">The unsupported string.</param>
        /// <returns>The <see cref="Position.Unknown"/> enum value.</returns>
        private static Position HandleUnsupportedString(string positionString)
        {
            var message = $"Unsupported string value: {positionString}";
            EnumException.LogWarning(nameof(ToEnum), nameof(PositionExtensions), message);
            return Position.Unknown;
        }
    }
}
