using System;
using System.Net;
using System.Text.RegularExpressions;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Mapster;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using TransfermarktScraper.BLL.Configuration;
using TransfermarktScraper.BLL.Enums.Extensions;
using TransfermarktScraper.BLL.Models;
using TransfermarktScraper.BLL.Models.Competition;
using TransfermarktScraper.BLL.Services.Interfaces;
using TransfermarktScraper.BLL.Utils;
using TransfermarktScraper.Data.Repositories.Interfaces;
using TransfermarktScraper.Domain.DTOs.Response;
using TransfermarktScraper.Domain.Entities;
using TransfermarktScraper.Domain.Enums;
using TransfermarktScraper.Domain.Exceptions;

namespace TransfermarktScraper.BLL.Services.Impl
{
    /// <inheritdoc/>
    public class CompetitionService : ICompetitionService
    {
        private readonly IPage _page;
        private readonly ICountryRepository _countryRepository;
        private readonly IClubService _clubService;
        private readonly ScraperSettings _scraperSettings;
        private readonly ILogger<CompetitionService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompetitionService"/> class.
        /// </summary>
        /// <param name="page">The Playwright page used for web scraping.</param>
        /// <param name="countryRepository">The country repository for accessing and managing the country data.</param>
        /// <param name="clubService">The club service for scraping club data from Transfermarkt.</param>
        /// <param name="scraperSettings">The scraper settings containing configuration values.</param>
        /// <param name="logger">The logger.</param>
        public CompetitionService(
            IPage page,
            ICountryRepository countryRepository,
            IClubService clubService,
            IOptions<ScraperSettings> scraperSettings,
            ILogger<CompetitionService> logger)
        {
            _page = page;
            _countryRepository = countryRepository;
            _clubService = clubService;
            _scraperSettings = scraperSettings.Value;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CompetitionResponse>> GetCompetitionsAsync(string countryTransfermarktId, bool forceScraping, CancellationToken cancellationToken)
        {
            var competitions = await _countryRepository.GetAllAsync(countryTransfermarktId, cancellationToken);

            forceScraping = forceScraping == true ? true : _scraperSettings.ForceScraping;

            if (forceScraping || competitions.Any(competition => string.IsNullOrEmpty(competition.Logo)))
            {
                var competitionsScraped = await ScrapeCompetitionsAsync(competitions, cancellationToken);

                competitions = await PersistCompetitionsAsync(countryTransfermarktId, competitionsScraped, cancellationToken);
            }

            var competitionDtos = competitions.Adapt<IEnumerable<CompetitionResponse>>();

            return competitionDtos;
        }

        /// <inheritdoc/>
        public CompetitionSearchResult ScrapeCompetitionFromSearchResults(IHtmlDocument document, string competitionTransfermarktId, string competitionName, string competitionLink, string url, int page, int competitionsSearched)
        {
            var competitionSearchResult = GetCompetitionTableDatasFromSearchResults(document, url, competitionTransfermarktId, page, competitionsSearched);

            if (competitionSearchResult.IsNextPageSearchNeeded || competitionSearchResult.CompetitionTableDatas == null)
            {
                return competitionSearchResult;
            }

            competitionSearchResult = GetCompetitionSearchResultFromCompetitionTableDatas(competitionSearchResult.CompetitionTableDatas, url, competitionName, competitionLink, competitionTransfermarktId);

            return competitionSearchResult;
        }

        /// <inheritdoc/>
        public async Task SetQuickSelectCompetitionsInterceptorAsync(Func<CountryQuickSelectResult, Task> onCountryQuickSelectResultCaptured)
        {
            await _page.RouteAsync("**/quickselect/competitions/**", async route =>
            {
                var url = route.Request.Url;
                _logger.LogDebug("Intercepted competition quickselect URL: {url}", url);

                var countryTransfermarktId = ExtractCountryTransfermarktId(url);

                IAPIResponse response;

                try
                {
                    response = await route.FetchAsync();

                    var competitionQuickSelectResults = await FormatQuickSelectCompetitionResponseAsync(response);

                    await route.AbortAsync();

                    var countryQuickSelectResult = new CountryQuickSelectResult
                    {
                        Id = countryTransfermarktId,
                        CompetitionQuickSelectResults = competitionQuickSelectResults,
                    };

                    await onCountryQuickSelectResultCaptured(countryQuickSelectResult);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"Error in interceptor for country: {countryTransfermarktId}");
                }
            });
        }

