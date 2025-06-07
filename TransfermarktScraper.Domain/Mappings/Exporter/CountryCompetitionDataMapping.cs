using Mapster;
using TransfermarktScraper.Domain.DTOs.Response.Exporter;
using TransfermarktScraper.Domain.Entities;
using TransfermarktScraper.Domain.Enums.Extensions;

namespace TransfermarktScraper.Domain.Mappings.Exporter
{
    /// <summary>
    /// Defines the mapping configuration between a <see cref="Country"/> and <see cref="Competition"/>
    /// to the export model <see cref="CountryCompetitionData"/> using Mapster.
    /// </summary>
    public class CountryCompetitionDataMapping : IRegister
    {
        /// <summary>
        /// Registers the mapping between Country, Competition and CountryCompetitionData.
        /// </summary>
        /// <param name="config">The Mapster type adapter configuration.</param>
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Country, CountryCompetitionData>()
                .Map(dest => dest.CountryTransfermarktId, src => src.TransfermarktId)
                .Map(dest => dest.CountryFlag, src => src.Flag)
                .Map(dest => dest.CountryName, src => src.Name);

            config.NewConfig<(Country Country, Competition Competition), CountryCompetitionData>()
                .Map(dest => dest.CountryTransfermarktId, src => src.Country.TransfermarktId)
                .Map(dest => dest.CountryFlag, src => src.Country.Flag)
                .Map(dest => dest.CountryName, src => src.Country.Name)

                .Map(dest => dest.CompetitionTransfermarktId, src => src.Competition.TransfermarktId)
                .Map(dest => dest.CompetitionAgeAverage, src => src.Competition.AgeAverage)
                .Map(dest => dest.CompetitionClubIds, src => string.Join(";", src.Competition.ClubIds))
                .Map(dest => dest.CompetitionCurrentChampion, src => src.Competition.CurrentChampion)
                .Map(dest => dest.CompetitionForeignersCount, src => src.Competition.ForeignersCount)
                .Map(dest => dest.CompetitionCup, src => CupExtensions.ToString(src.Competition.Cup))
                .Map(dest => dest.CompetitionLink, src => src.Competition.Link)
                .Map(dest => dest.CompetitionLogo, src => src.Competition.Logo)
                .Map(dest => dest.CompetitionMarketValue, src => src.Competition.MarketValue)
                .Map(dest => dest.CompetitionMarketValueAverage, src => src.Competition.MarketValueAverage)
                .Map(dest => dest.CompetitionMostTimesChampion, src => src.Competition.MostTimesChampion)
                .Map(dest => dest.CompetitionName, src => src.Competition.Name)
                .Map(dest => dest.CompetitionParticipants, src => src.Competition.Participants)
                .Map(dest => dest.CompetitionPlayersCount, src => src.Competition.PlayersCount)
                .Map(dest => dest.CompetitionClubsCount, src => src.Competition.ClubsCount)
                .Map(dest => dest.CompetitionTier, src => TierExtensions.ToString(src.Competition.Tier));
        }
    }
}
