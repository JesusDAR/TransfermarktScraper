namespace TransfermarktScraper.Domain.Enums
{
    /// <summary>
    /// Represents the reasons why a player don't played any minute of a match.
    /// </summary>
    public enum NotPlayingReason
    {
        /// <summary>
        /// Default value. The player played in the match
        /// </summary>
        None = 0,

        /// <summary>
        /// The player was on the bench during all match.
        /// </summary>
        OnTheBench = 1,

        /// <summary>
        /// The player was not included in the team squad for the match.
        /// </summary>
        NotInSquad = 2,

        /// <summary>
        /// The player did not participate due to an injure or illness.
        /// </summary>
        Injured = 3,

        /// <summary>
        /// Other reason.
        /// </summary>
        Other = 4,
    }
}
