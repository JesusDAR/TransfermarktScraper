using System.Globalization;
using TransfermarktScraper.BLL.Utils;
using TransfermarktScraper.Domain.Entities;

namespace TransfermarktScraper.BLL.Enums
{
    /// <summary>
    /// Represents different types of competition related information that can be found in the info box HTML element when scraping.
    /// </summary>
    public enum CompetitionInfoBox
    {
        /// <summary>
        /// Unknown or undefined competition information.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The competition number of clubs.
        /// </summary>
        ClubsCount,

        /// <summary>
        /// The competition number of players.
        /// </summary>
        PlayersCount,

        /// <summary>
        /// The competition number of foreign players.
        /// </summary>
        ForeignersCount,

        /// <summary>
        /// The competition average market value.
        /// </summary>
        MarketValueAverage,

        /// <summary>
        /// The competition average age of the players.
        /// </summary>
        AgeAverage,
    }

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
                CompetitionInfoBox.ClubsCount => "Number of clubs",
                CompetitionInfoBox.PlayersCount => "Players",
                CompetitionInfoBox.ForeignersCount => "Foreigners",
                CompetitionInfoBox.MarketValueAverage => "ø-Market value",
                CompetitionInfoBox.AgeAverage => "ø-Age",
                _ => string.Empty
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
                string s when s.Contains("Number of clubs", StringComparison.OrdinalIgnoreCase) => CompetitionInfoBox.ClubsCount,
                string s when s.Contains("Players", StringComparison.OrdinalIgnoreCase) => CompetitionInfoBox.PlayersCount,
                string s when s.Contains("Foreigners", StringComparison.OrdinalIgnoreCase) => CompetitionInfoBox.ForeignersCount,
                string s when s.Contains("ø-Market value", StringComparison.OrdinalIgnoreCase) => CompetitionInfoBox.MarketValueAverage,
                string s when s.Contains("ø-Age", StringComparison.OrdinalIgnoreCase) => CompetitionInfoBox.AgeAverage,
                _ => CompetitionInfoBox.Unknown
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
                    var money = MoneyUtils.ExtractNumericPart(spanText);
                    competition.MarketValueAverage = MoneyUtils.ToNumber(money);
                    break;

                case CompetitionInfoBox.AgeAverage:
                    var isAgeAverage = float.TryParse(spanText.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var ageAverage);
                    if (isAgeAverage)
                    {
                        competition.AgeAverage = ageAverage;
                    }

                    break;

                case CompetitionInfoBox.Unknown:
                default:
                    throw new ArgumentException($"Error in {nameof(CompetitionClubInfoExtensions)}.{nameof(AssignToCompetitionProperty)} for {competition.Name}: {spanText} is not a valid {nameof(CompetitionInfoBox)}");
            }
        }
    }
}
