using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TransfermarktScraper.Domain.DTOs.Response.Scraper;

namespace TransfermarktScraper.ApiService.Controllers
{
    /// <summary>
    /// Handles settings-related API requests.
    /// </summary>
    [Route("api/settings")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly Scraper.Services.Interfaces.ISettingsService _scraperSettingsService;
        private readonly Exporter.Services.Interfaces.ISettingsService _exporterSettingsService;
        private readonly ILogger<SettingsController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsController"/> class.
        /// </summary>
        /// <param name="scraperSettingsService">The service for managing the scraper settings.</param>
        /// <param name="exporterSettingsService">The service for managing the exporter settings.</param>
        /// <param name="logger">The logger.</param>
        public SettingsController(
            Scraper.Services.Interfaces.ISettingsService scraperSettingsService,
            Exporter.Services.Interfaces.ISettingsService exporterSettingsService,
            ILogger<SettingsController> logger)
        {
            _scraperSettingsService = scraperSettingsService;
            _exporterSettingsService = exporterSettingsService;
            _logger = logger;
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
                _logger.LogInformation("Received request to get settings.");

                var result = _scraperSettingsService.GetSettings();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected Error on {MethodName}. Message: {Message}", nameof(GetSettings), ex.Message);
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
        [HttpPost("scraper/headless-mode/{isHeadlessMode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult SetHeadlessMode(bool isHeadlessMode)
        {
            try
            {
                _scraperSettingsService.SetHeadlessMode(isHeadlessMode);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected Error on {MethodName}. Message: {Message}", nameof(SetHeadlessMode), ex.Message);
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
        [HttpPost("scraper/countries-to-scrape/{countriesCountToScrape}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult SetCountriesCountToScrape(int countriesCountToScrape)
        {
            try
            {
                _scraperSettingsService.SetCountriesCountToScrape(countriesCountToScrape);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected Error on {MethodName}. Message: {Message}", nameof(SetCountriesCountToScrape), ex.Message);
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
        [HttpPost("scraper/force-scraping/{isForceScraping}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult SetForceScraping(bool isForceScraping)
        {
            try
            {
                _scraperSettingsService.SetForceScraping(isForceScraping);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected Error on {MethodName}. Message: {Message}", nameof(SetForceScraping), ex.Message);
                return Problem(ex.Message);
            }
        }

        /// <summary>
        /// Gets the base flag URL to be used by the UI to display the nationalities flags of the players.
        /// </summary>
        /// <returns>
        /// 200 OK if successful. With a list of the supported formats.
        /// 500 Internal Server Error if an unexpected error occurs.
        /// </returns>
        [HttpGet("scraper/flag-url")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<string> GetFlagUrl()
        {
            try
            {
                var result = _scraperSettingsService.GetFlagUrl();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected Error on {MethodName}. Message: {Message}", nameof(GetFlagUrl), ex.Message);
                return Problem(ex.Message);
            }
        }

        /// <summary>
        /// Gets the list of supported format for the exporter.
        /// </summary>
        /// <returns>
        /// 200 OK if successful.
        /// 500 Internal Server Error if an unexpected error occurs.
        /// </returns>
        [HttpGet("exporter/supported-formats")]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<string>> GetSupportedFormats()
        {
            try
            {
                var result = _exporterSettingsService.GetSupportedFormats();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected Error on {MethodName}. Message: {Message}", nameof(GetSupportedFormats), ex.Message);
                return Problem(ex.Message);
            }
        }
    }
}
