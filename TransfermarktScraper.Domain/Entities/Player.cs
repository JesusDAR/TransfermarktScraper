using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using TransfermarktScraper.Domain.Enums;
using TransfermarktScraper.Domain.Utils;

namespace TransfermarktScraper.Domain.Entities
{
    /// <summary>
    /// Represents a player entity.
    /// </summary>
    public class Player : Base
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class using the Transfermarkt player ID.
        /// </summary>
        /// <param name="playerTransfermarktId">The unique Transfermarkt identifier for the player.</param>
        public Player(string playerTransfermarktId)
        {
            if (string.IsNullOrEmpty(playerTransfermarktId))
            {
                throw new ArgumentException($"{nameof(playerTransfermarktId)} cannot be null or empty.");
            }

            TransfermarktId = playerTransfermarktId;
            PlayerStatId = EntityUtils.GetHash($"{TransfermarktId}|stat");
        }

        /// <summary>
        /// Gets or sets the age of the player.
        /// </summary>
        [BsonElement("age")]
        public int? Age { get; set; }

        /// <summary>
        /// Gets or sets the ending date of the contract of the player with the club.
        /// </summary>
        [BsonElement("contractEnd")]
        public DateTime? ContractEnd { get; set; }

        /// <summary>
        /// Gets or sets the starting date of the contract of the player with the club.
        /// </summary>
        [BsonElement("contractStart")]
        public DateTime? ContractStart { get; set; }

        /// <summary>
        /// Gets or sets the date of birth of the player.
        /// </summary>
        [BsonElement("dateOfBirth")]
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the preferred foot of the player.
        /// </summary>
        [BsonElement("foot")]
        public Foot Foot { get; set; } = Foot.Unknown;

        /// <summary>
        /// Gets or sets the height in cm of the player.
        /// </summary>
        [BsonElement("height")]
        public int? Height { get; set; }

        /// <summary>
        /// Gets or sets the player link in Transfermarkt.
        /// </summary>
        [BsonElement("link")]
        required public string Link { get; set; }

        /// <summary>
        /// Gets or sets the current market value of the player.
        /// </summary>
        [BsonElement("marketValue")]
        public float? MarketValue { get; set; }

        /// <summary>
        /// Gets or sets the market valueS of the player.
        /// </summary>
        [BsonElement("marketValues")]
        public IEnumerable<MarketValue>? MarketValues { get; set; }

        /// <summary>
        /// Gets or sets the name of the player.
        /// </summary>
        [BsonElement("name")]
        required public string Name { get; set; }

        /// <summary>
        /// Gets or sets the nationalities of the player.
        /// </summary>
        [BsonElement("nationalities")]
        required public IEnumerable<string> Nationalities { get; set; }

        /// <summary>
        /// Gets or sets the number of the player.
        /// </summary>
        [BsonElement("number")]
        public string? Number { get; set; }

        /// <summary>
        /// Gets or sets the portrait of the player.
        /// </summary>
        [BsonElement("portrait")]
        required public string Portrait { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the position of the player.
        /// </summary>
        [BsonElement("position")]
        public Position Position { get; set; } = Position.Unknown;

        /// <summary>
        /// Gets or sets the unique player stat Id.
        /// </summary>
        [BsonElement("playerStatId")]
        public string PlayerStatId { get; set; }
    }
}
