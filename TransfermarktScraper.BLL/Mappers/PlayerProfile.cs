using AutoMapper;
using Microsoft.Extensions.Options;
using TransfermarktScraper.BLL.Configuration;
using TransfermarktScraper.BLL.Utils;
using TransfermarktScraper.Domain.Enums.Extensions;

namespace TransfermarktScraper.BLL.Mappers
{
    /// <summary>
    /// Represents the player mapping profile for AutoMapper.
    /// </summary>
    public class PlayerProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerProfile"/> class.
        /// </summary>
        /// <param name="scraperSettings">The scraper settings containing configuration values.</param>
        public PlayerProfile(IOptions<ScraperSettings> scraperSettings)
        {
            var tinyFlagUrl = scraperSettings.Value.TinyFlagUrl;

            CreateMap<Domain.Entities.Player, Domain.DTOs.Response.Player>()
                .ForMember(destination => destination.Foot, options => options.MapFrom(source => FootExtensions.ToString(source.Foot)))
                .ForMember(destination => destination.Position, options => options.MapFrom(source => PositionExtensions.ToString(source.Position)))
                .ForMember(destination => destination.MarketValue, options => options.MapFrom(source => MoneyUtils.ConvertToString(source.MarketValue)))
                .ForMember(destination => destination.Nationalities, options => options.MapFrom(source => ImageUtils.ConvertCountryTransfermarktIdsToImageUrls(source.Nationalities, tinyFlagUrl)));
            CreateMap<Domain.DTOs.Response.Player, Domain.Entities.Player>()
                .ForMember(destination => destination.Foot, options => options.MapFrom(source => TierExtensions.ToEnum(source.Foot)))
                .ForMember(destination => destination.Position, options => options.MapFrom(source => CupExtensions.ToEnum(source.Position)))
                .ForMember(destination => destination.MarketValue, options => options.MapFrom(source => MoneyUtils.ConvertToFloat(source.MarketValue)))
                .ForMember(destination => destination.Nationalities, options => options.MapFrom(source => ImageUtils.ConvertImageUrlsToCountryTransfermarktIds(source.Nationalities)));
        }
    }
}
