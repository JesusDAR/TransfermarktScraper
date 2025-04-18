using AutoMapper;

namespace TransfermarktScraper.BLL.Mappers
{
    /// <summary>
    /// Represents the market value mapping profile for AutoMapper.
    /// </summary>
    public class MarketValueProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarketValueProfile"/> class.
        /// </summary>
        public MarketValueProfile()
        {
            CreateMap<Domain.Entities.MarketValue, Domain.DTOs.Response.MarketValue>();
            CreateMap<Domain.DTOs.Response.MarketValue, Domain.Entities.MarketValue>();
        }
    }
}
