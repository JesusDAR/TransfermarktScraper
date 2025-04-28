using System.Collections.Generic;
using System.Threading.Tasks;
using TransfermarktScraper.Domain.DTOs.Response;

namespace TransfermarktScraper.Web.Clients.Interfaces
{
    /// <summary>
    /// Interface for interacting with the player API.
    /// </summary>
    public interface IPlayerClient
    {
        /// <summary>
        /// Retrieves a list of players from the API.
        /// </summary>
        /// <param name="clubTransfermarktId">The Transfermarkt club ID used to identify the club.</param>
        /// <returns>
        /// A task representing the asynchronous operation, returning an <see cref="IEnumerable{T}"/> of <see cref="ClubResponse"/>.
        /// </returns>
        Task<IEnumerable<PlayerResponse>> GetPlayersAsync(string clubTransfermarktId);
    }
}
