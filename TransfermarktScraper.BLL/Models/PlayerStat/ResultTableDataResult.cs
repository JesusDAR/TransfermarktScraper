using TransfermarktScraper.Domain.Enums;

namespace TransfermarktScraper.BLL.Models.PlayerStat
{
    /// <summary>
    /// Represents the table data for a the match result listed in the matched stats table.
    /// </summary>
    public class ResultTableDataResult
    {
        /// <summary>
        /// Gets or sets the home club number of goals in the match.
        /// </summary>
        public int HomeClubGoals { get; set; } = 0;

        /// <summary>
        /// Gets or sets the away club number of goals in the match.
        /// </summary>
        public int AwayClubGoals { get; set; } = 0;

        /// <summary>
        /// Gets or sets the result of the match.
        /// </summary>
        public MatchResult MatchResult { get; set; } = MatchResult.Unknown;

        /// <summary>
        /// Gets or sets the link of the result of the match in Transfermarkt.
        /// </summary>
        public string MatchResultLink { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the match was decided in the addition time.
        /// </summary>
        public bool IsResultAddition { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the match was decided in the penalties.
        /// </summary>
        public bool IsResultPenalties { get; set; } = false;
    }
}
