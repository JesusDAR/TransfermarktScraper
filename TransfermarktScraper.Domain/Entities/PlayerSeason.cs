using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TransfermarktScraper.Domain.Utils;

namespace TransfermarktScraper.Domain.Entities
{
    /// <summary>
    /// Represents a player season entity.
    /// </summary>
    public class PlayerSeason
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerSeason"/> class.
        /// </summary>
        /// <param name="playerTransfermarktId">The unique player Transfermarkt identifier.</param>
        /// <param name="seasonTransfermarktId">The unique season Transfermarkt identifier.</param>
        public PlayerSeason(string playerTransfermarktId, string seasonTransfermarktId)
        {
            if (string.IsNullOrEmpty(playerTransfermarktId))
            {
                throw new ArgumentException($"{nameof(PlayerTransfermarktId)} cannot be null or empty", nameof(playerTransfermarktId));
            }

            if (string.IsNullOrEmpty(seasonTransfermarktId))
            {
                throw new ArgumentException($"{nameof(SeasonTransfermarktId)} cannot be null or empty", nameof(seasonTransfermarktId));
            }

            PlayerTransfermarktId = playerTransfermarktId;
            SeasonTransfermarktId = seasonTransfermarktId;
            TransfermarktId = EntityUtils.GetHash($"{playerTransfermarktId}|{seasonTransfermarktId}");
            UpdateDate = DateTime.UtcNow;
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
        /// Gets the unique season Transfermarkt identifier.
        /// </summary>
        [BsonRequired]
        [BsonElement("seasonTransfermarktId")]
        public string SeasonTransfermarktId { get; private set; }

        /// <summary>
        /// Gets or sets the last update date.
        /// </summary>
        [BsonElement("updateDate")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// Gets or sets the competition stats for the player.
        /// </summary>
        [BsonElement("playerCompetitionStats")]
        public IEnumerable<PlayerSeasonCompetitionStat>? PlayerCompetitionStats { get; set; }
    }
}
