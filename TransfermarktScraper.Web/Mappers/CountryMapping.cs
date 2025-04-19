using Mapster;

namespace TransfermarktScraper.Web.Mappers
{
    /// <summary>
    /// Represents the country mapping profile for Mapster.
    /// </summary>
    public class CountryMapping : IRegister
    {
        /// <summary>
        /// Registers bidirectional mappings between <see cref="Domain.DTOs.Request.Country"/>
        /// and <see cref="Domain.DTOs.Response.Country"/> using Mapster.
        /// </summary>
        /// <param name="config">The Mapster configuration instance.</param>
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Domain.DTOs.Request.Country, Domain.DTOs.Response.Country>();
            config.NewConfig<Domain.DTOs.Response.Country, Domain.DTOs.Request.Country>();
        }
    }
}
