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
        }
    }
}
