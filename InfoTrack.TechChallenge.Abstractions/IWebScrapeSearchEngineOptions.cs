using System;
using System.Xml;

namespace InfoTrack.TechChallenge.Abstractions
{
    public interface IWebScraperSearchEngineOptions
    {
        public string SearchEngineName { get; set; }
        bool StaticPages { get; set; }
        public string SearchEngineBaseUrlPath { get; set; }
        string ResultXpathSelector { get; set; }
        Func<XmlElement, string> UrlFromResultElement { get; set; }
        Uri GetUrl(string query, int pageNumber, int pageSize, int dynamicPageCursorPosition);
    }
}
