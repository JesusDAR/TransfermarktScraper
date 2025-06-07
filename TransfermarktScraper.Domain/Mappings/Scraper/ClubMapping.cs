using Mapster;
using TransfermarktScraper.Domain.DTOs.Response.Scraper;
using TransfermarktScraper.Domain.Entities;
using TransfermarktScraper.Domain.Utils.DTO;

namespace TransfermarktScraper.Domain.Mappings.Scraper
{
    /// <summary>
    /// Represents the club mapping profile for Mapster.
    /// </summary>
    public class ClubMapping : IRegister
    {
        /// <summary>
        /// Registers bidirectional mappings between <see cref="Club"/>
        /// and <see cref="ClubResponse"/> using Mapster.
        /// </summary>
        /// <param name="config">The Mapster configuration instance.</param>
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Club, ClubResponse>()
                .Map(dest => dest.MarketValue, src => MoneyUtils.ConvertToString(src.MarketValue))
                .Map(dest => dest.MarketValueAverage, src => MoneyUtils.ConvertToString(src.MarketValueAverage));

            config.NewConfig<ClubResponse, Club>()
                .Map(dest => dest.MarketValue, src => MoneyUtils.ConvertToFloat(src.MarketValue))
                .Map(dest => dest.MarketValueAverage, src => MoneyUtils.ConvertToFloat(src.MarketValueAverage));
        }
    }
}