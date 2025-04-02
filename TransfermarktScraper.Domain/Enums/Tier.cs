namespace TransfermarktScraper.Domain.Enums
{
    /// <summary>
    /// Represents different tiers of a league system.
    /// </summary>
    public enum Tier
    {
        /// <summary>
        /// No league level.
        /// </summary>
        None = 0,

        /// <summary>
        /// Unknown league level.
        /// </summary>
        Unknown = 1,

        /// <summary>
        /// First division league level.
        /// </summary>
        FirstTier = 2,

        /// <summary>
        /// Second division league level.
        /// </summary>
        SecondTier = 3,

        /// <summary>
        /// Third division league level.
        /// </summary>
        ThirdTier = 4,

        /// <summary>
        /// Youth league level.
        /// </summary>
        YouthLeague = 5,
    }
}
