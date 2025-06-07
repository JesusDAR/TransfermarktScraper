using System.IO.Compression;
using TransfermarktScraper.Domain.DTOs.Response.Exporter;
using TransfermarktScraper.Exporter.Services.Interfaces;

namespace TransfermarktScraper.Exporter.Services.Impl
{
    /// <inheritdoc/>
    public class MasterExporterService : IMasterExporterService
    {
        private readonly ICountryCompetitionExporterService _countryCompetitionExporterService;
        private readonly IClubPlayerExporterService _clubPlayerExporterService;
        private readonly IPlayerStatExporterService _playerStatExporterService;

        public MasterExporterService(
            ICountryCompetitionExporterService countryCompetitionExporterService,
            IClubPlayerExporterService clubPlayerExporterService,
            IPlayerStatExporterService playerStatExporterService)
        {
            _countryCompetitionExporterService = countryCompetitionExporterService;
            _clubPlayerExporterService = clubPlayerExporterService;
            _playerStatExporterService = playerStatExporterService;
        }

        /// <inheritdoc/>
        public async Task<FileResponse> ExportMasterDataAsync(string format, CancellationToken cancellationToken)
        {
            format = format.ToLowerInvariant();

            var countryCompetitionData = await _countryCompetitionExporterService.ExportCountryCompetitionDataAsync(format, cancellationToken);
            var clubPlayerData = await _clubPlayerExporterService.ExportClubPlayerDataAsync(format, cancellationToken);
            var playerStatData = await _playerStatExporterService.ExportPlayerStatDataAsync(format, cancellationToken);

            using var zipStream = new MemoryStream();
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                AddEntry(archive, countryCompetitionData);
                AddEntry(archive, clubPlayerData);
                AddEntry(archive, playerStatData);
            }

            zipStream.Seek(0, SeekOrigin.Begin);

            return new FileResponse
            {
                Bytes = zipStream.ToArray(),
                Format = "application/zip",
                Name = "MasterData.zip"
            };
        }

        /// <summary>
        /// Adds a new file entry to the specified ZIP archive using the given <see cref="FileResponse"/>.
        /// </summary>
        /// <param name="archive">The <see cref="ZipArchive"/> to which the file entry will be added.</param>
        /// <param name="file">The <see cref="FileResponse"/> containing the file name and byte content to add.</param>
        private void AddEntry(ZipArchive archive, FileResponse file)
        {
            var entry = archive.CreateEntry(file.Name, CompressionLevel.Optimal);
            using var entryStream = entry.Open();
            entryStream.Write(file.Bytes, 0, file.Bytes.Length);
        }
    }
}
