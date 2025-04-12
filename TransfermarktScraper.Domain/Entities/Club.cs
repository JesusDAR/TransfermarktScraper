using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;

namespace TransfermarktScraper.Domain.Entities
{
    /// <summary>
    /// Represents a club entity.
    /// </summary>
    public class Club : Base
    {
        /// <summary>
        /// Gets or sets the average age of the players of the club.
        /// </summary>
        [BsonElement("ageAverage")]
        public float? AgeAverage { get; set; }

        /// <summary>
        /// Gets or sets the list of competition IDs associated with the club.
        /// </summary>
        [BsonElement("competitionIds")]
        public IEnumerable<string> CompetitionIds { get; set; } = Enumerable.Empty<string>();

        /// <summary>
        /// Gets or sets the crest of the club.
        /// </summary>
        [BsonElement("crest")]
        required public string Crest { get; set; }

        /// <summary>
        /// Gets or sets the number of foreign players of the club.
        /// </summary>
        [BsonElement("foreignersCount")]
        public int? ForeignersCount { get; set; }

        /// <summary>
        /// Gets or sets the club link in Transfermarkt.
        /// </summary>
        [BsonElement("link")]
        required public string Link { get; set; }

        /// <summary>
        /// Gets or sets the market value of the club.
        /// </summary>
        [BsonElement("marketValue")]
        public float? MarketValue { get; set; }

        /// <summary>
        /// Gets or sets the market value average of the players of the club.
        /// </summary>
        [BsonElement("marketValueAverage")]
        public float? MarketValueAverage { get; set; }

        /// <summary>
        /// Gets or sets the name of the club.
        /// </summary>
        [BsonElement("name")]
        required public string Name { get; set; }

        /// <summary>
        /// Gets or sets the players of the club.
        /// </summary>
        [BsonElement("players")]
        public IEnumerable<Player>? Players { get; set; }

        /// <summary>
        /// Gets or sets the number of players of the club.
        /// </summary>
        [BsonElement("playersCount")]
        public int? PlayersCount { get; set; }
    }
}
