using InfoTrack.TechChallenge.Abstractions;
using InfoTrack.TechChallenge.WebScraperEngine.Models;
using System.Collections.Generic;
using System.Linq;

namespace InfoTrack.TechChallenge.Logic
{
    public static class BusinessLogicOptions
    {
        // Static so it can temporarily showcase runtime configuration changes
        // Ideally in appsettings.json with services.Configure Options pattern
        public static List<string> InfotrackDomains { get; private set; }
        private static List<WebScraperSearchEngineOptionsInfotrackStatic> StaticSearchEngineOptions;
        private static List<WebScraperSearchEngineOptions> LiveSearchEngineOptions;

        static BusinessLogicOptions()
        {
            StaticSearchEngineOptions = new List<WebScraperSearchEngineOptionsInfotrackStatic> {
                new WebScraperSearchEngineOptionsInfotrackStatic { SearchEngineName = "Bing", ResultXpathSelector = "//LI[@class=\"b_algo\"]//CITE" },
                new WebScraperSearchEngineOptionsInfotrackStatic { SearchEngineName = "Google", ResultXpathSelector = "//DIV//A//CITE" }
            };
            LiveSearchEngineOptions = new List<WebScraperSearchEngineOptions>
            {

            };
            InfotrackDomains = new List<string> {
                "infotrack.nz",
                "infotrack.com.au",
                "infotrackgo.com.au"
            };
        }

        public static IWebScraperSearchEngineOptions GetSearchEngineOptions(string searchEngine, bool useStaticPages)
        {
            if (useStaticPages)
            {
                var staticSearchEngineOptions = StaticSearchEngineOptions.FirstOrDefault(options => options.SearchEngineName == searchEngine);
                return staticSearchEngineOptions;
            }

            var liveSearchEngineOptions = LiveSearchEngineOptions.FirstOrDefault(options => options.SearchEngineName == searchEngine);
            return liveSearchEngineOptions;
        }
    }
}
