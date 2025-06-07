using TransfermarktScraper.Domain.Enums;
using TransfermarktScraper.Domain.Enums.Extensions;

namespace TransfermarktScraper.Domain.DTOs.Response.Exporter
{
    /// <summary>
    /// Represents the data exported from a player stat document.
    /// </summary>
    public class PlayerStatData
    {
        /// <summary>
        /// Gets or sets the unique player Transfermarkt identifier.
        /// </summary>
        public string? PlayerStatPlayerTransfermarktId { get; set; }

        /// <summary>
        /// Gets or sets the unique season Transfermarkt identifier.
        /// </summary>
        public string? PlayerSeasonStatSeasonTransfermarktId { get; set; }

        /// <summary>
        /// Gets or sets the number of appearances in all competitions of the season.
        /// </summary>
        public int? PlayerSeasonStatAppearances { get; set; }

        /// <summary>
        /// Gets or sets the number of goals in all competitions of the season.
        /// </summary>
        public int? PlayerSeasonStatGoals { get; set; }

        /// <summary>
        /// Gets or sets the number of assists in all competitions of the season.
        /// </summary>
        public int? PlayerSeasonStatAssists { get; set; }

        /// <summary>
        /// Gets or sets the number of own goals in all competitions of the season.
        /// </summary>
        public int? PlayerSeasonStatOwnGoals { get; set; }

        /// <summary>
        /// Gets or sets the number of yellow cards received in all competitions of the season.
        /// </summary>
        public int? PlayerSeasonStatYellowCards { get; set; }

        /// <summary>
        /// Gets or sets the number of second yellow cards received in all competitions of the season.
        /// </summary>
        public int? PlayerSeasonStatSecondYellowCards { get; set; }

        /// <summary>
        /// Gets or sets the number of red cards received in all competitions of the season.
        /// </summary>
        public int? PlayerSeasonStatRedCards { get; set; }

        /// <summary>
        /// Gets or sets the number of penalty goals scored in all competitions of the season.
        /// </summary>
        public int? PlayerSeasonStatPenaltyGoals { get; set; }

        /// <summary>
        /// Gets or sets the number of goals that the goalkeeper has conceded in all competitions of the season.
        /// </summary>
        public int? PlayerSeasonStatGoalsConceded { get; set; }

        /// <summary>
        /// Gets or sets the number of clean sheets for the goalkeeper in all competitions of the season.
        /// </summary>
        public int? PlayerSeasonStatCleanSheets { get; set; }

        /// <summary>
        /// Gets or sets the number of minutes per goal scored in all competitions of the season.
        /// </summary>
        public int? PlayerSeasonStatMinutesPerGoal { get; set; }

        /// <summary>
        /// Gets or sets the number of minutes the player was on the field in all competitions of the season.
        /// </summary>
        public int? PlayerSeasonStatMinutesPlayed { get; set; }

        /// <summary>
        /// Gets or sets the unique competition Transfermarkt identifier.
        /// </summary>
        public string? PlayerSeasonCompetitionStatCompetitionTransfermarktId { get; set; }

        /// <summary>
        /// Gets or sets the competition name.
        /// </summary>
        public string? PlayerSeasonCompetitionStatCompetitionName { get; set; }

        /// <summary>
        /// Gets or sets the competition link.
        /// </summary>
        public string? PlayerSeasonCompetitionStatCompetitionLink { get; set; }

        /// <summary>
        /// Gets or sets the competition logo.
        /// </summary>
        public string? PlayerSeasonCompetitionStatCompetitionLogo { get; set; }

        /// <summary>
        /// Gets or sets the number of appearances in the competition for the season.
        /// </summary>
        public int? PlayerSeasonCompetitionStatAppearances { get; set; }

        /// <summary>
        /// Gets or sets the number of goals in the competition for the season.
        /// </summary>
        public int? PlayerSeasonCompetitionStatGoals { get; set; }

        /// <summary>
        /// Gets or sets the number of assists in the competition for the season.
        /// </summary>
        public int? PlayerSeasonCompetitionStatAssists { get; set; }

