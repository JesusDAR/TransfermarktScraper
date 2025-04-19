using Mapster;

namespace TransfermarktScraper.BLL.Mappers
{
    /// <summary>
    /// Represents the market value mapping profile for Mapster.
    /// </summary>
    public class MarketValueMapping : IRegister
    {
        /// <summary>
        /// Registers bidirectional mappings between <see cref="Domain.Entities.MarketValue"/>
        /// and <see cref="Domain.DTOs.Response.MarketValue"/> using Mapster.
        /// </summary>
        /// <param name="config">The Mapster configuration instance.</param>
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Domain.Entities.MarketValue, Domain.DTOs.Response.MarketValue>();
            config.NewConfig<Domain.DTOs.Response.MarketValue, Domain.Entities.MarketValue>();
        }
    }
}