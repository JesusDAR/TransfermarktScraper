using System;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TransfermarktScraper.Domain.Entities
{
    /// <summary>
    /// Represents the base entitiy.
    /// </summary>
    public class Base
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique Transfermarkt identifier.
        /// </summary>
        [BsonElement("transfermarktId")]
        public string? TransfermarktId { get; set; }

        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        [BsonElement("creationDate")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the last update date.
        /// </summary>
        [BsonElement("updateDate")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime UpdateDate { get; set; } = DateTime.UtcNow;
    }
}
