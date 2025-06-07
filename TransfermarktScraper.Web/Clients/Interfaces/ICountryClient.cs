using System.Collections.Generic;
using System.Threading.Tasks;
using TransfermarktScraper.Domain.DTOs.Request.Scraper;
using TransfermarktScraper.Domain.DTOs.Response.Scraper;

namespace TransfermarktScraper.Web.Clients.Interfaces
{
    /// <summary>
    /// Interface for interacting with the country API.
    /// </summary>
    public interface ICountryClient
    {
        /// <summary>
        /// Retrieves a list of countries from the API.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation, returning an <see cref="IEnumerable{T}"/> of <see cref="CountryResponse"/>.
        /// </returns>
        Task<IEnumerable<CountryResponse>> GetCountriesAsync();

        /// <summary>
        /// Retrieves a list of countries with full competition data from the API.
        /// </summary>
        /// <param name="countries">
        /// A collection of <see cref="CountryRequest"/> objects.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation, returning an <see cref="IEnumerable{T}"/> of <see cref="CountryResponse"/> with full <see cref="CompetitionResponse"/> data.
        /// </returns>
        Task<IEnumerable<CountryResponse>> UpdateCountriesCompetitionsAsync(IEnumerable<CountryRequest> countries);
    }
}
