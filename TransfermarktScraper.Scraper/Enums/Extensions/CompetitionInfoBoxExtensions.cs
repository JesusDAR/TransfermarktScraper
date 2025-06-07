using System.Globalization;
using TransfermarktScraper.Domain.Entities;
using TransfermarktScraper.Domain.Enums.Extensions;
using TransfermarktScraper.Domain.Exceptions;
using TransfermarktScraper.Domain.Utils.DTO;

namespace TransfermarktScraper.Scraper.Enums.Extensions
{
    /// <summary>
    /// Extension methods for the <see cref="CompetitionInfoBox"/> enum.
    /// </summary>
    public static class CompetitionInfoBoxExtensions
    {
        /// <summary>
        /// Converts a <see cref="CompetitionInfoBox"/> enum value to its corresponding string representation.
        /// </summary>
        /// <param name="competitionInfoBox">The <see cref="CompetitionInfoBox"/> enum value.</param>
        /// <returns>A user friendly string representation of the <see cref="CompetitionInfoBox"/>.</returns>
        public static string ToString(this CompetitionInfoBox competitionInfoBox)
        {
            return competitionInfoBox switch
            {
                CompetitionInfoBox.ClubsCount => "Number of teams",
                CompetitionInfoBox.PlayersCount => "Players",
                CompetitionInfoBox.ForeignersCount => "Foreigners",
                CompetitionInfoBox.MarketValueAverage => "ø-Market value",
                CompetitionInfoBox.AgeAverage => "ø-Age",
                CompetitionInfoBox.Cup => "Type of cup",
                CompetitionInfoBox.Participants => "Participants",
                _ => HandleUnsupportedEnum(competitionInfoBox)
            };
        }

        /// <summary>
        /// Converts a string representation of a <see cref="CompetitionInfoBox"/> to its corresponding enum value.
        /// This method checks if the input string contains specific keywords associated with each <see cref="CompetitionInfoBox"/> value
        /// and returns the corresponding enum value if a match is found.
        /// </summary>
        /// <param name="competitionInfoBoxString">
        /// The string representation of the <see cref="CompetitionInfoBox"/>.
        /// This string is checked for specific keywords to determine the corresponding enum value.
        /// </param>
        /// <returns>
        /// The corresponding <see cref="CompetitionInfoBox"/> enum value if a match is found;
        /// otherwise, returns <see cref="CompetitionInfoBox.Unknown"/>.
        /// </returns>
        public static CompetitionInfoBox ToEnum(string competitionInfoBoxString)
        {
            return competitionInfoBoxString switch
            {
                string s when s.Contains("Number of teams", StringComparison.OrdinalIgnoreCase) => CompetitionInfoBox.ClubsCount,
                string s when s.Contains("Players", StringComparison.OrdinalIgnoreCase) => CompetitionInfoBox.PlayersCount,
                string s when s.Contains("Foreigners", StringComparison.OrdinalIgnoreCase) => CompetitionInfoBox.ForeignersCount,
                string s when s.Contains("ø-Market value", StringComparison.OrdinalIgnoreCase) => CompetitionInfoBox.MarketValueAverage,
                string s when s.Contains("ø-Age", StringComparison.OrdinalIgnoreCase) => CompetitionInfoBox.AgeAverage,
                string s when s.Contains("Type of cup", StringComparison.OrdinalIgnoreCase) => CompetitionInfoBox.Cup,
                string s when s.Contains("participants", StringComparison.OrdinalIgnoreCase) => CompetitionInfoBox.Participants,
                _ => HandleUnsupportedString(competitionInfoBoxString)
            };
        }

