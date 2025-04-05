using TransfermarktScraper.Web.Services.Interfaces;

namespace TransfermarktScraper.Web.Services.Impl
{
    /// <inheritdoc/>
    public class ItemSelectionService : IItemSelectionService
    {
        /// <summary>
        /// Gets or sets the currently selected country.
        /// </summary>
        public Domain.DTOs.Response.Country? SelectedCountry { get; set; }

        /// <summary>
        /// Gets or sets the currently selected competition.
        /// </summary>
        public Domain.DTOs.Response.Competition? SelectedCompetition { get; set; }

        /// <summary>
        /// Gets or sets the currently selected club.
        /// </summary>
        public Domain.DTOs.Response.Club? SelectedClub { get; set; }

        /// <summary>
        /// Gets or sets the currently selected player.
        /// </summary>
        public Domain.DTOs.Response.Player? SelectedPlayer { get; set; }
    }
}
