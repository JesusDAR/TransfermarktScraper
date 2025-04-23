using System.Collections.Generic;

namespace TransfermarktScraper.Domain.DTOs.Response.Stat
{
    /// <summary>
    /// Represents the response DTO for a player season stat.
    /// </summary>
    public class PlayerSeasonStat
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
        /// Gets or sets the unique season Transfermarkt identifier.
        /// </summary>
        required public string SeasonTransfermarktId { get; set; }

        /// <summary>
        /// Gets or sets the number of appearances in all competitions of the season.
        /// </summary>
        public int Appearances { get; set; }

        /// <summary>
        /// Gets or sets the number of goals in all competitions of the season.
        /// </summary>
        public int Goals { get; set; }

        /// <summary>
        /// Gets or sets the number of assists in all competitions of the season.
        /// </summary>
        public int Assists { get; set; }

        /// <summary>
        /// Gets or sets the number of own goals in all competitions of the season.
        /// </summary>
        public int OwnGoals { get; set; }

        /// <summary>
        /// Gets or sets the number of yellow cards received in all competitions of the season.
        /// </summary>
        public int YellowCards { get; set; }

        /// <summary>
        /// Gets or sets the number of second yellow cards received in all competitions of the season.
        /// </summary>
        public int SecondYellowCards { get; set; }

        /// <summary>
        /// Gets or sets the number of red cards received in all competitions of the season.
        /// </summary>
        public int RedCards { get; set; }

        /// <summary>
        /// Gets or sets the number of penalty goals scored in all competitions of the season.
        /// </summary>
        public int PenaltyGoals { get; set; }

        /// <summary>
        /// Gets or sets the number of goals that the goalkeeper has conceded in all competitions of the season.
        /// </summary>
        public int GoalsConceded { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of clean sheets for the goalkeeper in all competitions of the season.
        /// </summary>
        public int CleanSheets { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of minutes per goal scored in all competitions of the season.
        /// </summary>
        public int MinutesPerGoal { get; set; }

        /// <summary>
        /// Gets or sets the number of minutes the player was on the field in all competitions of the season.
        /// </summary>
        public int MinutesPlayed { get; set; }

        /// <summary>
        /// Gets or sets the competition stats for the player in the season.
        /// </summary>
        public IEnumerable<PlayerSeasonCompetitionStat>? PlayerSeasonCompetitionStats { get; set; }
    }
}
