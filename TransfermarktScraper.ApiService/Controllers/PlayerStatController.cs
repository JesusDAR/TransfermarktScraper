using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TransfermarktScraper.BLL.Services.Interfaces;
using TransfermarktScraper.Domain.DTOs.Request.Stat;
using TransfermarktScraper.Domain.DTOs.Response.Stat;

namespace TransfermarktScraper.ApiService.Controllers
{
    /// <summary>
    /// Handles player stat related API requests.
    /// </summary>
    [Route("api/players/stats")]
    [ApiController]
    public class PlayerStatController : ControllerBase
    {
        private readonly IPlayerStatService _playerStatService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerStatController"/> class.
        /// </summary>
        /// <param name="playerStatService">The service for scraping player stat data.</param>
        public PlayerStatController(IPlayerStatService playerStatService)
        {
            _playerStatService = playerStatService;
        }

        /// <summary>
        /// Retrieves the player stats from Transfermarkt or from the database.
        /// </summary>
        /// <param name="playerStatRequests">The list of player stat request DTO.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns> An <see cref="ActionResult{T}"/> containing a <see cref="PlayerStatResponse"/> object.</returns>
        /// <response code="200">Returns the of player stat successfully scraped.</response>
        /// <response code="500">If there is an error while processing the request, such as a problem with the server or unexpected exception.</response>
        /// <response code="503">If there is an error while requesting the Transfermarkt page or if the external resource is unavailable.</response>
        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<PlayerStatResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<IEnumerable<PlayerStatResponse>>> GetPlayerStatsAsync(
            [FromBody] IEnumerable<PlayerStatRequest> playerStatRequests,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _playerStatService.GetPlayerStatsAsync(playerStatRequests, cancellationToken);
                return Ok(result);
            }
            catch (HttpRequestException e)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, e.Message);
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }
    }
}
