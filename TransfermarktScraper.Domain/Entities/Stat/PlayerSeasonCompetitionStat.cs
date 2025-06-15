using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TransfermarktScraper.Domain.Utils.Entity;

namespace TransfermarktScraper.Domain.Entities.Stat
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
                throw new ArgumentException($"{nameof(PlayerTransfermarktId)} cannot be null or empty");
            }

            if (string.IsNullOrEmpty(seasonTransfermarktId))
            {
                throw new ArgumentException($"{nameof(SeasonTransfermarktId)} cannot be null or empty");
            }

            if (string.IsNullOrEmpty(competitionTransfermarktId))
            {
                throw new ArgumentException($"{nameof(CompetitionTransfermarktId)} cannot be null or empty");
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
        /// Gets or sets the competition link.
        /// </summary>
        [BsonElement("competitionLink")]
        required public string CompetitionLink { get; set; }

        /// <summary>
        /// Gets or sets the competition logo.
        /// </summary>
        [BsonElement("competitionLogo")]
        public string? CompetitionLogo { get; set; }

        /// <summary>
        /// Gets or sets the number of appearances in the competition for the season.
        /// </summary>
        [BsonElement("appearances")]
        public int? Appearances { get; set; }

        /// <summary>
        /// Gets or sets the number of goals in the competition for the season.
        /// </summary>
        [BsonElement("goals")]
        public int? Goals { get; set; }

        /// <summary>
        /// Gets or sets the number of assists in the competition for the season.
        /// </summary>
        [BsonElement("assists")]
        public int? Assists { get; set; }

        /// <summary>
        /// Gets or sets the number of own goals in the competition for the season.
        /// </summary>
        [BsonElement("ownGoals")]
        public int? OwnGoals { get; set; }

        /// <summary>
        /// Gets or sets the number of times the player was substituted on in the competition for the season.
        /// </summary>
        [BsonElement("substitutionsOn")]
        public int? SubstitutionsOn { get; set; }

        /// <summary>
        /// Gets or sets the number of times the player was substituted off in the competition for the season.
        /// </summary>
        [BsonElement("substitutionsOff")]
        public int? SubstitutionsOff { get; set; }

        /// <summary>
        /// Gets or sets the number of yellow cards received in the competition for the season.
        /// </summary>
        [BsonElement("yellowCards")]
        public int? YellowCards { get; set; }

        /// <summary>
        /// Gets or sets the number of second yellow cards received in the competition for the season.
        /// </summary>
        [BsonElement("secondYellowCards")]
        public int? SecondYellowCards { get; set; }

        /// <summary>
        /// Gets or sets the number of red cards received in the competition for the season.
        /// </summary>
        [BsonElement("redCards")]
        public int? RedCards { get; set; }

        /// <summary>
        /// Gets or sets the number of penalty goals scored in the competition for the season.
        /// </summary>
        [BsonElement("penaltyGoals")]
        public int? PenaltyGoals { get; set; }

        /// <summary>
        /// Gets or sets the number of goals that the goalkeeper has conceded in the competition for the season.
        /// </summary>
        [BsonElement("goalsConceded")]
        public int? GoalsConceded { get; set; }

        /// <summary>
        /// Gets or sets the number of clean sheets for the goalkeeper in the competition for the season.
        /// </summary>
        [BsonElement("cleanSheets")]
        public int? CleanSheets { get; set; }

        /// <summary>
        /// Gets or sets the number of minutes per goal scored in the competition for the season.
        /// </summary>
        [BsonElement("minutesPerGoal")]
        public int? MinutesPerGoal { get; set; }

        /// <summary>
        /// Gets or sets the number of minutes played in the competition for the season.
        /// </summary>
        [BsonElement("minutesPlayed")]
        public int? MinutesPlayed { get; set; }

        /// <summary>
        /// Gets or sets the number of times the player was in the squad in the competition for the season.
        /// </summary>
        [BsonElement("squad")]
        public int? Squad { get; set; }

        /// <summary>
        /// Gets or sets the number of times the player was in the starting eleven in the competition for the season.
        /// </summary>
        [BsonElement("startingEleven")]
        public int? StartingEleven { get; set; }

        /// <summary>
        /// Gets or sets the number of times the player was on the bench the whole match in the competition for the season.
        /// </summary>
        [BsonElement("onTheBench")]
        public int? OnTheBench { get; set; }

        /// <summary>
        /// Gets or sets the number of matches the player missed because of a suspension in the competition for the season.
        /// </summary>
        [BsonElement("suspended")]
        public int? Suspended { get; set; }

        /// <summary>
        /// Gets or sets the number of matches the player missed because of an injure in the competition for the season.
        /// </summary>
        [BsonElement("injured")]
        public int? Injured { get; set; }

        /// <summary>
        /// Gets or sets the player match stats in the competition for the season.
        /// </summary>
        [BsonElement("playerSeasonCompetitionMatchStats")]
        public IList<PlayerSeasonCompetitionMatchStat> PlayerSeasonCompetitionMatchStats { get; set; } = new List<PlayerSeasonCompetitionMatchStat>();
    }
}
