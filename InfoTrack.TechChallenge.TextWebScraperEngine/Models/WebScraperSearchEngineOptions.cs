using InfoTrack.TechChallenge.Abstractions;
using System;
using System.Linq;
using System.Web;
using System.Xml;

namespace InfoTrack.TechChallenge.WebScraperEngine.Models
{
    public class WebScraperSearchEngineOptionsInfotrackStatic : IWebScraperSearchEngineOptions
    {
        public string ResultXpathSelector { get; set; }
        public Func<XmlElement, string> UrlFromResultElement { get; set; }
        public string SearchEngineName { get; set; }

        public Uri GetUrl(string query, int pageNumber, int pageSize)
        {
            // TODO: behavior based Generalised url formation
            var uri = new Uri($"https://infotrack-tests.infotrack.com.au/{SearchEngineName}/Page{String.Format("{0:00}", pageNumber + 1)}.html");
            return uri;
        }
    }

    public class WebScraperSearchEngineOptions : IWebScraperSearchEngineOptions
    {
        public string ResultXpathSelector { get; set; }
        public Func<XmlElement, string> UrlFromResultElement { get; set; }
        public string SearchEngineName { get; set; }
        public string SearchEngineBaseUrlPath { get; set; }
        public string ParameterNameQuery { get; set; }
        public string ParameterNamePage { get; set; } = null;
        public string ParameterNamePageSize { get; set; } = null;
        public string ParameterNameRecordsSkip { get; set; } = null;

        public Uri GetUrl(string query, int pageNumber, int pageSize)
        {
            var searchUrlParts = new KeyValueList<string, string> {
                { ParameterNameQuery, HttpUtility.UrlEncode(query) },
                { ParameterNamePage, pageNumber.ToString() },
                { ParameterNamePageSize, pageSize.ToString() },
                { ParameterNameRecordsSkip, (pageNumber * pageSize).ToString() },
                { ParameterNameQuery, HttpUtility.UrlEncode(query) }
            };
            var searchUrl = $"{SearchEngineBaseUrlPath}?" + string.Join("&", searchUrlParts.Where(part => part.Key != null));
            var uri = new Uri(searchUrl);
            return uri;
        }
    }
}
