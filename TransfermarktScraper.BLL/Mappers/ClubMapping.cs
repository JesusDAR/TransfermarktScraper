using Mapster;
using TransfermarktScraper.BLL.Utils;

namespace TransfermarktScraper.BLL.Mappers
{
    /// <summary>
    /// Represents the club mapping profile for Mapster.
    /// </summary>
    public class ClubMapping : IRegister
    {
        /// <summary>
        /// Registers bidirectional mappings between <see cref="Domain.Entities.Club"/>
        /// and <see cref="Domain.DTOs.Response.Club"/> using Mapster.
        /// </summary>
        /// <param name="config">The Mapster configuration instance.</param>
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Domain.Entities.Club, Domain.DTOs.Response.Club>()
                .Map(dest => dest.MarketValue, src => MoneyUtils.ConvertToString(src.MarketValue))
                .Map(dest => dest.MarketValueAverage, src => MoneyUtils.ConvertToString(src.MarketValueAverage));

            config.NewConfig<Domain.DTOs.Response.Club, Domain.Entities.Club>()
                .Map(dest => dest.MarketValue, src => MoneyUtils.ConvertToFloat(src.MarketValue))
                .Map(dest => dest.MarketValueAverage, src => MoneyUtils.ConvertToFloat(src.MarketValueAverage));
        }
    }
}