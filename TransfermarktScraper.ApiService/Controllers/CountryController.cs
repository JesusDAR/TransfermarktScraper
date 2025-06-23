using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DnsClient.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TransfermarktScraper.Domain.DTOs.Request.Scraper;
using TransfermarktScraper.Domain.DTOs.Response.Scraper;
using TransfermarktScraper.Domain.Exceptions;
using TransfermarktScraper.Scraper.Services.Interfaces;

namespace TransfermarktScraper.ApiService.Controllers
{
    /// <summary>
    /// Handles country-related API requests.
    /// </summary>
    [Route("api/countries")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ICountryService _countryService;
        private readonly ILogger<CountryController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CountryController"/> class.
        /// </summary>
        /// <param name="countryService">The service for scraping country data.</param>
        /// <param name="logger">The logger.</param>
        public CountryController(
            ICountryService countryService,
            ILogger<CountryController> logger)
        {
            _countryService = countryService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a list of countries, either from the database or by scraping Transfermarkt.
        /// If scraping is forced or the data is unavailable, it scrapes the countries and returns them.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing a list of <see cref="CountryResponse"/> objects,
        /// wrapped in a successful response or an appropriate error code.
        /// </returns>
        /// <response code="200">Returns the list of countries successfully scraped or retrieved from the database.</response>
        /// <response code="409">If there is an error due to the interceptor failing.</response>
        /// <response code="500">If there is an error while processing the request, such as a problem with the server or unexpected exception.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CountryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<CountryResponse>>> GetCountriesAsync(
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Received request to get countries.");

                var result = await _countryService.GetCountriesAsync(cancellationToken);
                return Ok(result);
            }
            catch (InterceptorException)
            {
                var message = "Interceptor failed. Restarting scraping countries process...";
                _logger.LogError(message);
                return StatusCode(StatusCodes.Status409Conflict, message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected Error on {MethodName}. Message: {Message}", nameof(GetCountriesAsync), ex.Message);
                return Problem(ex.Message);
            }
        }

        /// <summary>
        /// Requests a list of countries with the full competitions data, either from the database or by scraping Transfermarkt.
        /// If scraping is forced or the data is unavailable, it scrapes the competitions and returns the countries with the competition data.
        /// </summary>
        /// <param name="countries">
        /// The countries from which the full competitions data is requested.
        /// </param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing a list of <see cref="CountryResponse"/> objects with the full competitions data,
        /// wrapped in a successful response or an appropriate error code.
        /// </returns>
        /// <response code="200">Returns the list of countries with the competitions successfully scraped or retrieved from the database.</response>
        /// <response code="500">If there is an error while processing the request, such as a problem with the server or unexpected exception.</response>
        [HttpPut("competitions")]
        [ProducesResponseType(typeof(IEnumerable<CountryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<CountryResponse>>> UpdateCountriesCompetitionsAsync(
            [FromBody] IEnumerable<CountryRequest> countries,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Received request to update countries competitions.");

                var result = await _countryService.UpdateCountriesCompetitionsAsync(countries, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected Error on {MethodName}. Message: {Message}", nameof(UpdateCountriesCompetitionsAsync), ex.Message);
                return Problem(ex.Message);
            }
        }
    }
}
