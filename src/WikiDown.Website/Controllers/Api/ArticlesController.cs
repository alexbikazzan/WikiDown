using System;
using System.Web.Http;

namespace WikiDown.Website.Controllers.Api
{
    [Route("api/articles/{articleId?}", Name = ApiRouteNames.Articles)]
    public class ArticlesController : ApiControllerBase
    {
        [HttpGet]
        public ArticleRevision GetArticle(string articleId)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        //[Authorize(Roles = "root, admin, editor")]
        public Article SaveArticle(string articleId)
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        //[Authorize(Roles = "root, admin, editor")]
        public void DeleteArticle(string articleId)
        {
            throw new NotImplementedException();
        }
    }
}