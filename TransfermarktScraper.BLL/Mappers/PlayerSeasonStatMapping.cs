using Mapster;

namespace TransfermarktScraper.BLL.Mappers
{
    /// <summary>
    /// Represents the player season stat mapping profile for Mapster.
    /// </summary>
    public class PlayerSeasonStatMapping : IRegister
    {
        /// <summary>
        /// Registers player season stat mappings.
        /// </summary>
        /// <param name="config">The Mapster configuration instance.</param>
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Domain.DTOs.Response.Stat.Season.PlayerSeasonStat, Domain.Entities.Stat.Season.PlayerSeasonStat>();
            config.NewConfig<Domain.Entities.Stat.Season.PlayerSeasonStat, Domain.DTOs.Response.Stat.Season.PlayerSeasonStat>();

            config.NewConfig<Domain.DTOs.Response.Stat.Season.PlayerSeasonCompetitionStat, Domain.Entities.Stat.Season.PlayerSeasonCompetitionStat>();
            config.NewConfig<Domain.Entities.Stat.Season.PlayerSeasonCompetitionStat, Domain.DTOs.Response.Stat.Season.PlayerSeasonCompetitionStat>();

            config.NewConfig<Domain.DTOs.Response.Stat.Season.PlayerSeasonCompetitionMatchStat, Domain.Entities.Stat.Season.PlayerSeasonCompetitionMatchStat>();
            config.NewConfig<Domain.Entities.Stat.Season.PlayerSeasonCompetitionMatchStat, Domain.DTOs.Response.Stat.Season.PlayerSeasonCompetitionMatchStat>();
        }
    }
}