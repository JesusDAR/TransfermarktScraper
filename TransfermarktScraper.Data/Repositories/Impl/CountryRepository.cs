using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
        public async Task<Country?> GetAsync(string id)
        {
            try
            {
                return await _countries.Find(country => country.Id == id).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                throw new Exception($"Error in {nameof(GetAsync)}: Failed to retrieve the country with ID {id} from the database.", ex);
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
        public async Task<IEnumerable<Competition>> GetAllAsync(string countryId)
        {
            try
            {
                var country = await GetAsync(countryId);

                if (country == null)
                {
                    throw new InvalidOperationException($"Error in {nameof(GetAllAsync)}: Country with ID {countryId} not found in the database.");
                }

                var competitions = country.Competitions;

                return competitions;
            }
            catch (MongoException ex)
            {
                throw new Exception($"Error in {nameof(GetAllAsync)}: Failed to retrieve all competitions of the country with ID {countryId} from the database.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task AddRangeAsync(IEnumerable<Country> countries)
        {
            try
            {
                await _countries.InsertManyAsync(countries);
            }
            catch (MongoException ex)
            {
                throw new Exception($"Error in {nameof(AddRangeAsync)}: Failed to add countries to the database.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task InsertOrUpdateRangeAsync(IEnumerable<Country> countries)
        {
            try
            {
                var countryIds = countries
                    .Select(c => c.TransfermarktId)
                    .ToHashSet();

                var hasExistingCountries = await _countries
                    .Find(c => countryIds.Contains(c.TransfermarktId))
                    .AnyAsync();

                if (!hasExistingCountries)
                {
                    _logger.LogInformation("Inserting {Count} countries in the database...", countryIds.Count.ToString());
                    await _countries.InsertManyAsync(countries);
                    _logger.LogInformation("Successfully inserted {Count} countries in the database...", countryIds.Count.ToString());
                }
                else
                {
                    var existingCountryIds = (await _countries
                        .Find(c => countryIds.Contains(c.TransfermarktId))
                        .Project(c => c.TransfermarktId)
                        .ToListAsync())
                        .ToHashSet();

                    var countriesToInsert = countries
                        .Where(c => !existingCountryIds.Contains(c.TransfermarktId))
                        .ToList();

                    var countriesToUpdate = countries
                        .Where(c => existingCountryIds.Contains(c.TransfermarktId))
                        .ToList();

                    if (countriesToInsert.Any())
                    {
                        await _countries.InsertManyAsync(countriesToInsert);
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
                    await _countries.BulkWriteAsync(bulkOperations);
                    _logger.LogInformation("Successfully updated {Count} countries in the database...", bulkOperations.Count);
                }
            }
            catch (MongoException ex)
            {
                throw new Exception($"Error in {nameof(InsertOrUpdateRangeAsync)}: Failed inserting or updating {countries.Count()} countries in the database.", ex);
            }
        }

        public Task UpdateAsync(string countryId, IEnumerable<string> competitionTransfermarktIds)
        {
            throw new NotImplementedException();
        }
    }
}