        /// <summary>
        /// Assigns the extracted value from the given span text to the corresponding property of the Competition entity based on the identified <see cref="CompetitionInfoBox"/> type.
        /// </summary>
        /// <param name="competitionInfoBox">The info box information to assign.</param>
        /// <param name="spanText">The extracted text from the span element containing the relevant value.</param>
        /// <param name="competition">The competition entity where the value should be assigned.</param>
        public static void AssignToCompetitionProperty(this CompetitionInfoBox competitionInfoBox, string spanText, Competition competition)
        {
            spanText = spanText.Replace('\u00A0', ' ');
            IReadOnlyCollection<string> spanTextParts;
            var message = string.Empty;

            try
            {
                switch (competitionInfoBox)
                {
                    case CompetitionInfoBox.ClubsCount:
                        spanTextParts = spanText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).AsReadOnly();
                        foreach (var spanTextPart in spanTextParts)
                        {
                            var isClubsCount = int.TryParse(spanTextPart.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var clubsCount);
                            if (isClubsCount)
                            {
                                competition.ClubsCount = clubsCount;
                            }
                        }

                        break;

                    case CompetitionInfoBox.PlayersCount:
                        var isPlayersCount = int.TryParse(spanText.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var playersCount);
                        if (isPlayersCount)
                        {
                            competition.PlayersCount = playersCount;
                        }

                        break;

                    case CompetitionInfoBox.ForeignersCount:
                        spanTextParts = spanText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).AsReadOnly();
                        foreach (var spanTextPart in spanTextParts)
                        {
                            var isForeignersCount = int.TryParse(spanTextPart.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var foreignersCount);
                            if (isForeignersCount)
                            {
                                competition.ForeignersCount = foreignersCount;
                            }
                        }

                        break;

                    case CompetitionInfoBox.MarketValueAverage:
                        var marketValueString = MoneyUtils.ExtractNumericPart(spanText);
                        competition.MarketValueAverage = MoneyUtils.ConvertToFloat(marketValueString);
                        break;

                    case CompetitionInfoBox.AgeAverage:
                        var isAgeAverage = float.TryParse(spanText.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var ageAverage);
                        if (isAgeAverage)
                        {
                            competition.AgeAverage = (float)Math.Round(ageAverage, 2);
                        }

                        break;

                    case CompetitionInfoBox.Participants:
                        var isParticipants = int.TryParse(spanText.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var participants);
                        if (isParticipants)
                        {
                            competition.Participants = participants;
                        }

                        break;

                    case CompetitionInfoBox.Cup:
                        competition.Cup = CupExtensions.ToEnum(spanText);
                        break;

                    case CompetitionInfoBox.Unknown:
                        message = $"For competition {competition.Name}: '{spanText}' is {CompetitionInfoBox.Unknown} {nameof(CompetitionInfoBox)}";
                        throw EnumException.LogWarning(nameof(AssignToCompetitionProperty), nameof(CompetitionInfoBoxExtensions), message);

                    default:
                        message = $"For competition {competition.Name}: '{competitionInfoBox}' is not valid {nameof(CompetitionInfoBox)}";
                        throw EnumException.LogWarning(nameof(AssignToCompetitionProperty), nameof(CompetitionInfoBoxExtensions), message);
                }
            }
            catch (Exception ex)
            {
                message = $"Assigning span text: {spanText} failed.";
                throw EnumException.LogWarning(nameof(AssignToCompetitionProperty), nameof(CompetitionInfoBoxExtensions), message, default, default, ex);
            }
        }

        /// <summary>
        /// Handles unsupported values of the <see cref="CompetitionInfoBox"/> enum.
        /// Logs a warning indicating an unexpected enum value and returns an empty string.
        /// </summary>
        /// <param name="competitionInfoBox">The unsupported enum value.</param>
        /// <returns>An empty string.</returns>
        private static string HandleUnsupportedEnum(CompetitionInfoBox competitionInfoBox)
        {
            var message = $"Unsupported enum value: '{competitionInfoBox}'";
            EnumException.LogWarning(nameof(ToString), nameof(CompetitionInfoBoxExtensions), message);
            return string.Empty;
        }

        /// <summary>
        /// Handles unsupported string of the <see cref="CompetitionInfoBox"/> enum.
        /// Logs a warning indicating an unexpected string and returns an empty string.
        /// </summary>
        /// <param name="competitionInfoBoxString">The unsupported string.</param>
        /// <returns>The <see cref="CompetitionInfoBox.Unknown"/> enum value.</returns>
        private static CompetitionInfoBox HandleUnsupportedString(string competitionInfoBoxString)
        {
            var message = $"Unsupported string value: '{competitionInfoBoxString}'";
            EnumException.LogWarning(nameof(ToEnum), nameof(CompetitionInfoBoxExtensions), message);
            return CompetitionInfoBox.Unknown;
        }
    }
}
