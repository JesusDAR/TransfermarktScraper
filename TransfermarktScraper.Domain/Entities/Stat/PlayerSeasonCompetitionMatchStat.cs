using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TransfermarktScraper.Domain.Enums;
using TransfermarktScraper.Domain.Utils;

namespace TransfermarktScraper.Domain.Entities.Stat
{
    /// <summary>
    /// Represents the player season competition match stat entity.
    /// </summary>
    public class PlayerSeasonCompetitionMatchStat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerSeasonCompetitionMatchStat"/> class.
        /// </summary>
        /// <param name="playerTransfermarktId">The unique player Transfermarkt identifier.</param>
        /// <param name="homeClubTransfermarktId">The unique home club Transfermarkt identifier.</param>
        /// <param name="awayClubTransfermarktId">The unique away club Transfermarkt identifier.</param>
        /// <param name="date">The date of the match.</param>
        public PlayerSeasonCompetitionMatchStat(string playerTransfermarktId, string homeClubTransfermarktId, string awayClubTransfermarktId, DateTime date)
        {
            if (string.IsNullOrEmpty(playerTransfermarktId))
            {
                throw new ArgumentException($"{nameof(PlayerTransfermarktId)} cannot be null or empty");
            }

            if (string.IsNullOrEmpty(homeClubTransfermarktId))
            {
                throw new ArgumentException($"{nameof(HomeClubTransfermarktId)} cannot be null or empty");
            }

            if (string.IsNullOrEmpty(awayClubTransfermarktId))
            {
                throw new ArgumentException($"{nameof(AwayClubTransfermarktId)} cannot be null or empty");
            }

            if (date == default)
            {
                throw new ArgumentException($"{nameof(Date)} cannot be the default value");
            }

            PlayerTransfermarktId = playerTransfermarktId;
            HomeClubTransfermarktId = homeClubTransfermarktId;
            AwayClubTransfermarktId = awayClubTransfermarktId;
            Date = date;
            TransfermarktId = EntityUtils.GetHash($"{playerTransfermarktId}|{homeClubTransfermarktId}|{awayClubTransfermarktId}|{date:yyyyMMdd}");
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
        /// Gets the unique home club Transfermarkt identifier.
        /// </summary>
        [BsonRequired]
        [BsonElement("homeClubTransfermarktId")]
        public string HomeClubTransfermarktId { get; private set; }

        /// <summary>
        /// Gets the unique away club Transfermarkt identifier.
        /// </summary>
        [BsonRequired]
        [BsonElement("awayClubTransfermarktId")]
        public string AwayClubTransfermarktId { get; private set; }

        /// <summary>
        /// Gets the date of the match.
        /// </summary>
        [BsonRequired]
        [BsonElement("date")]
        [BsonDateTimeOptions(DateOnly = true)]
        public DateTime Date { get; private set; }

        /// <summary>
        /// Gets or sets the last update date.
        /// </summary>
        [BsonElement("updateDate")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// Gets or sets a naming for the match.
        /// </summary>
        [BsonElement("matchDay")]
        required public string MatchDay { get; set; }

        /// <summary>
        /// Gets or sets the link of the match.
        /// </summary>
        [BsonElement("link")]
        required public string Link { get; set; }

        /// <summary>
        /// Gets or sets the home club name.
        /// </summary>
        [BsonElement("homeClubName")]
        required public string HomeClubName { get; set; }

        /// <summary>
        /// Gets or sets the home club logo.
        /// </summary>
        [BsonElement("homeClubLogo")]
        required public string HomeClubLogo { get; set; }

        /// <summary>
        /// Gets or sets the away club name.
        /// </summary>
        [BsonElement("awayClubName")]
        required public string AwayClubName { get; set; }

        /// <summary>
        /// Gets or sets the away club logo.
        /// </summary>
        [BsonElement("awayClubLogo")]
        required public string AwayClubLogo { get; set; }

        /// <summary>
        /// Gets or sets the home club scored goals in the match.
        /// </summary>
        [BsonElement("homeClubGoals")]
        public int HomeClubGoals { get; set; } = 0;

        /// <summary>
        /// Gets or sets the away club scored goals in the match.
        /// </summary>
        [BsonElement("awayClubGoals")]
        public int AwayClubGoals { get; set; } = 0;

        /// <summary>
        /// Gets or sets the result of the match.
        /// </summary>
        [BsonElement("matchResult")]
        public MatchResult MatchResult { get; set; } = MatchResult.Unknown;

        /// <summary>
        /// Gets or sets a value indicating whether the match was decided in the addition time.
        /// </summary>
        [BsonElement("isResultAddition")]
        public bool IsResultAddition { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the match was decided in the penalties.
        /// </summary>
        [BsonElement("isResultPenalties")]
        public bool IsResultPenalties { get; set; } = false;

        /// <summary>
        /// Gets or sets the position of the player during the match.
        /// </summary>
        [BsonElement("position")]
        public Position Position { get; set; } = Position.Unknown;

        /// <summary>
        /// Gets or sets a value indicating whether the player was the captain during the match.
        /// </summary>
        [BsonElement("isCaptain")]
        public bool IsCaptain { get; set; } = false;

        /// <summary>
        /// Gets or sets the number of goals the player scored.
        /// </summary>
        [BsonElement("goals")]
        public int Goals { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of assists the player performed.
        /// </summary>
        [BsonElement("assists")]
        public int Assists { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of own goals the player scored.
        /// </summary>
        [BsonElement("ownGoals")]
        public int OwnGoals { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of yellow cards the player received.
        /// </summary>
        [BsonElement("yellowCards")]
        public int YellowCards { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of second yellow cards the player received.
        /// </summary>
        [BsonElement("secondYellowCards")]
        public int SecondYellowCards { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of red cards the player received.
        /// </summary>
        [BsonElement("redCards")]
        public int RedCards { get; set; } = 0;

        /// <summary>
        /// Gets or sets the minute when the player was substituted on.
        /// </summary>
        [BsonElement("substitutedOn")]
        public int? SubstitutedOn { get; set; }

        /// <summary>
        /// Gets or sets the minute when the player was substituted off.
        /// </summary>
        [BsonElement("substitutedOff")]
        public int? SubstitutedOff { get; set; }

        /// <summary>
        /// Gets or sets the number of minutes the player was on the field.
        /// </summary>
        [BsonElement("minutesPlayed")]
        public int? MinutesPlayed { get; set; }

        /// <summary>
        /// Gets or sets the reason why the player did not played any minute of the match.
        /// </summary>
        [BsonElement("notPlayingReason")]
        public NotPlayingReason NotPlayingReason { get; set; } = NotPlayingReason.None;
    }
}
