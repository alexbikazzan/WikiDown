using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

using WikiDown.Security;
using WikiDown.Website.Controllers.Api;

namespace WikiDown.Website.Areas.WikiEdit.Controllers.Api
{
    [Authorize(Roles = ArticleAccessHelper.Editor)]
    [AuthorizeArticle(ArticleAccessType.CanEdit)]
    [RoutePrefix("api/articles-meta/{slug}")]
    public class ArticlesMetaController : WikiDownApiControllerBase
    {
        [HttpGet]
        [Route("redirects")]
        public IReadOnlyCollection<string> GetRedirects([FromUri] ArticleId slug)
        {
            var redirects = this.CurrentRepository.GetArticleRedirectsList(slug).Select(x => x.Title);

            return redirects.ToList();
        }

        [HttpPost]
        [Route("redirects")]
        public IReadOnlyCollection<string> SaveRedirects([FromUri] ArticleId slug, [FromBody] IEnumerable<string> redirects)
        {
            var articleRedirects = GetFilteredList(redirects).ToArray();

            var savedRedirects = this.CurrentRepository.SaveArticleRedirects(slug, articleRedirects);
            return savedRedirects.Select(x => x.RedirectToArticleSlug).ToList();
        }

        [HttpGet]
        [Route("tags")]
        public IReadOnlyCollection<string> GetTags([FromUri] ArticleId slug)
        {
            var article = this.GetEnsuredArticle(slug);

            var tags = article.Tags ?? Enumerable.Empty<string>();

            return tags.ToList();
        }

        [HttpPost]
        [Route("tags")]
        public IReadOnlyCollection<string> SaveTags([FromUri] ArticleId slug, [FromBody] IEnumerable<string> tags)
        {
            var savedTags = this.CurrentRepository.SaveArticleTags(slug, tags);

            return savedTags;
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
                (values ?? Enumerable.Empty<string>()).Select(x => (x ?? string.Empty).Trim())
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct()
                    .ToList();
        }
    }
}