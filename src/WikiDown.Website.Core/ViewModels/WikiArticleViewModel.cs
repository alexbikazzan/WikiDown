using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

using WikiDown.Markdown;

namespace WikiDown.Website.ViewModels
{
    public class WikiArticleViewModel : WikiArticleViewModelBase
    {
        private static readonly IReadOnlyCollection<string> EmptyCollection = new List<string>(0);

        public WikiArticleViewModel(
            RequestContext requestContext,
            ArticleId articleId,
            ArticleRevisionDate articleRevisionDate = null,
            bool shouldRedirect = false)
            : base(requestContext, articleId, articleRevisionDate, HeaderTab.Article)
        {
            this.ShouldRedirect = shouldRedirect;

            this.HtmlContent = new HtmlString(string.Empty);
            this.ArticleTags = EmptyCollection;

            var articleResult = CurrentRepository.GetArticleResult(articleId, articleRevisionDate);

            this.ArticleRedirectFrom = articleResult.ArticleRedirectFromSlug;
            this.ArticleRedirectTo = articleResult.ArticleRedirectToSlug;

            if (articleResult.HasRedirect && !shouldRedirect)
            {
                return;
            }

            this.ArticleTags = (articleResult.HasArticle && articleResult.Article.Tags != null)
                                   ? articleResult.Article.Tags.ToList()
                                   : this.ArticleTags;

            this.DisplayArticleId = articleResult.HasArticle ? articleResult.Article.Id : this.DisplayArticleId;

            if (!articleResult.HasArticle || articleResult.ArticleRevision == null)
            {
                return;
            }

            this.HasArticle = true;

            this.HtmlContent = new WikiDownArticleHtmlString(articleResult, this.CurrentRepository);
        }

        public bool HasArticle { get; set; }

        public IHtmlString HtmlContent { get; set; }

        public ArticleId ArticleRedirectFrom { get; set; }

        public ArticleId ArticleRedirectTo { get; set; }

        public IReadOnlyCollection<string> ArticleTags { get; set; }

        public bool ShouldRedirect { get; set; }

        public override string PageTitle
        {
            get
            {
                if (this.ArticleRevisionDate.HasValue)
                {
                    return string.Format(
                        "{0} - Revision from {1}",
                        this.ArticleTitle,
                        this.ArticleRevisionDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                }

                return base.PageTitle;
            }
        }
    }
}