namespace TransfermarktScraper.Domain.Enums
{
    /// <summary>
    /// Represents the possible outcomes of a match.
    /// </summary>
    public enum MatchResult
    {
        /// <summary>
        /// Unknown match result.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The match was won.
        /// </summary>
        Win = 1,

        /// <summary>
        /// The match was lost.
        /// </summary>
        Loss = 2,

        /// <summary>
        /// The match ended in a draw.
        /// </summary>
        Draw = 3,
    }
}
