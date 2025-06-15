using System;
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
    /// Handles player-related API requests.
    /// </summary>
    [Route("api/players")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerService _playerService;
        private readonly ILogger<PlayerController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerController"/> class.
        /// </summary>
        /// <param name="playerService">The service for scraping player data.</param>
        /// <param name="logger">The logger.</param>
        public PlayerController(
            IPlayerService playerService,
            ILogger<PlayerController> logger)
        {
            _playerService = playerService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a list of players for a given club, either from the database or by scraping Transfermarkt.
        /// If scraping is forced or the data is unavailable, it scrapes the players and returns them.
        /// </summary>
        /// <param name="clubTransfermarktId">The Transfermarkt club ID used to identify the club.</param>
        /// <param name="forceScraping">
        /// A boolean flag that determines whether to force scraping of the players data, even if it exists in the database.
        /// </param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing a list of <see cref="PlayerResponse"/> objects,
        /// wrapped in a successful response or an appropriate error code.
        /// </returns>
        /// <response code="200">Returns the list of players successfully scraped or retrieved from the database.</response>
        /// <response code="500">If there is an error while processing the request, such as a problem with the server or unexpected exception.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PlayerResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<PlayerResponse>>> GetPlayersAsync(
            [FromQuery] string clubTransfermarktId,
            [FromQuery] bool forceScraping,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Received request to get players.");

                var result = await _playerService.GetPlayersAsync(clubTransfermarktId, forceScraping, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected Error on {MethodName}. Message: {Message}", nameof(GetPlayersAsync), ex.Message);
                return Problem(ex.Message);
            }
        }
    }
}
