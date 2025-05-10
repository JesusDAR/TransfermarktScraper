namespace TransfermarktScraper.Web.Services.Interfaces
{
    /// <summary>
    /// Interface to define a contract for storing and retrieving a reference to the currently active refreshable page.
    /// </summary>
    public interface IPageReferenceService
    {
        /// <summary>
        /// Gets or sets the currently active page that implements <see cref="IRefreshablePage"/>.
        /// </summary>
        IRefreshablePage? CurrentPage { get; set; }
    }
}
