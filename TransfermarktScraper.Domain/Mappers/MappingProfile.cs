using AutoMapper;
using TransfermarktScraper.Domain.Enums.Extensions;
using TransfermarktScraper.Domain.Utils;

namespace TransfermarktScraper.Domain.Mappers
{
    /// <summary>
    /// Represents the mapping profile for AutoMapper.
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MappingProfile"/> class.
        /// </summary>
        public MappingProfile()
        {
            // Country
            CreateMap<Entities.Country, DTOs.Response.Country>();
            CreateMap<DTOs.Response.Country, Entities.Country>();
            CreateMap<DTOs.Request.Country, DTOs.Response.Country>();
            CreateMap<DTOs.Response.Country, DTOs.Request.Country>();

            // Competition
            CreateMap<Entities.Competition, DTOs.Response.Competition>()
                .ForMember(destination => destination.Tier, options => options.MapFrom(source => TierExtensions.ToString(source.Tier)))
                .ForMember(destination => destination.Cup, options => options.MapFrom(source => CupExtensions.ToString(source.Cup)))
                .ForMember(destination => destination.MarketValue, options => options.MapFrom(source => MoneyUtils.ConvertToString(source.MarketValue)))
                .ForMember(destination => destination.MarketValueAverage, options => options.MapFrom(source => MoneyUtils.ConvertToString(source.MarketValueAverage)));
            CreateMap<DTOs.Response.Competition, Entities.Competition>()
                .ForMember(destination => destination.Tier, options => options.MapFrom(source => TierExtensions.ToEnum(source.Tier)))
                .ForMember(destination => destination.Cup, options => options.MapFrom(source => CupExtensions.ToEnum(source.Cup)))
                .ForMember(destination => destination.MarketValue, options => options.MapFrom(source => MoneyUtils.ConvertToFloat(source.MarketValue)))
                .ForMember(destination => destination.MarketValueAverage, options => options.MapFrom(source => MoneyUtils.ConvertToFloat(source.MarketValueAverage)));

            // Club
            CreateMap<Entities.Club, DTOs.Response.Club>()
                .ForMember(destination => destination.MarketValue, options => options.MapFrom(source => MoneyUtils.ConvertToString(source.MarketValue)))
                .ForMember(destination => destination.MarketValueAverage, options => options.MapFrom(source => MoneyUtils.ConvertToString(source.MarketValueAverage)));
            CreateMap<DTOs.Response.Club, Entities.Club>()
                .ForMember(destination => destination.MarketValue, options => options.MapFrom(source => MoneyUtils.ConvertToFloat(source.MarketValue)))
                .ForMember(destination => destination.MarketValueAverage, options => options.MapFrom(source => MoneyUtils.ConvertToFloat(source.MarketValueAverage)));

            // Player
            CreateMap<Entities.Player, DTOs.Response.Player>();
            CreateMap<DTOs.Response.Player, Entities.Player>();

            // MarketValue
            CreateMap<Entities.MarketValue, DTOs.Response.MarketValue>();
            CreateMap<DTOs.Response.MarketValue, Entities.MarketValue>();

            // PlayerStat
            CreateMap<Entities.Stat.PlayerStat, DTOs.Response.Stat.PlayerStat>();
            CreateMap<DTOs.Response.Stat.PlayerStat, Entities.Stat.PlayerStat>();

            CreateMap<DTOs.Response.Stat.Career.PlayerCareerStat, Entities.Stat.Career.PlayerCareerStat>();
            CreateMap<Entities.Stat.Career.PlayerCareerStat, DTOs.Response.Stat.Career.PlayerCareerStat>();

            CreateMap<DTOs.Response.Stat.Career.PlayerCareerCompetitionStat, Entities.Stat.Career.PlayerCareerCompetitionStat>();
            CreateMap<Entities.Stat.Career.PlayerCareerCompetitionStat, DTOs.Response.Stat.Career.PlayerCareerCompetitionStat>();

            CreateMap<DTOs.Response.Stat.Season.PlayerSeasonStat, Entities.Stat.Season.PlayerSeasonStat>();
            CreateMap<Entities.Stat.Season.PlayerSeasonStat, DTOs.Response.Stat.Season.PlayerSeasonStat>();

            CreateMap<DTOs.Response.Stat.Season.PlayerSeasonCompetitionStat, Entities.Stat.Season.PlayerSeasonCompetitionStat>();
            CreateMap<Entities.Stat.Season.PlayerSeasonCompetitionStat, DTOs.Response.Stat.Season.PlayerSeasonCompetitionStat>();

            CreateMap<DTOs.Response.Stat.Season.PlayerSeasonCompetitionMatchStat, Entities.Stat.Season.PlayerSeasonCompetitionMatchStat>();
            CreateMap<Entities.Stat.Season.PlayerSeasonCompetitionMatchStat, DTOs.Response.Stat.Season.PlayerSeasonCompetitionMatchStat>();
        }
    }
}
