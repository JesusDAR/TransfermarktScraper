using Mapster;
using TransfermarktScraper.Domain.DTOs.Response.Scraper.Stat;
using TransfermarktScraper.Domain.Entities.Stat;
using TransfermarktScraper.Domain.Enums.Extensions;
using TransfermarktScraper.Domain.Utils.DTO;

namespace TransfermarktScraper.Domain.Mappings.Scraper
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
            config.NewConfig<PlayerSeasonStat, PlayerSeasonStatResponse>();
            config.NewConfig<PlayerSeasonStatResponse, PlayerSeasonStat>();

            config.NewConfig<PlayerSeasonCompetitionStat, PlayerSeasonCompetitionStatResponse>();
            config.NewConfig<PlayerSeasonCompetitionStatResponse, PlayerSeasonCompetitionStat>();

            config.NewConfig<PlayerSeasonCompetitionMatchStat, PlayerSeasonCompetitionMatchStatResponse>()
                .Map(dest => dest.Date, src => DateUtils.ConvertToString(src.Date))
                .Map(dest => dest.Position, src => PositionExtensions.ToString(src.Position))
                .Map(dest => dest.NotPlayingReason, src => NotPlayingReasonExtension.ToString(src.NotPlayingReason));
            config.NewConfig<PlayerSeasonCompetitionMatchStatResponse, PlayerSeasonCompetitionMatchStat>()
                .Map(dest => dest.Date, src => DateUtils.ConvertToDateTime(src.Date))
                .Map(dest => dest.Position, src => CupExtensions.ToEnum(src.Position))
                .Map(dest => dest.NotPlayingReason, src => NotPlayingReasonExtension.ToEnum(src.NotPlayingReason));
        }
    }
}