using Mapster;
using TransfermarktScraper.Domain.DTOs.Response.Exporter;
using TransfermarktScraper.Domain.Entities.Stat;

namespace TransfermarktScraper.Domain.Mappings.Exporter
{
    /// <summary>
    /// Defines the mapping configuration between <see cref="PlayerStat"/>, <see cref="PlayerSeasonStat"/>, <see cref="PlayerSeasonCompetitionStat"/> and <see cref="PlayerSeasonCompetitionMatchStat"/>
    /// to the export model <see cref="PlayerStatData"/> using Mapster.
    /// </summary>
    public class PlayerStatDataMapping : IRegister
    {
        /// <summary>
        /// Registers the mapping between Club, Player and ClubPlayerData.
        /// </summary>
        /// <param name="config">The Mapster type adapter configuration.</param>
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<PlayerStat, PlayerStatData>()
                .Map(dest => dest.PlayerStatPlayerTransfermarktId, src => src.PlayerTransfermarktId);

            config.NewConfig<(PlayerStat PlayerStat, PlayerSeasonStat PlayerSeasonStat), PlayerStatData>()
                .Map(dest => dest.PlayerStatPlayerTransfermarktId, src => src.PlayerStat.PlayerTransfermarktId)

                .Map(dest => dest.PlayerSeasonStatSeasonTransfermarktId, src => src.PlayerSeasonStat.SeasonTransfermarktId)
                .Map(dest => dest.PlayerSeasonStatAppearances, src => src.PlayerSeasonStat.Appearances)
                .Map(dest => dest.PlayerSeasonStatGoals, src => src.PlayerSeasonStat.Goals)
                .Map(dest => dest.PlayerSeasonStatAssists, src => src.PlayerSeasonStat.Assists)
                .Map(dest => dest.PlayerSeasonStatOwnGoals, src => src.PlayerSeasonStat.OwnGoals)
                .Map(dest => dest.PlayerSeasonStatYellowCards, src => src.PlayerSeasonStat.YellowCards)
                .Map(dest => dest.PlayerSeasonStatSecondYellowCards, src => src.PlayerSeasonStat.SecondYellowCards)
                .Map(dest => dest.PlayerSeasonStatRedCards, src => src.PlayerSeasonStat.RedCards)
                .Map(dest => dest.PlayerSeasonStatPenaltyGoals, src => src.PlayerSeasonStat.PenaltyGoals)
                .Map(dest => dest.PlayerSeasonStatGoalsConceded, src => src.PlayerSeasonStat.GoalsConceded)
                .Map(dest => dest.PlayerSeasonStatMinutesPerGoal, src => src.PlayerSeasonStat.MinutesPerGoal)
                .Map(dest => dest.PlayerSeasonStatMinutesPlayed, src => src.PlayerSeasonStat.MinutesPlayed)
                .Map(dest => dest.PlayerSeasonStatMinutesPlayed, src => src.PlayerSeasonStat.MinutesPlayed);

            config.NewConfig<(PlayerStat PlayerStat, PlayerSeasonStat PlayerSeasonStat, PlayerSeasonCompetitionStat PlayerSeasonCompetitionStat), PlayerStatData>()
                .Map(dest => dest.PlayerStatPlayerTransfermarktId, src => src.PlayerStat.PlayerTransfermarktId)

                .Map(dest => dest.PlayerSeasonStatSeasonTransfermarktId, src => src.PlayerSeasonStat.SeasonTransfermarktId)
                .Map(dest => dest.PlayerSeasonStatAppearances, src => src.PlayerSeasonStat.Appearances)
                .Map(dest => dest.PlayerSeasonStatGoals, src => src.PlayerSeasonStat.Goals)
                .Map(dest => dest.PlayerSeasonStatAssists, src => src.PlayerSeasonStat.Assists)
                .Map(dest => dest.PlayerSeasonStatOwnGoals, src => src.PlayerSeasonStat.OwnGoals)
                .Map(dest => dest.PlayerSeasonStatYellowCards, src => src.PlayerSeasonStat.YellowCards)
                .Map(dest => dest.PlayerSeasonStatSecondYellowCards, src => src.PlayerSeasonStat.SecondYellowCards)
                .Map(dest => dest.PlayerSeasonStatRedCards, src => src.PlayerSeasonStat.RedCards)
                .Map(dest => dest.PlayerSeasonStatPenaltyGoals, src => src.PlayerSeasonStat.PenaltyGoals)
                .Map(dest => dest.PlayerSeasonStatGoalsConceded, src => src.PlayerSeasonStat.GoalsConceded)
                .Map(dest => dest.PlayerSeasonStatMinutesPerGoal, src => src.PlayerSeasonStat.MinutesPerGoal)
                .Map(dest => dest.PlayerSeasonStatMinutesPlayed, src => src.PlayerSeasonStat.MinutesPlayed)
                .Map(dest => dest.PlayerSeasonStatMinutesPlayed, src => src.PlayerSeasonStat.MinutesPlayed)

