using AutoMapper;

namespace TransfermarktScraper.BLL.Mappers
{
    /// <summary>
    /// Represents the country mapping profile for AutoMapper.
    /// </summary>
    public class CountryProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CountryProfile"/> class.
        /// </summary>
        public CountryProfile()
        {
            CreateMap<Domain.Entities.Country, Domain.DTOs.Response.Country>();
            CreateMap<Domain.DTOs.Response.Country, Domain.Entities.Country>();
        }
    }
}
