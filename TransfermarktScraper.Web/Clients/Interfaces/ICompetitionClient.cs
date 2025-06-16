using System.Collections.Generic;
using System.Threading.Tasks;
using TransfermarktScraper.Domain.DTOs.Response.Scraper;

namespace TransfermarktScraper.Web.Clients.Interfaces
{
    /// <summary>
    /// Interface for interacting with the competition API.
    /// </summary>
    public interface ICompetitionClient
    {
        /// <summary>
        /// Retrieves a list of competitions from the API.
        /// </summary>
        /// <param name="countryTransfermarktId">The Transfermarkt country ID used to identify the country.</param>
        /// <returns>
        /// A task representing the asynchronous operation, returning an <see cref="IEnumerable{T}"/> of <see cref="CompetitionResponse"/>.
        /// </returns>
        Task<IEnumerable<CompetitionResponse>> GetCompetitionsAsync(string countryTransfermarktId);
    }
}
