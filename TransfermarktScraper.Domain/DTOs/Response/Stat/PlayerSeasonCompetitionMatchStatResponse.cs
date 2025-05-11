using System;
using TransfermarktScraper.Domain.Enums;
using TransfermarktScraper.Domain.Enums.Extensions;

namespace TransfermarktScraper.Domain.DTOs.Response.Stat
{
    /// <summary>
    /// Represents the response DTO for a player season competition match stat.
    /// </summary>
    public class PlayerSeasonCompetitionMatchStatResponse
    {
        /// <summary>
        /// Gets or sets the unique composite identifier.
        /// </summary>
        required public string TransfermarktId { get; set; }

        /// <summary>
        /// Gets or sets the unique player Transfermarkt identifier.
        /// </summary>
        required public string PlayerTransfermarktId { get; set; }

        /// <summary>
        /// Gets or sets the unique home club Transfermarkt identifier.
        /// </summary>
        required public string HomeClubTransfermarktId { get; set; }

        /// <summary>
        /// Gets or sets the unique away club Transfermarkt identifier.
        /// </summary>
        required public string AwayClubTransfermarktId { get; set; }

        /// <summary>
        /// Gets or sets the date of the match.
        /// </summary>
        required public string Date { get; set; }

        /// <summary>
        /// Gets or sets a naming for the match.
        /// </summary>
        required public string MatchDay { get; set; }

        /// <summary>
        /// Gets or sets the link of the match.
        /// </summary>
        required public string Link { get; set; }

        /// <summary>
        /// Gets or sets the home club name.
        /// </summary>
        required public string HomeClubName { get; set; }

        /// <summary>
        /// Gets or sets the home club link.
        /// </summary>
        required public string HomeClubLink { get; set; }

        /// <summary>
        /// Gets or sets the home club logo.
        /// </summary>
        required public string HomeClubLogo { get; set; }

        /// <summary>
        /// Gets or sets the away club name.
        /// </summary>
        required public string AwayClubName { get; set; }

        /// <summary>
        /// Gets or sets the away club link.
        /// </summary>
        required public string AwayClubLink { get; set; }

        /// <summary>
        /// Gets or sets the away club logo.
        /// </summary>
        required public string AwayClubLogo { get; set; }

        /// <summary>
        /// Gets or sets the home club scored goals in the match.
        /// </summary>
        public int? HomeClubGoals { get; set; }

        /// <summary>
        /// Gets or sets the away club scored goals in the match.
        /// </summary>
        public int? AwayClubGoals { get; set; }

        /// <summary>
        /// Gets or sets the result of the match for the club.
        /// </summary>
        public MatchResult MatchResult { get; set; } = MatchResult.Unknown;

        /// <summary>
        /// Gets or sets the link of the result of the match for the club.
        /// </summary>
        public string? MatchResultLink { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the match was decided in the addition time.
        /// </summary>
        public bool IsResultAddition { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the match was decided in the penalties.
        /// </summary>
        public bool IsResultPenalties { get; set; } = false;

        /// <summary>
        /// Gets or sets the position of the player during the match.
        /// </summary>
        public string Position { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the player was the captain during the match.
        /// </summary>
        public bool IsCaptain { get; set; } = false;

        /// <summary>
        /// Gets or sets the number of goals the player scored.
        /// </summary>
        public int? Goals { get; set; }

        /// <summary>
        /// Gets or sets the number of assists the player performed.
        /// </summary>
        public int? Assists { get; set; }

        /// <summary>
        /// Gets or sets the number of own goals the player scored.
        /// </summary>
        public int? OwnGoals { get; set; }

        /// <summary>
        /// Gets or sets the time in minutes when the player received a yellow card during the match.
        /// </summary>
        public int? YellowCard { get; set; }

        /// <summary>
        /// Gets or sets the time in minutes when the player received a second yellow card during the match.
        /// </summary>
        public int? SecondYellowCard { get; set; }

        /// <summary>
        /// Gets or sets the time in minutes when the player received a red card during the match.
        /// </summary>
        public int? RedCard { get; set; }

        /// <summary>
        /// Gets or sets the minute when the player was substituted on.
        /// </summary>
        public int? SubstitutedOn { get; set; }

        /// <summary>
        /// Gets or sets the minute when the player was substituted off.
        /// </summary>
        public int? SubstitutedOff { get; set; }

        /// <summary>
        /// Gets or sets the number of minutes the player was on the field.
        /// </summary>
        public int? MinutesPlayed { get; set; }

        /// <summary>
        /// Gets or sets the reason why the player did not played any minute of the match.
        /// </summary>
        public string NotPlayingReason { get; set; } = NotPlayingReasonExtension.ToString(Enums.NotPlayingReason.None);
    }
}
