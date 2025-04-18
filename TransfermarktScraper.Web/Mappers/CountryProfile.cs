using AutoMapper;

namespace TransfermarktScraper.Web.Mappers
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
            CreateMap<Domain.DTOs.Request.Country, Domain.DTOs.Response.Country>();
            CreateMap<Domain.DTOs.Response.Country, Domain.DTOs.Request.Country>();
        }
    }
}
