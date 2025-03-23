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
        /// Gets or sets the currently selected country.
        /// </summary>
        Domain.DTOs.Response.Country? SelectedCountry { get; set; }

        /// <summary>
        /// Gets a value indicating whether a country is currently selected.
        /// </summary>
        bool IsCountrySelected { get; }
    }
}
