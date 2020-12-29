﻿using InfoTrack.TechChallenge.Abstractions;
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
                new WebScraperSearchEngineOptionsInfotrackStatic { SearchEngineName = "Google", ResultXpathSelector = "//DIV//A[@href][DIV/CITE]" }
            };
            LiveSearchEngineOptions = new List<WebScraperSearchEngineOptions>
            {
                new WebScraperSearchEngineOptions{
                    SearchEngineName = "Bing",
                    ResultXpathSelector = "//LI[@class=\"b_algo\"]//CITE",
                    SearchEngineBaseUrlPath = "https://www.bing.com/search",
                    ParameterNameQuery = "q",
                    ParameterNameRecordsSkip = "first",
                    DynamicPageSize = true,
                    IndexStartsAtOne = true },
                new WebScraperSearchEngineOptions{
                    SearchEngineName = "Google",
                    ResultXpathSelector = "//DIV//A[@href][DIV/CITE]",
                    SearchEngineBaseUrlPath = "https://www.google.com.au/search",
                    ParameterNameQuery = "q",
                    ParameterNameRecordsSkip = "start" }
            };
            InfotrackDomains = new List<string> {
                "infotrack.nz",
                "infotrack.co.uk",
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
