using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using TransfermarktScraper.Data.Configuration.Context.Interfaces;
using TransfermarktScraper.Data.Repositories.Interfaces;
using TransfermarktScraper.Domain.Entities;

namespace TransfermarktScraper.Data.Repositories.Impl
{
    /// <inheritdoc/>
    public class CountryRepository : ICountryRepository
    {
        private readonly IMongoCollection<Country> _countries;
        private readonly ILogger<CountryRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CountryRepository"/> class.
        /// </summary>
        /// <param name="dbContext">the database context.</param>
        /// <param name="logger">The logger.</param>
        public CountryRepository(IDbContext dbContext, ILogger<CountryRepository> logger)
        {
            _countries = dbContext.Countries;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<Country?> GetAsync(string countryTransfermarktId)
        {
            try
            {
                return await _countries.Find(country => country.TransfermarktId == countryTransfermarktId).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                throw new Exception($"Error in {nameof(GetAsync)}: Failed to retrieve the country with TransfermarktId ID {countryTransfermarktId} from the database.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Country>> GetAllAsync()
        {
            try
            {
                return await _countries.Find(_ => true).ToListAsync();
            }
            catch (MongoException ex)
            {
                throw new Exception($"Error in {nameof(GetAllAsync)}: Failed to retrieve all countries from the database.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Competition>> GetAllAsync(string countryTransfermarktId)
        {
            try
            {
                var country = await GetAsync(countryTransfermarktId);

                if (country == null)
                {
                    throw new InvalidOperationException($"Error in {nameof(GetAllAsync)}: Country with Transfermarkt ID {countryTransfermarktId} not found in the database.");
                }

                var competitions = country.Competitions;

                return competitions;
            }
            catch (MongoException ex)
            {
                throw new Exception($"Error in {nameof(GetAllAsync)}: Failed to retrieve all competitions of the country with Transfermarkt ID {countryTransfermarktId} from the database.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task AddRangeAsync(IEnumerable<Country> countries)
        {
            try
            {
                SetUpdateTime(countries);

                await _countries.InsertManyAsync(countries);
            }
            catch (MongoException ex)
            {
                throw new Exception($"Error in {nameof(AddRangeAsync)}: Failed to add countries to the database.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Country>> InsertOrUpdateRangeAsync(IEnumerable<Country> countries)
        {
            try
            {
                SetUpdateTime(countries);

                var countryTransfermarktIds = countries
                    .Select(c => c.TransfermarktId)
                    .ToHashSet();

                var hasExistingCountries = await _countries
                    .Find(c => countryTransfermarktIds.Contains(c.TransfermarktId))
                    .AnyAsync();

                if (!hasExistingCountries)
                {
                    _logger.LogInformation("Inserting {Count} countries in the database...", countryTransfermarktIds.Count.ToString());
                    await _countries.InsertManyAsync(countries);
                    _logger.LogInformation("Successfully inserted {Count} countries in the database...", countryTransfermarktIds.Count.ToString());
                }
                else
                {
                    var existingCountryTransfermarktIds = (await _countries
                        .Find(c => countryTransfermarktIds.Contains(c.TransfermarktId))
                        .Project(c => c.TransfermarktId)
                        .ToListAsync())
                        .ToHashSet();

                    var countriesToInsert = countries
                        .Where(c => !existingCountryTransfermarktIds.Contains(c.TransfermarktId))
                        .ToList();

                    var countriesToUpdate = countries
                        .Where(c => existingCountryTransfermarktIds.Contains(c.TransfermarktId))
                        .ToList();

                    if (countriesToInsert.Any())
                    {
                        _logger.LogInformation("Inserting {Count} countries in the database...", countriesToInsert.Count.ToString());
                        await _countries.InsertManyAsync(countriesToInsert);
                        _logger.LogInformation("Successfully inserted {Count} countries in the database...", countriesToInsert.Count.ToString());
                    }

                    var bulkOperations = countriesToUpdate
                        .Select(country => new UpdateOneModel<Country>(
                            Builders<Country>.Filter.Eq(c => c.TransfermarktId, country.TransfermarktId),
                            Builders<Country>.Update
                                .Set(c => c.Name, country.Name)
                                .Set(c => c.Flag, country.Flag)
                                .Set(c => c.Competitions, country.Competitions)))
                        .ToList();

                    _logger.LogInformation("Updating {Count} countries in the database...", bulkOperations.Count);
                    var result = await _countries.BulkWriteAsync(bulkOperations);
                    _logger.LogInformation("Successfully updated {Count} countries in the database...", bulkOperations.Count);
                }

                return countries;
            }
            catch (MongoException ex)
            {
                throw new Exception($"Error in {nameof(InsertOrUpdateRangeAsync)}: Failed inserting or updating {countries.Count()} countries in the database.", ex);
            }
        }

        public Task UpdateAsync(string countryTransfermarktIds, IEnumerable<string> competitionTransfermarktIds)
        {
            throw new NotImplementedException();
        }

        private void SetUpdateTime(IEnumerable<Country> countries)
        {
            var time = DateTime.UtcNow;

            foreach (var country in countries)
            {
                country.UpdateDate = time;

                foreach (var competition in country.Competitions)
                {
                    competition.UpdateDate = time;
                }
            }
        }
    }
}
