using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<PlayerStat?> GetAsync(string playerTransfermarktId, CancellationToken cancellationToken)
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
                throw DatabaseException.LogError(message, nameof(GetAsync), nameof(PlayerStatRepository), _logger);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<PlayerStat>> GetAllAsync(IEnumerable<string> playerTransfermarktIds, CancellationToken cancellationToken)
        {
            try
            {
                var transfermarktIds = playerTransfermarktIds
                    .Select(playerTransfermarktId => EntityUtils.GetHash($"{playerTransfermarktId}|stat"))
                    .ToHashSet();

                var playerStats = await _playerStats
                    .Find(playerStat => transfermarktIds.Contains(playerStat.TransfermarktId))
                    .ToListAsync(cancellationToken);

                return playerStats;
            }
            catch (Exception)
            {
                var message = $"Failed to retrieve player stats for the provided Transfermarkt IDs: {string.Join(", ", playerTransfermarktIds)} from the database.";
                throw DatabaseException.LogError(message, nameof(GetAllAsync), nameof(PlayerStatRepository), _logger);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<PlayerSeasonStat>> GetPlayerSeasonStatsAsync(string playerTransfermarktId, IEnumerable<string> seasonTransfermarktIds, CancellationToken cancellationToken)
        {
            try
            {
                var transfermarktIds = seasonTransfermarktIds
                    .Select(seasonTransfermarktId => EntityUtils.GetHash($"{playerTransfermarktId}|{seasonTransfermarktId}"))
                    .ToHashSet();

                var projection = Builders<PlayerStat>.Projection
                    .ElemMatch(
                        playerStat => playerStat.PlayerSeasonStats,
                        Builders<PlayerSeasonStat>.Filter.In(playerSeasonStat => playerSeasonStat.TransfermarktId, transfermarktIds));

                var filter = Builders<PlayerStat>.Filter
                    .Eq(
                        playerStat => playerStat.TransfermarktId,
                        EntityUtils.GetHash($"{playerTransfermarktId}|stat"));

                var playerStat = await _playerStats
                    .Find(filter)
                    .Project<PlayerStat>(projection)
                    .FirstOrDefaultAsync(cancellationToken);

                return playerStat.PlayerSeasonStats ?? Enumerable.Empty<PlayerSeasonStat>();
            }
            catch (Exception)
            {
                var message = $"Failed to retrieve the player season stats with player Transfermarkt ID: {playerTransfermarktId} from the database.";
                throw DatabaseException.LogError(message, nameof(GetPlayerSeasonStatsAsync), nameof(PlayerStatRepository), _logger);
            }
        }

        /// <inheritdoc/>
        public async Task<PlayerStat> InsertOrUpdateAsync(PlayerStat playerStat, CancellationToken cancellationToken)
        {
            try
            {
                var existingPlayerStat = await GetAsync(playerStat.PlayerTransfermarktId, cancellationToken);

                if (existingPlayerStat == null)
                {
                    _logger.LogDebug("Inserting player stat with {PlayerStat.PlayerTransfermarktId} player Transfermarkt ID in the database...", playerStat.PlayerTransfermarktId);
                    await _playerStats.InsertOneAsync(playerStat, options: null, cancellationToken);
                    _logger.LogInformation("Successfully inserted player stat with {PlayerStat.PlayerTransfermarktId} player Transfermarkt ID in the database...", playerStat.PlayerTransfermarktId);

                    return playerStat;
                }

                var transfermarktIds = playerStat.PlayerSeasonStats
                    .ToDictionary(playerSeasonStat => EntityUtils.GetHash($"{playerSeasonStat.PlayerTransfermarktId}|{playerSeasonStat.SeasonTransfermarktId}"));

                var newPlayerSeasonStats = new List<PlayerSeasonStat>();

                foreach (var existingPlayerSeasonStat in existingPlayerStat.PlayerSeasonStats)
                {
                    if (transfermarktIds.TryGetValue(existingPlayerSeasonStat.TransfermarktId, out var newPlayerSeasonStat))
                    {
                        newPlayerSeasonStats.Add(newPlayerSeasonStat);
                    }
                    else
                    {
                        newPlayerSeasonStats.Add(existingPlayerSeasonStat);
                    }
                }

                var filter = Builders<PlayerStat>.Filter
                    .Eq(
                    playerStat => playerStat.TransfermarktId,
                    playerStat.TransfermarktId);

                var update = Builders<PlayerStat>.Update
                    .Set(
                    playerStat => playerStat.PlayerSeasonStats,
                    playerStat.PlayerSeasonStats);

                await _playerStats.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

                return playerStat;
            }
            catch (Exception)
            {
                var message = $"Failed to update season stats for player with {nameof(playerStat.PlayerTransfermarktId)}: {playerStat.PlayerTransfermarktId} into the database.";
                throw DatabaseException.LogError(message, nameof(InsertOrUpdateAsync), nameof(PlayerStatRepository), _logger);
            }
        }
    }
}
