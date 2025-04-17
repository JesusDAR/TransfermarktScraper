namespace TransfermarktScraper.Domain.DTOs.Response.Stat.Career
{
    /// <summary>
    /// Represents the response DTO for a player career competition stat.
    /// </summary>
    public class PlayerCareerCompetitionStat
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
        /// Gets or sets the unique competition Transfermarkt identifier.
        /// </summary>
        required public string CompetitionTransfermarktId { get; set; }

        /// <summary>
        /// Gets or sets the competition name.
        /// </summary>
        required public string CompetitionName { get; set; }

        /// <summary>
        /// Gets or sets the number of appearances in the competition for the whole player career.
        /// </summary>
        public int Appearances { get; set; }

        /// <summary>
        /// Gets or sets the number of goals in the competition for the whole player career.
        /// </summary>
        public int Goals { get; set; }

        /// <summary>
        /// Gets or sets the number of assists in the competition for the whole player career.
        /// </summary>
        public int Assists { get; set; }

        /// <summary>
        /// Gets or sets the number of own goals in the competition for the whole player career.
        /// </summary>
        public int OwnGoals { get; set; }

        /// <summary>
        /// Gets or sets the number of times the player was substituted on in the competition for the whole player career.
        /// </summary>
        public int SubstitutionsOn { get; set; }

        /// <summary>
        /// Gets or sets the number of times the player was substituted off in the competition for the whole player career.
        /// </summary>
        public int SubstitutionsOff { get; set; }

        /// <summary>
        /// Gets or sets the number of yellow cards received in the competition for the whole player career.
        /// </summary>
        public int YellowCards { get; set; }

        /// <summary>
        /// Gets or sets the number of second yellow cards received in the competition for the whole player career.
        /// </summary>
        public int SecondYellowCards { get; set; }

        /// <summary>
        /// Gets or sets the number of red cards received in the competition for the whole player career.
        /// </summary>
        public int RedCards { get; set; }

        /// <summary>
        /// Gets or sets the number of penalty goals scored in the competition for the whole player career.
        /// </summary>
        public int PenaltyGoals { get; set; }

        /// <summary>
        /// Gets or sets the number of minutes per goal scored in the competition for the whole player career.
        /// </summary>
        public int MinutesPerGoal { get; set; }

        /// <summary>
        /// Gets or sets the number of minutes played in the competition for the whole the player career.
        /// </summary>
        public int MinutesPlayed { get; set; }
    }
}
