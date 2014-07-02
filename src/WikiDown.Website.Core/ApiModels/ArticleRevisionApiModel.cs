namespace WikiDown.Website.ApiModels
{
    public class ArticleRevisionApiModel
    {
        public ArticleRevisionApiModel(ArticleRevision articleRevision)
        {
            this.MarkdownContent = articleRevision.MarkdownContent;
        }

        public string MarkdownContent { get; set; }
    }
}