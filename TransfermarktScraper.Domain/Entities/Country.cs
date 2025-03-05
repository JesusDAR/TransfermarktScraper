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
        /// Gets or sets the name of the country.
        /// </summary>
        [BsonElement("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the flag of the country.
        /// </summary>
        [BsonElement("flag")]
        public string? Flag { get; set; }

        /// <summary>
        /// Gets or sets the competitions of the country.
        /// </summary>
        [BsonElement("competitions")]
        public IList<Competition> Competitions { get; set; } = new List<Competition>();
    }
}
