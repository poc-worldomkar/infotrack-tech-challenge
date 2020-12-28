using InfoTrack.TechChallenge.Abstractions;
using InfoTrack.TechChallenge.WebScraperEngine.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var remainingResults = maximumResults;

            int pageNumber = 0;
            int pageSize = 10;
            do
            {
                var page = await WebScraperClient.GetPage(options, query, pageNumber, pageSize);

                if (options?.ResultXpathSelector == null)
                {
                    return searchResultUrls;
                }

                var xmlResultNodes = page.DocumentElement.SelectNodes(options.ResultXpathSelector);
                var links = xmlResultNodes.Cast<XmlNode>()
                    .Select(x =>
                    {
                        var rawUrl = (x.InnerText.StartsWith("http") ? x.InnerText : $"https://{x.InnerText}");
                        var encodedUrl = HttpUtility.UrlPathEncode(rawUrl);
                        if (!Uri.TryCreate(encodedUrl, UriKind.RelativeOrAbsolute, out var result))
                        {
                            Uri.TryCreate(rawUrl.Split(' ')[0], UriKind.RelativeOrAbsolute, out result);
                        }

                        return result;
                    })
                    .ToList();
                if (links.Count() == 0)
                {
                    break;
                }

                searchResultUrls.AddRange(
                    links.Select(link => new WebScrapeSearchResult { Url = link })
                );
                remainingResults -= links.Count();
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