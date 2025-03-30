using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace TransfermarktScraper.Domain.Entities.Embedded
{
    /// <summary>
    /// Represents a player stat embedded entity.
    /// This entity is always embedded in Player.
    /// </summary>
    public class PlayerStat
    {
        /// <summary>
        /// Gets or sets the overall career stats of the player.
        /// </summary>
        [BsonElement("playerCareerStat")]
        required public PlayerCareerStat PlayerCareerStat { get; set; }

        /// <summary>
        /// Gets or sets the player season Ids.
        /// </summary>
        [BsonElement("playerSeasonIds")]
        required public IEnumerable<string> PlayerSeasonIds { get; set; }
    }
}
