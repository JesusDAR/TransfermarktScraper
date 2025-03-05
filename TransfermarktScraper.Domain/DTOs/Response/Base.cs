namespace TransfermarktScraper.Domain.DTOs.Response
{
    /// <summary>
    /// Represents the response DTO.
    /// </summary>
    public class Base
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Gets or sets the unique Transfermarkt identifier.
        /// </summary>
        public string? TransfermarktId { get; set; }
    }
}
