using InfoTrack.TechChallenge.Logic;
using InfoTrack.TechChallenge.Models;
using Microsoft.AspNetCore.Mvc;
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
        private const string DefaultSearchEngine = "Bing";

        public TechChallengeController(BusinessLogic logic)
        {
            Logic = logic;
        }

        [HttpPost]
        [Route("seo-index-check")]
        public async Task<IEnumerable<int>> SeoIndexCheck([FromBody] SeoIndexCheckRequest seoIndexCheckRequest)
        {
            var seoIndexResult = await Logic.InfotrackSeoCheckIndexes(
                seoIndexCheckRequest.SearchEngine,
                seoIndexCheckRequest.UseStaticPages,
                seoIndexCheckRequest.Query,
                DefaultMaxResults);
            return seoIndexResult;
        }
    }
}
