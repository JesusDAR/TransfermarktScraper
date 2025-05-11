using System.Collections.Generic;
using System.Linq;

namespace TransfermarktScraper.Domain.DTOs.Response.Stat
{
    /// <summary>
    /// Represents the response DTO for a player season competition stat.
    /// </summary>
    public class PlayerSeasonCompetitionStatResponse
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
        /// Gets or sets the unique competition Transfermarkt identifier.
        /// </summary>
        required public string CompetitionTransfermarktId { get; set; }

        /// <summary>
        /// Gets or sets the competition name.
        /// </summary>
        required public string CompetitionName { get; set; }

        /// <summary>
        /// Gets or sets the competition link.
        /// </summary>
        required public string CompetitionLink { get; set; }

        /// <summary>
        /// Gets or sets the competition logo.
        /// </summary>
        required public string CompetitionLogo { get; set; }

        /// <summary>
        /// Gets or sets the number of appearances in the competition for the season.
        /// </summary>
        public int? Appearances { get; set; }

        /// <summary>
        /// Gets or sets the number of goals in the competition for the season.
        /// </summary>
        public int? Goals { get; set; }

        /// <summary>
        /// Gets or sets the number of assists in the competition for the season.
        /// </summary>
        public int? Assists { get; set; }

        /// <summary>
        /// Gets or sets the number of own goals in the competition for the season.
        /// </summary>
        public int? OwnGoals { get; set; }

        /// <summary>
        /// Gets or sets the number of times the player was substituted on in the competition for the season.
        /// </summary>
        public int? SubstitutionsOn { get; set; }

        /// <summary>
        /// Gets or sets the number of times the player was substituted off in the competition for the season.
        /// </summary>
        public int? SubstitutionsOff { get; set; }

        /// <summary>
        /// Gets or sets the number of yellow cards received in the competition for the season.
        /// </summary>
        public int? YellowCards { get; set; }

        /// <summary>
        /// Gets or sets the number of second yellow cards received in the competition for the season.
        /// </summary>
        public int? SecondYellowCards { get; set; }

        /// <summary>
        /// Gets or sets the number of red cards received in the competition for the season.
        /// </summary>
        public int? RedCards { get; set; }

        /// <summary>
        /// Gets or sets the number of penalty goals scored in the competition for the season.
        /// </summary>
        public int? PenaltyGoals { get; set; }

        /// <summary>
        /// Gets or sets the number of goals that the goalkeeper has conceded in the competition for the season.
        /// </summary>
        public int? GoalsConceded { get; set; }

        /// <summary>
        /// Gets or sets the number of clean sheets for the goalkeeper in the competition for the season.
        /// </summary>
        public int? CleanSheets { get; set; }

        /// <summary>
        /// Gets or sets the number of minutes per goal scored in the competition for the season.
        /// </summary>
        public int? MinutesPerGoal { get; set; }

        /// <summary>
        /// Gets or sets the number of minutes played in the competition for the season.
        /// </summary>
        public int? MinutesPlayed { get; set; }

        /// <summary>
        /// Gets or sets the number of times the player was in the squad in the competition for the season.
        /// </summary>
        public int? Squad { get; set; }

        /// <summary>
        /// Gets or sets the number of times the player was in the starting eleven in the competition for the season.
        /// </summary>
        public int? StartingEleven { get; set; }

        /// <summary>
        /// Gets or sets the number of times the player was on the bench the whole match in the competition for the season.
        /// </summary>
        public int? OnTheBench { get; set; }

        /// <summary>
        /// Gets or sets the number of matches the player missed because of a suspension in the competition for the season.
        /// </summary>
        public int? Suspended { get; set; }

        /// <summary>
        /// Gets or sets the number of matches the player missed because of an injure in the competition for the season.
        /// </summary>
        public int? Injured { get; set; }

        /// <summary>
        /// Gets or sets the player match stats in the competition for the season.
        /// </summary>
        public IEnumerable<PlayerSeasonCompetitionMatchStatResponse> PlayerSeasonCompetitionMatchStats { get; set; } = Enumerable.Empty<PlayerSeasonCompetitionMatchStatResponse>();
    }
}
