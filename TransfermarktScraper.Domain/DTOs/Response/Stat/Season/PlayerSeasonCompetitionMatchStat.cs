using System;
using TransfermarktScraper.Domain.Enums;

namespace TransfermarktScraper.Domain.DTOs.Response.Stat.Season
{
    /// <summary>
    /// Represents the response DTO for a player season competition match stat.
    /// </summary>
    public class PlayerSeasonCompetitionMatchStat
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
        required public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets a naming for the match.
        /// </summary>
        required public string Matchday { get; set; }

        /// <summary>
        /// Gets or sets the home club name.
        /// </summary>
        required public string HomeClubName { get; set; }

        /// <summary>
        /// Gets or sets the away club name.
        /// </summary>
        required public string AwayClubName { get; set; }

        /// <summary>
        /// Gets or sets the link of the match.
        /// </summary>
        required public string Link { get; set; }

        /// <summary>
        /// Gets or sets the home club scored goals in the match.
        /// </summary>
        public int HomeClubGoals { get; set; }

        /// <summary>
        /// Gets or sets the away club scored goals in the match.
        /// </summary>
        public int AwayClubGoals { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player club won the match.
        /// </summary>
        public bool IsWon { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the match was decided in the addition time.
        /// </summary>
        public bool IsResultAddition { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the match was decided in the penalties.
        /// </summary>
        public bool IsResultPenalties { get; set; }

        /// <summary>
        /// Gets or sets the position of the player during the match.
        /// </summary>
        public Position Position { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player was the captain during the match.
        /// </summary>
        public bool IsCaptain { get; set; }

        /// <summary>
        /// Gets or sets the number of goals the player scored.
        /// </summary>
        public int Goals { get; set; }

        /// <summary>
        /// Gets or sets the number of assists the player performed.
        /// </summary>
        public int Assists { get; set; }

        /// <summary>
        /// Gets or sets the number of own goals the player scored.
        /// </summary>
        public int OwnGoals { get; set; }

        /// <summary>
        /// Gets or sets the number of yellow cards the player received.
        /// </summary>
        public int YellowCards { get; set; }

        /// <summary>
        /// Gets or sets the number of second yellow cards the player received.
        /// </summary>
        public int SecondYellowCards { get; set; }

        /// <summary>
        /// Gets or sets the number of red cards the player received.
        /// </summary>
        public int RedCards { get; set; }

        /// <summary>
        /// Gets or sets the minute when the player was substituted on.
        /// </summary>
        public int SubstitutedOn { get; set; }

        /// <summary>
        /// Gets or sets the minute when the player was substituted off.
        /// </summary>
        public int SubstitutedOff { get; set; }

        /// <summary>
        /// Gets or sets the number of minutes the player was on the field.
        /// </summary>
        public int? MinutesPlayed { get; set; }
    }
}
