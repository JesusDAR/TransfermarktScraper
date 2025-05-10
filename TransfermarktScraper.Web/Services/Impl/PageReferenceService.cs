using TransfermarktScraper.Web.Services.Interfaces;

namespace TransfermarktScraper.Web.Services.Impl
{
    /// <inheritdoc/>
    public class PageReferenceService : IPageReferenceService
    {
        /// <inheritdoc/>
        public IRefreshablePage? CurrentPage { get; set; }
    }
}