                .Map(dest => dest.PlayerSeasonCompetitionStatCompetitionTransfermarktId, src => src.PlayerSeasonCompetitionStat.CompetitionTransfermarktId)
                .Map(dest => dest.PlayerSeasonCompetitionStatCompetitionName, src => src.PlayerSeasonCompetitionStat.CompetitionName)
                .Map(dest => dest.PlayerSeasonCompetitionStatCompetitionLink, src => src.PlayerSeasonCompetitionStat.CompetitionLink)
                .Map(dest => dest.PlayerSeasonCompetitionStatCompetitionLogo, src => src.PlayerSeasonCompetitionStat.CompetitionLogo)
                .Map(dest => dest.PlayerSeasonCompetitionStatAppearances, src => src.PlayerSeasonCompetitionStat.Appearances)
                .Map(dest => dest.PlayerSeasonCompetitionStatGoals, src => src.PlayerSeasonCompetitionStat.Goals)
                .Map(dest => dest.PlayerSeasonCompetitionStatAssists, src => src.PlayerSeasonCompetitionStat.Assists)
                .Map(dest => dest.PlayerSeasonCompetitionStatOwnGoals, src => src.PlayerSeasonCompetitionStat.OwnGoals)
                .Map(dest => dest.PlayerSeasonCompetitionStatSubstitutionsOn, src => src.PlayerSeasonCompetitionStat.SubstitutionsOn)
                .Map(dest => dest.PlayerSeasonCompetitionStatSubstitutionsOff, src => src.PlayerSeasonCompetitionStat.SubstitutionsOff)
                .Map(dest => dest.PlayerSeasonCompetitionStatYellowCards, src => src.PlayerSeasonCompetitionStat.YellowCards)
                .Map(dest => dest.PlayerSeasonCompetitionStatSecondYellowCards, src => src.PlayerSeasonCompetitionStat.SecondYellowCards)
                .Map(dest => dest.PlayerSeasonCompetitionStatRedCards, src => src.PlayerSeasonCompetitionStat.RedCards)
                .Map(dest => dest.PlayerSeasonCompetitionPenaltyGoals, src => src.PlayerSeasonCompetitionStat.PenaltyGoals)
                .Map(dest => dest.PlayerSeasonCompetitionGoalsConceded, src => src.PlayerSeasonCompetitionStat.GoalsConceded)
                .Map(dest => dest.PlayerSeasonCompetitionCleanSheets, src => src.PlayerSeasonCompetitionStat.CleanSheets)
                .Map(dest => dest.PlayerSeasonCompetitionMinutesPerGoal, src => src.PlayerSeasonCompetitionStat.MinutesPerGoal)
                .Map(dest => dest.PlayerSeasonCompetitionMinutesPlayed, src => src.PlayerSeasonCompetitionStat.MinutesPlayed)
                .Map(dest => dest.PlayerSeasonCompetitionSquad, src => src.PlayerSeasonCompetitionStat.Squad)
                .Map(dest => dest.PlayerSeasonCompetitionStartingEleven, src => src.PlayerSeasonCompetitionStat.StartingEleven)
                .Map(dest => dest.PlayerSeasonCompetitionOnTheBench, src => src.PlayerSeasonCompetitionStat.OnTheBench)
                .Map(dest => dest.PlayerSeasonCompetitionSuspended, src => src.PlayerSeasonCompetitionStat.Suspended)
                .Map(dest => dest.PlayerSeasonCompetitionInjured, src => src.PlayerSeasonCompetitionStat.Injured);

            config.NewConfig<(PlayerStat PlayerStat, PlayerSeasonStat PlayerSeasonStat, PlayerSeasonCompetitionStat PlayerSeasonCompetitionStat, PlayerSeasonCompetitionMatchStat PlayerSeasonCompetitionMatchStat), PlayerStatData>()
                .Map(dest => dest.PlayerStatPlayerTransfermarktId, src => src.PlayerStat.PlayerTransfermarktId)

