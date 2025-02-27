using System;
using System.Collections.Generic;
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
            catch (Exception ex)
            {
                throw new Exception("Error adding countries to the database.", ex);
            }
        }
    }
}
