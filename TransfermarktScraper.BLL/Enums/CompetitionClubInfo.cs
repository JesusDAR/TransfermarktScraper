using System.Globalization;
using Microsoft.Playwright;
using TransfermarktScraper.Domain.Entities;
using TransfermarktScraper.Domain.Enums;

namespace TransfermarktScraper.BLL.Enums
{
    /// <summary>
    /// Represents different types of competition related information that can be found in the club info HTML element when scraping.
    /// </summary>
    public enum CompetitionClubInfo
    {
        /// <summary>
        /// Unknown or undefined competition information.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The competition tier.
        /// </summary>
        Tier,

        /// <summary>
        /// The current champion of the competition.
        /// </summary>
        CurrentChampion,

        /// <summary>
        /// The team that has won the competition the most times.
        /// </summary>
        MostTimesChampion,

        /// <summary>
        /// The coefficient of the competition.
        /// </summary>
        Coefficient,
    }

    /// <summary>
    /// Extension methods for the <see cref="CompetitionClubInfo"/> enum.
    /// </summary>
    public static class CompetitionClubInfoExtensions
    {
        /// <summary>
        /// Converts a <see cref="CompetitionClubInfo"/> enum value to its corresponding string representation.
        /// </summary>
        /// <param name="competitionClubInfo">The <see cref="CompetitionClubInfo"/> enum value.</param>
        /// <returns>A user friendly string representation of the <see cref="CompetitionClubInfo"/>.</returns>
        public static string ToString(this CompetitionClubInfo competitionClubInfo)
        {
            return competitionClubInfo switch
            {
                CompetitionClubInfo.Tier => "League level",
                CompetitionClubInfo.CurrentChampion => "Reigning champion",
                CompetitionClubInfo.MostTimesChampion => "Record-holding champions",
                CompetitionClubInfo.Coefficient => "UEFA coefficient",
                _ => string.Empty
            };
        }

        /// <summary>
        /// Converts a string representation of a <see cref="CompetitionClubInfo"/> to its corresponding enum value.
        /// This method checks if the input string contains specific keywords associated with each <see cref="CompetitionClubInfo"/> value
        /// and returns the corresponding enum value if a match is found.
        /// </summary>
        /// <param name="competitionClubInfoString">
        /// The string representation of the <see cref="CompetitionClubInfo"/>.
        /// This string is checked for specific keywords to determine the corresponding enum value.
        /// </param>
        /// <returns>
        /// The corresponding <see cref="CompetitionClubInfo"/> enum value if a match is found;
        /// otherwise, returns <see cref="CompetitionClubInfo.Unknown"/>.
        /// </returns>
        public static CompetitionClubInfo ToEnum(string competitionClubInfoString)
        {
            return competitionClubInfoString switch
            {
                string s when s.Contains("League level", StringComparison.OrdinalIgnoreCase) => CompetitionClubInfo.Tier,
                string s when s.Contains("Reigning champion", StringComparison.OrdinalIgnoreCase) => CompetitionClubInfo.CurrentChampion,
                string s when s.Contains("Record-holding champions", StringComparison.OrdinalIgnoreCase) => CompetitionClubInfo.MostTimesChampion,
                string s when s.Contains("UEFA coefficient", StringComparison.OrdinalIgnoreCase) => CompetitionClubInfo.Coefficient,
                _ => CompetitionClubInfo.Unknown
            };
        }

        /// <summary>
        /// Assigns the value to the corresponding property of the competition entity based on the <see cref="CompetitionClubInfo"/>.
        /// </summary>
        /// <param name="competitionClubInfo">The type of competition information.</param>
        /// <param name="labelLocator">The label element locator who is the parent of the span elements.</param>
        /// <param name="competition">The competition entity.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task AssignToCompetitionProperty(this CompetitionClubInfo competitionClubInfo, ILocator labelLocator, Competition competition)
        {
            string spanText;
            ILocator spanLocator;

            switch (competitionClubInfo)
            {
                case CompetitionClubInfo.Tier:
                    spanLocator = labelLocator.Locator("span");
                    spanText = await spanLocator.InnerTextAsync();
                    competition.Tier = TierExtensions.FromString(spanText);
                    break;

                case CompetitionClubInfo.CurrentChampion:
                    spanLocator = labelLocator.Locator("span");
                    spanText = await spanLocator.InnerTextAsync();
                    competition.CurrentChampion = spanText;
                    break;

                case CompetitionClubInfo.MostTimesChampion:
                    var linkLocator = labelLocator.Locator("span[itemprop='dataItem'] > a");
                    spanText = await linkLocator.InnerTextAsync();
                    competition.MostTimesChampion = spanText;
                    break;

                case CompetitionClubInfo.Coefficient:
                    spanLocator = labelLocator.Locator("span");
                    var texts = await spanLocator.AllInnerTextsAsync();

                    foreach (var text in texts)
                    {
                        var isCoefficient = float.TryParse(text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var coefficient);
                        if (isCoefficient)
                        {
                            competition.Coefficient = coefficient;
                        }
                    }

                    break;

                case CompetitionClubInfo.Unknown:
                default:
                    throw new ArgumentException($"Error in {nameof(CompetitionClubInfoExtensions)}.{nameof(AssignToCompetitionProperty)} for competition {competition.Name}: {competitionClubInfo} is not a valid {nameof(CompetitionClubInfo)} ");
            }
        }
    }
}
