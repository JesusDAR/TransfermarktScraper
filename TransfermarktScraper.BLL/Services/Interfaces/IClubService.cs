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
        /// <param name="competitionTransfermarktId">The unique identifier of the competition on Transfermarkt.</param>
        /// <param name="clubRowLocator">The locator for the club row element, used to find and extract data from the HTML grid table row.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation, containing a <see cref="Club"/> DTO.
        /// </returns>
        public Task<Club> GetClubAsync(string competitionTransfermarktId, ILocator clubRowLocator, CancellationToken cancellationToken);
    }
}
