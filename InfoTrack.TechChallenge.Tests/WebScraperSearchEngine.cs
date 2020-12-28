using InfoTrack.TechChallenge.Abstractions;
using InfoTrack.TechChallenge.WebScraperEngine;
using InfoTrack.TechChallenge.WebScraperEngine.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace InfoTrack.TechChallenge.Tests
{
    public class WebScraperSearchEngine
    {
        [Fact]
        public async Task ShouldReturnResultsFromPages()
        {
            IEnumerable<IWebScrapeSearchResult> results = default;
            try
            {
                var webScraperOptions = new WebScraperSearchEngineOptionsInfotrackStatic
                {
                    SearchEngineName = "Bing",
                    ResultXpathSelector = "//CITE",
                    UrlFromResultElement = (resultElement) => resultElement.InnerText
                };
                var webScraperClient = new WebScraperClient(null);
                var webScrapeSearchEngine = new TextWebScraperEngine(webScraperClient);
                results = await webScrapeSearchEngine.SearchWithQueryAsync(webScraperOptions, "", 50);
            }
            catch (Exception)
            {
            }

            Assert.NotNull(results);
            Assert.NotEmpty(results);
        }
    }
}
