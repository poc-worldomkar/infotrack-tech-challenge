using InfoTrack.TechChallenge.Abstractions;
using System;
using System.Linq;
using System.Text.Json.Serialization;
using System.Web;
using System.Xml;

namespace InfoTrack.TechChallenge.WebScraperEngine.Models
{
    public class WebScraperSearchEngineOptionsInfotrackStatic : IWebScraperSearchEngineOptions
    {
        public string SearchEngineName { get; set; }
        public bool StaticPages { get; set; } = true;
        public string SearchEngineBaseUrlPath { get; set; } = "https://infotrack-tests.infotrack.com.au/";
        public string ResultXpathSelector { get; set; }
        [JsonIgnore]
        public Func<XmlElement, string> UrlFromResultElement { get; set; }

        public WebScraperSearchEngineOptionsInfotrackStatic()
        {
            UrlFromResultElement = (XmlElement resultElement) =>
            {
                var resultUrl = resultElement.GetAttribute("href");
                if (string.IsNullOrWhiteSpace(resultUrl))
                {
                    resultUrl = resultElement.InnerText;
                }

                return resultUrl;
            };
        }

        public Uri GetUrl(string query, int pageNumber, int pageSize, int dynamicPageCursorPosition)
        {
            // TODO: behavior based Generalised url formation
            var uri = new Uri($"{SearchEngineBaseUrlPath}{SearchEngineName}/Page{String.Format("{0:00}", pageNumber + 1)}.html");
            return uri;
        }
    }

    public class WebScraperSearchEngineOptions : IWebScraperSearchEngineOptions
    {
        public string SearchEngineName { get; set; }
        public bool StaticPages { get; set; } = false;
        public string SearchEngineBaseUrlPath { get; set; }
        public string ResultXpathSelector { get; set; }
        public string ParameterNameQuery { get; set; }
        public string ParameterNamePage { get; set; } = null;
        public string ParameterNamePageSize { get; set; } = null;
        public string ParameterNameRecordsSkip { get; set; } = null;
        public bool DynamicPageSize { get; set; } = false;
        public bool IndexStartsAtOne { get; set; } = false; //  Default is start at 0
        [JsonIgnore]
        public Func<XmlElement, string> UrlFromResultElement { get; set; }

        public WebScraperSearchEngineOptions()
        {
            UrlFromResultElement = (XmlElement resultElement) =>
            {
                var resultUrl = resultElement.GetAttribute("href");
                if (string.IsNullOrWhiteSpace(resultUrl))
                {
                    resultUrl = resultElement.InnerText;
                }

                return resultUrl;
            };
        }

        public Uri GetUrl(string query, int pageNumber, int pageSize, int dynamicPageCursorPosition)
        {
            var searchUrlParts = new KeyValueList<string, string> {
                { ParameterNameQuery, HttpUtility.UrlEncode(query) },
                { ParameterNamePage, pageNumber.ToString() },
                { ParameterNamePageSize, pageSize.ToString() },
                { ParameterNameRecordsSkip,
                    (DynamicPageSize && dynamicPageCursorPosition != -1)
                        ? (dynamicPageCursorPosition + (IndexStartsAtOne ? 1: 0)).ToString()
                        : (pageNumber * pageSize).ToString() },
            };
            var searchUrl = $"{SearchEngineBaseUrlPath}?" +
                string.Join(
                    "&",
                    searchUrlParts
                        .Where(part => !string.IsNullOrWhiteSpace(part.Key))
                        .Select(part => $"{part.Key}={part.Value}")
                );
            var uri = new Uri(searchUrl);
            return uri;
        }
    }
}
