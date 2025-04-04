using System;

namespace TransfermarktScraper.Web.Services.Interfaces
{
    /// <summary>
    /// Defines a service for managing the selected items.
    /// </summary>
    public interface IItemSelectionService
    {
        /// <summary>
        /// Event triggered when the selected country changes.
        /// </summary>
        event Action OnCountrySelectionChange;

        /// <summary>
        /// Event triggered when the selected competition changes.
        /// </summary>
        event Action OnCompetitionSelectionChange;

        /// <summary>
        /// Event triggered when the selected club changes.
        /// </summary>
        event Action OnClubSelectionChange;

        /// <summary>
        /// Event triggered when the selected player changes.
        /// </summary>
        event Action OnPlayerSelectionChange;

        /// <summary>
        /// Gets or sets the currently selected country.
        /// </summary>
        Domain.DTOs.Response.Country? SelectedCountry { get; set; }

        /// <summary>
        /// Gets or sets the currently selected competition.
        /// </summary>
        Domain.DTOs.Response.Competition? SelectedCompetition { get; set; }

        /// <summary>
        /// Gets or sets the currently selected club.
        /// </summary>
        Domain.DTOs.Response.Club? SelectedClub { get; set; }

        /// <summary>
        /// Gets or sets the currently selected player.
        /// </summary>
        Domain.DTOs.Response.Player? SelectedPlayer { get; set; }

        /// <summary>
        /// Gets a value indicating whether a country is currently selected.
        /// </summary>
        bool IsCountrySelected { get; }

        /// <summary>
        /// Gets a value indicating whether a competition is currently selected.
        /// </summary>
        bool IsCompetitionSelected { get; }

        /// <summary>
        /// Gets a value indicating whether a club is currently selected.
        /// </summary>
        bool IsClubSelected { get; }

        /// <summary>
        /// Gets a value indicating whether a player is currently selected.
        /// </summary>
        bool IsPlayerSelected { get; }
    }
}
