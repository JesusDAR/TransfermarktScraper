using MongoDB.Driver;

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
    }
}
