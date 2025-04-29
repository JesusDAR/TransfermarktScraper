using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TransfermarktScraper.Domain.Utils;

namespace TransfermarktScraper.Domain.Entities.Stat
{
    /// <summary>
    /// Represents a player stat entity.
    /// </summary>
    public class PlayerStat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerStat"/> class.
        /// </summary>
        /// <param name="playerTransfermarktId">The unique player Transfermarkt identifier.</param>
        public PlayerStat(string playerTransfermarktId)
        {
            if (string.IsNullOrEmpty(playerTransfermarktId))
            {
                throw new ArgumentException($"{nameof(playerTransfermarktId)} cannot be null or empty.");
            }

            PlayerTransfermarktId = playerTransfermarktId;
            TransfermarktId = EntityUtils.GetHash($"{playerTransfermarktId}|stat");
            UpdateDate = DateTime.Now;
        }

        /// <summary>
        /// Gets the unique composite identifier.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [BsonElement("transfermarktId")]
        public string TransfermarktId { get; private set; }

        /// <summary>
        /// Gets the unique player Transfermarkt identifier.
        /// </summary>
        [BsonRequired]
        [BsonElement("playerTransfermarktId")]
        public string PlayerTransfermarktId { get; private set; }

        /// <summary>
        /// Gets or sets the last update date.
        /// </summary>
        [BsonElement("updateDate")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// Gets or sets the player season stats.
        /// </summary>
        [BsonElement("playerSeasonStats")]
        required public IList<PlayerSeasonStat> PlayerSeasonStats { get; set; }
    }
}
