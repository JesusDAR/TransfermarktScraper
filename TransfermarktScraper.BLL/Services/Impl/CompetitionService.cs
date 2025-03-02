using Microsoft.Playwright;
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

        public async Task<List<Competition>> FormatQuickSelectCompetitionResponse(IAPIResponse response)
        {
            var json = await response.JsonAsync();

            var competitions = new List<Competition>();

            try
            {
                foreach (var competitionElement in json.Value.EnumerateArray())
                {
                    var competition = new Competition
                    {
                        Name = competitionElement.GetProperty(nameof(Competition.Name).ToLower()).GetRawText(),
                        Link = competitionElement.GetProperty(nameof(Competition.Link).ToLower()).GetRawText(),
                        TransfermarktId = competitionElement.GetProperty(nameof(Competition.Id).ToLower()).GetRawText(),
                    };

                    competitions.Add(competition);
                }
            }
            catch (Exception e)
            {

                throw e;
            }

            return competitions;
        }
    }
}
