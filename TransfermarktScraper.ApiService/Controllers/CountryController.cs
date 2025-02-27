using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TransfermarktScraper.BLL.Services.Interfaces;
using TransfermarktScraper.Domain.DTOs.Response;

namespace TransfermarktScraper.ApiService.Controllers
{
    /// <summary>
    /// Handles country-related API requests.
    /// </summary>
    [Route("api/country")]
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
        /// Scrapes country data and returns the list of countries scraped.
        /// </summary>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing a list of <see cref="Country"/> objects.
        /// </returns>
        /// <response code="200">Returns the list of countries successfully scraped.</response>
        /// <response code="500">If there is an error while scraping the countries.</response>
        /// <response code="503">If there is an error when requesting a transfermarkt page.</response>
        [HttpPost("scrape")]
        [ProducesResponseType(typeof(List<Country>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<List<Country>>> ScrapeCountriesAsync()
        {
            try
            {
                return Ok(await _countryService.ScrapeCountriesAsync());
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
