using System;
using TransfermarktScraper.Domain.DTOs.Response;
using TransfermarktScraper.Web.Services.Interfaces;

namespace TransfermarktScraper.Web.Services.Impl
{
    /// <inheritdoc/>
    public class ItemSelectionService : IItemSelectionService
    {
        private Country? _selectedCountry;

        private Competition? _selectedCompetition;

        /// <inheritdoc/>
        public event Action OnCountrySelectionChange = () => { }; // Event to notify changes

        /// <inheritdoc/>
        public event Action OnCompetitionSelectionChange = () => { };

        /// <inheritdoc/>
        public event Action OnClubSelectionChange = () => { };

        /// <inheritdoc/>
        public event Action OnPlayerSelectionChange = () => { };

        /// <inheritdoc/>
        public Country? SelectedCountry
        {
            get => _selectedCountry;
            set
            {
                _selectedCountry = value;
                NotifyCountrySelectionChange();
            }
        }

        /// <inheritdoc/>
        public Competition? SelectedCompetition
        {
            get => _selectedCompetition;
            set
            {
                _selectedCompetition = value;
                NotifyCompetitionSelectionChange();
            }
        }

        /// <inheritdoc/>
        public bool IsCountrySelected => _selectedCountry != null;

        /// <inheritdoc/>
        public bool IsCompetitionSelected => _selectedCompetition != null;

        /// <inheritdoc/>
        public Club? SelectedClub { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public Player? SelectedPlayer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public bool IsClubSelected => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool IsPlayerSelected => throw new NotImplementedException();

        private void NotifyCountrySelectionChange()
        {
            OnCountrySelectionChange?.Invoke();
        }

        private void NotifyCompetitionSelectionChange()
        {
            OnCompetitionSelectionChange?.Invoke();
        }
    }
}
