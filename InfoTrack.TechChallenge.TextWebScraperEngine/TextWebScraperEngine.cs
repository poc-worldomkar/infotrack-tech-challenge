using InfoTrack.TechChallenge.Abstractions;
using InfoTrack.TechChallenge.WebScraperEngine.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace InfoTrack.TechChallenge.WebScraperEngine
{
    public class TextWebScraperEngine : IWebScraperSearchEngine
    {
        IWebScraperClient WebScraperClient;

        public TextWebScraperEngine(IWebScraperClient webScrapeClient)
        {
            WebScraperClient = webScrapeClient;
        }

        public async Task<IEnumerable<IWebScrapeSearchResult>> SearchWithQueryAsync(IWebScraperSearchEngineOptions options, string query, int maximumResults)
        {
            var searchResultUrls = new List<IWebScrapeSearchResult> { };

            if (options.ResultXpathSelector == null)
            {
                return searchResultUrls;
            }

            int pageNumber = 0;
            int pageSize = 10;
            int dynamicPageCursorPosition = 0;
            var remainingResults = maximumResults;

            do
            {
                var page = await WebScraperClient.GetPage(options, query, pageNumber, pageSize, dynamicPageCursorPosition);
                var xmlResultNodes = page.DocumentElement.SelectNodes(options.ResultXpathSelector);
                if (page.DocumentElement.ChildNodes.Count == 0 || xmlResultNodes.Count == 0)
                {
                    // Google complains about unusual traffic
                }

                var links = xmlResultNodes.Cast<XmlNode>()
                    .Select(x =>
                    {
                        Uri result = default;
                        try
                        {
                            var extractedUrl = options.UrlFromResultElement(x as XmlElement);
                            var rawUrl = (extractedUrl.StartsWith("http") ? extractedUrl : $"https://{extractedUrl}");
                            var encodedUrl = HttpUtility.UrlPathEncode(rawUrl);
                            if (!Uri.TryCreate(encodedUrl, UriKind.RelativeOrAbsolute, out result))
                            {
                                Uri.TryCreate(rawUrl.Split(' ')[0], UriKind.RelativeOrAbsolute, out result);
                            }
                        }
                        catch (Exception)
                        {
                        }
                        return result;
                    })
                    .ToList();
                if (links.Count() == 0)
                {
                    // Out of search results most probably
                    break;
                }

                searchResultUrls.AddRange(
                    links.Select(link => new WebScrapeSearchResult { Url = link })
                );
                remainingResults -= links.Count();
                dynamicPageCursorPosition += links.Count();
                pageNumber++;
            } while (remainingResults > 0);
            return searchResultUrls.Take(maximumResults);
        }

        public static void Configure(IServiceCollection services)
        {
            services.AddSingleton<IWebScraperClient, WebScraperClient>();
            services.AddSingleton<IWebScraperSearchEngine, TextWebScraperEngine>();
        }
    }
}