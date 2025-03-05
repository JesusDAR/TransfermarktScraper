using TransfermarktScraper.Domain.Enums;

namespace TransfermarktScraper.Domain.DTOs.Response
{
    /// <summary>
    /// Represents the response DTO for a competition.
    /// </summary>
    public class Competition : Base
    {
        /// <summary>
        /// Gets or sets the competition link in Transfermarkt.
        /// </summary>
        public string? Link { get; set; }

        /// <summary>
        /// Gets or sets the name of the competition.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the logo of the competition.
        /// </summary>
        public string? Logo { get; set; }

        /// <summary>
        /// Gets or sets the tier of the competition.
        /// </summary>
        public Tier Tier { get; set; }

        /// <summary>
        /// Gets or sets the coefficient of the competition.
        /// </summary>
        public float? Coefficient { get; set; }

        /// <summary>
        /// Gets or sets the number of teams of the competition.
        /// </summary>
        public int? TeamsCount { get; set; }

        /// <summary>
        /// Gets or sets the number of players of the competition.
        /// </summary>
        public int? PlayersCount { get; set; }

        /// <summary>
        /// Gets or sets the number of foreign players of the competition.
        /// </summary>
        public int? ForeignersCount { get; set; }

        /// <summary>
        /// Gets or sets the average age of the players of the competition.
        /// </summary>
        public float? AgeAverage { get; set; }

        /// <summary>
        /// Gets or sets the market value of the competition.
        /// </summary>
        public float? MarketValue { get; set; }

        /// <summary>
        /// Gets or sets the average market value of the competition.
        /// </summary>
        public float? MarketValueAverage { get; set; }

        /// <summary>
        /// Gets or sets the most valuable player of the competition.
        /// </summary>
        public string? PlayerMostValuable { get; set; }

        /// <summary>
        /// Gets or sets the current champion of the competition.
        /// </summary>
        public string? CurrentChampion { get; set; }

        /// <summary>
        /// Gets or sets the team most successful of the competition.
        /// </summary>
        public string? MostTimesChampion { get; set; }
    }
}
