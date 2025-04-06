using System.Collections.Generic;
using TransfermarktScraper.Domain.DTOs.Response;
using TransfermarktScraper.Web.Services.Interfaces;

namespace TransfermarktScraper.Web.Services.Impl
{
    /// <inheritdoc/>
    public class ItemSelectionService : IItemSelectionService
    {
        /// <inheritdoc/>
        public HashSet<Country> SelectedCountries { get; set; } = [];

        /// <inheritdoc/>
        public HashSet<Competition> SelectedCompetitions { get; set; } = [];

        /// <inheritdoc/>
        public HashSet<Club> SelectedClubs { get; set; } = [];

        /// <inheritdoc/>
        public Player? SelectedPlayer { get; set; }
    }
}
