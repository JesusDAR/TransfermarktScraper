using ClosedXML.Excel;
using Mapster;
using System.Text;
using TransfermarktScraper.Data.Repositories.Interfaces;
using TransfermarktScraper.Domain.DTOs.Response.Exporter;
using TransfermarktScraper.Domain.Entities.Stat;
using TransfermarktScraper.Exporter.Services.Extensions;
using TransfermarktScraper.Exporter.Services.Interfaces;

namespace TransfermarktScraper.Exporter.Services.Impl
{
    /// <inheritdoc/>
    public class PlayerStatExporterService : IPlayerStatExporterService
    {
        private readonly IPlayerStatRepository _playerStatRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerStatExporterService"/> class.
        /// </summary>
        /// <param name="playerStatRepository">The player stat repository for accessing and managing the player stat data.</param>
        public PlayerStatExporterService(IPlayerStatRepository playerStatRepository)
        {
            _playerStatRepository = playerStatRepository;
        }

        /// <inheritdoc/>
        public async Task<FileResponse> ExportPlayerStatDataAsync(string format, CancellationToken cancellationToken)
        {
            var countries = await _playerStatRepository.GetAllAsync(cancellationToken);

            format = format.ToLowerInvariant();

            return format switch
            {
                "json" => JsonExporterExtension.ExportToJson(countries, "PlayersStats.json"),
                "csv" => ExportToCsv(countries, "PlayersStats.csv"),
                "xlsx" => ExportToXlsx(countries, "PlayersStats.xlsx"),
                _ => throw new InvalidOperationException($"Format not allowed: {format}. Supported formats: json, csv, xlsx")
            };
        }

        /// <summary>
        /// Exports a collection of players stats to a CSV file.
        /// </summary>
        /// <param name="playerStats">A collection of <see cref="PlayerStat"/> objects containing the player stat data.</param>
        /// <param name="name">The name of the exported CSV file.</param>
        /// <returns>
        /// A <see cref="FileResponse"/> object that contains the CSV data as a byte array, the MIME type as <c>text/csv</c>,
        /// and the specified file name.
        /// </returns>
        private FileResponse ExportToCsv(IEnumerable<PlayerStat> playerStats, string name)
        {
            var properties = typeof(PlayerStatData).GetProperties();
            var csv = new StringBuilder();

            // Header
            var headers = string.Join(",", properties.Select(property => property.Name));
            csv.AppendLine(headers);

            // Data
            foreach (var playerStat in playerStats)
            {
                if (playerStat.PlayerSeasonStats != null && playerStat.PlayerSeasonStats.Any() 
                    && playerStat.PlayerSeasonStats.Any(pss => pss.PlayerSeasonCompetitionStats != null && pss.PlayerSeasonCompetitionStats.Any())
                    && playerStat.PlayerSeasonStats.Any(pss => pss.PlayerSeasonCompetitionStats.Any(pscs => pscs.PlayerSeasonCompetitionMatchStats != null && pscs.PlayerSeasonCompetitionMatchStats.Any())))
                {
                    foreach (var playerSeasonStat in playerStat.PlayerSeasonStats)
                    {
                        foreach (var playerSeasonCompetitionStat in playerSeasonStat.PlayerSeasonCompetitionStats)
                        {
                            foreach (var playerSeasonCompetitionMatchStat in playerSeasonCompetitionStat.PlayerSeasonCompetitionMatchStats)
                            {
                                var playerStatData = (playerStat, playerSeasonStat, playerSeasonCompetitionStat, playerSeasonCompetitionMatchStat).Adapt<PlayerStatData>();

                                var playerStatDataValues = properties
                                    .Select(property => property.GetValue(playerStatData))
                                    .AsEnumerable();

                                csv.AppendLine(string.Join(",", playerStatDataValues)); 
                            }
                        }
                    }
                }
                else if (playerStat.PlayerSeasonStats != null && playerStat.PlayerSeasonStats.Any()
                    && playerStat.PlayerSeasonStats.Any(pss => pss.PlayerSeasonCompetitionStats != null && pss.PlayerSeasonCompetitionStats.Any()))
                {
                    foreach (var playerSeasonStat in playerStat.PlayerSeasonStats)
                    {
                        foreach (var playerSeasonCompetitionStat in playerSeasonStat.PlayerSeasonCompetitionStats)
                        {
                            var playerStatData = (playerStat, playerSeasonStat, playerSeasonCompetitionStat).Adapt<PlayerStatData>();

                            var playerStatDataValues = properties
                                .Select(property => property.GetValue(playerStatData))
                                .AsEnumerable();

                            csv.AppendLine(string.Join(",", playerStatDataValues)); 
                        }
                    }
                }
                else if (playerStat.PlayerSeasonStats != null && playerStat.PlayerSeasonStats.Any())
                {
                    foreach (var playerSeasonStat in playerStat.PlayerSeasonStats)
                    {
                        var playerStatData = (playerStat, playerSeasonStat).Adapt<PlayerStatData>();

                        var playerStatDataValues = properties
                            .Select(property => property.GetValue(playerStatData))
                            .AsEnumerable();

                        csv.AppendLine(string.Join(",", playerStatDataValues));
                    }
                }
                else
                {
                    var playerStatData = playerStat.Adapt<PlayerStat>();

                    var playerStatDataValues = properties
                        .Select(property => property.GetValue(playerStatData))
                        .AsEnumerable();

                    csv.AppendLine(string.Join(",", playerStatDataValues));
                }
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());

            var fileResponse = new FileResponse()
            {
                Bytes = bytes,
                Format = "text/csv",
                Name = name,
            };

            return fileResponse;
        }