        /// <summary>
        /// Gets or sets the number of own goals in the competition for the season.
        /// </summary>
        public int? PlayerSeasonCompetitionStatOwnGoals { get; set; }

        /// <summary>
        /// Gets or sets the number of times the player was substituted on in the competition for the season.
        /// </summary>
        public int? PlayerSeasonCompetitionStatSubstitutionsOn { get; set; }

        /// <summary>
        /// Gets or sets the number of times the player was substituted off in the competition for the season.
        /// </summary>
        public int? PlayerSeasonCompetitionStatSubstitutionsOff { get; set; }

        /// <summary>
        /// Gets or sets the number of yellow cards received in the competition for the season.
        /// </summary>
        public int? PlayerSeasonCompetitionStatYellowCards { get; set; }

        /// <summary>
        /// Gets or sets the number of second yellow cards received in the competition for the season.
        /// </summary>
        public int? PlayerSeasonCompetitionStatSecondYellowCards { get; set; }

        /// <summary>
        /// Gets or sets the number of red cards received in the competition for the season.
        /// </summary>
        public int? PlayerSeasonCompetitionStatRedCards { get; set; }

        /// <summary>
        /// Gets or sets the number of penalty goals scored in the competition for the season.
        /// </summary>
        public int? PlayerSeasonCompetitionPenaltyGoals { get; set; }

        /// <summary>
        /// Gets or sets the number of goals that the goalkeeper has conceded in the competition for the season.
        /// </summary>
        public int? PlayerSeasonCompetitionGoalsConceded { get; set; }

        /// <summary>
        /// Gets or sets the number of clean sheets for the goalkeeper in the competition for the season.
        /// </summary>
        public int? PlayerSeasonCompetitionCleanSheets { get; set; }

        /// <summary>
        /// Gets or sets the number of minutes per goal scored in the competition for the season.
        /// </summary>
        public int? PlayerSeasonCompetitionMinutesPerGoal { get; set; }

        /// <summary>
        /// Gets or sets the number of minutes played in the competition for the season.
        /// </summary>
        public int? PlayerSeasonCompetitionMinutesPlayed { get; set; }

        /// <summary>
        /// Gets or sets the number of times the player was in the squad in the competition for the season.
        /// </summary>
        public int? PlayerSeasonCompetitionSquad { get; set; }

        /// <summary>
        /// Gets or sets the number of times the player was in the starting eleven in the competition for the season.
        /// </summary>
        public int? PlayerSeasonCompetitionStartingEleven { get; set; }

        /// <summary>
        /// Gets or sets the number of times the player was on the bench the whole match in the competition for the season.
        /// </summary>
        public int? PlayerSeasonCompetitionOnTheBench { get; set; }

        /// <summary>
        /// Gets or sets the number of matches the player missed because of a suspension in the competition for the season.
        /// </summary>
        public int? PlayerSeasonCompetitionSuspended { get; set; }

        /// <summary>
        /// Gets or sets the number of matches the player missed because of an injure in the competition for the season.
        /// </summary>
        public int? PlayerSeasonCompetitionInjured { get; set; }

        /// <summary>
        /// Gets or sets the unique home club Transfermarkt identifier.
        /// </summary>
        public string? PlayerSeasonCompetitionMatchStatHomeClubTransfermarktId { get; set; }

        /// <summary>
        /// Gets or sets the unique away club Transfermarkt identifier.
        /// </summary>
        public string? PlayerSeasonCompetitionMatchStatAwayClubTransfermarktId { get; set; }

        /// <summary>
        /// Gets or sets the date of the match.
        /// </summary>
        public string? PlayerSeasonCompetitionMatchStatDate { get; set; }

        /// <summary>
        /// Gets or sets a naming for the match.
        /// </summary>
        public string? PlayerSeasonCompetitionMatchStatMatchDay { get; set; }

        /// <summary>
        /// Gets or sets the link of the match.
        /// </summary>
        public string? PlayerSeasonCompetitionMatchStatLink { get; set; }

        /// <summary>
        /// Gets or sets the home club name.
        /// </summary>
        public string? PlayerSeasonCompetitionMatchStatHomeClubName { get; set; }

