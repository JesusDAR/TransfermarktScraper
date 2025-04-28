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
            config.NewConfig<PlayerSeasonStatResponse, PlayerSeasonStat>();
            config.NewConfig<PlayerSeasonStat, PlayerSeasonStatResponse>();

            config.NewConfig<PlayerSeasonCompetitionStatResponse, PlayerSeasonCompetitionStat>();
            config.NewConfig<PlayerSeasonCompetitionStat, PlayerSeasonCompetitionStatResponse>();

            config.NewConfig<PlayerSeasonCompetitionMatchStatResponse, PlayerSeasonCompetitionMatchStat>();
            config.NewConfig<PlayerSeasonCompetitionMatchStat, PlayerSeasonCompetitionMatchStatResponse>();
        }
    }
}