using Mapster;
using TransfermarktScraper.Domain.DTOs.Response.Scraper;
using TransfermarktScraper.Domain.Entities;
using TransfermarktScraper.Domain.Enums.Extensions;
using TransfermarktScraper.Domain.Utils.DTO;

namespace TransfermarktScraper.Domain.Mappings.Scraper
{
    /// <summary>
    /// Represents the player mapping profile for Mapster.
    /// </summary>
    public class PlayerMapping : IRegister
    {
        /// <summary>
        /// Registers bidirectional mappings between <see cref="Player"/> and <see cref="PlayerResponse"/> using Mapster.
        /// </summary>
        /// <param name="config">The Mapster configuration instance.</param>
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Player, PlayerResponse>()
                .Map(dest => dest.Foot, src => FootExtensions.ToString(src.Foot))
                .Map(dest => dest.Position, src => PositionExtensions.ToString(src.Position))
                .Map(dest => dest.MarketValue, src => MoneyUtils.ConvertToString(src.MarketValue))
                .Map(dest => dest.Nationalities, src => src.Nationalities)
                .Map(dest => dest.ContractEnd, src => DateUtils.ConvertToString(src.ContractEnd))
                .Map(dest => dest.ContractStart, src => DateUtils.ConvertToString(src.ContractStart))
                .Map(dest => dest.DateOfBirth, src => DateUtils.ConvertToString(src.DateOfBirth));

            config.NewConfig<PlayerResponse, Player>()
                .Map(dest => dest.Foot, src => TierExtensions.ToEnum(src.Foot))
                .Map(dest => dest.Position, src => CupExtensions.ToEnum(src.Position))
                .Map(dest => dest.MarketValue, src => MoneyUtils.ConvertToFloat(src.MarketValue))
                .Map(dest => dest.Nationalities, src => ImageUtils.ConvertImageUrlsToCountryTransfermarktIds(src.Nationalities))
                .Map(dest => dest.ContractEnd, src => DateUtils.ConvertToDateTime(src.ContractEnd))
                .Map(dest => dest.ContractStart, src => DateUtils.ConvertToDateTime(src.ContractStart))
                .Map(dest => dest.DateOfBirth, src => DateUtils.ConvertToDateTime(src.DateOfBirth));
        }
    }
}
