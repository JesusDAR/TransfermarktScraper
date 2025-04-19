using Mapster;
using TransfermarktScraper.BLL.Utils;
using TransfermarktScraper.Domain.Enums.Extensions;

namespace TransfermarktScraper.BLL.Mappers
{
    /// <summary>
    /// Represents the competition mapping profile for Mapster.
    /// </summary>
    public class CompetitionMapping : IRegister
    {
        /// <summary>
        /// Registers bidirectional mappings between <see cref="Domain.Entities.Competition"/>
        /// and <see cref="Domain.DTOs.Response.Competition"/> using Mapster.
        /// </summary>
        /// <param name="config">The Mapster configuration instance.</param>
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Domain.Entities.Competition, Domain.DTOs.Response.Competition>()
                .Map(dest => dest.Tier, src => TierExtensions.ToString(src.Tier))
                .Map(dest => dest.Cup, src => CupExtensions.ToString(src.Cup))
                .Map(dest => dest.MarketValue, src => MoneyUtils.ConvertToString(src.MarketValue))
                .Map(dest => dest.MarketValueAverage, src => MoneyUtils.ConvertToString(src.MarketValueAverage));

            config.NewConfig<Domain.DTOs.Response.Competition, Domain.Entities.Competition>()
                .Map(dest => dest.Tier, src => TierExtensions.ToEnum(src.Tier))
                .Map(dest => dest.Cup, src => CupExtensions.ToEnum(src.Cup))
                .Map(dest => dest.MarketValue, src => MoneyUtils.ConvertToFloat(src.MarketValue))
                .Map(dest => dest.MarketValueAverage, src => MoneyUtils.ConvertToFloat(src.MarketValueAverage));
        }
    }
}