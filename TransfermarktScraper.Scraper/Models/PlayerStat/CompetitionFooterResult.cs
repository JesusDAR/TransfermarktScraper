namespace TransfermarktScraper.Scraper.Models.PlayerStat
{
    /// <summary>
    /// Represents the values of a competition extracted from the footer of a match table in the player stats.
    /// </summary>
    public class CompetitionFooterResult
    {
        /// <summary>
        /// Gets or sets the number of times the player was in the squad in the competition for the season.
        /// </summary>
        public int Squad { get; set; }

        /// <summary>
        /// Gets or sets the number of times the player was in the starting eleven in the competition for the season.
        /// </summary>
        public int StartingEleven { get; set; }

        /// <summary>
        /// Gets or sets the number of times the player was substituted on in the competition for the season.
        /// </summary>
        public int SubstitutionsOn { get; set; }

        /// <summary>
        /// Gets or sets the number of times the player was substituted off in the competition for the season.
        /// </summary>
        public int SubstitutionsOff { get; set; }

        /// <summary>
        /// Gets or sets the number of times the player was on the bench the whole match in the competition for the season.
        /// </summary>
        public int OnTheBench { get; set; }

        /// <summary>
        /// Gets or sets the number of matches the player missed because of a suspension in the competition for the season.
        /// </summary>
        public int Suspended { get; set; }

        /// <summary>
        /// Gets or sets the number of matches the player missed because of an injure in the competition for the season.
        /// </summary>
        public int Injured { get; set; }
    }
}
