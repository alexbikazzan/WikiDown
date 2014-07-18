using System.Web.Http;

using WikiDown.Security;
using WikiDown.Website.Controllers.Api;

namespace WikiDown.Website.Areas.WikiEdit.Controllers.Api
{
    [Authorize(Roles = ArticleAccessHelper.Editor)]
    [AuthorizeArticle(ArticleAccessType.CanEdit)]
    [RoutePrefix("api/articles/{slug}")]
    public class ArticlesController : WikiDownApiControllerBase
    {
        [HttpGet]
        [Route("exists")]
        public dynamic GetArticleExists([FromUri] ArticleId slug)
        {
            var articleExists = this.CurrentRepository.GetArticleExists(slug);
            return new { exists = (articleExists != null) };
        }
    }
}