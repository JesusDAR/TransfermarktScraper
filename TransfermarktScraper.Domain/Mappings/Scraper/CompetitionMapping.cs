using Mapster;
using TransfermarktScraper.Domain.DTOs.Response.Scraper;
using TransfermarktScraper.Domain.Entities;
using TransfermarktScraper.Domain.Enums.Extensions;
using TransfermarktScraper.Domain.Utils.DTO;

namespace TransfermarktScraper.Domain.Mappings.Scraper
{
    /// <summary>
    /// Represents the competition mapping profile for Mapster.
    /// </summary>
    public class CompetitionMapping : IRegister
    {
        /// <summary>
        /// Registers bidirectional mappings between <see cref=Competition"/>
        /// and <see cref="CompetitionResponse"/> using Mapster.
        /// </summary>
        /// <param name="config">The Mapster configuration instance.</param>
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Competition, CompetitionResponse>()
                .Map(dest => dest.Tier, src => TierExtensions.ToString(src.Tier))
                .Map(dest => dest.Cup, src => CupExtensions.ToString(src.Cup))
                .Map(dest => dest.MarketValue, src => MoneyUtils.ConvertToString(src.MarketValue))
                .Map(dest => dest.MarketValueAverage, src => MoneyUtils.ConvertToString(src.MarketValueAverage));

            config.NewConfig<CompetitionResponse, Competition>()
                .Map(dest => dest.Tier, src => TierExtensions.ToEnum(src.Tier))
                .Map(dest => dest.Cup, src => CupExtensions.ToEnum(src.Cup))
                .Map(dest => dest.MarketValue, src => MoneyUtils.ConvertToFloat(src.MarketValue))
                .Map(dest => dest.MarketValueAverage, src => MoneyUtils.ConvertToFloat(src.MarketValueAverage));
        }
    }
}