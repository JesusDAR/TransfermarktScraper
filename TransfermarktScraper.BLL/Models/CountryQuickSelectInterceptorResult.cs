namespace TransfermarktScraper.BLL.Models
{
    /// <summary>
    /// Represents the result of the country quick select interceptor,
    /// containing the intercepted data and the associated task.
    /// </summary>
    public class CountryQuickSelectInterceptorResult
    {
        /// <summary>
        /// Gets or sets the asynchronous task associated with the interceptor execution.
        /// </summary>
        required public Task InterceptorTask { get; set; }

        /// <summary>
        /// Gets or sets the list of quick select results for countries.
        /// </summary>
        public IList<CountryQuickSelectResult> CountryQuickSelectResults { get; set; } = new List<CountryQuickSelectResult>();
    }
}
