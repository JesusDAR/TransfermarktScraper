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
    /// Handles club-related API requests.
    /// </summary>
    [Route("api/clubs")]
    [ApiController]
    public class ClubController : ControllerBase
    {
        private readonly IClubService _clubService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClubController"/> class.
        /// </summary>
        /// <param name="clubService">The service for scraping club data.</param>
        public ClubController(IClubService clubService)
        {
            _clubService = clubService;
        }

        /// <summary>
        /// Asynchronously retrieves from database a list of clubs belonging to a competition.
        /// </summary>
        /// <param name="competitionTransfermarktId">The Transfermarkt competition ID used to identify the competition.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing a list of <see cref="ClubResponse"/> objects,
        /// wrapped in a successful response or an appropriate error code.
        /// </returns>
        /// <response code="200">Returns the list of clubs successfully retrieved from the database.</response>
        /// <response code="500">If there is an error while processing the request, such as a problem with the server or unexpected exception.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ClubResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ClubResponse>>> GetClubsAsync(
            [FromQuery] string competitionTransfermarktId,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _clubService.GetClubsAsync(competitionTransfermarktId, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
