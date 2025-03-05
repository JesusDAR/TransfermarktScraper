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
        /// The competition level.
        /// </summary>
        Level,

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
                CompetitionClubInfo.Level => "League level",
                CompetitionClubInfo.CurrentChampion => "Reigning champion",
                CompetitionClubInfo.MostTimesChampion => "Record-holding champions",
                CompetitionClubInfo.Coefficient => "UEFA coefficient",
                _ => string.Empty
            };
        }

        /// <summary>
        /// Converts a string representation of a <see cref="CompetitionClubInfo"/> to its corresponding enum value.
        /// </summary>
        /// <param name="competitionClubInfoString">The string representation of the <see cref="CompetitionClubInfo"/>.</param>
        /// <returns>The corresponding <see cref="CompetitionClubInfo"/> enum value.</returns>
        public static CompetitionClubInfo FromString(string competitionClubInfoString)
        {
            return competitionClubInfoString switch
            {
                "League level" => CompetitionClubInfo.Level,
                "Reigning champion" => CompetitionClubInfo.CurrentChampion,
                "Record-holding champions" => CompetitionClubInfo.MostTimesChampion,
                "UEFA coefficient" => CompetitionClubInfo.Coefficient,
                _ => default(CompetitionClubInfo)
            };
        }
    }
}
