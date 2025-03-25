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
        /// The club that has won the competition the most times.
        /// </summary>
        MostTimesChampion,

        /// <summary>
        /// The coefficient of the competition.
        /// </summary>
        Coefficient,
    }
}
