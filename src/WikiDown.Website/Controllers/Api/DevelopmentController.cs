using System.Web.Http;

using WikiDown.Security;

#if DEBUG

namespace WikiDown.Website.Controllers.Api
{
    [Authorize(Roles = ArticleAccessHelper.Admin)]
    [Route("api/dev/{action}")]
    public class DevelopmentController : WikiDownApiControllerBase
    {
        [HttpGet]
        public dynamic SaveArticles()
        {
            return this.CurrentRepository.DebugSaveAllArticles();
        }

        [HttpGet]
        public dynamic SaveArticleRevisions()
        {
            return this.CurrentRepository.DebugSaveAllArticleRevisions();
        }

        [HttpGet]
        public dynamic SaveArticleRedirects()
        {
            return this.CurrentRepository.DebugSaveAllArticleRedirects();
        }
    }
}

#endif