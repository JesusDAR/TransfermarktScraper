using System.Collections.Generic;
using System.Threading.Tasks;
using TransfermarktScraper.Domain.DTOs.Request.Scraper.Stat;
using TransfermarktScraper.Domain.DTOs.Response.Scraper.Stat;

namespace TransfermarktScraper.Web.Clients.Interfaces
{
    /// <summary>
    /// Interface for interacting with the player stat API.
    /// </summary>
    public interface IPlayerStatClient
    {
        /// <summary>
        /// Retrieves the player stats based on the specified Transfermarkt player ID.
        /// </summary>
        /// <param name="playerStats">The player stat request.</param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="PlayerStatResponse"/> object if found; otherwise, null.
        /// </returns>
        Task<IEnumerable<PlayerStatResponse>?> GetPlayerStatsAsync(IEnumerable<PlayerStatRequest> playerStats);
    }
}
