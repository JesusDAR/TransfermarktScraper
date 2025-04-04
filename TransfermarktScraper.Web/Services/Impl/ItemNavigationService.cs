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
        public event Action OnNavigateToClubs = () => { };

        /// <inheritdoc/>
        public event Action OnNavigateToPlayers = () => { };

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

        /// <inheritdoc/>
        public void NotifyNavigateToClubs()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void NotifyNavigateToPlayers()
        {
            throw new NotImplementedException();
        }
    }
}