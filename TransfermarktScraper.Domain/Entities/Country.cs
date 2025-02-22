using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TransfermarktScraper.Domain.Entities
{
    /// <summary>
    /// Represents a country entity.
    /// </summary>
    public class Country
    {
        /// <summary>
        /// Gets or sets the unique identifier for the country.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the country.
        /// </summary>
        [BsonElement("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the flag of the country.
        /// </summary>
        [BsonElement("flag")]
        public string? Flag { get; set; }
    }
}