        /// <summary>
        /// Asynchronously formats the quick select competition response into a list of <see cref="CompetitionQuickSelectResult"/> objects.
        /// </summary>
        /// <param name="response">The API response containing quick select competition data in JSON format.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="CompetitionQuickSelectResult"/> objects.</returns>
        public async Task<IList<CompetitionQuickSelectResult>> FormatQuickSelectCompetitionResponseAsync(IAPIResponse response)
        {
            var json = await response.JsonAsync();

            if (json == null)
            {
                throw new Exception($"Error in {nameof(CompetitionService)}.{nameof(FormatQuickSelectCompetitionResponseAsync)}: json is null");
            }

            var competitionQuickSelectResults = new List<CompetitionQuickSelectResult>();

            foreach (var competitionElement in json.Value.EnumerateArray())
            {
                var name = competitionElement.GetProperty(nameof(CompetitionQuickSelectResult.Name).ToLower()).GetString();
                var link = competitionElement.GetProperty(nameof(CompetitionQuickSelectResult.Link).ToLower()).GetString();
                var id = competitionElement.GetProperty(nameof(CompetitionQuickSelectResult.Id).ToLower()).GetString();

                if (name == null || link == null || id == null)
                {
                    throw new Exception($"Error in {nameof(CompetitionService)}.{nameof(FormatQuickSelectCompetitionResponseAsync)}: There are empty fields: name {name} or link {link} or id {id}.");
                }

                var competitionQuickSelectResult = new CompetitionQuickSelectResult
                {
                    Name = name,
                    Link = link,
                    Id = id,
                };

                competitionQuickSelectResults.Add(competitionQuickSelectResult);
            }

            return competitionQuickSelectResults;
        }

        /// <summary>
        /// Scrapes competition data from a given URL for each competition in the provided list.
        /// Updates each competition's details with data fetched from Transfermarkt.
        /// </summary>
        /// <param name="competitions">The list of competitions to scrape data for.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A collection of updated <see cref="Competition"/> objects with scraped data.
        /// </returns>
        private async Task<IEnumerable<Competition>> ScrapeCompetitionsAsync(
            IEnumerable<Competition> competitions,
            CancellationToken cancellationToken)
        {
            foreach (var competition in competitions)
            {
                var response = await _page.GotoAsync(competition.Link);

                if (response == null || response.Status != (int)HttpStatusCode.OK)
                {
                    var message = $"Navigating to page: {_page.Url} failed. status code: {response?.Status.ToString() ?? "null"}";
                    throw ScrapingException.LogError(nameof(ScrapeCompetitionsAsync), nameof(CompetitionService), message, _page.Url, _logger);
                }

                competition.Logo = string.Concat(_scraperSettings.LogoUrl, "/", competition.TransfermarktId.ToLower(), ".png");

                // Competition Club Info
                var clubInfoLocator = GetClubInfoLocatorAsync();
                if (clubInfoLocator != null)
                {
                    await SetClubInfoValuesAsync(competition, clubInfoLocator);
                }

                // Competition Info Box
                var infoBoxLocator = GetInfoBoxLocator();
                if (infoBoxLocator != null)
                {
                    await SetInfoBoxValuesAsync(competition, infoBoxLocator);
                }

                // Competition Market Value Box
                var marKetValueBoxLocator = GetMarketValueBoxLocator();

                if (marKetValueBoxLocator != null)
                {
                    await SetMarketValueAsync(competition, marKetValueBoxLocator);
                }

                // Competition Clubs
                if (competition.Cup == Domain.Enums.Cup.None)
                {
                    var clubIds = await GetClubsAsync(competition.TransfermarktId, cancellationToken);
                    competition.ClubIds = clubIds;
                }
            }

            return competitions;
        }

