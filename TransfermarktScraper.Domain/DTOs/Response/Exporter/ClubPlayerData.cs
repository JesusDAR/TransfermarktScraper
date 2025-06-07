using System.Collections.Generic;

namespace TransfermarktScraper.Domain.DTOs.Response.Exporter
{
    /// <summary>
    /// Represents the data exported from a club-player document.
    /// </summary>
    public class ClubPlayerData
    {
        /// <summary>
        /// Gets or sets the Transfermarkt ID of the club.
        /// </summary>
        public string? ClubTransfermarktId { get; set; }

        /// <summary>
        /// Gets or sets the average age of the players of the club.
        /// </summary>
        public float? ClubAgeAverage { get; set; }

        /// <summary>
        /// Gets or sets the list of competition IDs associated with the club.
        /// </summary>
        public IEnumerable<string>? ClubCompetitionIds { get; set; }

        /// <summary>
        /// Gets or sets the crest of the club.
        /// </summary>
        public string? ClubCrest { get; set; }

        /// <summary>
        /// Gets or sets the number of foreign players of the club.
        /// </summary>
        public int? ClubForeignersCount { get; set; }

        /// <summary>
        /// Gets or sets the club link in Transfermarkt.
        /// </summary>
        public string? ClubLink { get; set; }

        /// <summary>
        /// Gets or sets the market value of the club.
        /// </summary>
        public string? ClubMarketValue { get; set; }

        /// <summary>
        /// Gets or sets the market value average of the players of the club.
        /// </summary>
        public string? ClubMarketValueAverage { get; set; }

        /// <summary>
        /// Gets or sets the name of the club.
        /// </summary>
        public string? ClubName { get; set; }

        /// <summary>
        /// Gets or sets the number of players of the club.
        /// </summary>
        public int? ClubPlayersCount { get; set; }

        /// <summary>
        /// Gets or sets the age of the player.
        /// </summary>
        public int? PlayerAge { get; set; }

        /// <summary>
        /// Gets or sets the ending date of the contract of the player with the club.
        /// </summary>
        public string? PlayerContractEnd { get; set; }

        /// <summary>
        /// Gets or sets the starting date of the contract of the player with the club.
        /// </summary>
        public string? PlayerContractStart { get; set; }

        /// <summary>
        /// Gets or sets the date of birth of the player.
        /// </summary>
        public string? PlayerDateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the preferred foot of the player.
        /// </summary>
        public string? PlayerFoot { get; set; }

        /// <summary>
        /// Gets or sets the height in cm of the player.
        /// </summary>
        public int? PlayerHeight { get; set; }

        /// <summary>
        /// Gets or sets the player link in Transfermarkt.
        /// </summary>
        public string? PlayerLink { get; set; }

        /// <summary>
        /// Gets or sets the current market value of the player.
        /// </summary>
        public string? PlayerMarketValue { get; set; }

        /// <summary>
        /// Gets or sets the market values of the player.
        /// </summary>
        public string? PlayerMarketValues { get; set; }

        /// <summary>
        /// Gets or sets the name of the player.
        /// </summary>
        public string? PlayerName { get; set; }

        /// <summary>
        /// Gets or sets the nationalities of the player.
        /// </summary>
        public string? PlayerNationalities { get; set; }

        /// <summary>
        /// Gets or sets the portrait of the player.
        /// </summary>
        public string? PlayerNumber { get; set; }

        /// <summary>
        /// Gets or sets the portrait of the player.
        /// </summary>
        public string? PlayerPortrait { get; set; }

        /// <summary>
        /// Gets or sets the position of the player.
        /// </summary>
        public string? PlayerPosition { get; set; }

        /// <summary>
        /// Gets or sets the unique player stat Id.
        /// </summary>
        public string? PlayerPlayerStatId { get; set; }
    }
}
