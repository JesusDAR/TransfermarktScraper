using AutoMapper;

namespace TransfermarktScraper.Domain.Mappers
{
    /// <summary>
    /// Represents the mapping profile for AutoMapper.
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MappingProfile"/> class.
        /// </summary>
        public MappingProfile()
        {
            // Country
            CreateMap<Entities.Country, DTOs.Response.Country>();
            CreateMap<DTOs.Response.Country, Entities.Country>();
            CreateMap<DTOs.Request.Country, DTOs.Response.Country>();
            CreateMap<DTOs.Response.Country, DTOs.Request.Country>();

            // Competition
            CreateMap<Entities.Competition, DTOs.Response.Competition>();
            CreateMap<DTOs.Response.Competition, Entities.Competition>();

            // Club
            CreateMap<Entities.Club, DTOs.Response.Club>();
            CreateMap<DTOs.Response.Club, Entities.Club>();

            // Player
            CreateMap<Entities.Player, DTOs.Response.Player>();
            CreateMap<DTOs.Response.Player, Entities.Player>();

            // MarketValue
            CreateMap<Entities.MarketValue, DTOs.Response.MarketValue>();
            CreateMap<DTOs.Response.MarketValue, Entities.MarketValue>();
        }
    }
}
