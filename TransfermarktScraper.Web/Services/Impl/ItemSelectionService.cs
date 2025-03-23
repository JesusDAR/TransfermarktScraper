using System;
using TransfermarktScraper.Web.Services.Interfaces;

namespace TransfermarktScraper.Web.Services.Impl
{
    /// <inheritdoc/>
    public class ItemSelectionService : IItemSelectionService
    {
        private Domain.DTOs.Response.Country? _selectedCountry;

        /// <inheritdoc/>
        public event Action OnCountrySelectionChange = () => { }; // Event to notify changes

        /// <inheritdoc/>
        public Domain.DTOs.Response.Country? SelectedCountry
        {
            get => _selectedCountry;
            set
            {
                _selectedCountry = value;
                NotifyCountrySelectionChange();
            }
        }

        /// <inheritdoc/>
        public bool IsCountrySelected => _selectedCountry != null;

        private void NotifyCountrySelectionChange()
        {
            OnCountrySelectionChange?.Invoke();
        }
    }
}