        /// <summary>
        /// Persists a collection of competitions by updating them in the repository.
        /// </summary>
        /// <param name="countryTransfermarktId">The identifier of the country in Transfermarkt.</param>
        /// <param name="competitions">The collection of competition entities to update.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation, returning the updated competitions.</returns>
        private async Task<IEnumerable<Competition>> PersistCompetitionsAsync(
            string countryTransfermarktId,
            IEnumerable<Competition> competitions,
            CancellationToken cancellationToken)
        {
            var competitionsUpdated = await _countryRepository.UpdateRangeAsync(countryTransfermarktId, competitions, cancellationToken);

            return competitionsUpdated;
        }

        /// <summary>
        /// Retrieves the locator for the club information section on the page.
        /// </summary>
        /// <returns>A <see cref="ILocator"/> for the club info section, null if there is none.</returns>
        private ILocator? GetClubInfoLocatorAsync()
        {
            var selector = ".data-header__club-info";
            try
            {
                var clubInfoLocator = _page.Locator(selector) ?? throw new Exception("club info section not found.");

                return clubInfoLocator;
            }
            catch (TimeoutException)
            {
                _logger.LogWarning("Timeout exceeded while waiting for club info in page URL: {Url}", _page.Url);
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed.";
                throw ScrapingException.LogError(nameof(GetClubInfoLocatorAsync), nameof(CompetitionService), message, _page.Url, _logger, ex);
            }

            return null;
        }

