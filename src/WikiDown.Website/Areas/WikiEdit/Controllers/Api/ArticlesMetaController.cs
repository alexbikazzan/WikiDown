using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

using WikiDown.Security;
using WikiDown.Website.ApiModels;
using WikiDown.Website.Controllers.Api;

namespace WikiDown.Website.Areas.WikiEdit.Controllers.Api
{
    [Authorize(Roles = ArticleAccessHelper.Editor)]
    [RoutePrefix("api/articles-meta/{slug}")]
    public class ArticlesMetaController : WikiDownApiControllerBase
    {
        [HttpGet]
        [Route("")]
        public ArticlesMetaApiModel GetMeta([FromUri] ArticleId slug)
        {
            this.EnsureCanEditArticle(slug, this.User);

            var article = this.GetEnsuredArticle(slug);

            return new ArticlesMetaApiModel(article);
        }

        [HttpPost]
        [Route("")]
        public void SaveMeta([FromUri] ArticleId slug, [FromBody] ArticlesMetaApiModel formData)
        {
            this.EnsureCanEditArticle(slug, this.User);

            formData.Save(slug, this.CurrentRepository);
        }

        [HttpGet]
        [Route("redirects")]
        public IReadOnlyCollection<string> GetRedirects([FromUri] ArticleId slug)
        {
            this.EnsureCanEditArticle(slug, this.User);

            var redirects = this.CurrentRepository.GetArticleRedirectList(slug).Select(x => x.Title);

            return redirects.ToList();
        }

        [HttpPost]
        [Route("redirects")]
        public void SaveRedirects([FromUri] ArticleId slug, [FromBody] IEnumerable<string> redirects)
        {
            this.EnsureCanEditArticle(slug, this.User);

            var articleRedirects = GetFilteredList(redirects).ToArray();

            this.CurrentRepository.SaveArticleRedirects(slug, articleRedirects);
        }

        [HttpGet]
        [Route("tags")]
        public IReadOnlyCollection<string> GetTags([FromUri] ArticleId slug)
        {
            this.EnsureCanEditArticle(slug, this.User);

            var article = this.GetEnsuredArticle(slug);

            var tags = article.Tags ?? Enumerable.Empty<string>();

            return tags.ToList();
        }

        [HttpPost]
        [Route("tags")]
        public void SaveTags([FromUri] ArticleId slug, [FromBody] IEnumerable<string> tags)
        {
            this.EnsureCanEditArticle(slug, this.User);

            var articleTags = GetFilteredList(tags);

            var article = this.GetEnsuredArticle(slug);

            article.Tags = articleTags;

            this.CurrentRepository.SaveArticle(article);
        }

        private Article GetEnsuredArticle(ArticleId slug)
        {
            var article = this.CurrentRepository.GetArticle(slug);
            if (article == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return article;
        }

        private static IEnumerable<string> GetFilteredList(IEnumerable<string> values)
        {
            return
                (values ?? Enumerable.Empty<string>())
                    .Select(x => (x ?? string.Empty).Trim())
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct()
                    .ToList();
        }
    }
}