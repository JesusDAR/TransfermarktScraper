using System.Text.Json.Serialization;

namespace TransfermarktScraper.Scraper.Models.MarketValue
{
    /// <summary>
    /// Represents the market value result for a player from Transfermarkt's market value graph.
    /// </summary>
    public class MarketValueResult
    {
        /// <summary>
        /// Gets or sets the collection of historical market value entries for a player.
        /// </summary>
        [JsonPropertyName("list")]
        public IList<MarketValueItemResult>? MarketValueItemResults { get; set; }
    }
}