        /// <summary>
        /// Extracts and assigns club info values from the provided locator to the given competition entity.
        /// </summary>
        /// <param name="competition">The competition entity to update with extracted club info values.</param>
        /// <param name="clubInfoLocator">The locator pointing to the club info section on the page.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task SetClubInfoValuesAsync(Competition competition, ILocator clubInfoLocator)
        {
            var selector = ".data-header__label";
            try
            {
                var labelLocators = clubInfoLocator.Locator(selector);

                var count = await labelLocators.CountAsync();

                for (int i = 0; i < count; i++)
                {
                    var labelLocator = labelLocators.Nth(i);

                    var labelText = await labelLocator.InnerTextAsync();

                    var competitionClubInfo = CompetitionClubInfoExtensions.ToEnum(labelText);

                    await CompetitionClubInfoExtensions.AssignToCompetitionProperty(competitionClubInfo, labelLocator, competition);
                }
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed.";
                ScrapingException.LogWarning(nameof(SetClubInfoValuesAsync), nameof(CompetitionService), message, _page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Retrieves the locator for the info box section on the page.
        /// </summary>
        /// <returns>A <see cref="ILocator"/> for the info box section, null if there is none.</returns>
        private ILocator GetInfoBoxLocator()
        {
            var selector = ".data-header__info-box";
            try
            {
                var infoBoxLocator = _page.Locator(selector) ?? throw new Exception("info box section not found.");

                return infoBoxLocator;
            }
            catch (TimeoutException)
            {
                _logger.LogWarning("Timeout exceeded while waiting for info box in page URL: {Url}", _page.Url);
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed.";
                throw ScrapingException.LogError(nameof(GetInfoBoxLocator), nameof(CompetitionService), message, _page.Url, _logger, ex);
            }

            return null;
        }

        /// <summary>
        /// Extracts and assigns info box values from the provided locator to the given competition entity.
        /// </summary>
        /// <param name="competition">The competition entity to update with extracted info box values.</param>
        /// <param name="infoBoxLocator">The locator pointing to the box info section on the page.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task SetInfoBoxValuesAsync(Competition competition, ILocator infoBoxLocator)
        {
            var selector = "li";
            try
            {
                var itemLocators = await infoBoxLocator.Locator(selector).AllAsync();

                foreach (var itemLocator in itemLocators)
                {
                    var itemText = await itemLocator.InnerTextAsync();
                    selector = "span";
                    var spanLocator = itemLocator.Locator(selector);
                    var spanTexts = await spanLocator.AllInnerTextsAsync();
                    var spanText = spanTexts.First();
                    var labelText = itemText.Replace(spanText, string.Empty).Trim();

                    var competitionInfoBox = CompetitionInfoBoxExtensions.ToEnum(labelText);
                    CompetitionInfoBoxExtensions.AssignToCompetitionProperty(competitionInfoBox, spanText, competition);
                }
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed.";
                ScrapingException.LogWarning(nameof(SetInfoBoxValuesAsync), nameof(CompetitionService), message, _page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Asynchronously waits for and retrieves the locator for the market value box on the page.
        /// </summary>
        /// <returns>A <see cref="ILocator"/> for the market value box element on the page, null if there is none.</returns>
        private ILocator? GetMarketValueBoxLocator()
        {
            var selector = ".data-header__box--small";
            try
            {
                var marKetValueBoxLocator = _page.Locator(selector) ?? throw new Exception("market value box not found.");

                return marKetValueBoxLocator;
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed.";
                ScrapingException.LogWarning(nameof(GetMarketValueBoxLocator), nameof(CompetitionService), message, _page.Url, _logger, ex);
            }

            return null;
        }

        /// <summary>
        /// Asynchronously sets the market value of a competition by extracting the relevant data from the market value box located on the page.
        /// </summary>
        /// <param name="competition">The competition object whose market value is to be set.</param>
        /// <param name="marKetValueBoxLocator">The locator for the market value box element on the page.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task SetMarketValueAsync(Competition competition, ILocator marKetValueBoxLocator)
        {
            var selector = ".data-header__last-update";
            try
            {
                var lastUpdateLocator = marKetValueBoxLocator.Locator(selector);
                var lastUpdateText = await lastUpdateLocator.InnerTextAsync();

                var boxTexts = await marKetValueBoxLocator.AllInnerTextsAsync();
                var boxText = boxTexts.First();

                var marketValueNumericString = boxText.Replace(lastUpdateText, string.Empty).Trim();
                var marketValueString = MoneyUtils.ExtractNumericPart(marketValueNumericString);
                competition.MarketValue = MoneyUtils.ConvertToFloat(marketValueString);
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed.";
                ScrapingException.LogWarning(nameof(SetMarketValueAsync), nameof(CompetitionService), message, _page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Retrieves the locators for club rows within the club grid on the page.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation that returns a read-only list of <see cref="ILocator"/> objects
        /// representing the rows within the club grid.
        /// </returns>
        private async Task<IReadOnlyList<ILocator>> GetClubRowLocatorsAsync()
        {
            var selector = "#yw1 tbody tr";
            try
            {
                var clubGridLocator = _page.Locator(selector);

                var clubRowLocators = await clubGridLocator.AllAsync();

                return clubRowLocators;
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed.";
                throw ScrapingException.LogError(nameof(GetClubRowLocatorsAsync), nameof(CompetitionService), message, _page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Retrieves the Transfermarkt IDs of clubs from a given competition.
        /// </summary>
        /// <param name="competitionTransfermarktId">The Transfermarkt ID of the competition.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A collection of Transfermarkt club IDs.</returns>
        private async Task<IEnumerable<string>> GetClubsAsync(string competitionTransfermarktId, CancellationToken cancellationToken)
        {
            var clubRowLocators = await GetClubRowLocatorsAsync();
            var clubIds = new List<string>();

            foreach (var clubRowLocator in clubRowLocators)
            {
                ClubResponse? club = null;

                try
                {
                    club = await _clubService.GetClubAsync(competitionTransfermarktId, clubRowLocator, cancellationToken);

                    clubIds.Add(club.TransfermarktId);
                }
                catch (ScrapingException ex)
                {
                    var message = $"Getting club: {club?.TransfermarktId ?? "null"} from competition {competitionTransfermarktId} failed.";
                    throw ScrapingException.LogError(nameof(GetClubsAsync), nameof(CompetitionService), message, _page.Url, _logger, ex);
                }
            }

            return clubIds;
        }

        /// <summary>
        /// Extracts the collection of HTML table data cells (<td> elements) for a specific competition from the Transfermarkt search results HTML document.
        /// </summary>
        /// <param name="document">The parsed HTML document representing the Transfermarkt search results page.</param>
        /// <param name="url">The source URL of the page being scraped. Used for logging in case of an error.</param>
        /// <param name="competitionTransfermarktId">The unique Transfermarkt identifier for the competition.</param>
        /// <param name="page">The page to search for the competitions in case the results are paginated.</param>
        /// <param name="competitionsSearched">The number of competitions already searched.</param>
        /// <returns>A <see cref="CompetitionSearchResult"/> containing both the <see cref="Competition"/> entity and the HTML element for the country column.
        /// Or in case the competition has not been found it contains the information needed to perform the search in the next page.</returns>
        private CompetitionSearchResult GetCompetitionTableDatasFromSearchResults(IHtmlDocument document, string url, string competitionTransfermarktId, int page, int competitionsSearched)
        {
            var selector = "h2.content-box-headline";
            try
            {
                var competitionHeaders = document.QuerySelectorAll(selector) ?? throw new Exception();

                selector = "competitions";
                var competitionHeader = competitionHeaders.FirstOrDefault(competitionHeader => competitionHeader.InnerHtml.Contains(selector, StringComparison.OrdinalIgnoreCase)) ?? throw new Exception();

                var competitionsHeaderString = competitionHeader.TextContent;

                if (!int.TryParse(Regex.Match(competitionsHeaderString, @"\d+").Value, out var competitionsTotal))
                {
                    throw new Exception("Failed to obtain the competitionsTotal.");
                }

                var competitionTable = competitionHeader?.ParentElement ?? throw new Exception("Failed to obtain the competitionTable.");

                selector = "table.items tbody tr";
                var competitionTableRows = competitionTable.QuerySelectorAll(selector) ?? throw new Exception("Failed to obtain the competitionTableRows.");

                var competitionTableRow = competitionTableRows.FirstOrDefault(competitionTableRow =>
                {
                    selector = "td:nth-child(2) a";
                    var competitionLinkElement = competitionTableRow.QuerySelector(selector) ?? throw new Exception("Failed to obtain the competitionLinkElement.");

                    selector = "href";
                    var competitionLink = competitionLinkElement.GetAttribute(selector) ?? throw new Exception("Failed to obtain the competitionLink.");

                    var competitionId = ExtractCompetitionTransfermarktId(competitionLink);

                    return competitionId.Equals(competitionTransfermarktId);
                });

                if (competitionTableRow == null)
                {
                    competitionsSearched = competitionsSearched + competitionTableRows.Count();

                    if (competitionsSearched >= competitionsTotal)
                    {
                        var message = $"Failed to find {nameof(Competition)} for the search input and {nameof(competitionTransfermarktId)} {competitionTransfermarktId}.";
                        throw ScrapingException.LogError(nameof(GetCompetitionTableDatasFromSearchResults), nameof(CompetitionService), message, url, _logger);
                    }

                    page = page + 1;

                    return new CompetitionSearchResult
                    {
                        CompetitionsSearched = competitionsSearched,
                        IsNextPageSearchNeeded = true,
                        Page = page,
                    };
                }

                selector = "td";
                var competitionTableDatas = competitionTableRow?.QuerySelectorAll(selector) ?? throw new Exception("Failed to obtain the competitionTableDatas.");

                return new CompetitionSearchResult
                {
                    CompetitionTableDatas = competitionTableDatas,
                    IsNextPageSearchNeeded = false,
                };
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed.";
                throw ScrapingException.LogError(nameof(GetCompetitionTableDatasFromSearchResults), nameof(CompetitionService), message, url, _logger, ex);
            }
        }

        /// <summary>
        /// Extracts competition details from a collection of HTML table cells representing a single competition row.
        /// </summary>
        /// <param name="competitionTableDatas">A collection of HTML table data cells corresponding to one competition row.</param>
        /// <param name="url">The source URL of the page being scraped. Used for logging in case of an error.</param>
        /// <param name="competitionName">The name of the competition.</param>
        /// <param name="competitionLink">The absolute URL to the competition page on Transfermarkt.</param>
        /// <param name="competitionTransfermarktId">The unique Transfermarkt identifier for the competition.</param>
        /// <returns>A <see cref="CompetitionSearchResult"/> containing both the <see cref="Competition"/> entity and the HTML element for the country column.</returns>
        private CompetitionSearchResult GetCompetitionSearchResultFromCompetitionTableDatas(IHtmlCollection<IElement> competitionTableDatas, string url, string competitionName, string competitionLink, string competitionTransfermarktId)
        {
            int index = 0;
            try
            {
                var logo = GetCompetitionLogo(competitionTableDatas, index, url);

                index = 2;
                var countryTableData = competitionTableDatas[index];
                Cup cup = default;
                if (countryTableData.ChildNodes.Count() == 0)
                {
                    cup = Cup.International;
                }

                index = 3;
                var clubsCountString = competitionTableDatas[index].TextContent.Trim();
                var clubsCount = string.IsNullOrEmpty(clubsCountString) ? default : int.Parse(clubsCountString);

                index = 4;
                var playersCountString = competitionTableDatas[index].TextContent.Trim();
                var playersCount = string.IsNullOrEmpty(playersCountString) ? default : int.Parse(playersCountString);

                index = 5;
                var marketValueNumericString = competitionTableDatas[index].TextContent.Trim();
                var marketValueString = string.IsNullOrEmpty(marketValueNumericString) ? default : MoneyUtils.ExtractNumericPart(marketValueNumericString);
                var marketValue = string.IsNullOrEmpty(marketValueString) ? default : MoneyUtils.ConvertToFloat(marketValueString);

                index = 6;
                var marketValueAverageNumericString = competitionTableDatas[index].TextContent.Trim();
                var marketValueAverageString = string.IsNullOrEmpty(marketValueAverageNumericString) ? default : MoneyUtils.ExtractNumericPart(marketValueAverageNumericString);
                var marketValueAverage = string.IsNullOrEmpty(marketValueAverageString) ? default : MoneyUtils.ConvertToFloat(marketValueAverageString);

                var competition = new Competition
                {
                    Name = competitionName,
                    Link = competitionLink,
                    Logo = logo,
                    TransfermarktId = competitionTransfermarktId,
                    ClubsCount = clubsCount,
                    PlayersCount = playersCount,
                    MarketValue = marketValue,
                    MarketValueAverage = marketValueAverage,
                    Cup = cup,
                };

                var competitionSearchResult = new CompetitionSearchResult
                {
                    Competition = competition,
                    CountryTableData = countryTableData,
                };

                return competitionSearchResult;
            }
            catch (Exception ex)
            {
                var message = $"Failed extracting {nameof(CompetitionSearchResult)} from {nameof(competitionTableDatas)}. Table data index: {index}.";
                throw ScrapingException.LogError(nameof(GetCompetitionSearchResultFromCompetitionTableDatas), nameof(CompetitionService), message, url, _logger, ex);
            }
        }

        /// <summary>
        /// Extracts the competition logo URL from the specified table data.
        /// </summary>
        /// <param name="competitionTableDatas">A collection of HTML elements representing the competition's table row data.</param>
        /// <param name="index">The index of the table data cell that contains the logo image.</param>
        /// <param name="url">The URL being scraped, used for logging purposes in case of an error.</param>
        /// <returns>The URL string of the competition logo.</returns>
        private string GetCompetitionLogo(IHtmlCollection<IElement> competitionTableDatas, int index, string url)
        {
            var selector = "img";
            try
            {
                var logoElement = competitionTableDatas[index].QuerySelector(selector) ?? throw new Exception($"Failed to obtain the competition logo using the selector {selector}");

                selector = "src";
                var logoString = logoElement.GetAttribute(selector) ?? throw new Exception($"Failed to obtain the competition logo using the attribute {selector}");
                var logo = logoString.Replace("mediumsmall", "header");

                return logo;
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed. Table data index: {index}.";
                throw ScrapingException.LogError(nameof(GetCompetitionLogo), nameof(CompetitionService), message, url, _logger, ex);
            }
        }

        /// <summary>
        /// Extracts the country Transfermarkt ID from a given URL.
        /// </summary>
        /// <param name="url">The competition link in Transfermarkt.</param>
        /// <returns>
        /// A string representing the extracted country Transfermarkt ID.
        /// </returns>
        private string ExtractCountryTransfermarktId(string url)
        {
            string pattern = @"/(\d+)$";
            var match = Regex.Match(url, pattern);
            string countryTransfermarktId = match.Groups[1].Value;
            return countryTransfermarktId;
        }

        /// <summary>
        /// Extracts the Transfermarkt competition identifier from the provided locators.
        /// </summary>
        /// <param name="link">The competition link in Transfermarkt.</param>
        /// <returns>The Transfermarkt competition identifier.</returns>
        private string ExtractCompetitionTransfermarktId(string link)
        {
            var index = link.LastIndexOf('/');
            string competitionTransfermarktId = link.Substring(index + 1);
            return competitionTransfermarktId;
        }
    }
}
