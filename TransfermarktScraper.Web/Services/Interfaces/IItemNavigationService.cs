using System;

namespace TransfermarktScraper.Web.Services.Interfaces
{
    /// <summary>
    /// Defines a service for handling navigation events within the application.
    /// </summary>
    public interface IItemNavigationService
    {
        /// <summary>
        /// Event triggered when navigation to the Countries page is requested.
        /// </summary>
        event Action OnNavigateToCountries;

        /// <summary>
        /// Event triggered when navigation to the Competitions page is requested.
        /// </summary>
        event Action OnNavigateToCompetitions;

        /// <summary>
        /// Event triggered when navigation to the Clubs page is requested.
        /// </summary>
        event Action OnNavigateToClubs;

        /// <summary>
        /// Event triggered when navigation to the Players page is requested.
        /// </summary>
        event Action OnNavigateToPlayers;

        /// <summary>
        /// Notifies subscribers that navigation to the Countries page has been requested.
        /// </summary>
        void NotifyNavigateToCountries();

        /// <summary>
        /// Notifies subscribers that navigation to the Competitions page has been requested.
        /// </summary>
        void NotifyNavigateToCompetitions();

        /// <summary>
        /// Notifies subscribers that navigation to the Clubs has been requested.
        /// </summary>
        void NotifyNavigateToClubs();

        /// <summary>
        /// Notifies subscribers that navigation to the Players has been requested.
        /// </summary>
        void NotifyNavigateToPlayers();
    }
}
