using System.Collections.Generic;

namespace TransfermarktScraper.Domain.DTOs.Response.Stat.Career
{
    /// <summary>
    /// Represents the response DTO for a player career stat.
    /// </summary>
    public class PlayerCareerStat
    {
        /// <summary>
        /// Gets or sets the number of appearances in the player career.
        /// </summary>
        public int Appearances { get; set; }

        /// <summary>
        /// Gets or sets the number of goals in the player career.
        /// </summary>
        public int Goals { get; set; }

        /// <summary>
        /// Gets or sets the number of assists in the player career.
        /// </summary>
        public int Assists { get; set; }

        /// <summary>
        /// Gets or sets the number of own goals in the player career.
        /// </summary>
        public int OwnGoals { get; set; }

        /// <summary>
        /// Gets or sets the number of times the player was substituted on in the player career.
        /// </summary>
        public int SubstitutionsOn { get; set; }

        /// <summary>
        /// Gets or sets the number of times the player was substituted off in the player career.
        /// </summary>
        public int SubstitutionsOff { get; set; }

        /// <summary>
        /// Gets or sets the number of yellow cards received in the player career.
        /// </summary>
        public int SecondYellowCards { get; set; }

        /// <summary>
        /// Gets or sets the number of red cards received in the player career.
        /// </summary>
        public int RedCards { get; set; }

        /// <summary>
        /// Gets or sets the number of penalty goals scored in the player career.
        /// </summary>
        public int PenaltyGoals { get; set; }

        /// <summary>
        /// Gets or sets the number of goals that the goalkeeper has conceded in the player career.
        /// </summary>
        public int GoalsConceded { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of clean sheets for the goalkeeper in the player career.
        /// </summary>
        public int CleanSheets { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of minutes per goal scored in the player career.
        /// </summary>
        public int MinutesPerGoal { get; set; }

        /// <summary>
        /// Gets or sets the number of minutes played in the player career.
        /// </summary>
        public int MinutesPlayed { get; set; }

        /// <summary>
        /// Gets or sets the player career stats per competition.
        /// </summary>
        public IEnumerable<PlayerCareerCompetitionStat>? PlayerCareerCompetitionStats { get; set; }
    }
}
