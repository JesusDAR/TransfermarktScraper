using System.Collections.Generic;
using System.Threading.Tasks;
using TransfermarktScraper.Domain.DTOs.Response;

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
        /// A task representing the asynchronous operation, returning an <see cref="IEnumerable{T}"/> of <see cref="Country"/>.
        /// </returns>
        Task<IEnumerable<Country>> GetCountriesAsync();
    }
}
