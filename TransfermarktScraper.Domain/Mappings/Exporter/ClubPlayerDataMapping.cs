using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mapster;
using TransfermarktScraper.Domain.DTOs.Response.Exporter;
using TransfermarktScraper.Domain.Entities;

namespace TransfermarktScraper.Domain.Mappings.Exporter
{
    /// <summary>
    /// Defines the mapping configuration between <see cref="Club"/> and <see cref="Player"/>
    /// to the export model <see cref="ClubPlayerData"/> using Mapster.
    /// </summary>
    public class ClubPlayerDataMapping : IRegister
    {
        /// <summary>
        /// Registers the mapping between Club, Player and ClubPlayerData.
        /// </summary>
        /// <param name="config">The Mapster type adapter configuration.</param>
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Club, ClubPlayerData>()
                .Map(dest => dest.ClubTransfermarktId, src => src.TransfermarktId)
                .Map(dest => dest.ClubAgeAverage, src => src.AgeAverage)
                .Map(dest => dest.ClubCompetitionIds, src => string.Join(";", src.CompetitionIds))
                .Map(dest => dest.ClubCrest, src => src.Crest)
                .Map(dest => dest.ClubForeignersCount, src => src.ForeignersCount)
                .Map(dest => dest.ClubLink, src => src.Link)
                .Map(dest => dest.ClubMarketValue, src => src.MarketValue)
                .Map(dest => dest.ClubMarketValueAverage, src => src.MarketValueAverage)
                .Map(dest => dest.ClubName, src => src.Name)
                .Map(dest => dest.ClubPlayersCount, src => src.PlayersCount);

            config.NewConfig<(Club Club, Player Player), ClubPlayerData>()
                .Map(dest => dest.ClubTransfermarktId, src => src.Club.TransfermarktId)
                .Map(dest => dest.ClubAgeAverage, src => src.Club.AgeAverage)
                .Map(dest => dest.ClubCompetitionIds, src => string.Join(";", src.Club.CompetitionIds))
                .Map(dest => dest.ClubCrest, src => src.Club.Crest)
                .Map(dest => dest.ClubForeignersCount, src => src.Club.ForeignersCount)
                .Map(dest => dest.ClubLink, src => src.Club.Link)
                .Map(dest => dest.ClubMarketValue, src => src.Club.MarketValue)
                .Map(dest => dest.ClubMarketValueAverage, src => src.Club.MarketValueAverage)
                .Map(dest => dest.ClubName, src => src.Club.Name)
                .Map(dest => dest.ClubPlayersCount, src => src.Club.PlayersCount)

                .Map(dest => dest.PlayerAge, src => src.Player.Age)
                .Map(dest => dest.PlayerContractEnd, src => src.Player.ContractEnd)
                .Map(dest => dest.PlayerContractStart, src => src.Player.ContractStart)
                .Map(dest => dest.PlayerDateOfBirth, src => src.Player.DateOfBirth)
                .Map(dest => dest.PlayerFoot, src => src.Player.Foot)
                .Map(dest => dest.PlayerHeight, src => src.Player.Height)
                .Map(dest => dest.PlayerLink, src => src.Player.Link)
                .Map(dest => dest.PlayerMarketValue, src => src.Player.MarketValue)
                .Map(dest => dest.PlayerMarketValues, src => FormatMarketValues(src.Player.MarketValues))
                .Map(dest => dest.PlayerName, src => src.Player.Name)
                .Map(dest => dest.PlayerNationalities, src => src.Player.Nationalities == null ? string.Empty : string.Join(";", src.Player.Nationalities))
                .Map(dest => dest.PlayerNumber, src => src.Player.Number)
                .Map(dest => dest.PlayerPortrait, src => src.Player.Portrait)
                .Map(dest => dest.PlayerPosition, src => src.Player.Position)
                .Map(dest => dest.PlayerPlayerStatId, src => src.Player.PlayerStatId);
        }

        /// <summary>
        /// Formats a collection of <see cref="MarketValue"/> entries into a single string,
        /// where each entry is represented as "yyyy-MM-dd|ClubTransfermarktId|Value;".
        /// </summary>
        /// <param name="marketValues">The collection of market values to format. Can be null.</param>
        /// <returns>
        /// A formatted string representing all market values, or an empty string if the input is null or empty.
        /// </returns>
        private static string FormatMarketValues(IEnumerable<MarketValue>? marketValues)
        {
            if (marketValues == null || !marketValues.Any())
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            foreach (var mv in marketValues)
            {
                sb.Append($"{mv.Date:yyyy-MM-dd}|{mv.ClubTransfermarktId}|{mv.Value};");
            }

            return sb.ToString();
        }
    }
}
