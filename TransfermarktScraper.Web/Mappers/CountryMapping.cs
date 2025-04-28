using Mapster;
using TransfermarktScraper.Domain.DTOs.Request;
using TransfermarktScraper.Domain.DTOs.Response;

namespace TransfermarktScraper.Web.Mappers
{
    /// <summary>
    /// Represents the country mapping profile for Mapster.
    /// </summary>
    public class CountryMapping : IRegister
    {
        /// <summary>
        /// Registers bidirectional mappings between <see cref="CountryRequest"/>
        /// and <see cref="CountryResponse"/> using Mapster.
        /// </summary>
        /// <param name="config">The Mapster configuration instance.</param>
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CountryRequest, CountryResponse>();
            config.NewConfig<CountryResponse, CountryRequest>();
        }
    }
}
