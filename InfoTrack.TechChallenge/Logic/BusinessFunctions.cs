using InfoTrack.TechChallenge.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoTrack.TechChallenge.Logic
{
    public class BusinessLogic
    {
        IWebScraperSearchEngine SearchEngine;

        public BusinessLogic(IWebScraperSearchEngine webScraperSearchEngine)
        {
            SearchEngine = webScraperSearchEngine;
        }

        public async Task<IEnumerable<int>> InfotrackSeoCheckIndexes(string searchEngine, bool useStaticPages, string query, int maximumResults = 10)
        {
            var searchEngineOptions = BusinessLogicOptions.GetSearchEngineOptions(searchEngine, useStaticPages);
            var scrapeSearchResults = await SearchEngine.SearchWithQueryAsync(searchEngineOptions, query, maximumResults);
            var searchResults = new List<int>();
            var index = 0;
            foreach (var result in scrapeSearchResults)
            {
                index++;
                if (BusinessLogicOptions.InfotrackDomains.Any(domain => result.Url.Host.EndsWith(domain)))
                {
                    searchResults.Add(index);
                }
            }

            return searchResults;
        }
    }
}
