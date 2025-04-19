using Mapster;

namespace TransfermarktScraper.BLL.Mappers
{
    /// <summary>
    /// Represents the player stat mapping profile for Mapster.
    /// </summary>
    public class PlayerStatMapping : IRegister
    {
        /// <summary>
        /// Configures the bidirectional mapping between <see cref="Domain.Entities.Stat.PlayerStat"/> and <see cref="Domain.DTOs.Response.Stat.PlayerStat"/>.
        /// </summary>
        /// <param name="config">The Mapster configuration instance.</param>
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Domain.Entities.Stat.PlayerStat, Domain.DTOs.Response.Stat.PlayerStat>();
            config.NewConfig<Domain.DTOs.Response.Stat.PlayerStat, Domain.Entities.Stat.PlayerStat>();
        }
    }
}
