using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

using WikiDown.Markdown;
using WikiDown.Security;
using WikiDown.Website.Areas.WikiEdit.Models;
using WikiDown.Website.Controllers.Api;

namespace WikiDown.Website.Areas.WikiEdit.Controllers.Api
{
    [Authorize(Roles = ArticleAccessHelper.Editor)]
    [AuthorizeArticle(ArticleAccessType.CanEdit)]
    [RoutePrefix("api/article-revisions/{slug}")]
    public class ArticleRevisionsController : WikiDownApiControllerBase
    {
        [HttpGet]
        [Route("")]
        public IReadOnlyCollection<ArticleRevisionListItemApiModel> ListRevisions([FromUri] ArticleId slug)
        {
            var articleRevisions = this.CurrentRepository.GetArticleRevisionsList(slug);

            return articleRevisions.Select(x => new ArticleRevisionListItemApiModel(x)).ToList();
        }

        [HttpGet]
        [Route("{revisionDate}")]
        public ArticleRevisionApiModel GetRevision([FromUri] ArticleId slug, [FromUri] ArticleRevisionDate revisionDate)
        {
            var articleRevision = this.GetEnsuredArticleRevision(slug, revisionDate);

            return new ArticleRevisionApiModel(slug, articleRevision);
        }

        [HttpGet]
        [Route("latest")]
        public ArticleRevisionApiModel GetLatestRevision([FromUri] ArticleId slug)
        {
            var articleRevision = this.CurrentRepository.GetArticleRevisionLatest(slug);

            return new ArticleRevisionApiModel(slug, articleRevision);
        }

        [HttpGet]
        [Route("{revisionDate}/preview")]
        public dynamic GetRevisionPreview([FromUri] ArticleId slug, [FromUri] ArticleRevisionDate revisionDate)
        {
            var articleRevision = this.GetEnsuredArticleRevision(slug, revisionDate);

            var htmlContent = MarkdownService.MakeHtml(articleRevision.MarkdownContent);
            return new { htmlContent };
        }

        [HttpDelete]
        [Route("{revisionDate}")]
        public void DeleteRevision([FromUri] ArticleId slug, [FromUri] ArticleRevisionDate revisionDate)
        {
            bool deleteSuccess = this.CurrentRepository.DeleteArticleRevision(slug, revisionDate);
            if (!deleteSuccess)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        [HttpPost]
        [Route("{revisionDate}/publish")]
        public void PublishRevision([FromUri] ArticleId slug, [FromUri] ArticleRevisionDate revisionDate)
        {
            bool setSuccess = this.CurrentRepository.PublishArticleRevision(slug, revisionDate);
            if (!setSuccess)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        [HttpPost]
        [Route("revert-to-draft")]
        public void RevertToDraft([FromUri] ArticleId slug)
        {
            bool revertSuccess = this.CurrentRepository.RevertArticleToDraft(slug);
            if (!revertSuccess)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        [HttpPost]
        [Route("")]
        public ArticleRevisionApiModel SaveRevision(
            [FromUri] ArticleId slug,
            [FromBody] ArticleRevisionApiModel formData,
            [FromUri] bool publish = false)
        {
            var userName = this.User.Identity.Name;

            var articleRevision = new ArticleRevision(
                slug,
                principal: this.User,
                markdownContent: formData.MarkdownContent,
                editSummary: formData.EditSummary);

            this.CurrentRepository.SaveArticleRevision(slug, articleRevision, publish);

            return new ArticleRevisionApiModel(slug, articleRevision);
        }

        private ArticleRevision GetEnsuredArticleRevision(ArticleId slug, ArticleRevisionDate articleRevisionDate)
        {
            var articleRevision = this.CurrentRepository.GetArticleRevision(slug, articleRevisionDate.DateTime);
            if (articleRevision == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return articleRevision;
        }
    }
}