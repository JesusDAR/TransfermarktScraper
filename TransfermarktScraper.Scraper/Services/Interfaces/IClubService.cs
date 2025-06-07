using Microsoft.Playwright;
using TransfermarktScraper.Domain.DTOs.Response.Scraper;

namespace TransfermarktScraper.Scraper.Services.Interfaces
{
    /// <summary>
    /// Defines a service for scraping club data from Transfermarkt.
    /// </summary>
    public interface IClubService
    {
        /// <summary>
        /// Asynchronously scrapes, persists, and maps a club's data into a <see cref="ClubResponse"/>.
        /// </summary>
        /// <remarks>
        /// This method is only to be used by the competition service.
        /// </remarks>
        /// <param name="competitionTransfermarktId">The unique identifier of the competition on Transfermarkt.</param>
        /// <param name="clubRowLocator">The locator for the club row element, used to find and extract data from the HTML grid table row.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation, containing a <see cref="ClubResponse"/> DTO.
        /// </returns>
        public Task<ClubResponse> GetClubAsync(string competitionTransfermarktId, ILocator clubRowLocator, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously retrieves from database a list of <see cref="ClubResponse"/> belonging to a <see cref="CompetitionResponse"/>.
        /// </summary>
        /// <param name="competitionTransfermarktId">The unique identifier of the competition on Transfermarkt.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation, containing a list of <see cref="ClubResponse"/> DTOs.
        /// </returns>
        public Task<IEnumerable<ClubResponse>> GetClubsAsync(string competitionTransfermarktId, CancellationToken cancellationToken);
    }
}
