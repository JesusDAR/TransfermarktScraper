using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TransfermarktScraper.Domain.Utils;

namespace TransfermarktScraper.Domain.Entities
{
    /// <summary>
    /// Represents a player season competition stat entity.
    /// </summary>
    public class PlayerSeasonCompetitionStat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerSeasonCompetitionStat"/> class.
        /// </summary>
        /// <param name="playerTransfermarktId">The unique player Transfermarkt identifier.</param>
        /// <param name="competitionTransfermarktId">The unique competition Transfermarkt identifier.</param>
        /// <param name="seasonTransfermarktId">The unique season Transfermarkt identifier.</param>
        public PlayerSeasonCompetitionStat(string playerTransfermarktId, string competitionTransfermarktId, string seasonTransfermarktId)
        {
            if (string.IsNullOrEmpty(playerTransfermarktId))
            {
                throw new ArgumentException($"{nameof(PlayerTransfermarktId)} cannot be null or empty", nameof(playerTransfermarktId));
            }

            if (string.IsNullOrEmpty(seasonTransfermarktId))
            {
                throw new ArgumentException($"{nameof(SeasonTransfermarktId)} cannot be null or empty", nameof(seasonTransfermarktId));
            }

            if (string.IsNullOrEmpty(competitionTransfermarktId))
            {
                throw new ArgumentException($"{nameof(CompetitionTransfermarktId)} cannot be null or empty", nameof(competitionTransfermarktId));
            }

            PlayerTransfermarktId = playerTransfermarktId;
            SeasonTransfermarktId = seasonTransfermarktId;
            CompetitionTransfermarktId = competitionTransfermarktId;
            TransfermarktId = EntityUtils.GetHash($"{playerTransfermarktId}|{seasonTransfermarktId}|{competitionTransfermarktId}");
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
        /// Gets the unique season Transfermarkt identifier.
        /// </summary>
        [BsonRequired]
        [BsonElement("seasonTransfermarktId")]
        public string SeasonTransfermarktId { get; private set; }

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
        /// Gets or sets the number of goals.
        /// </summary>
        [BsonElement("goals")]
        public int Goals { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of assists.
        /// </summary>
        [BsonElement("assists")]
        public int Assists { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of own goals in the competition.
        /// </summary>
        [BsonElement("ownGoals")]
        public int OwnGoals { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of yellow cards received in the competition.
        /// </summary>
        [BsonElement("yellowCards")]
        public int YellowCards { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of second yellow cards received in the competition.
        /// </summary>
        [BsonElement("secondYellowCards")]
        public int SecondYellowCards { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of red cards receive in the competition.
        /// </summary>
        [BsonElement("redCards")]
        public int RedCards { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of penalty goals scored in the competition.
        /// </summary>
        [BsonElement("penaltyGoals")]
        public int PenaltyGoals { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of minutes per goal scored in the competition.
        /// </summary>
        [BsonElement("minutesPerGoal")]
        public int? MinutesPerGoal { get; set; }

        /// <summary>
        /// Gets or sets the number of minutes played in the competition.
        /// </summary>
        [BsonElement("minutesPlayed")]
        public int? MinutesPlayed { get; set; }

        /// <summary>
        /// Gets or sets the number of times the player was in the squad in the competition.
        /// </summary>
        [BsonElement("squad")]
        public int Squad { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of times the player was in the starting eleven in the competition.
        /// </summary>
        [BsonElement("startingEleven")]
        public int StartingEleven { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of times the player was substituted in the competition.
        /// </summary>
        [BsonElement("substitutedIn")]
        public int SubstitutedIn { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of times the player was substituted off in the competition.
        /// </summary>
        [BsonElement("substitutedOff")]
        public int SubstitutedOff { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of times the player was on the bench the whole match in the competition.
        /// </summary>
        [BsonElement("onTheBench")]
        public int OnTheBench { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of matches the player missed because of a suspension in the competition.
        /// </summary>
        [BsonElement("suspended")]
        public int Suspended { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of matches the player missed because of an injure in the competition.
        /// </summary>
        [BsonElement("injured")]
        public int Injured { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of matches the player missed because of an absence in the competition.
        /// </summary>
        [BsonElement("absence")]
        public int Absence { get; set; } = 0;

        /// <summary>
        /// Gets or sets the player match stats.
        /// </summary>
        [BsonElement("playerMatchStats")]
        public IEnumerable<PlayerMatchStat>? PlayerMatchStats { get; set; }
    }
}
