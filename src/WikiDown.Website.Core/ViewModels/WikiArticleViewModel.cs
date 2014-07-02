using System;

using WikiDown.Markdown;

namespace WikiDown.Website.ViewModels
{
    public class WikiArticleViewModel : WikiArticleViewModelBase
    {
        public WikiArticleViewModel(
            ArticlePage articlePage,
            ArticleId articleId,
            ArticleRevisionDate revisionDateTime,
            string redirectedFrom)
            : base(articleId)
        {
            if (articlePage == null)
            {
                throw new ArgumentNullException("articlePage");
            }

            this.RedirectedFrom = redirectedFrom;
            this.RevisionDateTime = revisionDateTime;

            if (articlePage.Revision == null)
            {
                return;
            }

            this.LatestRevisionCreatedAt = revisionDateTime.HasValue ? articlePage.Revision.CreatedAt : (DateTime?)null;

            string markdown = articlePage.Revision.MarkdownContent;
            this.HtmlContent = MarkdownService.MakeHtml(markdown);
        }

        public string HtmlContent { get; set; }

        public DateTime? RevisionDateTime { get; set; }

        public DateTime? LatestRevisionCreatedAt { get; set; }

        public string RedirectedFrom { get; set; }

        public override string PageTitle
        {
            get
            {
                if (this.LatestRevisionCreatedAt.HasValue)
                {
                    return string.Format(
                        "{0} - Revision from {1}",
                        this.ArticleTitle,
                        this.LatestRevisionCreatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                }

                return this.ArticleTitle;
            }
        }
    }
}