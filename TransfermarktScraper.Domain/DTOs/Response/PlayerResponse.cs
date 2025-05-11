using System;
using System.Collections.Generic;
using System.Linq;

namespace TransfermarktScraper.Domain.DTOs.Response
{
    /// <summary>
    /// Represents the response DTO for a player.
    /// </summary>
    public class PlayerResponse : BaseResponse
    {
        /// <summary>
        /// Gets or sets the age of the player.
        /// </summary>
        public int? Age { get; set; }

        /// <summary>
        /// Gets or sets the ending date of the contract of the player with the club.
        /// </summary>
        public string? ContractEnd { get; set; }

        /// <summary>
        /// Gets or sets the starting date of the contract of the player with the club.
        /// </summary>
        public string? ContractStart { get; set; }

        /// <summary>
        /// Gets or sets the date of birth of the player.
        /// </summary>
        public string? DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the preferred foot of the player.
        /// </summary>
        public string Foot { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the height in cm of the player.
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// Gets or sets the player link in Transfermarkt.
        /// </summary>
        required public string Link { get; set; }

        /// <summary>
        /// Gets or sets the current market value of the player.
        /// </summary>
        public string? MarketValue { get; set; }

        /// <summary>
        /// Gets or sets the market valueS of the player.
        /// </summary>
        public IEnumerable<MarketValueResponse>? MarketValues { get; set; }

        /// <summary>
        /// Gets or sets the name of the player.
        /// </summary>
        required public string Name { get; set; }

        /// <summary>
        /// Gets or sets the nationalities of the player.
        /// </summary>
        public IEnumerable<string> Nationalities { get; set; } = Enumerable.Empty<string>();

        /// <summary>
        /// Gets or sets the portrait of the player.
        /// </summary>
        public string? Number { get; set; }

        /// <summary>
        /// Gets or sets the portrait of the player.
        /// </summary>
        public string Portrait { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the position of the player.
        /// </summary>
        public string Position { get; set; } = string.Empty;
    }
}
