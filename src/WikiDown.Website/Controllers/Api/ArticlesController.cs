using System;
using System.Web.Http;

namespace WikiDown.Website.Controllers.Api
{
    [RoutePrefix("api/articles")]
    public class ArticlesController : ApiControllerBase
    {
        [HttpGet]
        [Route("{articleId}")]
        public ArticleRevision GetArticle(string articleId)
        {
            throw new NotImplementedException();
        }
    }
}