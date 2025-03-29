using System.Text.Json.Serialization;

namespace TransfermarktScraper.BLL.Models.MarketValue
{
    /// <summary>
    /// Represents a single market value entry for a player from Transfermarkt's market value graph.
    /// </summary>
    public class MarketValueItemResult
    {
        /// <summary>
        /// Gets or sets the market value with currency formatting (e.g., "€1.00m", "€500k").
        /// </summary>
        [JsonPropertyName("mw")]
        public string Mw { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the formatted date when this market value was recorded (e.g., "Jun 20, 2007").
        /// </summary>
        [JsonPropertyName("datum_mw")]
        public string DatumMw { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the club name the player was associated with at this market value.
        /// </summary>
        [JsonPropertyName("verein")]
        public string Verein { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the player's age at the time of this market value.
        /// </summary>
        [JsonPropertyName("age")]
        public string Age { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the URL to the club's crest.
        /// May be a default one if the player is retired.
        /// </summary>
        [JsonPropertyName("wappen")]
        public string Wappen { get; set; } = string.Empty;
    }
}
