using System;
using System.Xml;

namespace InfoTrack.TechChallenge.Abstractions
{
    public interface IWebScraperSearchEngineOptions
    {
        string ResultXpathSelector { get; set; }
        Func<XmlElement, string> UrlFromResultElement { get; set; }
        Uri GetUrl(string query, int pageNumber, int pageSize);
    }
}
