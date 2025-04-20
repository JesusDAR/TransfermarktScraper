using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TransfermarktScraper.Domain.Utils;

namespace TransfermarktScraper.Domain.Entities.Stat.Season
{
    /// <summary>
    /// Represents a player season entity.
    /// </summary>
    public class PlayerSeasonStat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerSeasonStat"/> class.
        /// </summary>
        /// <param name="playerTransfermarktId">The unique player Transfermarkt identifier.</param>
        /// <param name="seasonTransfermarktId">The unique season Transfermarkt identifier.</param>
        public PlayerSeasonStat(string playerTransfermarktId, string seasonTransfermarktId)
        {
            if (string.IsNullOrEmpty(playerTransfermarktId))
            {
                throw new ArgumentException($"{nameof(PlayerTransfermarktId)} cannot be null or empty");
            }

            if (string.IsNullOrEmpty(seasonTransfermarktId))
            {
                throw new ArgumentException($"{nameof(SeasonTransfermarktId)} cannot be null or empty");
            }

            PlayerTransfermarktId = playerTransfermarktId;
            SeasonTransfermarktId = seasonTransfermarktId;
            TransfermarktId = EntityUtils.GetHash($"{playerTransfermarktId}|{seasonTransfermarktId}");
            UpdateDate = DateTime.Now;
        }

        /// <summary>
        /// Gets the unique composite identifier.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [BsonElement("transfermarktId")]
        public string TransfermarktId { get; private set; }

        /// <summary>
        /// Gets the unique player Transfermarkt identifier.
        /// </summary>
        [BsonRequired]
        [BsonElement("playerTransfermarktId")]
        public string PlayerTransfermarktId { get; private set; }

        /// <summary>
        /// Gets the unique season Transfermarkt identifier.
        /// </summary>
        [BsonRequired]
        [BsonElement("seasonTransfermarktId")]
        public string SeasonTransfermarktId { get; private set; }

        /// <summary>
        /// Gets or sets the last update date.
        /// </summary>
        [BsonElement("updateDate")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// Gets or sets the number of appearances in all competitions of the season.
        /// </summary>
        [BsonElement("appearances")]
        public int Appearances { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of goals in all competitions of the season.
        /// </summary>
        [BsonElement("goals")]
        public int Goals { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of assists in all competitions of the season.
        /// </summary>
        [BsonElement("assists")]
        public int Assists { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of own goals in all competitions of the season.
        /// </summary>
        [BsonElement("ownGoals")]
        public int OwnGoals { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of times the player was substituted on in all competitions of the season.
        /// </summary>
        [BsonElement("substitutionsOn")]
        public int SubstitutionsOn { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of times the player was substituted off in all competitions of the season.
        /// </summary>
        [BsonElement("substitutionsOff")]
        public int SubstitutionsOff { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of yellow cards received in all competitions of the season.
        /// </summary>
        [BsonElement("yellowCards")]
        public int YellowCards { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of second yellow cards received in all competitions of the season.
        /// </summary>
        [BsonElement("secondYellowCards")]
        public int SecondYellowCards { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of red cards received in all competitions of the season.
        /// </summary>
        [BsonElement("redCards")]
        public int RedCards { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of penalty goals scored in all competitions of the season.
        /// </summary>
        [BsonElement("penaltyGoals")]
        public int PenaltyGoals { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of goals that the goalkeeper has conceded in all competitions of the season.
        /// </summary>
        [BsonElement("goalsConceded")]
        public int GoalsConceded { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of clean sheets for the goalkeeper in all competitions of the season.
        /// </summary>
        [BsonElement("cleanSheets")]
        public int CleanSheets { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of minutes per goal scored in all competitions of the season.
        /// </summary>
        [BsonElement("minutesPerGoal")]
        public int? MinutesPerGoal { get; set; }

        /// <summary>
        /// Gets or sets the competition stats for the player in the season.
        /// </summary>
        [BsonElement("playerSeasonCompetitionStats")]
        public IEnumerable<PlayerSeasonCompetitionStat>? PlayerSeasonCompetitionStats { get; set; }
    }
}
