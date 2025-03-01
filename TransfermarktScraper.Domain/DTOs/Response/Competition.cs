namespace TransfermarktScraper.Domain.DTOs.Response
{
    public class Competition
    {
        /// <summary>
        /// Gets or sets the unique identifier for the competition.
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Gets or sets the unique Transfermarkt identifier for the competition.
        /// </summary>
        public string? TransfermarktId { get; set; }

        /// <summary>
        /// Gets or sets the name of the competition.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the flag of the competition.
        /// </summary>
        public string? Flag { get; set; }

        /// <summary>
        /// Gets or sets the number of teams.
        /// </summary>
        public int? TeamsCount { get; set; }

        /// <summary>
        /// Gets or sets the number of players.
        /// </summary>
        public int? PlayersCount { get; set; }

        /// <summary>
        /// Gets or sets the number of foreign players.
        /// </summary>
        public int? ForeignersCount { get; set; }

        /// <summary>
        /// Gets or sets the average age.
        /// </summary>
        public float? AgeAverage { get; set; }

        /// <summary>
        /// Gets or sets the average market value.
        /// </summary>
        public float? MarketValueAverage { get; set; }
    }
}
