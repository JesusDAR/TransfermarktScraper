using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TransfermarktScraper.Domain.Enums;

namespace TransfermarktScraper.Domain.Entities
{
    /// <summary>
    /// Represents a competition entity.
    /// </summary>
    public class Competition : Base
    {
        /// <summary>
        /// Gets or sets the average age of the players of the competition.
        /// </summary>
        [BsonElement("ageAverage")]
        public float? AgeAverage { get; set; }

        /// <summary>
        /// Gets or sets the list of club IDs associated with the competition.
        /// </summary>
        [BsonElement("clubIds")]
        public IEnumerable<string> ClubIds { get; set; } = Enumerable.Empty<string>();

        /// <summary>
        /// Gets or sets the number of clubs of the competition.
        /// </summary>
        [BsonElement("clubsCount")]
        public int? ClubsCount { get; set; }

        /// <summary>
        /// Gets or sets the coefficient of the competition.
        /// </summary>
        [BsonElement("coefficient")]
        public float? Coefficient { get; set; }

        /// <summary>
        /// Gets or sets the current champion of the competition.
        /// </summary>
        [BsonElement("currentChampion")]
        public string? CurrentChampion { get; set; }

        /// <summary>
        /// Gets or sets the number of foreign players of the competition.
        /// </summary>
        [BsonElement("foreignersCount")]
        public int? ForeignersCount { get; set; }

        /// <summary>
        /// Gets or sets the competition cup level.
        /// </summary>
        [BsonElement("cup")]
        public Cup Cup { get; set; } = Cup.None;

        /// <summary>
        /// Gets or sets the competition link in Transfermarkt.
        /// </summary>
        [BsonElement("link")]
        required public string Link { get; set; }

        /// <summary>
        /// Gets or sets the logo of the competition.
        /// </summary>
        [BsonElement("logo")]
        public string? Logo { get; set; }

        /// <summary>
        /// Gets or sets the market value of the competition.
        /// </summary>
        [BsonElement("marketValue")]
        public float? MarketValue { get; set; }

        /// <summary>
        /// Gets or sets the average market value of the competition.
        /// </summary>
        [BsonElement("marketValueAverage")]
        public float? MarketValueAverage { get; set; }

        /// <summary>
        /// Gets or sets the club most successful of the competition.
        /// </summary>
        [BsonElement("mostTimesChampion")]
        public string? MostTimesChampion { get; set; }

        /// <summary>
        /// Gets or sets the name of the competition.
        /// </summary>
        [BsonElement("name")]
        required public string Name { get; set; }

        /// <summary>
        /// Gets or sets the number of club participants in the cup.
        /// </summary>
        [BsonElement("participants")]
        public int Participants { get; set; }

        /// <summary>
        /// Gets or sets the number of players of the competition.
        /// </summary>
        [BsonElement("playersCount")]
        public int? PlayersCount { get; set; }

        /// <summary>
        /// Gets or sets the tier of the competition.
        /// </summary>
        [BsonElement("tier")]
        public Tier Tier { get; set; } = Tier.None;
    }
}
