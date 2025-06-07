using ClosedXML.Excel;
using Mapster;
using System.Text;
using TransfermarktScraper.Data.Repositories.Interfaces;
using TransfermarktScraper.Domain.DTOs.Response.Exporter;
using TransfermarktScraper.Domain.Entities;
using TransfermarktScraper.Exporter.Services.Extensions;
using TransfermarktScraper.Exporter.Services.Interfaces;

namespace TransfermarktScraper.Exporter.Services.Impl
{
    /// <inheritdoc/>
    public class ClubPlayerExporterService : IClubPlayerExporterService
    {
        private readonly IClubRepository _clubRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClubPlayerExporterService"/> class.
        /// </summary>
        /// <param name="clubRepository">The club repository for accessing and managing the club data.</param>
        public ClubPlayerExporterService(IClubRepository clubRepository)
        {
            _clubRepository = clubRepository;
        }

        /// <inheritdoc/>
        public async Task<FileResponse> ExportClubPlayerDataAsync(string format, CancellationToken cancellationToken)
        {
            var clubs = await _clubRepository.GetAllAsync(cancellationToken);

            format = format.ToLowerInvariant();

            return format switch
            {
                "json" => JsonExporterExtension.ExportToJson(clubs, "ClubsPlayers.json"),
                "csv" => ExportToCsv(clubs, "ClubsPlayers.csv"),
                "xlsx" => ExportToXlsx(clubs, "ClubsPlayers.xlsx"),
                _ => throw new InvalidOperationException($"Format not allowed: {format}. Supported formats: json, csv, xlsx")
            };
        }

        /// <summary>
        /// Exports a collection of clubs and their players to a CSV file.
        /// </summary>
        /// <param name="clubs">A collection of <see cref="Club"/> objects containing the club and player data.</param>
        /// <param name="name">The name of the exported CSV file.</param>
        /// <returns>
        /// A <see cref="FileResponse"/> object that contains the CSV data as a byte array, the MIME type as <c>text/csv</c>,
        /// and the specified file name.
        /// </returns>
        private static FileResponse ExportToCsv(IEnumerable<Club> clubs, string name)
        {
            var properties = typeof(ClubPlayerData).GetProperties();
            var csv = new StringBuilder();

            // Header
            var headers = string.Join(",", properties.Select(property => property.Name));
            csv.AppendLine(headers);

            // Data
            foreach (var club in clubs)
            {
                if (club.Players != null && club.Players.Any())
                {
                    foreach (var player in club.Players)
                    {
                        var clubPlayerData = (club, player).Adapt<ClubPlayerData>();

                        var clubPlayerDataValues = properties
                            .Select(property => property.GetValue(clubPlayerData))
                            .AsEnumerable();

                        csv.AppendLine(string.Join(",", clubPlayerDataValues));
                    } 
                }
                else
                {
                    var clubData = club.Adapt<ClubPlayerData>();

                    var clubDataValues = properties
                        .Select(property => property.GetValue(clubData))
                        .AsEnumerable();

                    csv.AppendLine(string.Join(",", clubDataValues));
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
        /// Exports a list of clubs and their players to an Excel (.xlsx) file.
        /// Each row in the generated spreadsheet represents a combination of a club and one of its players.
        /// </summary>
        /// <param name="clubs">A collection of clubs, each containing one or more players.</param>
        /// <param name="name">The name to assign to the resulting Excel file.</param>
        /// <returns>
        /// A <see cref="FileResponse"/> containing the binary content of the Excel file,
        /// its MIME type, and the specified file name.
        /// </returns>
        private static FileResponse ExportToXlsx(IEnumerable<Club> clubs, string name)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("ClubsPlayers");

                var properties = typeof(ClubPlayerData).GetProperties();

                // Header
                for (int column = 0; column < properties.Length; column++)
                {
                    worksheet.Cell(1, column + 1).Value = properties[column].Name;
                }

                // Data
                int row = 2;
                foreach (var club in clubs)
                {
                    if (club.Players != null && club.Players.Any())
                    {
                        foreach (var player in club.Players)
                        {
                            var ClubPlayerData = (club, player).Adapt<ClubPlayerData>();

                            for (int column = 0; column < properties.Length; column++)
                            {
                                var value = properties[column].GetValue(ClubPlayerData);
                                worksheet.Cell(row, column + 1).Value = value?.ToString();
                            }

                            row++;
                        } 
                    }
                    else
                    {
                        var clubData = club.Adapt<ClubPlayerData>();

                        for (int column = 0; column < properties.Length; column++)
                        {
                            var value = properties[column].GetValue(clubData);
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
