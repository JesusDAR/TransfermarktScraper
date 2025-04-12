using System.Collections.Generic;
using TransfermarktScraper.Domain.DTOs.Response;

namespace TransfermarktScraper.Web.Services.Interfaces
{
    /// <summary>
    /// Defines a service for managing the selected items.
    /// </summary>
    public interface IItemSelectionService
    {
        /// <summary>
        /// Gets or sets the currently selected countries.
        /// </summary>
        public HashSet<Country> SelectedCountries { get; set; }

        /// <summary>
        /// Gets or sets the currently selected competitions.
        /// </summary>
        public HashSet<Competition> SelectedCompetitions { get; set; }

        /// <summary>
        /// Gets or sets the currently selected clubs.
        /// </summary>
        public HashSet<Club> SelectedClubs { get; set; }

        /// <summary>
        /// Gets or sets the currently selected players.
        /// </summary>
        public HashSet<Player> SelectedPlayers { get; set; }
    }
}
