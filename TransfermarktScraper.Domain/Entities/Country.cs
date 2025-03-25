using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TransfermarktScraper.Domain.Entities
{
    /// <summary>
    /// Represents a country entity.
    /// </summary>
    public class Country : Base
    {
        /// <summary>
        /// Gets or sets the competitions of the country.
        /// </summary>
        [BsonElement("competitions")]
        public IEnumerable<Competition> Competitions { get; set; } = new List<Competition>();

        /// <summary>
        /// Gets or sets the flag of the country.
        /// </summary>
        [BsonElement("flag")]
        public string? Flag { get; set; }

        /// <summary>
        /// Gets or sets the name of the country.
        /// </summary>
        [BsonElement("name")]
        required public string Name { get; set; }
    }
}
