using Mapster;

namespace TransfermarktScraper.BLL.Mappers
{
    /// <summary>
    /// Represents the player career stat mapping profile for Mapster.
    /// </summary>
    public class PlayerCareerStatMapping : IRegister
    {
        /// <summary>
        /// Registers player career stat mappings.
        /// </summary>
        /// <param name="config">The Mapster configuration instance.</param>
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Domain.DTOs.Response.Stat.Career.PlayerCareerStat, Domain.Entities.Stat.Career.PlayerCareerStat>();
            config.NewConfig<Domain.Entities.Stat.Career.PlayerCareerStat, Domain.DTOs.Response.Stat.Career.PlayerCareerStat>();

            config.NewConfig<Domain.DTOs.Response.Stat.Career.PlayerCareerCompetitionStat, Domain.Entities.Stat.Career.PlayerCareerCompetitionStat>();
            config.NewConfig<Domain.Entities.Stat.Career.PlayerCareerCompetitionStat, Domain.DTOs.Response.Stat.Career.PlayerCareerCompetitionStat>();
        }
    }
}