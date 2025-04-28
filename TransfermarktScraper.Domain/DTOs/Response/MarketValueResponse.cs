using System;

namespace TransfermarktScraper.Domain.DTOs.Response
{
    /// <summary>
    /// Represents the response DTO for a market value.
    /// </summary>
    public class MarketValueResponse
    {
        /// <summary>
        /// Gets or sets the age of the player for the date of the market value.
        /// </summary>
        public int? Age { get; set; }

        /// <summary>
        /// Gets or sets the name of the club in which the player played during the date of the market value.
        /// </summary>
        public string? ClubName { get; set; }

        /// <summary>
        /// Gets or sets the crest of the club in which the player played during the date of the market value.
        /// </summary>
        public string? ClubCrest { get; set; }

        /// <summary>
        /// Gets or sets the TransfermarktId of the club in which the player played during the date of the market value.
        /// </summary>
        public string? ClubTransfermarktId { get; set; }

        /// <summary>
        /// Gets or sets the date of the market value of the player.
        /// </summary>
        required public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the value of the player for a given date.
        /// </summary>
        required public float Value { get; set; }
    }
}
