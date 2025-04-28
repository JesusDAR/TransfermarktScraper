using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TransfermarktScraper.BLL.Services.Interfaces;
using TransfermarktScraper.Domain.DTOs.Request;
using TransfermarktScraper.Domain.DTOs.Response;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="CountryController"/> class.
        /// </summary>
        /// <param name="countryService">The service for scraping country data.</param>
        public CountryController(ICountryService countryService)
        {
            _countryService = countryService;
        }

        /// <summary>
        /// Retrieves a list of countries, either from the database or by scraping Transfermarkt.
        /// If scraping is forced or the data is unavailable, it scrapes the countries and returns them.
        /// </summary>
        /// <param name="forceScraping">
        /// A boolean flag that determines whether to force scraping of the country data, even if it exists in the database.
        /// </param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing a list of <see cref="CountryResponse"/> objects,
        /// wrapped in a successful response or an appropriate error code.
        /// </returns>
        /// <response code="200">Returns the list of countries successfully scraped or retrieved from the database.</response>
        /// <response code="500">If there is an error while processing the request, such as a problem with the server or unexpected exception.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CountryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<CountryResponse>>> GetCountriesAsync(
            [FromQuery] bool forceScraping,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _countryService.GetCountriesAsync(forceScraping, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
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
        /// <param name="forceScraping">
        /// A boolean flag that determines whether to force scraping of the country data, even if it exists in the database.
        /// </param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing a list of <see cref="CountryResponse"/> objects with the full competitions data,
        /// wrapped in a successful response or an appropriate error code.
        /// </returns>
        /// <response code="200">Returns the list of countries with the competitions successfully scraped or retrieved from the database.</response>
        /// <response code="500">If there is an error while processing the request, such as a problem with the server or unexpected exception.</response>
        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<CountryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<CountryResponse>>> GetCountriesAsync(
            [FromBody] IEnumerable<CountryRequest> countries,
            [FromQuery] bool forceScraping,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _countryService.GetCountriesAsync(countries, forceScraping, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
