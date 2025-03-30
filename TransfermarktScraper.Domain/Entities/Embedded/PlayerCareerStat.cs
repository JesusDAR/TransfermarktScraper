using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TransfermarktScraper.Domain.Entities.Embedded
{
    /// <summary>
    /// Represents a player career stat embedded entity.
    /// This entity is always embedded in PlayerStat.
    /// </summary>
    public class PlayerCareerStat
    {
        /// <summary>
        /// Gets or sets the number of appearances in the player career.
        /// </summary>
        [BsonElement("appearances")]
        public int Appearances { get; set; }

        /// <summary>
        /// Gets or sets the number of goals in the player career.
        /// </summary>
        [BsonElement("goals")]
        public int Goals { get; set; }

        /// <summary>
        /// Gets or sets the number of assists in the player career.
        /// </summary>
        [BsonElement("assists")]
        public int Assists { get; set; }

        /// <summary>
        /// Gets or sets the number of own goals in the player career.
        /// </summary>
        [BsonElement("ownGoals")]
        public int OwnGoals { get; set; }

        /// <summary>
        /// Gets or sets the number of times the player was substituted in the player career.
        /// </summary>
        [BsonElement("substitutedIn")]
        public int SubstitutedIn { get; set; }

        /// <summary>
        /// Gets or sets the number of times the player was substituted off in the player career.
        /// </summary>
        [BsonElement("substitutedOff")]
        public int SubstitutedOff { get; set; }

        /// <summary>
        /// Gets or sets the number of yellow cards received in the player career.
        /// </summary>
        [BsonElement("yellowCards")]
        public int YellowCards { get; set; }

        /// <summary>
        /// Gets or sets the number of second yellow cards received in the player career.
        /// </summary>
        [BsonElement("secondYellowCards")]
        public int SecondYellowCards { get; set; }

        /// <summary>
        /// Gets or sets the number of red cards received in the player career.
        /// </summary>
        [BsonElement("redCards")]
        public int RedCards { get; set; }

        /// <summary>
        /// Gets or sets the number of penalty goals scored in the player career.
        /// </summary>
        [BsonElement("penaltyGoals")]
        public int PenaltyGoals { get; set; }

        /// <summary>
        /// Gets or sets the number of minutes per goal scored in the player career.
        /// </summary>
        [BsonElement("minutesPerGoal")]
        public int MinutesPerGoal { get; set; }

        /// <summary>
        /// Gets or sets the number of minutes played in the player career.
        /// </summary>
        [BsonElement("minutesPlayed")]
        public int MinutesPlayed { get; set; }

        /// <summary>
        /// Gets or sets the player stats per competition.
        /// </summary>
        [BsonElement("playerCompetitionStats")]
        public IEnumerable<PlayerSeasonCompetitionStat>? PlayerCompetitionStats { get; set; }
    }
}
