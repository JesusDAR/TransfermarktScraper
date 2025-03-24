using MongoDB.Driver;
using TransfermarktScraper.Domain.Entities;

namespace TransfermarktScraper.Data.Context.Interfaces
{
    /// <summary>
    /// Represents the database context for interacting with MongoDB.
    /// </summary>
    public interface IDbContext
    {
        /// <summary>
        /// Gets the MongoDB database instance for performing database operations.
        /// </summary>
        IMongoDatabase Database { get; }

        /// <summary>
        /// Gets the collection of countries.
        /// </summary>
        IMongoCollection<Country> Countries { get; }

        /// <summary>
        /// Gets the collection of clubs.
        /// </summary>
        IMongoCollection<Club> Clubs { get; }
    }
}
