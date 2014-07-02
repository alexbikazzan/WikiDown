namespace WikiDown.Website.ViewModels
{
    public class WikiArticleNotFoundViewModel : WikiArticleViewModelBase
    {
        public WikiArticleNotFoundViewModel(ArticleId articleId)
            : base(articleId)
        {
        }

        public override string PageTitle
        {
            get
            {
                return string.Format("Not found: {0}", this.ArticleTitle);
            }
        }
    }
}