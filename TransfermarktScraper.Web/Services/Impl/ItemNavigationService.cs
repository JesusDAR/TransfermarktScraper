using System;
using TransfermarktScraper.Web.Services.Interfaces;

namespace TransfermarktScraper.Web.Services.Impl
{
    /// <inheritdoc/>
    public class ItemNavigationService : IItemNavigationService
    {
        /// <inheritdoc/>
        public event Action OnNavigateToCountries = () => { };

        /// <inheritdoc/>
        public event Action OnNavigateToCompetitions = () => { };

        /// <inheritdoc/>
        public void NotifyNavigateToCountries()
        {
            OnNavigateToCountries?.Invoke();
        }

        /// <inheritdoc/>
        public void NotifyNavigateToCompetitions()
        {
            OnNavigateToCompetitions?.Invoke();
        }
    }
}