        /// <summary>
        /// Gets or sets the home club link.
        /// </summary>
        public string? PlayerSeasonCompetitionMatchStatHomeClubLink { get; set; }

        /// <summary>
        /// Gets or sets the home club logo.
        /// </summary>
        public string? PlayerSeasonCompetitionMatchStatHomeClubLogo { get; set; }

        /// <summary>
        /// Gets or sets the away club name.
        /// </summary>
        public string? PlayerSeasonCompetitionMatchStatAwayClubName { get; set; }

        /// <summary>
        /// Gets or sets the away club link.
        /// </summary>
        public string? PlayerSeasonCompetitionMatchStatAwayClubLink { get; set; }

        /// <summary>
        /// Gets or sets the away club logo.
        /// </summary>
        public string? PlayerSeasonCompetitionMatchStatAwayClubLogo { get; set; }

        /// <summary>
        /// Gets or sets the home club scored goals in the match.
        /// </summary>
        public int? PlayerSeasonCompetitionMatchStatHomeClubGoals { get; set; }

        /// <summary>
        /// Gets or sets the away club scored goals in the match.
        /// </summary>
        public int? PlayerSeasonCompetitionMatchStatAwayClubGoals { get; set; }

        /// <summary>
        /// Gets or sets the result of the match for the club.
        /// </summary>
        public MatchResult? PlayerSeasonCompetitionMatchStatMatchResult { get; set; }

        /// <summary>
        /// Gets or sets the link of the result of the match for the club.
        /// </summary>
        public string? PlayerSeasonCompetitionMatchStatMatchResultLink { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the match was decided in the addition time.
        /// </summary>
        public bool? PlayerSeasonCompetitionMatchStatIsResultAddition { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the match was decided in the penalties.
        /// </summary>
        public bool? PlayerSeasonCompetitionMatchStatIsResultPenalties { get; set; }

        /// <summary>
        /// Gets or sets the position of the player during the match.
        /// </summary>
        public string? PlayerSeasonCompetitionMatchStatPosition { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player was the captain during the match.
        /// </summary>
        public bool? PlayerSeasonCompetitionMatchStatIsCaptain { get; set; }

        /// <summary>
        /// Gets or sets the number of goals the player scored.
        /// </summary>
        public int? PlayerSeasonCompetitionMatchStatGoals { get; set; }

        /// <summary>
        /// Gets or sets the number of assists the player performed.
        /// </summary>
        public int? PlayerSeasonCompetitionMatchStatAssists { get; set; }

        /// <summary>
        /// Gets or sets the number of own goals the player scored.
        /// </summary>
        public int? PlayerSeasonCompetitionMatchStatOwnGoals { get; set; }

        /// <summary>
        /// Gets or sets the time in minutes when the player received a yellow card during the match.
        /// </summary>
        public int? PlayerSeasonCompetitionMatchStatYellowCard { get; set; }

        /// <summary>
        /// Gets or sets the time in minutes when the player received a second yellow card during the match.
        /// </summary>
        public int? PlayerSeasonCompetitionMatchStatSecondYellowCard { get; set; }

        /// <summary>
        /// Gets or sets the time in minutes when the player received a red card during the match.
        /// </summary>
        public int? PlayerSeasonCompetitionMatchStatRedCard { get; set; }

        /// <summary>
        /// Gets or sets the minute when the player was substituted on.
        /// </summary>
        public int? PlayerSeasonCompetitionMatchStatSubstitutedOn { get; set; }

        /// <summary>
        /// Gets or sets the minute when the player was substituted off.
        /// </summary>
        public int? PlayerSeasonCompetitionMatchStatSubstitutedOff { get; set; }

        /// <summary>
        /// Gets or sets the number of minutes the player was on the field.
        /// </summary>
        public int? PlayerSeasonCompetitionMatchStatMinutesPlayed { get; set; }

        /// <summary>
        /// Gets or sets the reason why the player did not played any minute of the match.
        /// </summary>
        public string? PlayerSeasonCompetitionMatchStatNotPlayingReason { get; set; }
    }
}
