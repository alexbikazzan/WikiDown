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
        }

        public string Slug { get; set; }

        public string EditSummary { get; set; }

        public string MarkdownContent { get; set; }
    }
}