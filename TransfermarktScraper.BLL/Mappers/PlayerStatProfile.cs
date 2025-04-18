using AutoMapper;

namespace TransfermarktScraper.BLL.Mappers
{
    /// <summary>
    /// Represents the player stat mapping profile for AutoMapper.
    /// </summary>
    public class PlayerStatProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerStatProfile"/> class.
        /// </summary>
        public PlayerStatProfile()
        {
            CreateMap<Domain.Entities.Stat.PlayerStat, Domain.DTOs.Response.Stat.PlayerStat>();
            CreateMap<Domain.DTOs.Response.Stat.PlayerStat, Domain.Entities.Stat.PlayerStat>();
        }
    }
}
