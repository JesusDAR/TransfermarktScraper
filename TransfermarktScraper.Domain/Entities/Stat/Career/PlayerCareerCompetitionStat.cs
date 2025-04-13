using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TransfermarktScraper.Domain.Utils;

namespace TransfermarktScraper.Domain.Entities.Stat.Career
{
    /// <summary>
    /// Represents a player career competition stat entity.
    /// </summary>
    public class PlayerCareerCompetitionStat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerCareerCompetitionStat"/> class.
        /// </summary>
        /// <param name="playerTransfermarktId">The unique player Transfermarkt identifier.</param>
        /// <param name="competitionTransfermarktId">The unique competition Transfermarkt identifier.</param>
        public PlayerCareerCompetitionStat(string playerTransfermarktId, string competitionTransfermarktId)
        {
            if (string.IsNullOrEmpty(playerTransfermarktId))
            {
                throw new ArgumentException($"{nameof(PlayerTransfermarktId)} cannot be null or empty", nameof(playerTransfermarktId));
            }

            if (string.IsNullOrEmpty(competitionTransfermarktId))
            {
                throw new ArgumentException($"{nameof(CompetitionTransfermarktId)} cannot be null or empty", nameof(competitionTransfermarktId));
            }

            PlayerTransfermarktId = playerTransfermarktId;
            CompetitionTransfermarktId = competitionTransfermarktId;
            TransfermarktId = EntityUtils.GetHash($"{playerTransfermarktId}|{competitionTransfermarktId}");
            UpdateDate = DateTime.UtcNow;
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
        /// Gets the unique competition Transfermarkt identifier.
        /// </summary>
        [BsonRequired]
        [BsonElement("competitionTransfermarktId")]
        public string CompetitionTransfermarktId { get; private set; }

        /// <summary>
        /// Gets or sets the last update date.
        /// </summary>
        [BsonElement("updateDate")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// Gets or sets the competition name.
        /// </summary>
        [BsonElement("competitionName")]
        required public string CompetitionName { get; set; }

        /// <summary>
        /// Gets or sets the number of appearances in the competition for the whole player career.
        /// </summary>
        [BsonElement("appearances")]
        public int Appearances { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of goals in the competition for the whole player career.
        /// </summary>
        [BsonElement("goals")]
        public int Goals { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of assists in the competition for the whole player career.
        /// </summary>
        [BsonElement("assists")]
        public int Assists { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of own goals in the competition for the whole player career.
        /// </summary>
        [BsonElement("ownGoals")]
        public int OwnGoals { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of times the player was substituted in the competition for the whole the player career.
        /// </summary>
        [BsonElement("substitutedIn")]
        public int SubstitutedIn { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of times the player was substituted off in the competition for the whole the player career.
        /// </summary>
        [BsonElement("substitutedOff")]
        public int SubstitutedOff { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of yellow cards received in the competition for the whole the player career.
        /// </summary>
        [BsonElement("yellowCards")]
        public int YellowCards { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of second yellow cards received in the competition for the whole the player career.
        /// </summary>
        [BsonElement("secondYellowCards")]
        public int SecondYellowCards { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of red cards received in the competition for the whole the player career.
        /// </summary>
        [BsonElement("redCards")]
        public int RedCards { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of penalty goals scored in the competition for the whole the player career.
        /// </summary>
        [BsonElement("penaltyGoals")]
        public int PenaltyGoals { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of minutes per goal scored in the competition for the whole the player career.
        /// </summary>
        [BsonElement("minutesPerGoal")]
        public int MinutesPerGoal { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of minutes played in the competition for the whole the player career.
        /// </summary>
        [BsonElement("minutesPlayed")]
        public int MinutesPlayed { get; set; } = 0;
    }
}
