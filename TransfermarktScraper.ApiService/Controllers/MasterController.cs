using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TransfermarktScraper.Scraper.Services.Interfaces;

namespace TransfermarktScraper.ApiService.Controllers
{
    /// <summary>
    /// Handles API requests that involve all database entities.
    /// </summary>
    [Route("api/master")]
    [ApiController]
    public class MasterController : ControllerBase
    {
        private readonly IMasterService _masterService;
        private readonly ILogger<MasterController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterController"/> class.
        /// </summary>
        /// <param name="masterService">The master scraping data service.</param>
        /// <param name="logger">The logger.</param>
        public MasterController(
            IMasterService masterService,
            ILogger<MasterController> logger)
        {
            _masterService = masterService;
            _logger = logger;
        }

        /// <summary>
        /// Deletes all data from the database that has been collected by the scraper.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// <response code="200">If the operation was successful.</response>
        /// <response code="500">For unexpected errors.</response>
        [HttpGet("clean-database")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CleanDatabaseAsync(
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Received request to clean database.");

                await _masterService.CleanDatabaseAsync(cancellationToken);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected Error.");
                return Problem(ex.Message);
            }
        }

        /// <summary>
        /// Scrapes all data available in Transfermarkt.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// <response code="200">If the operation was successful.</response>
        /// <response code="500">For unexpected errors.</response>
        [HttpGet("scrape-all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ScrapeAllAsync(
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Received request to scrape all data.");

                await _masterService.ScrapeAllAsync(cancellationToken);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected Error.");
                return Problem(ex.Message);
            }
        }
    }
}
