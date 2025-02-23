using System;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TransfermarktScraper.Data.Configuration.Context.Interfaces;
using TransfermarktScraper.Domain.Entities;

namespace TransfermarktScraper.Data.Configuration.Context.Impl
{
    /// <inheritdoc/>
    public class DbContext : IDbContext
    {
        private readonly IMongoDatabase _database;

        private readonly string? _countryCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbContext"/> class.
        /// </summary>
        /// <param name="options">The options containing database settings.</param>
        public DbContext(IOptions<DbSettings> options)
        {
            var connectionString = options.Value.ConnectionString;
            var databaseName = options.Value.DatabaseName;

            _countryCollection = options.Value.CountryCollection;

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

        /// <inheritdoc/>
        public IMongoCollection<Country> Countries => _database.GetCollection<Country>(_countryCollection);
    }
}
