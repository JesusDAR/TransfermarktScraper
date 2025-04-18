using AutoMapper;

namespace TransfermarktScraper.BLL.Mappers
{
    /// <summary>
    /// Represents the player season stat mapping profile for AutoMapper.
    /// </summary>
    public class PlayerSeasonStatProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerSeasonStatProfile"/> class.
        /// </summary>
        public PlayerSeasonStatProfile()
        {
            CreateMap<Domain.DTOs.Response.Stat.Season.PlayerSeasonStat, Domain.Entities.Stat.Season.PlayerSeasonStat>();
            CreateMap<Domain.Entities.Stat.Season.PlayerSeasonStat, Domain.DTOs.Response.Stat.Season.PlayerSeasonStat>();

            CreateMap<Domain.DTOs.Response.Stat.Season.PlayerSeasonCompetitionStat, Domain.Entities.Stat.Season.PlayerSeasonCompetitionStat>();
            CreateMap<Domain.Entities.Stat.Season.PlayerSeasonCompetitionStat, Domain.DTOs.Response.Stat.Season.PlayerSeasonCompetitionStat>();

            CreateMap<Domain.DTOs.Response.Stat.Season.PlayerSeasonCompetitionMatchStat, Domain.Entities.Stat.Season.PlayerSeasonCompetitionMatchStat>();
            CreateMap<Domain.Entities.Stat.Season.PlayerSeasonCompetitionMatchStat, Domain.DTOs.Response.Stat.Season.PlayerSeasonCompetitionMatchStat>();
        }
    }
}
