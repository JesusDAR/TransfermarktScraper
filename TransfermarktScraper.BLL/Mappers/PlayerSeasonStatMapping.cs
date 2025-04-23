using Mapster;
using TransfermarktScraper.Domain.DTOs.Response.Stat;
using TransfermarktScraper.Domain.Entities.Stat;

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
            config.NewConfig<Domain.DTOs.Response.Stat.PlayerSeasonStat, Domain.Entities.Stat.PlayerSeasonStat>();
            config.NewConfig<Domain.Entities.Stat.PlayerSeasonStat, Domain.DTOs.Response.Stat.PlayerSeasonStat>();

            config.NewConfig<Domain.DTOs.Response.Stat.PlayerSeasonCompetitionStat, Domain.Entities.Stat.PlayerSeasonCompetitionStat>();
            config.NewConfig<Domain.Entities.Stat.PlayerSeasonCompetitionStat, Domain.DTOs.Response.Stat.PlayerSeasonCompetitionStat>();

            config.NewConfig<Domain.DTOs.Response.Stat.PlayerSeasonCompetitionMatchStat, Domain.Entities.Stat.PlayerSeasonCompetitionMatchStat>();
            config.NewConfig<Domain.Entities.Stat.PlayerSeasonCompetitionMatchStat, Domain.DTOs.Response.Stat.PlayerSeasonCompetitionMatchStat>();
        }
    }
}