using InfoTrack.TechChallenge.Logic;
using InfoTrack.TechChallenge.Models;
using InfoTrack.TechChallenge.WebScraperEngine.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Practices.EnterpriseLibrary.Common.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InfoTrack.TechChallenge.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public partial class TechChallengeController : ControllerBase
    {
        private readonly BusinessLogic Logic;
        private const int DefaultMaxResults = 50;

        public TechChallengeController(BusinessLogic logic)
        {
            Logic = logic;
        }

        [HttpPost]
        [Route("seo-index-check")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> SeoIndexCheck([FromBody] SeoIndexCheckRequest seoIndexCheckRequest)
        {
            Guard.ArgumentNotNullOrEmpty(seoIndexCheckRequest.SearchEngine, "searchEngine");
            Guard.ArgumentNotNullOrEmpty(seoIndexCheckRequest.Query, "query");

            try
            {
                var seoIndexResult = await Logic.InfotrackSeoCheckIndexes(
                    seoIndexCheckRequest.SearchEngine,
                    seoIndexCheckRequest.UseStaticPages,
                    seoIndexCheckRequest.Query,
                    DefaultMaxResults);
                return new JsonResult(seoIndexResult);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(new { Error = e.Message });
            }
        }

        [HttpGet]
        [Route("get-search-engines")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<WebScraperSearchEngineOptions> GetSearchEngines()
        {
            var searchEngines = BusinessLogicOptions.GetSearchEngines();
            return searchEngines;
        }

        [HttpPost]
        [Route("new-search-engine")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult NewSearchEngine([FromBody] WebScraperSearchEngineOptions newSearchEngine)
        {
            Guard.ArgumentNotNullOrEmpty(newSearchEngine.SearchEngineName, "searchEngineName");
            Guard.ArgumentNotNullOrEmpty(newSearchEngine.SearchEngineBaseUrlPath, "searchEngineBaseUrlPath");
            Guard.ArgumentNotNullOrEmpty(newSearchEngine.ResultXpathSelector, "ResultXpathSelector");
            Guard.ArgumentNotNullOrEmpty(newSearchEngine.ParameterNameQuery, "ParameterNameQuery");

            try
            {
                // TODO: Persist to sqlite or something
                BusinessLogicOptions.AddNewSearchEngine(newSearchEngine);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(new { Error = e.Message });
            }

            return new NoContentResult();
        }
    }
}
