namespace TransfermarktScraper.Data.Configuration
{
    /// <summary>
    /// Represents the settings for the database.
    /// </summary>
    public class DbSettings
    {
        /// <summary>
        /// Gets or sets the connection string for the database.
        /// </summary>
        public string? ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the name of the database.
        /// </summary>
        public string? DatabaseName { get; set; }

        /// <summary>
        /// Gets or sets the name of the collection that stores country data.
        /// </summary>
        public string? CountryCollection { get; set; }

        /// <summary>
        /// Gets or sets the name of the collection that stores competition data.
        /// </summary>
        public string? CompetitionCollection { get; set; }
    }
}
