using System;
using System.Collections.Generic;
using TransfermarktScraper.Domain.Enums;

namespace TransfermarktScraper.Domain.DTOs.Response
{
    /// <summary>
    /// Represents the response DTO for a player.
    /// </summary>
    public class Player : Base
    {
        /// <summary>
        /// Gets or sets the age of the player.
        /// </summary>
        public int? Age { get; set; }

        /// <summary>
        /// Gets or sets the ending date of the contract of the player with the club.
        /// </summary>
        public DateTime? ContractEnd { get; set; }

        /// <summary>
        /// Gets or sets the starting date of the contract of the player with the club.
        /// </summary>
        public DateTime? ContractStart { get; set; }

        /// <summary>
        /// Gets or sets the date of birth of the player.
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the preferred foot of the player.
        /// </summary>
        public Foot Foot { get; set; } = Foot.Unknown;

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
        public float? MarketValue { get; set; }

        /// <summary>
        /// Gets or sets the market valueS of the player.
        /// </summary>
        public IEnumerable<MarketValue>? MarketValues { get; set; }

        /// <summary>
        /// Gets or sets the name of the player.
        /// </summary>
        required public string Name { get; set; }

        /// <summary>
        /// Gets or sets the nationalities of the player.
        /// </summary>
        required public IEnumerable<string> Nationalities { get; set; }

        /// <summary>
        /// Gets or sets the portrait of the player.
        /// </summary>
        public string? Number { get; set; }

        /// <summary>
        /// Gets or sets the position of the player.
        /// </summary>
        public Position Position { get; set; } = Position.Unknown;
    }
}
