using AutoMapper;

namespace TransfermarktScraper.BLL.Mappers
{
    /// <summary>
    /// Represents the player career stat mapping profile for AutoMapper.
    /// </summary>
    public class PlayerCareerStatProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerCareerStatProfile"/> class.
        /// </summary>
        public PlayerCareerStatProfile()
        {
            CreateMap<Domain.DTOs.Response.Stat.Career.PlayerCareerStat, Domain.Entities.Stat.Career.PlayerCareerStat>();
            CreateMap<Domain.Entities.Stat.Career.PlayerCareerStat, Domain.DTOs.Response.Stat.Career.PlayerCareerStat>();

            CreateMap<Domain.DTOs.Response.Stat.Career.PlayerCareerCompetitionStat, Domain.Entities.Stat.Career.PlayerCareerCompetitionStat>();
            CreateMap<Domain.Entities.Stat.Career.PlayerCareerCompetitionStat, Domain.DTOs.Response.Stat.Career.PlayerCareerCompetitionStat>();
        }
    }
}
