using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TransfermarktScraper.BLL.Services.Interfaces;
using TransfermarktScraper.Domain.DTOs.Response;

namespace TransfermarktScraper.ApiService.Controllers
{
    /// <summary>
    /// Handles competition-related API requests.
    /// </summary>
    [Route("api/competitions")]
    [ApiController]
    public class CompetitionController : ControllerBase
    {
        private readonly ICompetitionService _competitionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompetitionController"/> class.
        /// </summary>
        /// <param name="competitionService">The service for scraping competition data.</param>
        public CompetitionController(ICompetitionService competitionService)
        {
            _competitionService = competitionService;
        }

        /// <summary>
        /// Retrieves a list of competitions for a given country, either from the database or by scraping Transfermarkt.
        /// If scraping is forced or the data is unavailable, it scrapes the countries and returns them.
        /// </summary>
        /// <param name="countryTransfermarktId">The Transfermarkt country ID used to identify the country.</param>
        /// <param name="forceScraping"> A boolean flag that determines whether to force scraping of the competitions data, even if it exists in the database. </param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing a list of <see cref="CompetitionResponse"/> objects,
        /// wrapped in a successful response or an appropriate error code.
        /// </returns>
        /// <response code="200">Returns the list of competitions successfully scraped or retrieved from the database.</response>
        /// <response code="500">If there is an error while processing the request, such as a problem with the server or unexpected exception.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CompetitionResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<CompetitionResponse>>> GetCompetitionsAsync(
            [FromQuery] string countryTransfermarktId,
            [FromQuery] bool forceScraping,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _competitionService.GetCompetitionsAsync(countryTransfermarktId, forceScraping, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
