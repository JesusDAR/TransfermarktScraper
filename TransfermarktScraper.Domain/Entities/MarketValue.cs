using System;
using MongoDB.Bson.Serialization.Attributes;

namespace TransfermarktScraper.Domain.Entities
{
    /// <summary>
    /// Represents a market value entity.
    /// </summary>
    public class MarketValue
    {
        /// <summary>
        /// Gets or sets the age of the player for the date of the market value.
        /// </summary>
        [BsonElement("age")]
        public int? Age { get; set; }

        /// <summary>
        /// Gets or sets the name of the club in which the player played during the date of the market value.
        /// </summary>
        [BsonElement("clubName")]
        public string? ClubName { get; set; }

        /// <summary>
        /// Gets or sets the crest of the club in which the player played during the date of the market value.
        /// </summary>
        [BsonElement("clubCrest")]
        public string? ClubCrest { get; set; }

        /// <summary>
        /// Gets or sets the TransfermarktId of the club in which the player played during the date of the market value.
        /// </summary>
        [BsonElement("clubTransfermarktId")]
        public string? ClubTransfermarktId { get; set; }

        /// <summary>
        /// Gets or sets the date of the market value of the player.
        /// </summary>
        [BsonElement("date")]
        required public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the value of the player for a given date.
        /// </summary>
        [BsonElement("value")]
        required public float Value { get; set; }
    }
}
