using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TransfermarktScraper.Domain.Entities
{
    /// <summary>
    /// Represents a competition entity.
    /// </summary>
    public class Competition : Base
    {
        /// <summary>
        /// Gets or sets the competition link in Transfermarkt.
        /// </summary>
        [BsonElement("link")]
        public string? Link { get; set; }

        /// <summary>
        /// Gets or sets the name of the competition.
        /// </summary>
        [BsonElement("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the logo of the competition.
        /// </summary>
        [BsonElement("logo")]
        public string? Logo { get; set; }

        /// <summary>
        /// Gets or sets the number of teams.
        /// </summary>
        [BsonElement("teamsCount")]
        public int? TeamsCount { get; set; }

        /// <summary>
        /// Gets or sets the number of players.
        /// </summary>
        [BsonElement("playersCount")]
        public int? PlayersCount { get; set; }

        /// <summary>
        /// Gets or sets the number of foreign players.
        /// </summary>
        [BsonElement("foreignersCount")]
        public int? ForeignersCount { get; set; }

        /// <summary>
        /// Gets or sets the average age.
        /// </summary>
        [BsonElement("ageAverage")]
        public float? AgeAverage { get; set; }

        /// <summary>
        /// Gets or sets the average market value.
        /// </summary>
        [BsonElement("marketValueAverage")]
        public float? MarketValueAverage { get; set; }
    }
}
