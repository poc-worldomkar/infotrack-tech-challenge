using System.Collections.Generic;
using System.Threading.Tasks;

namespace InfoTrack.TechChallenge.Abstractions
{
    public interface IWebScraperSearchEngine
    {
        Task<IEnumerable<IWebScrapeSearchResult>> SearchWithQueryAsync(IWebScraperSearchEngineOptions options, string query, int maximumResults);
    }
}
