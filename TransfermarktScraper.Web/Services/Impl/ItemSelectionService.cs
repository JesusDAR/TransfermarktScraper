using System.Collections.Generic;
using TransfermarktScraper.Domain.DTOs.Response;
using TransfermarktScraper.Web.Services.Interfaces;

namespace TransfermarktScraper.Web.Services.Impl
{
    /// <inheritdoc/>
    public class ItemSelectionService : IItemSelectionService
    {
        /// <inheritdoc/>
        public IList<Country> SelectedCountries { get; set; } = [];

        /// <inheritdoc/>
        public IList<Competition> SelectedCompetitions { get; set; } = [];

        /// <inheritdoc/>
        public IList<Club> SelectedClubs { get; set; } = [];

        /// <inheritdoc/>
        public Player? SelectedPlayer { get; set; }
    }
}
