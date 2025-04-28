using System.Collections.Generic;
using System.Threading.Tasks;
using TransfermarktScraper.Domain.DTOs.Response;

namespace TransfermarktScraper.Web.Clients.Interfaces
{
    /// <summary>
    /// Interface for interacting with the club API.
    /// </summary>
    public interface IClubClient
    {
        /// <summary>
        /// Retrieves a list of clubs from the API.
        /// </summary>
        /// <param name="competitionTransfermarktId">The Transfermarkt competition ID used to identify the competition.</param>
        /// <returns>
        /// A task representing the asynchronous operation, returning an <see cref="IEnumerable{T}"/> of <see cref="ClubResponse"/>.
        /// </returns>
        Task<IEnumerable<ClubResponse>> GetClubsAsync(string competitionTransfermarktId);
    }
}
