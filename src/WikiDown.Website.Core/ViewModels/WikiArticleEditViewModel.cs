using System.Web.Routing;

namespace WikiDown.Website.ViewModels
{
    public class WikiArticleEditViewModel : WikiArticleViewModelBase
    {
        public WikiArticleEditViewModel(
            RequestContext requestContext,
            ArticleId articleId,
            ArticleRevisionDate articleRevisionDate = null)
            : base(requestContext, articleId, articleRevisionDate, HeaderTab.Edit)
        {
            this.IsCreateMode = (this.Article == null || string.IsNullOrWhiteSpace(this.Article.Id));
        }

        public bool IsCreateMode { get; private set; }
    }
}