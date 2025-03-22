using System.Collections.Generic;
using System.Threading.Tasks;
using TransfermarktScraper.Domain.DTOs.Response;

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
        /// <returns>
        /// A task representing the asynchronous operation, returning an <see cref="IEnumerable{T}"/> of <see cref="Competition"/>.
        /// </returns>
        Task<IEnumerable<Competition>> GetCompetitionsAsync();
    }
}
