using Mapster;
using TransfermarktScraper.Domain.DTOs.Request.Scraper;
using TransfermarktScraper.Domain.DTOs.Response.Scraper;
using TransfermarktScraper.Domain.Entities;

namespace TransfermarktScraper.Domain.Mappings.Scraper
{
    /// <summary>
    /// Represents the country mapping profile for Mapster.
    /// </summary>
    public class CountryMapping : IRegister
    {
        /// <summary>
        /// Registers bidirectional mappings between <see cref="Country"/>
        /// and <see cref="CountryResponse"/> using Mapster.
        /// </summary>
        /// <param name="config">The Mapster configuration instance.</param>
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Country, CountryResponse>();
            config.NewConfig<CountryResponse, Country>();

            config.NewConfig<CountryRequest, CountryResponse>();
            config.NewConfig<CountryResponse, CountryRequest>();
        }
    }
}