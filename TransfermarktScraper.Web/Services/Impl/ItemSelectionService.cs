using System.Collections.Generic;
using TransfermarktScraper.Domain.DTOs.Response.Scraper;
using TransfermarktScraper.Web.Services.Interfaces;

namespace TransfermarktScraper.Web.Services.Impl
{
    /// <inheritdoc/>
    public class ItemSelectionService : IItemSelectionService
    {
        private HashSet<CountryResponse> _selectedCountries = new ();
        private HashSet<CompetitionResponse> _selectedCompetitions = new ();
        private HashSet<ClubResponse> _selectedClubs = new ();
        private PlayerResponse? _selectedPlayer;

        /// <inheritdoc/>
        public HashSet<CountryResponse> SelectedCountries
        {
            get => _selectedCountries;
            set => _selectedCountries = value;
        }

        /// <inheritdoc/>
        public HashSet<CompetitionResponse> SelectedCompetitions
        {
            get => _selectedCompetitions;
            set => _selectedCompetitions = value;
        }

        /// <inheritdoc/>
        public HashSet<ClubResponse> SelectedClubs
        {
            get => _selectedClubs;
            set => _selectedClubs = value;
        }

        /// <inheritdoc/>
        public PlayerResponse? SelectedPlayer
        {
            get => _selectedPlayer;
            set => _selectedPlayer = value;
        }
    }
}
