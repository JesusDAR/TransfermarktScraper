using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using TransfermarktScraper.Data.Context.Interfaces;
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
        public async Task<Country?> GetAsync(string countryTransfermarktId, CancellationToken cancellationToken)
        {
            try
            {
                var country = await _countries.Find(country => country.TransfermarktId == countryTransfermarktId).FirstOrDefaultAsync(cancellationToken);
                return country;
            }
            catch (MongoException ex)
            {
                throw new Exception($"Error in {nameof(GetAsync)}: Failed to retrieve the country with Transfermarkt ID {countryTransfermarktId} from the database.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Country>> GetAllAsync(CancellationToken cancellationToken)
        {
            try
            {
                var countries = await _countries.Find(_ => true).ToListAsync(cancellationToken);
                return countries;
            }
            catch (MongoException ex)
            {
                throw new Exception($"Error in {nameof(GetAllAsync)}: Failed to retrieve all countries from the database.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Competition>> GetAllAsync(string countryTransfermarktId, CancellationToken cancellationToken)
        {
            try
            {
                var country = await GetAsync(countryTransfermarktId, cancellationToken);

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
        public async Task AddRangeAsync(IEnumerable<Country> countries, CancellationToken cancellationToken)
        {
            try
            {
                SetUpdateTime(countries);

                await _countries.InsertManyAsync(countries, cancellationToken: cancellationToken);
            }
            catch (MongoException ex)
            {
                throw new Exception($"Error in {nameof(AddRangeAsync)}: Failed to add countries to the database.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Country>> InsertOrUpdateRangeAsync(IEnumerable<Country> countries, CancellationToken cancellationToken)
        {
            try
            {
                SetUpdateTime(countries);

                var countryTransfermarktIds = countries
                    .Select(c => c.TransfermarktId)
                    .ToHashSet();

                var hasExistingCountries = await _countries
                    .Find(c => countryTransfermarktIds.Contains(c.TransfermarktId))
                    .AnyAsync(cancellationToken);

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
                        .ToListAsync(cancellationToken))
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
                        await _countries.InsertManyAsync(countriesToInsert, cancellationToken: cancellationToken);
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
                    var result = await _countries.BulkWriteAsync(bulkOperations, cancellationToken: cancellationToken);
                    _logger.LogInformation("Successfully updated {Count} countries in the database...", bulkOperations.Count);
                }

                return countries;
            }
            catch (MongoException ex)
            {
                throw new Exception($"Error in {nameof(InsertOrUpdateRangeAsync)}: Failed inserting or updating {countries.Count()} countries in the database.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Competition>> UpdateRangeAsync(string countryTransfermarktId, IEnumerable<Competition> competitions, CancellationToken cancellationToken)
        {
            var country = await GetAsync(countryTransfermarktId, cancellationToken);

            if (country == null)
            {
                throw new InvalidOperationException($"Error in {nameof(UpdateRangeAsync)}: Country with Transfermarkt ID {countryTransfermarktId} not found in the database.");
            }

            SetUpdateTime(new List<Country> { country });

            var filter = Builders<Country>.Filter.Eq(c => c.TransfermarktId, country.TransfermarktId);

            var competitionsList = competitions.ToList();

            var update = Builders<Country>.Update.Set(c => c.Competitions, competitionsList);

            _logger.LogInformation(
                "Updating {Count} competitions from {Country.Name} in the database...",
                competitionsList.Count.ToString(),
                country.Name);
            await _countries.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
            _logger.LogInformation(
                "Successfully updated {Count} competitions from {Country.Name} in the database...",
                competitionsList.Count.ToString(),
                country.Name);

            return competitionsList;
        }

        /// <summary>
        /// Sets the update time for a collection of countries and their associated competitions.
        /// </summary>
        /// <param name="countries">The collection of countries to update.</param>
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
