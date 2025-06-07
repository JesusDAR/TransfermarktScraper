using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TransfermarktScraper.Exporter.Configuration;
using TransfermarktScraper.Exporter.Services.Interfaces;

namespace TransfermarktScraper.Exporter.Services.Impl
{
    /// <inheritdoc/>
    public class SettingsService : ISettingsService
    {
        private readonly ExporterSettings _exporterSettings;
        private readonly ILogger<SettingsService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsService"/> class.
        /// </summary>
        /// <param name="exporterSettings">The exporter settings containing configuration values.</param>
        /// <param name="logger">The logger.</param>
        public SettingsService(IOptions<ExporterSettings> exporterSettings, ILogger<SettingsService> logger)
        {
            _exporterSettings = exporterSettings.Value;
            _logger = logger;
        }

        /// <inheritdoc/>
        public IEnumerable<string> GetSupportedFormats()
        {
            _logger.LogInformation("Getting exporter supporter formats...");

            return _exporterSettings.SupportedFormats;
        }
    }
}
