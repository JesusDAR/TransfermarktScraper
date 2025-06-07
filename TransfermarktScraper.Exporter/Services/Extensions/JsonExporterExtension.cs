using System.Text;
using System.Text.Json;
using TransfermarktScraper.Domain.DTOs.Response.Exporter;

namespace TransfermarktScraper.Exporter.Services.Extensions
{
    /// <summary>
    /// Provides extension methods for exporting data in JSON format.
    /// </summary>
    public static class JsonExporterExtension
    {

        /// <summary>
        /// Exports a collection of data to a JSON file.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the data collection.</typeparam>
        /// <param name="data">The data to be exported.</param>
        /// <param name="name">The name of the resulting file (including extension).</param>
        /// <returns>A <see cref="FileResponse"/> containing the serialized JSON data and metadata.</returns>
        public static FileResponse ExportToJson<T>(IEnumerable<T> data, string name)
        {
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });

            var bytes = Encoding.UTF8.GetBytes(json);

            var fileResponse = new FileResponse()
            {
                Bytes = bytes,
                Format = "application/json",
                Name = name,
            };

            return fileResponse;
        }
    }
}
