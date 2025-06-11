namespace TransfermarktScraper.Scraper.Services.Interfaces
{
    /// <summary>
    /// Provides operations that affect the entire scraping system or database.
    /// </summary>
    public interface IMasterService
    {
        /// <summary>
        /// Deletes all data from the database that has been collected by the scraper.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task CleanDatabaseAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Scrapes all data available in Transfermarkt.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task ScrapeAllAsync(CancellationToken cancellationToken);
    }
}
