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
                var club = await _clubs.Find(club => club.TransfermarktId == clubTransfermarktId)
                    .FirstOrDefaultAsync(cancellationToken);
                return club;
            }
            catch (Exception)
            {
                var message = $"Failed to retrieve the club with Transfermarkt ID: {clubTransfermarktId} from the database.";
                throw DatabaseException.LogError(message, nameof(GetAsync), nameof(ClubRepository), _logger);
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
            catch (Exception)
            {
                var message = $"Failed to retrieve all clubs from competition with Transfermarkt ID: {competitionTransfermarktId} from the database.";
                throw DatabaseException.LogError(message, nameof(GetAllAsync), nameof(ClubRepository), _logger);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Club>> GetAllAsync(CancellationToken cancellationToken)
        {
            try
            {
                var clubs = await _clubs.Find(_ => true).ToListAsync(cancellationToken);

                return clubs;
            }
            catch (Exception)
            {
                var message = $"Failed to retrieve all clubs from the database.";
                throw DatabaseException.LogError(message, nameof(GetAllAsync), nameof(ClubRepository), _logger);
            }
        }

        /// <inheritdoc/>
        public async Task<Club> InsertOrUpdateAsync(Club club, CancellationToken cancellationToken)
        {
            try
            {
                club.UpdateDate = DateTime.Now;

                var existingClub = await GetAsync(club.TransfermarktId, cancellationToken);

                if (existingClub == null)
                {
                    _logger.LogDebug(
                        "Inserting club with name {Club.Name} and Transfermarkt ID {Club.TransfermarktId} in the database...",
                        club.Name,
                        club.TransfermarktId);
                    await _clubs.InsertOneAsync(club);
                    _logger.LogInformation(
                        "Successfully inserted club with name {Club.Name} Transfermarkt ID {Club.TransfermarktId} in the database...",
                        club.Name,
                        club.TransfermarktId);

                    return club;
                }

                // clubs exists, current competition links need to be added
                existingClub.CompetitionIds.ToList().AddRange(club.CompetitionIds);
                var updatedCompetitionIds = existingClub.CompetitionIds;

                var filter = Builders<Club>.Filter.Eq(c => c.TransfermarktId, club.TransfermarktId);

                var update = Builders<Club>.Update
                    .Set(c => c.AgeAverage, club.AgeAverage)
                    .Set(c => c.CompetitionIds, updatedCompetitionIds)
                    .Set(c => c.Crest, club.Crest)
                    .Set(c => c.Link, club.Link)
                    .Set(c => c.MarketValue, club.MarketValue)
                    .Set(c => c.MarketValueAverage, club.MarketValueAverage)
                    .Set(c => c.Name, club.Name)
                    .Set(c => c.PlayersCount, club.PlayersCount)
                    .Set(c => c.UpdateDate, club.UpdateDate);

                _logger.LogDebug(
                    "Updating club with name {Club.Name} and Transfermarkt ID {Club.TransfermarktId} in the database...",
                    club.Name,
                    club.TransfermarktId);
                await _clubs.UpdateOneAsync(
                    filter,
                    update,
                    new UpdateOptions { IsUpsert = false },
                    cancellationToken);
                _logger.LogInformation(
                    "Successfully updated club with name {Club.Name} Transfermarkt ID {Club.TransfermarktId} in the database...",
                    club.Name,
                    club.TransfermarktId);

                return new Club
                {
                    AgeAverage = club.AgeAverage,
                    CompetitionIds = existingClub.CompetitionIds,
                    Crest = club.Crest,
                    Link = club.Link,
                    MarketValue = club.MarketValue,
                    MarketValueAverage = club.MarketValueAverage,
                    Name = club.Name,
                    PlayersCount = club.PlayersCount,
                    TransfermarktId = club.TransfermarktId,
                    UpdateDate = club.UpdateDate,
                };
            }
            catch (Exception)
            {
                var message = $"Failed inserting or updating club with Transfermarkt ID: {club.TransfermarktId} in the database.";
                throw DatabaseException.LogError(message, nameof(InsertOrUpdateAsync), nameof(ClubRepository), _logger);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Player>> InsertOrUpdateRangeAsync(Club club, IEnumerable<Player> players, CancellationToken cancellationToken)
        {
            var newPlayers = new List<Player>();

            try
            {
                var existingPlayersDict = club.Players?.ToDictionary(p => p.TransfermarktId) ?? new Dictionary<string, Player>();

                foreach (var player in players)
                {
                    if (existingPlayersDict.TryGetValue(player.TransfermarktId, out var existingPlayer))
                    {
                        // Update existing player
                        existingPlayer.Age = player.Age;
                        existingPlayer.ContractEnd = player.ContractEnd;
                        existingPlayer.ContractStart = player.ContractStart;
                        existingPlayer.DateOfBirth = player.DateOfBirth;
                        existingPlayer.Foot = player.Foot;
                        existingPlayer.Height = player.Height;
                        existingPlayer.Link = player.Link;
                        existingPlayer.MarketValue = player.MarketValue;
                        existingPlayer.MarketValues = player.MarketValues;
                        existingPlayer.Name = player.Name;
                        existingPlayer.Nationalities = player.Nationalities;
                        existingPlayer.Number = player.Number;
                        existingPlayer.Portrait = player.Portrait;
                        existingPlayer.UpdateDate = DateTime.Now;

                        newPlayers.Add(existingPlayer);
                    }
                    else
                    {
                        // Insert new player
                        player.UpdateDate = DateTime.Now;
                        newPlayers.Add(player);
                    }
                }

                var filter = Builders<Club>.Filter.Eq(c => c.TransfermarktId, club.TransfermarktId);

                var update = Builders<Club>.Update
                    .Set(c => c.Players, newPlayers);

                _logger.LogDebug("Inserting/Updating {Count} players of {Club} in the database...", newPlayers.Count, club.Name);
                await _clubs.UpdateOneAsync(
                    filter,
                    update,
                    new UpdateOptions { IsUpsert = false },
                    cancellationToken: cancellationToken);
                _logger.LogInformation("Successfully Inserted/Updated {Count} players of {Club} in the database...", newPlayers.Count, club.Name);

                return players;
            }
            catch (Exception)
            {
                var message = $"Failed inserting or updating players range in club with Transfermarkt ID: {club.TransfermarktId} in the database.";
                throw DatabaseException.LogError(message, nameof(InsertOrUpdateRangeAsync), nameof(ClubRepository), _logger);
            }
        }
    }
}
