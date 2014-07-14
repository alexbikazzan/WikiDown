﻿using System.Web.Routing;

using WikiDown.Markdown;

namespace WikiDown.Website.ViewModels
{
    public class WikiArticleViewModel : WikiArticleViewModelBase
    {
        public WikiArticleViewModel(
            RequestContext requestContext,
            ArticleId articleId,
            ArticleRevisionDate articleRevisionDate = null,
            bool shouldRedirect = false)
            : base(requestContext, articleId, articleRevisionDate, HeaderTab.Article)
        {
            this.ShouldRedirect = shouldRedirect;

            var articleResult = CurrentRepository.GetArticleResult(articleId, articleRevisionDate);

            this.ArticleRedirectFrom = articleResult.ArticleRedirectFromSlug;
            this.ArticleRedirectTo = articleResult.ArticleRedirectToSlug;

            if (articleResult.HasRedirect && !shouldRedirect)
            {
                return;
            }

            this.DisplayArticleId = articleResult.HasArticle ? articleResult.Article.Id : this.DisplayArticleId;

            if (!articleResult.HasArticle || articleResult.ArticleRevision == null)
            {
                return;
            }

            string markdown = articleResult.ArticleRevision.MarkdownContent;
            this.HtmlContent = MarkdownService.MakeHtml(markdown);

            this.HasArticle = true;
        }

        public bool HasArticle { get; set; }

        public string HtmlContent { get; set; }

        public ArticleId ArticleRedirectFrom { get; set; }

        public ArticleId ArticleRedirectTo { get; set; }

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