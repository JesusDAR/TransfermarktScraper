using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="CountryRepository"/> class.
        /// </summary>
        /// <param name="dbContext">the database context.</param>
        public CountryRepository(IDbContext dbContext)
        {
            _countries = dbContext.Countries;
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
                    await _countries.InsertManyAsync(countries);
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
                        .Select(country => new ReplaceOneModel<Country>(
                            Builders<Country>.Filter.Eq(c => c.TransfermarktId, country.TransfermarktId),
                            country))
                        .ToList();

                    await _countries.BulkWriteAsync(bulkOperations);
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
