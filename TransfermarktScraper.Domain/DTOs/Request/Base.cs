namespace TransfermarktScraper.Domain.DTOs.Request
{
    /// <summary>
    /// Represents the base request DTO.
    /// </summary>
    public class Base
    {
        /// <summary>
        /// Gets or sets the unique Transfermarkt identifier.
        /// </summary>
        required public string TransfermarktId { get; set; }
    }
}
