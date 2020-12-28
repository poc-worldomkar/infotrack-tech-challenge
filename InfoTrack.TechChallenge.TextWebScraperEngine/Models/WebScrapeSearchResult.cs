using InfoTrack.TechChallenge.Abstractions;
using System;

namespace InfoTrack.TechChallenge.WebScraperEngine.Models
{
    public class WebScrapeSearchResult : IWebScrapeSearchResult
    {
        public Uri Url { get; set; }
    }
}
