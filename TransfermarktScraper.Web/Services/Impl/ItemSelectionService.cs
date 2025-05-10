using System.Collections.Generic;
using TransfermarktScraper.Domain.DTOs.Response;
using TransfermarktScraper.Web.Services.Interfaces;

namespace TransfermarktScraper.Web.Services.Impl
{
    /// <inheritdoc/>
    public class ItemSelectionService : IItemSelectionService
    {
        /// <inheritdoc/>
        public HashSet<CountryResponse> SelectedCountries { get; set; } = [];

        /// <inheritdoc/>
        public HashSet<CompetitionResponse> SelectedCompetitions { get; set; } = [];

        /// <inheritdoc/>
        public HashSet<ClubResponse> SelectedClubs { get; set; } = [];

        /// <inheritdoc/>
        public PlayerResponse? SelectedPlayer { get; set; }
    }
}
