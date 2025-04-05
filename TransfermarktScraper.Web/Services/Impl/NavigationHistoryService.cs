using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using TransfermarktScraper.Web.Services.Interfaces;

namespace TransfermarktScraper.Web.Services.Impl
{
    /// <inheritdoc/>
    public class NavigationHistoryService : INavigationHistoryService
    {
        private readonly NavigationManager _navigationManager;
        private Stack<string> _backHistory = new Stack<string>();
        private Stack<string> _forwardHistory = new Stack<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationHistoryService"/> class.
        /// </summary>
        /// <param name="navigationManager">The navigation manager.</param>
        public NavigationHistoryService(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

        /// <inheritdoc/>
        public void NavigateTo(string url)
        {
            _backHistory.Push(_navigationManager.Uri);  // Push the current URL to the back stack
            _forwardHistory.Clear();  // Clear forward history since we're navigating to a new page
            _navigationManager.NavigateTo(url);
        }

        /// <inheritdoc/>
        public void GoBack()
        {
            if (CanGoBack())
            {
                var previousUrl = _backHistory.Pop();
                _forwardHistory.Push(_navigationManager.Uri); // Push current URL to the forward stack
                _navigationManager.NavigateTo(previousUrl);
            }
        }

        /// <inheritdoc/>
        public void GoForward()
        {
            if (CanGoForward())
            {
                var nextUrl = _forwardHistory.Pop();
                _backHistory.Push(_navigationManager.Uri); // Push current URL to the back stack
                _navigationManager.NavigateTo(nextUrl);
            }
        }

        /// <inheritdoc/>
        public bool CanGoBack() => _backHistory.Count > 0;

        /// <inheritdoc/>
        public bool CanGoForward() => _forwardHistory.Count > 0;
    }
}
