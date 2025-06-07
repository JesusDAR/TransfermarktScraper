namespace TransfermarktScraper.Domain.DTOs.Response.Exporter
{
    /// <summary>
    /// Represents the data exported from a country-competition document.
    /// </summary>
    public class CountryCompetitionData
    {
        /// <summary>
        /// Gets or sets the Transfermarkt ID of the country.
        /// </summary>
        public string? CountryTransfermarktId { get; set; }

        /// <summary>
        /// Gets or sets the flag of the country.
        /// </summary>
        public string? CountryFlag { get; set; }

        /// <summary>
        /// Gets or sets the name of the country.
        /// </summary>
        public string? CountryName { get; set; }

        /// <summary>
        /// Gets or sets the Transfermarkt ID of the competition.
        /// </summary>
        public string? CompetitionTransfermarktId { get; set; }

        /// <summary>
        /// Gets or sets the average age of the players of the competition.
        /// </summary>
        public float? CompetitionAgeAverage { get; set; }

        /// <summary>
        /// Gets or sets the string of club IDs associated with the competition.
        /// </summary>
        public string? CompetitionClubIds { get; set; }

        /// <summary>
        /// Gets or sets the current champion of the competition.
        /// </summary>
        public string? CompetitionCurrentChampion { get; set; }

        /// <summary>
        /// Gets or sets the number of foreign players of the competition.
        /// </summary>
        public int? CompetitionForeignersCount { get; set; }

        /// <summary>
        /// Gets or sets the competition cup level.
        /// </summary>
        public string? CompetitionCup { get; set; }

        /// <summary>
        /// Gets or sets the competition link in Transfermarkt.
        /// </summary>
        public string? CompetitionLink { get; set; }

        /// <summary>
        /// Gets or sets the logo of the competition.
        /// </summary>
        public string? CompetitionLogo { get; set; }

        /// <summary>
        /// Gets or sets the market value of the competition.
        /// </summary>
        public float? CompetitionMarketValue { get; set; }

        /// <summary>
        /// Gets or sets the average market value of the competition.
        /// </summary>
        public float? CompetitionMarketValueAverage { get; set; }

        /// <summary>
        /// Gets or sets the club most successful of the competition.
        /// </summary>
        public string? CompetitionMostTimesChampion { get; set; }

        /// <summary>
        /// Gets or sets the name of the competition.
        /// </summary>
        public string? CompetitionName { get; set; }

        /// <summary>
        /// Gets or sets the number of club participants in the cup.
        /// </summary>
        public int? CompetitionParticipants { get; set; }

        /// <summary>
        /// Gets or sets the number of players of the competition.
        /// </summary>
        public int? CompetitionPlayersCount { get; set; }

        /// <summary>
        /// Gets or sets the number of clubs of the competition.
        /// </summary>
        public int? CompetitionClubsCount { get; set; }

        /// <summary>
        /// Gets or sets the tier of the competition.
        /// </summary>
        public string? CompetitionTier { get; set; }
    }
}
