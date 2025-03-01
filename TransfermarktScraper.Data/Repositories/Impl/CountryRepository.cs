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
                throw new Exception("Error retrieving the country from the database.", ex);
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
                throw new Exception("Error retrieving all countries from the database.", ex);
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
                throw new Exception("Error adding countries to the database.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task InsertOrUpdateRangeAsync(IEnumerable<Country> countries)
        {
            try
            {
                var countryNames = countries.Select(c => c.Name).ToHashSet();

                var countriesToUpdate = await _countries
                    .Find(c => countryNames.Contains(c.Name))
                    .ToListAsync();

                if (countriesToUpdate.Count == 0)
                {
                    await _countries.InsertManyAsync(countries);
                }
                else
                {
                    var existingCountryNames = countriesToUpdate.Select(c => c.Name).ToHashSet();

                    var countriesToInsert = countries
                        .Where(c => !existingCountryNames.Contains(c.Name))
                        .ToList();

                    if (countriesToInsert.Count > 0)
                    {
                        await _countries.InsertManyAsync(countriesToInsert);
                    }

                    var bulkOperations = new List<WriteModel<Country>>();

                    foreach (var country in countriesToUpdate)
                    {
                        var filter = Builders<Country>.Filter.Eq(c => c.Name, country.Name);

                        var update = new ReplaceOneModel<Country>(filter, country);

                        bulkOperations.Add(update);
                    }

                    await _countries.BulkWriteAsync(bulkOperations);
                }
            }
            catch (MongoException ex)
            {
                throw new Exception("Error inserting or updating countries in the database.", ex);
            }
        }
    }
}
