﻿using System.Collections.Generic;
using System.Linq;

namespace TransfermarktScraper.Domain.DTOs.Response.Scraper
{
    /// <summary>
    /// Represents the response DTO for a competition.
    /// </summary>
    public class CompetitionResponse : BaseResponse
    {
        /// <summary>
        /// Gets or sets the average age of the players of the competition.
        /// </summary>
        public float? AgeAverage { get; set; }

        /// <summary>
        /// Gets or sets the list of club IDs associated with the competition.
        /// </summary>
        public IEnumerable<string> ClubIds { get; set; } = Enumerable.Empty<string>();

        /// <summary>
        /// Gets or sets the current champion of the competition.
        /// </summary>
        public string? CurrentChampion { get; set; }

        /// <summary>
        /// Gets or sets the number of foreign players of the competition.
        /// </summary>
        public int? ForeignersCount { get; set; }

        /// <summary>
        /// Gets or sets the competition cup level.
        /// </summary>
        public string Cup { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the competition link in Transfermarkt.
        /// </summary>
        required public string Link { get; set; }

        /// <summary>
        /// Gets or sets the logo of the competition.
        /// </summary>
        public string? Logo { get; set; }

        /// <summary>
        /// Gets or sets the market value of the competition.
        /// </summary>
        public string? MarketValue { get; set; }

        /// <summary>
        /// Gets or sets the average market value of the competition.
        /// </summary>
        public string? MarketValueAverage { get; set; }

        /// <summary>
        /// Gets or sets the club most successful of the competition.
        /// </summary>
        public string? MostTimesChampion { get; set; }

        /// <summary>
        /// Gets or sets the name of the competition.
        /// </summary>
        required public string Name { get; set; }

        /// <summary>
        /// Gets or sets the number of club participants in the cup.
        /// </summary>
        public int? Participants { get; set; }

        /// <summary>
        /// Gets or sets the number of players of the competition.
        /// </summary>
        public int? PlayersCount { get; set; }

        /// <summary>
        /// Gets or sets the number of clubs of the competition.
        /// </summary>
        public int? ClubsCount { get; set; }

        /// <summary>
        /// Gets or sets the tier of the competition.
        /// </summary>
        public string Tier { get; set; } = string.Empty;
    }
}
