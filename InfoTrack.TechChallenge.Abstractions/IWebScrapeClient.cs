using System.Threading.Tasks;
using System.Xml;

namespace InfoTrack.TechChallenge.Abstractions
{
    public interface IWebScraperClient
    {
        Task<XmlDocument> GetPage(IWebScraperSearchEngineOptions options, string query, int pageNumber, int pageSize, int dynamicPageCursorPosition);
    }
}