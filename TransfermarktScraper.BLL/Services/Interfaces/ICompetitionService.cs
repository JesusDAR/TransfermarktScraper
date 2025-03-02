using Microsoft.Playwright;
using TransfermarktScraper.Domain.DTOs.Response;

namespace TransfermarktScraper.BLL.Services.Interfaces
{
    /// <summary>
    /// Defines a service for scraping competition data from Transfermarkt.
    /// </summary>
    public interface ICompetitionService
    {
        public Task<IEnumerable<Competition>> GetCompetitionsAsync(bool forceScraping = false);

        public Task<List<Competition>> FormatQuickSelectCompetitionResponse(IAPIResponse response);
    }
}
