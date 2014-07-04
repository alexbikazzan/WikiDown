using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

using WikiDown.Markdown;
using WikiDown.Website.ApiModels;

namespace WikiDown.Website.Controllers.Api
{
    [RoutePrefix("api/articlerevisions/{articleId}")]
    public class ArticleRevisionsController : ApiControllerBase
    {
        [HttpGet]
        [Route("{articleRevisionDate}", Name = ApiRouteNames.ArticleRevisions)]
        public ArticleRevisionApiModel GetArticleRevision(
            [FromUri] ArticleId articleId,
            [FromUri] ArticleRevisionDate articleRevisionDate)
        {
            var articleRevision = this.GetEnsuredArticleRevision(articleId, articleRevisionDate.DateTime);

            return new ArticleRevisionApiModel(articleRevision);
        }

        [HttpGet]
        [Route("")]
        public IReadOnlyCollection<KeyValuePair<string, string>> ListArticleRevisions([FromUri] ArticleId articleId)
        {
            var revisions = this.CurrentRepository.GetArticleRevisions(articleId);

            return
                revisions.Select(x => x.DateTime)
                    .Select(
                        x =>
                        new KeyValuePair<string, string>(
                            x.ToString(ArticleRevision.IdDateTimeFormat),
                            x.ToString(ArticleRevision.ReadableDateTimeFormat)))
                    .ToList();
        }

        [HttpGet]
        [Route("{articleRevisionDate}/preview", Name = ApiRouteNames.ArticleRevisionPreview)]
        public dynamic GetArticleRevisionPreview(
            [FromUri] ArticleId articleId,
            [FromUri] ArticleRevisionDate articleRevisionDate)
        {
            var articleRevision = this.GetEnsuredArticleRevision(articleId, articleRevisionDate.DateTime);

            string htmlContent = MarkdownService.MakeHtml(articleRevision.MarkdownContent);
            return new { htmlContent = htmlContent };
        }

        private ArticleRevision GetEnsuredArticleRevision(ArticleId articleId, ArticleRevisionDate articleRevisionDate)
        {
            var articleRevision = this.CurrentRepository.GetArticleRevision(articleId, articleRevisionDate.DateTime);
            if (articleRevision == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return articleRevision;
        }
    }
}