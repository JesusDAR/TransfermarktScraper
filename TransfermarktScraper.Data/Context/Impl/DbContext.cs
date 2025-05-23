﻿using System;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TransfermarktScraper.Data.Configuration;
using TransfermarktScraper.Data.Context.Interfaces;
using TransfermarktScraper.Domain.Entities;
using TransfermarktScraper.Domain.Entities.Stat;

namespace TransfermarktScraper.Data.Context.Impl
{
    /// <inheritdoc/>
    public class DbContext : IDbContext
    {
        private readonly IMongoDatabase _database;

        private readonly string? _countryCollection;
        private readonly string? _clubCollection;
        private readonly string? _playerStatCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbContext"/> class.
        /// </summary>
        /// <param name="options">The options containing database settings.</param>
        public DbContext(IOptions<DbSettings> options)
        {
            var connectionString = options.Value.ConnectionString;
            var databaseName = options.Value.DatabaseName;

            _countryCollection = options.Value.CountryCollection;
            _clubCollection = options.Value.ClubCollection;
            _playerStatCollection = options.Value.PlayerStatCollection;

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

        /// <inheritdoc/>
        public IMongoCollection<Club> Clubs => _database.GetCollection<Club>(_clubCollection);

        /// <inheritdoc/>
        public IMongoCollection<PlayerStat> PlayerStats => _database.GetCollection<PlayerStat>(_playerStatCollection);
    }
}
