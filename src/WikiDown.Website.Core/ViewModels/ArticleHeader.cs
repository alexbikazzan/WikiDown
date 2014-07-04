namespace WikiDown.Website.ViewModels
{
    public class ArticleHeader
    {
        public ArticleHeader(
            string pageTitle,
            ArticleId articleId,
            ArticleRevisionDate articleRevisionDate = null,
            ArticleHeaderTab? activeTab = null)
        {
            this.PageTitle = pageTitle;
            this.ArticleId = articleId;
            this.ArticleRevisionDate = articleRevisionDate;
            this.ActiveTab = activeTab;
        }

        public ArticleRevisionDate ArticleRevisionDate { get; set; }

        public ArticleId ArticleId { get; set; }

        public ArticleHeaderTab? ActiveTab { get; set; }

        public string PageTitle { get; set; }
    }
}