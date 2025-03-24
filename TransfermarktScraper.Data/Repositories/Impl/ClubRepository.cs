using System;
using System.Collections.Generic;
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
    public class ClubRepository : IClubRepository
    {
        private readonly IMongoCollection<Club> _clubs;
        private readonly ILogger<ClubRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClubRepository"/> class.
        /// </summary>
        /// <param name="dbContext">the database context.</param>
        /// <param name="logger">The logger.</param>
        public ClubRepository(IDbContext dbContext, ILogger<ClubRepository> logger)
        {
            _clubs = dbContext.Clubs;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<Club?> GetAsync(string clubTransfermarktId, CancellationToken cancellationToken)
        {
            try
            {
                var club = await _clubs.Find(club => club.TransfermarktId == clubTransfermarktId).FirstOrDefaultAsync(cancellationToken);
                return club;
            }
            catch (MongoException ex)
            {
                throw new Exception($"Error in {nameof(GetAsync)}: Failed to retrieve the club with Transfermarkt ID {clubTransfermarktId} from the database.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Club>> GetAllAsync(string competitionTransfermarktId, CancellationToken cancellationToken)
        {
            try
            {
                var clubs = await _clubs.Find(club =>
                    club.CompetitionIds.Contains(competitionTransfermarktId)).ToListAsync(cancellationToken);
                return clubs;
            }
            catch (MongoException ex)
            {
                throw new Exception($"Error in {nameof(GetAllAsync)}: Failed to retrieve all clubs from competition with TransfermarktId {competitionTransfermarktId} from the database.", ex);
            }
        }
    }
}
