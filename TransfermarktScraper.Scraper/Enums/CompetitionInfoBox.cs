namespace TransfermarktScraper.Scraper.Enums
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

        /// <summary>
        /// The competition is a cup.
        /// </summary>
        Cup,

        /// <summary>
        /// The number of club participants in the cup.
        /// </summary>
        Participants,
    }
}
