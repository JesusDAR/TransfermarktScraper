using Mapster;
using Microsoft.Extensions.Options;
using TransfermarktScraper.BLL.Configuration;
using TransfermarktScraper.BLL.Utils;
using TransfermarktScraper.Domain.Enums.Extensions;

namespace TransfermarktScraper.BLL.Mappers
{
    /// <summary>
    /// Represents the player mapping profile for Mapster.
    /// </summary>
    public class PlayerMapping : IRegister
    {
        private readonly ScraperSettings _scraperSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerMapping"/> class.
        /// </summary>
        /// <param name="scraperSettings">The scraper settings containing configuration values.</param>
        public PlayerMapping(IOptions<ScraperSettings> scraperSettings)
        {
            _scraperSettings = scraperSettings.Value;
        }

        /// <summary>
        /// Registers bidirectional mappings between <see cref="Domain.Entities.Player"/> and <see cref="Domain.DTOs.Response.Player"/> using Mapster.
        /// </summary>
        /// <param name="config">The Mapster configuration instance.</param>
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Domain.Entities.Player, Domain.DTOs.Response.Player>()
                .Map(dest => dest.Foot, src => FootExtensions.ToString(src.Foot))
                .Map(dest => dest.Position, src => PositionExtensions.ToString(src.Position))
                .Map(dest => dest.MarketValue, src => MoneyUtils.ConvertToString(src.MarketValue))
                .Map(dest => dest.Nationalities, src => ImageUtils.ConvertCountryTransfermarktIdsToImageUrls(src.Nationalities, _scraperSettings.TinyFlagUrl));

            config.NewConfig<Domain.DTOs.Response.Player, Domain.Entities.Player>()
                .Map(dest => dest.Foot, src => TierExtensions.ToEnum(src.Foot))
                .Map(dest => dest.Position, src => CupExtensions.ToEnum(src.Position))
                .Map(dest => dest.MarketValue, src => MoneyUtils.ConvertToFloat(src.MarketValue))
                .Map(dest => dest.Nationalities, src => ImageUtils.ConvertImageUrlsToCountryTransfermarktIds(src.Nationalities));
        }
    }
}
