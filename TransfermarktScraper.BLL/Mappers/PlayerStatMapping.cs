using Mapster;
using TransfermarktScraper.Domain.DTOs.Response.Stat;
using TransfermarktScraper.Domain.Entities.Stat;

namespace TransfermarktScraper.BLL.Mappers
{
    /// <summary>
    /// Represents the player stat mapping profile for Mapster.
    /// </summary>
    public class PlayerStatMapping : IRegister
    {
        /// <summary>
        /// Configures the bidirectional mapping between <see cref="PlayerStat"/> and <see cref="PlayerStatResponse"/>.
        /// </summary>
        /// <param name="config">The Mapster configuration instance.</param>
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<PlayerStat, PlayerStatResponse>();
            config.NewConfig<PlayerStatResponse, PlayerStat>();
        }
    }
}
