using System.Threading.Tasks;
using PlayerStat = TransfermarktScraper.Domain.DTOs.Response.Stat.PlayerStat;

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
        /// <param name="playerStat">The player stat request.</param>
        /// <returns>
        /// A <see cref="PlayerStat"/> object if found; otherwise, null.
        /// </returns>
        Task<PlayerStat?> GetPlayerStatAsync(Domain.DTOs.Request.Stat.PlayerStat playerStat);
    }
}
