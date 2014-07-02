using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

using WikiDown.Website.ApiModels;

namespace WikiDown.Website.Controllers.Api
{
    [RoutePrefix("api/articlerevisions/{slug}")]
    public class ArticleRevisionsController : ApiControllerBase
    {
        [HttpGet]
        [Route("{revisionDateTime}")]
        public ArticleRevisionApiModel GetArticleRevision(string slug, string revisionDateTime)
        {
            var articleId = new ArticleId(slug);
            if (!articleId.HasValue)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var revisionDate = new ArticleRevisionDate(revisionDateTime);
            if (!revisionDate.HasValue)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var articleRevision = this.CurrentRepository.GetArticleRevision(articleId, revisionDate.DateTime);
            if (articleRevision == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return new ArticleRevisionApiModel(articleRevision);
        }

        [HttpGet]
        [Route("")]
        public IReadOnlyCollection<KeyValuePair<string, string>> ListArticleRevisions(string slug)
        {
            var revisions = this.CurrentRepository.GetArticleRevisions(slug);

            return
                revisions.Select(x => x.DateTime)
                    .Select(
                        x =>
                        new KeyValuePair<string, string>(
                            x.ToString(ArticleRevision.IdDateTimeFormat),
                            x.ToString(ArticleRevision.ReadableDateTimeFormat)))
                    .ToList();
        }
    }
}