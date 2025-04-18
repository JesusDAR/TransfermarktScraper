using AutoMapper;

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
        public PlayerProfile()
        {
            CreateMap<Domain.Entities.Player, Domain.DTOs.Response.Player>();
            CreateMap<Domain.DTOs.Response.Player, Domain.Entities.Player>();
        }
    }
}
