using System;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using TransfermarktScraper.Data.Context.Interfaces;

namespace TransfermarktScraper.Data.Context.Impl
{
    /// <inheritdoc/>
    public class DbContext : IDbContext
    {
        private readonly IMongoDatabase _database;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbContext"/> class.
        /// </summary>
        /// <param name="configuration">The configuration containing the connection settings.</param>
        public DbContext(IConfiguration configuration)
        {
            var connectionString = configuration["DbSettings:ConnectionString"];
            var databaseName = configuration["DbSettings:DatabaseName"];

            try
            {
                var client = new MongoClient(connectionString);
                _database = client.GetDatabase(databaseName);
            }
            catch (MongoConnectionException ex)
            {
                throw new Exception("Cannot connect to MongoDB", ex);
            }
        }

        /// <inheritdoc/>
        public IMongoDatabase Database => _database;
    }
}
