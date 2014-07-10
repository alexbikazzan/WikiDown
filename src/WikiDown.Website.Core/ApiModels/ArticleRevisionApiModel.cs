namespace WikiDown.Website.ApiModels
{
    public class ArticleRevisionApiModel
    {
        public ArticleRevisionApiModel()
        {
        }

        public ArticleRevisionApiModel(ArticleRevision articleRevision)
        {
            this.MarkdownContent = articleRevision.MarkdownContent;

            var articleId = new ArticleId(articleRevision.ArticleId);
            this.Slug = articleId.Slug;

            this.DateId = articleRevision.CreatedAt.ToString(ArticleRevision.IdDateTimeFormat);
            this.DateFormatted = articleRevision.CreatedAt.ToString(ArticleRevision.ReadableDateTimeFormat);
        }

        public string DateId { get; set; }

        public string DateFormatted { get; set; }

        public string EditSummary { get; set; }

        public string MarkdownContent { get; set; }

        public string Slug { get; set; }
    }
}