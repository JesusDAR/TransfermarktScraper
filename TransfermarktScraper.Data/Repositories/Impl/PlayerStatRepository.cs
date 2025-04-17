using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using TransfermarktScraper.Data.Context.Interfaces;
using TransfermarktScraper.Data.Repositories.Interfaces;
using TransfermarktScraper.Domain.Entities.Stat;
using TransfermarktScraper.Domain.Exceptions;
using TransfermarktScraper.Domain.Utils;

namespace TransfermarktScraper.Data.Repositories.Impl
{
    /// <inheritdoc/>
    public class PlayerStatRepository : IPlayerStatRepository
    {
        private readonly IMongoCollection<PlayerStat> _playerStats;
        private readonly ILogger<PlayerStatRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerStatRepository"/> class.
        /// </summary>
        /// <param name="dbContext">the database context.</param>
        /// <param name="logger">The logger.</param>
        public PlayerStatRepository(IDbContext dbContext, ILogger<PlayerStatRepository> logger)
        {
            _playerStats = dbContext.PlayerStats;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<PlayerStat?> GetPlayerStatAsync(string playerTransfermarktId, CancellationToken cancellationToken)
        {
            try
            {
                var transfermarktId = EntityUtils.GetHash($"{playerTransfermarktId}|stat");
                var playerStat = await _playerStats.Find(playerStat => playerStat.TransfermarktId == transfermarktId)
                    .FirstOrDefaultAsync(cancellationToken);
                return playerStat;
            }
            catch (System.Exception)
            {
                var message = $"Failed to retrieve the player stat with player Transfermarkt ID: {playerTransfermarktId} from the database.";
                throw DatabaseException.LogError(message, nameof(GetPlayerStatAsync), nameof(PlayerStatRepository), _logger);
            }
        }

        /// <inheritdoc/>
        public async Task<PlayerStat> InsertAsync(PlayerStat playerStat, CancellationToken cancellationToken)
        {
            try
            {
                await _playerStats.InsertOneAsync(playerStat, options: null, cancellationToken);
                return playerStat;
            }
            catch (System.Exception)
            {
                var message = $"Failed to insert player stat with {nameof(playerStat.PlayerTransfermarktId)}: {playerStat.PlayerTransfermarktId} to the database.";
                throw DatabaseException.LogError(message, nameof(InsertAsync), nameof(PlayerStatRepository), _logger);
            }
        }
    }
}
