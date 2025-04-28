using Mapster;
using TransfermarktScraper.Domain.DTOs.Response;
using TransfermarktScraper.Domain.Entities;

namespace TransfermarktScraper.BLL.Mappers
{
    /// <summary>
    /// Represents the market value mapping profile for Mapster.
    /// </summary>
    public class MarketValueMapping : IRegister
    {
        /// <summary>
        /// Registers bidirectional mappings between <see cref="MarketValue"/>
        /// and <see cref="MarketValueResponse"/> using Mapster.
        /// </summary>
        /// <param name="config">The Mapster configuration instance.</param>
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<MarketValue, MarketValueResponse>();
            config.NewConfig<MarketValueResponse, MarketValue>();
        }
    }
}