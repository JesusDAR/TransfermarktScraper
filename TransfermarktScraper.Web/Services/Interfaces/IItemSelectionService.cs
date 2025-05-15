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
        public HashSet<CountryResponse> SelectedCountries { get; set; }

        /// <summary>
        /// Gets or sets the currently selected competitions.
        /// </summary>
        public HashSet<CompetitionResponse> SelectedCompetitions { get; set; }

        /// <summary>
        /// Gets or sets the currently selected clubs.
        /// </summary>
        public HashSet<ClubResponse> SelectedClubs { get; set; }

        /// <summary>
        /// Gets or sets the currently selected player.
        /// </summary>
        public PlayerResponse? SelectedPlayer { get; set; }
    }
}
