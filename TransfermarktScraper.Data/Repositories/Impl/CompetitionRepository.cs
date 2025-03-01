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
    public class CompetitionRepository : ICompetitionRepository
    {
        private readonly IMongoCollection<Competition> _competitions;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompetitionRepository"/> class.
        /// </summary>
        /// <param name="dbContext">the database context.</param>
        public CompetitionRepository(IDbContext dbContext)
        {
            _competitions = dbContext.Competitions;
        }

        /// <inheritdoc/>
        public async Task<Competition?> GetAsync(string id)
        {
            try
            {
                return await _competitions.Find(competition => competition.Id == id).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                throw new Exception("Error retrieving the competition from the database.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Competition>> GetAllAsync()
        {
            try
            {
                return await _competitions.Find(_ => true).ToListAsync();
            }
            catch (MongoException ex)
            {
                throw new Exception("Error retrieving all competitions from the database.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task AddRangeAsync(IEnumerable<Competition> competitions)
        {
            try
            {
                await _competitions.InsertManyAsync(competitions);
            }
            catch (MongoException ex)
            {
                throw new Exception("Error adding competitions to the database.", ex);
            }
        }
    }
}