                .Map(dest => dest.PlayerSeasonStatSeasonTransfermarktId, src => src.PlayerSeasonStat.SeasonTransfermarktId)
                .Map(dest => dest.PlayerSeasonStatAppearances, src => src.PlayerSeasonStat.Appearances)
                .Map(dest => dest.PlayerSeasonStatGoals, src => src.PlayerSeasonStat.Goals)
                .Map(dest => dest.PlayerSeasonStatAssists, src => src.PlayerSeasonStat.Assists)
                .Map(dest => dest.PlayerSeasonStatOwnGoals, src => src.PlayerSeasonStat.OwnGoals)
                .Map(dest => dest.PlayerSeasonStatYellowCards, src => src.PlayerSeasonStat.YellowCards)
                .Map(dest => dest.PlayerSeasonStatSecondYellowCards, src => src.PlayerSeasonStat.SecondYellowCards)
                .Map(dest => dest.PlayerSeasonStatRedCards, src => src.PlayerSeasonStat.RedCards)
                .Map(dest => dest.PlayerSeasonStatPenaltyGoals, src => src.PlayerSeasonStat.PenaltyGoals)
                .Map(dest => dest.PlayerSeasonStatGoalsConceded, src => src.PlayerSeasonStat.GoalsConceded)
                .Map(dest => dest.PlayerSeasonStatMinutesPerGoal, src => src.PlayerSeasonStat.MinutesPerGoal)
                .Map(dest => dest.PlayerSeasonStatMinutesPlayed, src => src.PlayerSeasonStat.MinutesPlayed)
                .Map(dest => dest.PlayerSeasonStatMinutesPlayed, src => src.PlayerSeasonStat.MinutesPlayed)

                .Map(dest => dest.PlayerSeasonCompetitionStatCompetitionTransfermarktId, src => src.PlayerSeasonCompetitionStat.CompetitionTransfermarktId)
                .Map(dest => dest.PlayerSeasonCompetitionStatCompetitionName, src => src.PlayerSeasonCompetitionStat.CompetitionName)
                .Map(dest => dest.PlayerSeasonCompetitionStatCompetitionLink, src => src.PlayerSeasonCompetitionStat.CompetitionLink)
                .Map(dest => dest.PlayerSeasonCompetitionStatCompetitionLogo, src => src.PlayerSeasonCompetitionStat.CompetitionLogo)
                .Map(dest => dest.PlayerSeasonCompetitionStatAppearances, src => src.PlayerSeasonCompetitionStat.Appearances)
                .Map(dest => dest.PlayerSeasonCompetitionStatGoals, src => src.PlayerSeasonCompetitionStat.Goals)
                .Map(dest => dest.PlayerSeasonCompetitionStatAssists, src => src.PlayerSeasonCompetitionStat.Assists)
                .Map(dest => dest.PlayerSeasonCompetitionStatOwnGoals, src => src.PlayerSeasonCompetitionStat.OwnGoals)
                .Map(dest => dest.PlayerSeasonCompetitionStatSubstitutionsOn, src => src.PlayerSeasonCompetitionStat.SubstitutionsOn)
                .Map(dest => dest.PlayerSeasonCompetitionStatSubstitutionsOff, src => src.PlayerSeasonCompetitionStat.SubstitutionsOff)
                .Map(dest => dest.PlayerSeasonCompetitionStatYellowCards, src => src.PlayerSeasonCompetitionStat.YellowCards)
                .Map(dest => dest.PlayerSeasonCompetitionStatSecondYellowCards, src => src.PlayerSeasonCompetitionStat.SecondYellowCards)
                .Map(dest => dest.PlayerSeasonCompetitionStatRedCards, src => src.PlayerSeasonCompetitionStat.RedCards)
                .Map(dest => dest.PlayerSeasonCompetitionPenaltyGoals, src => src.PlayerSeasonCompetitionStat.PenaltyGoals)
                .Map(dest => dest.PlayerSeasonCompetitionGoalsConceded, src => src.PlayerSeasonCompetitionStat.GoalsConceded)
                .Map(dest => dest.PlayerSeasonCompetitionCleanSheets, src => src.PlayerSeasonCompetitionStat.CleanSheets)
                .Map(dest => dest.PlayerSeasonCompetitionMinutesPerGoal, src => src.PlayerSeasonCompetitionStat.MinutesPerGoal)
                .Map(dest => dest.PlayerSeasonCompetitionMinutesPlayed, src => src.PlayerSeasonCompetitionStat.MinutesPlayed)
                .Map(dest => dest.PlayerSeasonCompetitionSquad, src => src.PlayerSeasonCompetitionStat.Squad)
                .Map(dest => dest.PlayerSeasonCompetitionStartingEleven, src => src.PlayerSeasonCompetitionStat.StartingEleven)
                .Map(dest => dest.PlayerSeasonCompetitionOnTheBench, src => src.PlayerSeasonCompetitionStat.OnTheBench)
                .Map(dest => dest.PlayerSeasonCompetitionSuspended, src => src.PlayerSeasonCompetitionStat.Suspended)
                .Map(dest => dest.PlayerSeasonCompetitionInjured, src => src.PlayerSeasonCompetitionStat.Injured)

