using System.Net;
using System.Web.Http;

using WikiDown.Security;
using WikiDown.Website.Areas.WikiEdit.Models;
using WikiDown.Website.Controllers.Api;

namespace WikiDown.Website.Areas.WikiEdit.Controllers.Api
{
    [Authorize(Roles = ArticleAccessHelper.Admin)]
    [AuthorizeArticle(ArticleAccessType.CanAdmin)]
    [Route("api/articles-admin/{slug}")]
    public class ArticlesAdminController : WikiDownApiControllerBase
    {
        [HttpGet]
        public ArticleAdminApiModel GetArticle([FromUri] ArticleId slug)
        {
            var article = this.CurrentRepository.GetArticle(slug);
            if (article == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return new ArticleAdminApiModel(article);
        }

        [HttpPost]
        public void SaveArticle([FromUri] ArticleId slug, [FromBody] ArticleAdminApiModel formData)
        {
            formData.Save(slug, this.CurrentRepository);
        }

        [HttpDelete]
        public void DeleteArticle([FromUri] ArticleId slug)
        {
            bool deleteSuccess = this.CurrentRepository.DeleteArticle(slug);
            if (!deleteSuccess)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }
    }
}