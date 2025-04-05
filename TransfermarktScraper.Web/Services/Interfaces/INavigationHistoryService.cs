namespace TransfermarktScraper.Web.Services.Interfaces
{
    /// <summary>
    /// Interface for managing navigation history within the application.
    /// It provides methods to track and navigate through back and forward history.
    /// </summary>
    public interface INavigationHistoryService
    {
        /// <summary>
        /// Navigates to a given URL and records the current URL in the history stack.
        /// Clears the forward history when a new navigation occurs.
        /// </summary>
        /// <param name="url">The URL to navigate to.</param>
        public void NavigateTo(string url);

        /// <summary>
        /// Navigates to the previous URL in the back history stack, if available.
        /// If no previous URL exists, this method does nothing.
        /// </summary>
        public void GoBack();

        /// <summary>
        /// Navigates to the next URL in the forward history stack, if available.
        /// If no next URL exists, this method does nothing.
        /// </summary>
        public void GoForward();

        /// <summary>
        /// Determines if there are any URLs in the back history stack to navigate to.
        /// </summary>
        /// <returns>True if there are URLs in the back history stack; otherwise, false.</returns>
        public bool CanGoBack();

        /// <summary>
        /// Determines if there are any URLs in the forward history stack to navigate to.
        /// </summary>
        /// <returns>True if there are URLs in the forward history stack; otherwise, false.</returns>
        public bool CanGoForward();
    }
}
