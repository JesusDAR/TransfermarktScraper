using AutoMapper;
using TransfermarktScraper.Domain.Enums.Extensions;
using TransfermarktScraper.Domain.Utils;

namespace TransfermarktScraper.BLL.Mappers
{
    /// <summary>
    /// Represents the competition mapping profile for AutoMapper.
    /// </summary>
    public class CompetitionProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompetitionProfile"/> class.
        /// </summary>
        public CompetitionProfile()
        {
            CreateMap<Domain.Entities.Competition, Domain.DTOs.Response.Competition>()
                .ForMember(destination => destination.Tier, options => options.MapFrom(source => TierExtensions.ToString(source.Tier)))
                .ForMember(destination => destination.Cup, options => options.MapFrom(source => CupExtensions.ToString(source.Cup)))
                .ForMember(destination => destination.MarketValue, options => options.MapFrom(source => MoneyUtils.ConvertToString(source.MarketValue)))
                .ForMember(destination => destination.MarketValueAverage, options => options.MapFrom(source => MoneyUtils.ConvertToString(source.MarketValueAverage)));
            CreateMap<Domain.DTOs.Response.Competition, Domain.Entities.Competition>()
                .ForMember(destination => destination.Tier, options => options.MapFrom(source => TierExtensions.ToEnum(source.Tier)))
                .ForMember(destination => destination.Cup, options => options.MapFrom(source => CupExtensions.ToEnum(source.Cup)))
                .ForMember(destination => destination.MarketValue, options => options.MapFrom(source => MoneyUtils.ConvertToFloat(source.MarketValue)))
                .ForMember(destination => destination.MarketValueAverage, options => options.MapFrom(source => MoneyUtils.ConvertToFloat(source.MarketValueAverage)));
        }
    }
}