        /// <summary>
        /// Exports a list of player stast to an Excel (.xlsx) file.
        /// </summary>
        /// <param name="playerStats">A collection of player stats, including per season, competition and match.</param>
        /// <param name="name">The name to assign to the resulting Excel file.</param>
        /// <returns>
        /// A <see cref="FileResponse"/> containing the binary content of the Excel file,
        /// its MIME type, and the specified file name.
        /// </returns>
        private static FileResponse ExportToXlsx(IEnumerable<PlayerStat> playerStats, string name)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("PlayersStats");

                var properties = typeof(ClubPlayerData).GetProperties();

                // Header
                for (int column = 0; column < properties.Length; column++)
                {
                    worksheet.Cell(1, column + 1).Value = properties[column].Name;
                }

                // Data
                int row = 2;
                foreach (var playerStat in playerStats)
                {
                    if (playerStat.PlayerSeasonStats != null && playerStat.PlayerSeasonStats.Any()
                        && playerStat.PlayerSeasonStats.Any(pss => pss.PlayerSeasonCompetitionStats != null && pss.PlayerSeasonCompetitionStats.Any())
                        && playerStat.PlayerSeasonStats.Any(pss => pss.PlayerSeasonCompetitionStats.Any(pscs => pscs.PlayerSeasonCompetitionMatchStats != null && pscs.PlayerSeasonCompetitionMatchStats.Any())))
                    {
                        foreach (var playerSeasonStat in playerStat.PlayerSeasonStats)
                        {
                            foreach (var playerSeasonCompetitionStat in playerSeasonStat.PlayerSeasonCompetitionStats)
                            {
                                foreach (var playerSeasonCompetitionMatchStat in playerSeasonCompetitionStat.PlayerSeasonCompetitionMatchStats)
                                {
                                    var playerStatData = (playerStat, playerSeasonStat, playerSeasonCompetitionStat, playerSeasonCompetitionMatchStat).Adapt<PlayerStatData>();

                                    for (int column = 0; column < properties.Length; column++)
                                    {
                                        var value = properties[column].GetValue(playerStatData);
                                        worksheet.Cell(row, column + 1).Value = value?.ToString();
                                    }

                                    row++;
                                }
                            }
                        }
                    }
                    else if (playerStat.PlayerSeasonStats != null && playerStat.PlayerSeasonStats.Any()
                        && playerStat.PlayerSeasonStats.Any(pss => pss.PlayerSeasonCompetitionStats != null && pss.PlayerSeasonCompetitionStats.Any()))
                    {
                        foreach (var playerSeasonStat in playerStat.PlayerSeasonStats)
                        {
                            foreach (var playerSeasonCompetitionStat in playerSeasonStat.PlayerSeasonCompetitionStats)
                            {
                                var playerStatData = (playerStat, playerSeasonStat, playerSeasonCompetitionStat).Adapt<PlayerStatData>();

                                for (int column = 0; column < properties.Length; column++)
                                {
                                    var value = properties[column].GetValue(playerStatData);
                                    worksheet.Cell(row, column + 1).Value = value?.ToString();
                                }

                                row++;
                            }
                        }
                    }
                    else if (playerStat.PlayerSeasonStats != null && playerStat.PlayerSeasonStats.Any())
                    {
                        foreach (var playerSeasonStat in playerStat.PlayerSeasonStats)
                        {
                            var playerStatData = (playerStat, playerSeasonStat).Adapt<PlayerStatData>();

                            for (int column = 0; column < properties.Length; column++)
                            {
                                var value = properties[column].GetValue(playerStatData);
                                worksheet.Cell(row, column + 1).Value = value?.ToString();
                            }

                            row++;
                        }
                    }
                    else
                    {
                        var playerStatData = playerStat.Adapt<PlayerStatData>();

                        for (int column = 0; column < properties.Length; column++)
                        {
                            var value = properties[column].GetValue(playerStatData);
                            worksheet.Cell(row, column + 1).Value = value?.ToString();
                        }

                        row++;
                    }
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var bytes = stream.ToArray();

                    return new FileResponse()
                    {
                        Bytes = bytes,
                        Format = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        Name = name
                    };
                }
            } 
        }
    }
}
