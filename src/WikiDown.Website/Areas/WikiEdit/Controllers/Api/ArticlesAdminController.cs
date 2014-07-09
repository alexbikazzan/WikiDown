using System.Net;
using System.Web.Http;

using WikiDown.Security;
using WikiDown.Website.ApiModels;
using WikiDown.Website.Controllers.Api;

namespace WikiDown.Website.Areas.WikiEdit.Controllers.Api
{
    [Authorize(Roles = ArticleAccessHelper.SuperUser)]
    [Route("api/articles-admin/{slug}")]
    public class ArticlesAdminController : WikiDownApiControllerBase
    {
        [HttpGet]
        public ArticlesAdminApiModel GetArticle([FromUri] ArticleId slug)
        {
            this.EnsureCanAdminArticle(slug, this.User);

            var article = this.CurrentRepository.GetArticle(slug);
            if (article == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var articleAccess = this.CurrentRepository.GetArticleAccess(slug);

            return new ArticlesAdminApiModel(articleAccess);
        }

        [HttpPost]
        public void SaveArticle([FromUri] ArticleId slug, [FromBody] ArticlesAdminApiModel formData)
        {
            this.EnsureCanAdminArticle(slug, this.User);

            formData.Save(slug, this.CurrentRepository);
        }

        [HttpDelete]
        public void DeleteArticle([FromUri] ArticleId slug, [FromUri] bool? permanent = null)
        {
            this.EnsureCanAdminArticle(slug, this.User);

            bool deletePermanent = (permanent.HasValue && permanent.Value
                                    && this.User.IsInRole(ArticleAccessHelper.Admin));

            bool deleteSuccess = this.CurrentRepository.DeleteArticle(slug, deletePermanent);
            if (!deleteSuccess)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }
    }
}