using Microsoft.Playwright;
using TransfermarktScraper.Domain.Entities;
using TransfermarktScraper.Domain.Enums.Extensions;
using TransfermarktScraper.Domain.Exceptions;

namespace TransfermarktScraper.Scraper.Enums.Extensions
{
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
                _ => HandleUnsupportedEnum(competitionClubInfo),
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
                _ => HandleUnsupportedString(competitionClubInfoString)
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
            ILocator spanLocator;
            string spanText = string.Empty;
            string selector = string.Empty;
            var message = string.Empty;

            try
            {
                switch (competitionClubInfo)
                {
                    case CompetitionClubInfo.Tier:
                        selector = "span";
                        spanLocator = labelLocator.Locator(selector);
                        await spanLocator.WaitForAsync(new ()
                        {
                            State = WaitForSelectorState.Visible,
                            Timeout = 100,
                        });
                        spanText = await spanLocator.InnerTextAsync();
                        competition.Tier = TierExtensions.ToEnum(spanText);
                        break;

                    case CompetitionClubInfo.CurrentChampion:
                        selector = "span";
                        spanLocator = labelLocator.Locator(selector);
                        await spanLocator.WaitForAsync(new ()
                        {
                            State = WaitForSelectorState.Visible,
                            Timeout = 100,
                        });
                        spanText = await spanLocator.InnerTextAsync();
                        competition.CurrentChampion = spanText;
                        break;

                    case CompetitionClubInfo.MostTimesChampion:
                        selector = "span[itemprop='dataItem'] > a";
                        var linkLocator = labelLocator.Locator(selector);
                        await linkLocator.WaitForAsync(new ()
                        {
                            State = WaitForSelectorState.Visible,
                            Timeout = 100,
                        });
                        spanText = await linkLocator.InnerTextAsync();
                        competition.MostTimesChampion = spanText;
                        break;

                    case CompetitionClubInfo.Unknown:
                        message = $"For competition {competition.Name}: '{spanText}' is {CompetitionClubInfo.Unknown} {nameof(CompetitionClubInfo)}";
                        throw EnumException.LogWarning(nameof(AssignToCompetitionProperty), nameof(CompetitionClubInfoExtensions), message);

                    default:
                        message = $"For competition {competition.Name}: '{competitionClubInfo}' is not a valid {nameof(CompetitionClubInfo)}";
                        throw EnumException.LogWarning(nameof(AssignToCompetitionProperty), nameof(CompetitionClubInfoExtensions), message);
                }
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(selector))
                {
                    message = $"Using selector: '{selector}' failed.";
                    throw EnumException.LogWarning(nameof(AssignToCompetitionProperty), nameof(CompetitionClubInfoExtensions), message, default, default, ex);
                }
            }
        }

        /// <summary>
        /// Handles unsupported values of the <see cref="CompetitionClubInfo"/> enum.
        /// Logs a warning indicating an unexpected enum value and returns an empty string.
        /// </summary>
        /// <param name="competitionClubInfo">The unsupported enum value.</param>
        /// <returns>An empty string.</returns>
        private static string HandleUnsupportedEnum(CompetitionClubInfo competitionClubInfo)
        {
            var message = $"Unsupported enum value: '{competitionClubInfo}'";
            EnumException.LogWarning(nameof(ToString), nameof(CompetitionClubInfoExtensions), message);
            return string.Empty;
        }

        /// <summary>
        /// Handles unsupported string of the <see cref="CompetitionClubInfo"/> enum.
        /// Logs a warning indicating an unexpected string and returns an empty string.
        /// </summary>
        /// <param name="competitionClubInfoString">The unsupported string.</param>
        /// <returns>The <see cref="CompetitionClubInfo.Unknown"/> enum value.</returns>
        private static CompetitionClubInfo HandleUnsupportedString(string competitionClubInfoString)
        {
            var message = $"Unsupported string value: '{competitionClubInfoString}'";
            EnumException.LogWarning(nameof(ToEnum), nameof(CompetitionClubInfoExtensions), message);
            return CompetitionClubInfo.Unknown;
        }
    }
}
