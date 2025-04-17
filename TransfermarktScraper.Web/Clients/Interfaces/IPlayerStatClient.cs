using System.Threading.Tasks;
using TransfermarktScraper.Domain.DTOs.Response.Stat;

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
        /// <param name="playerTransfermarktId">The unique Transfermarkt ID of the player.</param>
        /// <returns>
        /// A <see cref="PlayerStat"/> object if found; otherwise, null.
        /// </returns>
        Task<PlayerStat?> GetPlayerStatAsync(string playerTransfermarktId);
    }
}
