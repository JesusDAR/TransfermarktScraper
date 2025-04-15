using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TransfermarktScraper.BLL.Services.Interfaces;
using TransfermarktScraper.Domain.Entities.Stat;

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
        /// Retrieves the overall stats of a player by scraping TransfermarktId.
        /// </summary>
        /// <param name="playerTransfermarktId">The Transfermarkt player ID used to identify the player.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns> An <see cref="ActionResult{T}"/> containing a list of <see cref="PlayerStat"/> objects.</returns>
        /// <response code="200">Returns the list of player stats successfully scraped.</response>
        /// <response code="500">If there is an error while processing the request, such as a problem with the server or unexpected exception.</response>
        /// <response code="503">If there is an error while requesting the Transfermarkt page or if the external resource is unavailable.</response>
        [HttpGet]
        [ProducesResponseType(typeof(PlayerStat), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<PlayerStat>> GetPlayerStatAsync(
            [FromQuery] string playerTransfermarktId,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _playerStatService.GetPlayerStatAsync(playerTransfermarktId, cancellationToken);
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
