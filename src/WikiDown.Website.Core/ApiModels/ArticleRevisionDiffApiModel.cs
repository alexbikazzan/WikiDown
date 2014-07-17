using System.Web;

using WikiDown.Markdown;

namespace WikiDown.Website.ApiModels
{
    public class ArticleRevisionDiffApiModel
    {
        public ArticleRevisionDiffApiModel(
            ArticleId articleId,
            ArticleRevisionDate oldRevisionDate,
            ArticleRevisionDate newRevisionDate,
            Repository repository)
        {
            var oldRevision = repository.GetArticleRevision(articleId, oldRevisionDate);
            string oldHtml = ((oldRevision != null) ? MarkdownService.MakeTextHtmlLinebreaks(oldRevision.MarkdownContent) : null)
                             ?? string.Empty;

            var newRevision = repository.GetArticleRevision(articleId, newRevisionDate);
            string newHtml = ((newRevision != null) ? MarkdownService.MakeTextHtmlLinebreaks(newRevision.MarkdownContent) : null)
                             ?? string.Empty;

            var diff = new Helpers.HtmlDiff(oldHtml, newHtml);

            string diffHtml = diff.Build();
            this.HtmlDiff = diffHtml;

            this.OldText = (oldRevision != null)
                               ? oldRevision.CreatedAt.ToString(ArticleRevisionDate.FormattedDateTimeFormat)
                               : null;
            this.NewText = (newRevision != null)
                               ? newRevision.CreatedAt.ToString(ArticleRevisionDate.FormattedDateTimeFormat)
                               : null;
        }

        public string HtmlDiff { get; set; }

        public string NewText { get; set; }

        public string OldText { get; set; }
    }
}