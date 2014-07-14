namespace WikiDown.Website.Areas.WikiEdit.Models
{
    public class ArticleRevisionApiModel
    {
        public ArticleRevisionApiModel()
        {
        }

        public ArticleRevisionApiModel(ArticleId articleId, ArticleRevision articleRevision)
        {
            this.Slug = articleId.Slug;

            if (articleRevision == null)
            {
                return;
            }

            this.MarkdownContent = articleRevision.MarkdownContent;

            var articleRevisionDate = new ArticleRevisionDate(articleRevision.CreatedAt);

            this.DateId = articleRevisionDate.DateTimeId;
            this.DateFormatted = articleRevisionDate.DateTimeFormatted;
        }

        public string DateId { get; set; }

        public string DateFormatted { get; set; }

        public string EditSummary { get; set; }

        public string MarkdownContent { get; set; }

        public string Slug { get; set; }
    }
}