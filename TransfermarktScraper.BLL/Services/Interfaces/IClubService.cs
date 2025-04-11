using Microsoft.Playwright;
using TransfermarktScraper.Domain.DTOs.Response;

namespace TransfermarktScraper.BLL.Services.Interfaces
{
    /// <summary>
    /// Defines a service for scraping club data from Transfermarkt.
    /// </summary>
    public interface IClubService
    {
        /// <summary>
        /// Asynchronously scrapes, persists, and maps a club's data into a <see cref="Club"/>.
        /// </summary>
        /// <remarks>
        /// This method is only to be used by the competition service.
        /// </remarks>
        /// <param name="competitionTransfermarktId">The unique identifier of the competition on Transfermarkt.</param>
        /// <param name="clubRowLocator">The locator for the club row element, used to find and extract data from the HTML grid table row.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation, containing a <see cref="Club"/> DTO.
        /// </returns>
        public Task<Club> GetClubAsync(string competitionTransfermarktId, ILocator clubRowLocator, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously retrieves from database a list of <see cref="Club"/> belonging to a <see cref="Competition"/>.
        /// </summary>
        /// <param name="competitionTransfermarktId">The unique identifier of the competition on Transfermarkt.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation, containing a list of <see cref="Club"/> DTOs.
        /// </returns>
        public Task<IEnumerable<Club>> GetClubsAsync(string competitionTransfermarktId, CancellationToken cancellationToken);
    }
}
