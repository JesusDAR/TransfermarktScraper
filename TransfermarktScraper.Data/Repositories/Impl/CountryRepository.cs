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
using TransfermarktScraper.Domain.Exceptions;

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
            catch (Exception)
            {
                var message = $"Failed to retrieve the country with Transfermarkt ID: {countryTransfermarktId} from the database.";
                throw DatabaseException.LogError(message, nameof(GetAsync), nameof(CountryRepository), _logger);
            }
        }

        /// <inheritdoc/>
        public async Task<long> GetCountAsync(CancellationToken cancellationToken)
        {
            try
            {
                var count = await _countries.CountDocumentsAsync(FilterDefinition<Country>.Empty, cancellationToken: cancellationToken);

                return count;
            }
            catch (Exception)
            {
                var message = $"Failed to retrieve the number of countries in the database.";
                throw DatabaseException.LogError(message, nameof(GetCountAsync), nameof(CountryRepository), _logger);
            }
        }

        /// <inheritdoc/>
        public async Task<Competition?> GetCompetitionAsync(string competitionTransfermarktId, CancellationToken cancellationToken)
        {
            try
            {
                var country = await _countries
                    .Find(country => country.Competitions.Any(competition => competition.TransfermarktId == competitionTransfermarktId))
                    .FirstOrDefaultAsync(cancellationToken);

                var competition = country?.Competitions.FirstOrDefault(competition => competition.TransfermarktId == competitionTransfermarktId);

                return competition;
            }
            catch (Exception)
            {
                var message = $"Failed to retrieve the competition with Transfermarkt ID: {competitionTransfermarktId} from the database.";
                throw DatabaseException.LogError(message, nameof(GetCompetitionAsync), nameof(CountryRepository), _logger);
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
            catch (Exception)
            {
                var message = $"Failed to retrieve all countries from the database.";
                throw DatabaseException.LogError(message, nameof(GetAllAsync), nameof(CountryRepository), _logger);
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
                    var message = $"Country with Transfermarkt ID: {countryTransfermarktId} not found in the database.";
                    throw DatabaseException.LogError(message, nameof(GetAllAsync), nameof(CountryRepository), _logger);
                }

                var competitions = country.Competitions;

                return competitions;
            }
            catch (DatabaseException ex)
            {
                var message = string.Concat($"Failed to retrieve all competitions of the country with Transfermarkt ID: {countryTransfermarktId} from the database. ", ex.Message);
                throw DatabaseException.LogError(message, nameof(GetAllAsync), nameof(CountryRepository), _logger);
            }
            catch (Exception)
            {
                var message = $"Failed to retrieve all competitions of the country with Transfermarkt ID: {countryTransfermarktId} from the database.";
                throw DatabaseException.LogError(message, nameof(GetAllAsync), nameof(CountryRepository), _logger);
            }
        }

        /// <inheritdoc/>
        public async Task RemoveAllAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _countries.DeleteManyAsync(FilterDefinition<Country>.Empty, cancellationToken);
            }
            catch (Exception)
            {
                var message = "Failed to delete all countries from the database.";
                throw DatabaseException.LogError(message, nameof(RemoveAllAsync), nameof(CountryRepository), _logger);
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
            catch (Exception)
            {
                var message = $"Failed to add range of countries to the database.";
                throw DatabaseException.LogError(message, nameof(AddRangeAsync), nameof(CountryRepository), _logger);
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
                    _logger.LogDebug("Inserting {Count} countries in the database...", countryTransfermarktIds.Count.ToString());
                    countries = countries.OrderBy(country => int.Parse(country.TransfermarktId));
                    await _countries.InsertManyAsync(countries);
                    _logger.LogInformation("Successfully inserted {Count} countries in the database...", countryTransfermarktIds.Count.ToString());
                }
                else
                {
                    var existingCountryTransfermarktIds = (await _countries
                        .Find(country => countryTransfermarktIds.Contains(country.TransfermarktId))
                        .Project(country => country.TransfermarktId)
                        .ToListAsync(cancellationToken))
                        .ToHashSet();

                    var countriesToInsert = countries
                        .Where(country => !existingCountryTransfermarktIds.Contains(country.TransfermarktId))
                        .ToList();

                    var countriesToUpdate = countries
                        .Where(country => existingCountryTransfermarktIds.Contains(country.TransfermarktId))
                        .ToList();

                    if (countriesToInsert.Any())
                    {
                        _logger.LogDebug("Inserting {Count} countries in the database...", countriesToInsert.Count.ToString());
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

                    _logger.LogDebug("Updating {Count} countries in the database...", bulkOperations.Count);
                    var result = await _countries.BulkWriteAsync(bulkOperations, cancellationToken: cancellationToken);
                    _logger.LogInformation("Successfully updated {Count} countries in the database...", bulkOperations.Count);
                }

                return countries;
            }
            catch (Exception)
            {
                var message = $"Failed inserting or updating {countries.Count()} countries in the database.";
                throw DatabaseException.LogError(message, nameof(InsertOrUpdateRangeAsync), nameof(CountryRepository), _logger);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Competition>> UpdateRangeAsync(string countryTransfermarktId, IEnumerable<Competition> competitions, CancellationToken cancellationToken)
        {
            try
            {
                var country = await GetAsync(countryTransfermarktId, cancellationToken);

                if (country == null)
                {
                    var message = $"Country with Transfermarkt ID: {countryTransfermarktId} not found in the database.";
                    throw DatabaseException.LogError(message, nameof(UpdateRangeAsync), nameof(CountryRepository), _logger);
                }

                SetUpdateTime(new List<Country> { country });

                var filter = Builders<Country>.Filter.Eq(c => c.TransfermarktId, country.TransfermarktId);

                var update = Builders<Country>.Update.Set(c => c.Competitions, competitions);

                _logger.LogDebug(
                    "Updating {Count} competitions from {Country.Name} in the database...",
                    competitions.Count(),
                    country.Name);
                await _countries.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
                _logger.LogInformation(
                    "Successfully updated {Count} competitions from {Country.Name} in the database...",
                    competitions.Count(),
                    country.Name);

                return competitions;
            }
            catch (DatabaseException ex)
            {
                var message = string.Concat($"Failed to update competitions of the country with Transfermarkt ID: {countryTransfermarktId} in the database. ", ex.Message);
                throw DatabaseException.LogError(message, nameof(UpdateRangeAsync), nameof(CountryRepository), _logger);
            }
            catch (Exception)
            {
                var message = $"Failed to update competitions of the country with Transfermarkt ID: {countryTransfermarktId} in the database.";
                throw DatabaseException.LogError(message, nameof(UpdateRangeAsync), nameof(CountryRepository), _logger);
            }
        }

        /// <summary>
        /// Sets the update time for a collection of countries and their associated competitions.
        /// </summary>
        /// <param name="countries">The collection of countries to update.</param>
        private void SetUpdateTime(IEnumerable<Country> countries)
        {
            var time = DateTime.Now;

            foreach (var country in countries)
            {
                country.UpdateDate = time;

                foreach (var competition in country.Competitions)
                {
                    if (competition != null)
                    {
                        competition.UpdateDate = time;
                    }
                }
            }
        }
    }
}
