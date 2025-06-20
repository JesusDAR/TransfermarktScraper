﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TransfermarktScraper.Domain.DTOs.Response.Scraper;
using TransfermarktScraper.Scraper.Services.Interfaces;

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
        private readonly ILogger<CompetitionController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompetitionController"/> class.
        /// </summary>
        /// <param name="competitionService">The service for scraping competition data.</param>
        /// <param name="logger">The logger.</param>
        public CompetitionController(
            ICompetitionService competitionService,
            ILogger<CompetitionController> logger)
        {
            _competitionService = competitionService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a list of competitions for a given country, either from the database or by scraping Transfermarkt.
        /// If scraping is forced or the data is unavailable, it scrapes the countries and returns them.
        /// </summary>
        /// <param name="countryTransfermarktId">The Transfermarkt country ID used to identify the country.</param>
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
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Received request to get competitions.");

                var result = await _competitionService.GetCompetitionsAsync(countryTransfermarktId, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected Error on {MethodName}. Message: {Message}", nameof(GetCompetitionsAsync), ex.Message);
                return Problem(ex.Message);
            }
        }
    }
}