                .Map(dest => dest.PlayerSeasonCompetitionMatchStatHomeClubTransfermarktId, src => src.PlayerSeasonCompetitionMatchStat.HomeClubTransfermarktId)
                .Map(dest => dest.PlayerSeasonCompetitionMatchStatAwayClubTransfermarktId, src => src.PlayerSeasonCompetitionMatchStat.AwayClubTransfermarktId)
                .Map(dest => dest.PlayerSeasonCompetitionMatchStatDate, src => src.PlayerSeasonCompetitionMatchStat.Date)
                .Map(dest => dest.PlayerSeasonCompetitionMatchStatMatchDay, src => src.PlayerSeasonCompetitionMatchStat.MatchDay)
                .Map(dest => dest.PlayerSeasonCompetitionMatchStatLink, src => src.PlayerSeasonCompetitionMatchStat.Link)
                .Map(dest => dest.PlayerSeasonCompetitionMatchStatHomeClubName, src => src.PlayerSeasonCompetitionMatchStat.HomeClubName)
                .Map(dest => dest.PlayerSeasonCompetitionMatchStatHomeClubLink, src => src.PlayerSeasonCompetitionMatchStat.HomeClubLink)
                .Map(dest => dest.PlayerSeasonCompetitionMatchStatHomeClubLogo, src => src.PlayerSeasonCompetitionMatchStat.HomeClubLogo)
                .Map(dest => dest.PlayerSeasonCompetitionMatchStatAwayClubName, src => src.PlayerSeasonCompetitionMatchStat.AwayClubName)
                .Map(dest => dest.PlayerSeasonCompetitionMatchStatAwayClubLink, src => src.PlayerSeasonCompetitionMatchStat.AwayClubLink)
                .Map(dest => dest.PlayerSeasonCompetitionMatchStatAwayClubLogo, src => src.PlayerSeasonCompetitionMatchStat.AwayClubLogo)
                .Map(dest => dest.PlayerSeasonCompetitionMatchStatHomeClubGoals, src => src.PlayerSeasonCompetitionMatchStat.HomeClubGoals)
                .Map(dest => dest.PlayerSeasonCompetitionMatchStatAwayClubGoals, src => src.PlayerSeasonCompetitionMatchStat.AwayClubGoals)
                .Map(dest => dest.PlayerSeasonCompetitionMatchStatMatchResult, src => src.PlayerSeasonCompetitionMatchStat.MatchResult)
                .Map(dest => dest.PlayerSeasonCompetitionMatchStatMatchResultLink, src => src.PlayerSeasonCompetitionMatchStat.MatchResultLink)
                .Map(dest => dest.PlayerSeasonCompetitionMatchStatIsResultAddition, src => src.PlayerSeasonCompetitionMatchStat.IsResultAddition)
                .Map(dest => dest.PlayerSeasonCompetitionMatchStatIsResultPenalties, src => src.PlayerSeasonCompetitionMatchStat.IsResultPenalties)
                .Map(dest => dest.PlayerSeasonCompetitionMatchStatPosition, src => src.PlayerSeasonCompetitionMatchStat.Position)
                .Map(dest => dest.PlayerSeasonCompetitionMatchStatIsCaptain, src => src.PlayerSeasonCompetitionMatchStat.IsCaptain)
                .Map(dest => dest.PlayerSeasonCompetitionMatchStatGoals, src => src.PlayerSeasonCompetitionMatchStat.Goals)
                .Map(dest => dest.PlayerSeasonCompetitionMatchStatAssists, src => src.PlayerSeasonCompetitionMatchStat.Assists)
                .Map(dest => dest.PlayerSeasonCompetitionMatchStatOwnGoals, src => src.PlayerSeasonCompetitionMatchStat.OwnGoals)
                .Map(dest => dest.PlayerSeasonCompetitionMatchStatYellowCard, src => src.PlayerSeasonCompetitionMatchStat.YellowCard)
                .Map(dest => dest.PlayerSeasonCompetitionMatchStatSecondYellowCard, src => src.PlayerSeasonCompetitionMatchStat.SecondYellowCard)
                .Map(dest => dest.PlayerSeasonCompetitionMatchStatRedCard, src => src.PlayerSeasonCompetitionMatchStat.RedCard)
                .Map(dest => dest.PlayerSeasonCompetitionMatchStatSubstitutedOn, src => src.PlayerSeasonCompetitionMatchStat.SubstitutedOn)
                .Map(dest => dest.PlayerSeasonCompetitionMatchStatSubstitutedOff, src => src.PlayerSeasonCompetitionMatchStat.SubstitutedOff)
                .Map(dest => dest.PlayerSeasonCompetitionMatchStatMinutesPlayed, src => src.PlayerSeasonCompetitionMatchStat.MinutesPlayed)
                .Map(dest => dest.PlayerSeasonCompetitionMatchStatNotPlayingReason, src => src.PlayerSeasonCompetitionMatchStat.NotPlayingReason);
        }

    }
}
