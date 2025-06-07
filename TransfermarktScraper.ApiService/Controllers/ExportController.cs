using System;
using System.Threading;
using System.Threading.Tasks;
using DnsClient.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TransfermarktScraper.Exporter.Services.Interfaces;

namespace TransfermarktScraper.ApiService.Controllers
{
    /// <summary>
    /// Handles data file exporting API requests.
    /// </summary>
    [Route("api/export")]
    [ApiController]
    public class ExportController : ControllerBase
    {
        private readonly ICountryCompetitionExporterService _countryCompetitionExporterService;
        private readonly IClubPlayerExporterService _clubPlayerExporterService;
        private readonly IPlayerStatExporterService _playerStatExporterService;
        private readonly IMasterExporterService _masterExporterService;
        private readonly ILogger<ExportController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="countryCompetitionExporterService">The country-competition export data service.</param>
        /// <param name="clubPlayerExporterService">The club-player export data service.</param>
        /// <param name="playerStatExporterService">The player stats export data service.</param>
        /// <param name="playerStatExporterService">The master export data service.</param>
        public ExportController(
            ICountryCompetitionExporterService countryCompetitionExporterService,
            IClubPlayerExporterService clubPlayerExporterService,
            IPlayerStatExporterService playerStatExporterService,
            IMasterExporterService masterExporterService,
            ILogger<ExportController> logger)
        {
            _countryCompetitionExporterService = countryCompetitionExporterService;
            _clubPlayerExporterService = clubPlayerExporterService;
            _playerStatExporterService = playerStatExporterService;
            _masterExporterService = masterExporterService;
            _logger = logger;
        }

        /// <summary>
        /// Exports a list of countries and subentities in the specified file format.
        /// </summary>
        /// <param name="format">The export file format (e.g., "csv", "json", "xml").</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// <response code="200">A <see cref="FileContentResult"/> containing the exported file content.</response>
        /// <response code="400">A <see cref="BadRequestObjectResult"/> if the format is invalid</response>
        /// <response code="500">A <see cref="ObjectResult"/> with status 500 for unexpected errors.</response>
        [HttpGet("countries")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ExportCountryCompetitionDataAsync(
            [FromQuery] string format,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Received request to export countries to {Format} file.", format);

                var result = await _countryCompetitionExporterService.ExportCountryCompetitionDataAsync(format, cancellationToken);

                return File(result.Bytes, result.Format, result.Name);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected Error.");
                return Problem(ex.Message);
            }
        }

        /// <summary>
        /// Exports a list of clubs and subentities in the specified file format.
        /// </summary>
        /// <param name="format">The export file format (e.g., "csv", "json", "xml").</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// <response code="200">A <see cref="FileContentResult"/> containing the exported file content.</response>
        /// <response code="400">A <see cref="BadRequestObjectResult"/> if the format is invalid</response>
        /// <response code="500">A <see cref="ObjectResult"/> with status 500 for unexpected errors.</response>
        [HttpGet("clubs")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ExportClubPlayerDataAsync(
            [FromQuery] string format,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Received request to export clubs to {Format} file.", format);

                var result = await _clubPlayerExporterService.ExportClubPlayerDataAsync(format, cancellationToken);

                return File(result.Bytes, result.Format, result.Name);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected Error.");
                return Problem(ex.Message);
            }
        }

        /// <summary>
        /// Exports a list of player stats and subentities in the specified file format.
        /// </summary>
        /// <param name="format">The export file format (e.g., "csv", "json", "xml").</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// <response code="200">A <see cref="FileContentResult"/> containing the exported file content.</response>
        /// <response code="400">A <see cref="BadRequestObjectResult"/> if the format is invalid</response>
        /// <response code="500">A <see cref="ObjectResult"/> with status 500 for unexpected errors.</response>
        [HttpGet("player-stats")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ExportPlayerStatDataAsync(
            [FromQuery] string format,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Received request to export player stats to {Format} file.", format);

                var result = await _playerStatExporterService.ExportPlayerStatDataAsync(format, cancellationToken);

                return File(result.Bytes, result.Format, result.Name);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected Error.");
                return Problem(ex.Message);
            }
        }

        /// <summary>
        /// Exports a zip containing all exported files with all data available in the database.
        /// </summary>
        /// <param name="format">The export file format (e.g., "csv", "json", "xml").</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// <response code="200">A <see cref="FileContentResult"/> containing the exported file content.</response>
        /// <response code="400">A <see cref="BadRequestObjectResult"/> if the format is invalid</response>
        /// <response code="500">A <see cref="ObjectResult"/> with status 500 for unexpected errors.</response>
        [HttpGet("master")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ExportMasterAsync(
            [FromQuery] string format,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Received request to export master data to {Format} file.", format);

                var result = await _masterExporterService.ExportMasterDataAsync(format, cancellationToken);

                return File(result.Bytes, result.Format, result.Name);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected Error.");
                return Problem(ex.Message);
            }
        }
    }
}
