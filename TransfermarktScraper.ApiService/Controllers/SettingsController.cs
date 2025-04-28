using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TransfermarktScraper.BLL.Services.Interfaces;
using TransfermarktScraper.Domain.DTOs.Response;

namespace TransfermarktScraper.ApiService.Controllers
{
    /// <summary>
    /// Handles settings-related API requests.
    /// </summary>
    [Route("api/settings")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingsService _settingsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsController"/> class.
        /// </summary>
        /// <param name="settingsService">The service for managing the application settings.</param>
        public SettingsController(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        /// <summary>
        /// Retrieves the current settings.
        /// </summary>
        /// <returns>
        /// An <see cref="ActionResult{Settings}"/> containing the current settings if successful, or a 500 error if an exception occurs.
        /// </returns>
        /// <response code="200">The current settings were successfully retrieved.</response>
        /// <response code="500">An internal server error occurred while retrieving the settings.</response>
        [HttpGet]
        [ProducesResponseType(typeof(SettingsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<SettingsResponse> GetSettings()
        {
            try
            {
                var result = _settingsService.GetSettings();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        /// <summary>
        /// Updates the headless mode of the scraper.
        /// </summary>
        /// <param name="isHeadlessMode">
        /// A boolean indicating whether to enable headless mode.
        /// True to enable headless mode; false to disable it.
        /// </param>
        /// <returns>
        /// 200 OK if successful.
        /// 500 Internal Server Error if an unexpected error occurs.
        /// </returns>
        [HttpPost("headless-mode/{isHeadlessMode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult SetHeadlessMode(bool isHeadlessMode)
        {
            try
            {
                _settingsService.SetHeadlessMode(isHeadlessMode);
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        /// <summary>
        /// Updates the number of countries to scrape.
        /// </summary>
        /// <param name="countriesCountToScrape">
        /// The number of countries to scrape. Must be zero or greater.
        /// Zero to scrape all available countries.
        /// </param>
        /// <returns>
        /// 200 OK if successful.
        /// 400 Bad Request if the value is invalid.
        /// 500 Internal Server Error if an unexpected error occurs.
        /// </returns>
        [HttpPost("countries-to-scrape/{countriesCountToScrape}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult SetCountriesCountToScrape(int countriesCountToScrape)
        {
            try
            {
                _settingsService.SetCountriesCountToScrape(countriesCountToScrape);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        /// <summary>
        /// Enables or disables the force scraping for the fetching of data.
        /// </summary>
        /// <param name="isForceScraping">
        /// A boolean indicating whether to always force the scraping.
        /// True to enable always scraping; false to get the data from database whenever it exists.
        /// </param>
        /// <returns>
        /// 200 OK if successful.
        /// 500 Internal Server Error if an unexpected error occurs.
        /// </returns>
        [HttpPost("force-scraping/{isForceScraping}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult SetForceScraping(bool isForceScraping)
        {
            try
            {
                _settingsService.SetForceScraping(isForceScraping);
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
