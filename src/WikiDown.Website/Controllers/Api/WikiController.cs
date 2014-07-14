using System.Web.Http;

namespace WikiDown.Website.Controllers.Api
{
    [RoutePrefix("api/wiki")]
    public class WikiController : WikiDownApiControllerBase
    {
        [HttpGet]
        [Route("search")]
        public dynamic SearchArticles([FromUri] string q)
        {
            return this.CurrentRepository.SearchArticles(q);
        }
    }
}