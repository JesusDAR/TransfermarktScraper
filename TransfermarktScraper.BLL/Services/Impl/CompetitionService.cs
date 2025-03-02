using Microsoft.Playwright;
using TransfermarktScraper.BLL.Models;
using TransfermarktScraper.BLL.Services.Interfaces;
using TransfermarktScraper.Domain.DTOs.Response;

namespace TransfermarktScraper.BLL.Services.Impl
{
    /// <inheritdoc/>
    public class CompetitionService : ICompetitionService
    {
        private readonly IPage _page;

        public CompetitionService(IPage page)
        {
            _page = page;
        }

        public async Task<IEnumerable<Competition>> GetCompetitionsAsync(bool forceScraping = false)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task<List<CompetitionQuickSelectResult>> FormatQuickSelectCompetitionResponseAsync(IAPIResponse response)
        {
            var json = await response.JsonAsync();

            var competitionQuickSelectResults = new List<CompetitionQuickSelectResult>();

            foreach (var competitionElement in json.Value.EnumerateArray())
            {
                var competitionQuickSelectResult = new CompetitionQuickSelectResult
                {
                    Name = competitionElement.GetProperty(nameof(CompetitionQuickSelectResult.Name).ToLower()).GetRawText(),
                    Link = competitionElement.GetProperty(nameof(CompetitionQuickSelectResult.Link).ToLower()).GetRawText(),
                    Id = competitionElement.GetProperty(nameof(CompetitionQuickSelectResult.Id).ToLower()).GetRawText(),
                };

                competitionQuickSelectResults.Add(competitionQuickSelectResult);
            }

            return competitionQuickSelectResults;
        }
    }
}
