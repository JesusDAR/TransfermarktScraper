using AutoMapper;
using TransfermarktScraper.BLL.Utils;

namespace TransfermarktScraper.BLL.Mappers
{
    /// <summary>
    /// Represents the club mapping profile for AutoMapper.
    /// </summary>
    public class ClubProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClubProfile"/> class.
        /// </summary>
        public ClubProfile()
        {
            CreateMap<Domain.Entities.Club, Domain.DTOs.Response.Club>()
                .ForMember(destination => destination.MarketValue, options => options.MapFrom(source => MoneyUtils.ConvertToString(source.MarketValue)))
                .ForMember(destination => destination.MarketValueAverage, options => options.MapFrom(source => MoneyUtils.ConvertToString(source.MarketValueAverage)));
            CreateMap<Domain.DTOs.Response.Club, Domain.Entities.Club>()
                .ForMember(destination => destination.MarketValue, options => options.MapFrom(source => MoneyUtils.ConvertToFloat(source.MarketValue)))
                .ForMember(destination => destination.MarketValueAverage, options => options.MapFrom(source => MoneyUtils.ConvertToFloat(source.MarketValueAverage)));
        }
    }
}
