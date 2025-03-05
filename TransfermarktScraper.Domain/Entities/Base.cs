using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TransfermarktScraper.Domain.Entities
{
    /// <summary>
    /// Represents the base entitiy.
    /// </summary>
    public class Base
    {
        /// <summary>
        /// Gets or sets the unique Transfermarkt identifier.
        /// </summary>
        [BsonId]
        [BsonElement("transfermarktId")]
        public required string TransfermarktId { get; set; }

        /// <summary>
        /// Gets or sets the last update date.
        /// </summary>
        [BsonElement("updateDate")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime? UpdateDate { get; set; }
    }
}
