namespace TransfermarktScraper.Domain.DTOs.Response
{
    public class Settings
    {
        public bool isHeadlessMode { get; set; }

        public bool isForceScraping { get; set; }

        public int CountriesCountToScrape { get; set; }
    }
}
