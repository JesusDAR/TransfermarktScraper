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
    public class CountryCompetitionExporterService : ICountryCompetitionExporterService
    {
        private readonly ICountryRepository _countryRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CountryCompetitionExporterService"/> class.
        /// </summary>
        /// <param name="countryRepository">The country repository for accessing and managing the country data.</param>
        public CountryCompetitionExporterService(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        /// <inheritdoc/>
        public async Task<FileResponse> ExportCountryCompetitionDataAsync(string format, CancellationToken cancellationToken)
        {
            var countries = await _countryRepository.GetAllAsync(cancellationToken);

            format = format.ToLowerInvariant();

            return format switch
            {
                "json" => JsonExporterExtension.ExportToJson(countries, "CountriesCompetitions.json"),
                "csv" => ExportToCsv(countries, "CountriesCompetitions.csv"),
                "xlsx" => ExportToXlsx(countries, "CountriesCompetitions.xlsx"),
                _ => throw new InvalidOperationException($"Format not allowed: {format}. Supported formats: json, csv, xlsx")
            };
        }

        /// <summary>
        /// Exports a collection of countries and their competitions to a CSV file.
        /// </summary>
        /// <param name="countries">A collection of <see cref="Country"/> objects containing the country and competition data.</param>
        /// <param name="name">The name of the exported CSV file.</param>
        /// <returns>
        /// A <see cref="FileResponse"/> object that contains the CSV data as a byte array, the MIME type as <c>text/csv</c>,
        /// and the specified file name.
        /// </returns>
        private static FileResponse ExportToCsv(IEnumerable<Country> countries, string name)
        {
            var properties = typeof(CountryCompetitionData).GetProperties();
            var csv = new StringBuilder();

            // Header
            var headers = string.Join(",", properties.Select(property => property.Name));
            csv.AppendLine(headers);

            // Data
            foreach (var country in countries)
            {
                if (country.Competitions != null && country.Competitions.Any())
                {
                    foreach (var competition in country.Competitions)
                    {
                        var countryCompetitionData = (country, competition).Adapt<CountryCompetitionData>();

                        var countryCompetitionDataValues = properties
                            .Select(property => property.GetValue(countryCompetitionData))
                            .AsEnumerable();

                        csv.AppendLine(string.Join(",", countryCompetitionDataValues));
                    } 
                }
                else 
                {
                    var countryData = country.Adapt<CountryCompetitionData>();

                    var countryDataValues = properties
                        .Select(property => property.GetValue(countryData))
                        .AsEnumerable();

                    csv.AppendLine(string.Join(",", countryDataValues));
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
        /// Exports a list of countries and their competitions to an Excel (.xlsx) file.
        /// Each row in the generated spreadsheet represents a combination of a country and one of its competitions.
        /// </summary>
        /// <param name="countries">A collection of countries, each containing one or more competitions.</param>
        /// <param name="name">The name to assign to the resulting Excel file.</param>
        /// <returns>
        /// A <see cref="FileResponse"/> containing the binary content of the Excel file,
        /// its MIME type, and the specified file name.
        /// </returns>
        private static FileResponse ExportToXlsx(IEnumerable<Country> countries, string name) 
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("CountriesCompetitions");

                var properties = typeof(CountryCompetitionData).GetProperties();

                // Header
                for (int column = 0; column < properties.Length; column++)
                {
                    worksheet.Cell(1, column + 1).Value = properties[column].Name;
                }

                // Data
                int row = 2;
                foreach (var country in countries)
                {
                    if (country.Competitions != null && country.Competitions.Any())
                    {
                        foreach (var competition in country.Competitions)
                        {
                            var countryCompetitionData = (country, competition).Adapt<CountryCompetitionData>();

                            for (int column = 0; column < properties.Length; column++)
                            {
                                var value = properties[column].GetValue(countryCompetitionData);
                                worksheet.Cell(row, column + 1).Value = value?.ToString();
                            }

                            row++;
                        } 
                    }
                    else
                    {
                        var countryData = country.Adapt<CountryCompetitionData>();

                        for (int column = 0; column < properties.Length; column++)
                        {
                            var value = properties[column].GetValue(countryData);
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
