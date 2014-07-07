using WikiDown.Security;

namespace WikiDown
{
    public class ArticleResult
    {
        public ArticleResult(
            Article article = null,
            ArticleRevision articleRevision = null)
        {
            this.Article = article;
            this.ArticleRevision = articleRevision;
        }

        public Article Article { get; internal set; }

        public ArticleRedirect ArticleRedirect { get; internal set; }

        public ArticleId ArticleRedirectId
        {
            get
            {
                return (this.ArticleRedirect != null) ? this.ArticleRedirect.RedirectToArticleSlug : null;
            }
        }

        public ArticleRevision ArticleRevision { get; internal set; }

        public string ArticleRedirectFromSlug
        {
            get
            {
                return (this.ArticleRedirect != null) ? this.ArticleRedirect.OriginalArticleSlug : null;
            }
        }

        public string ArticleRedirectToSlug
        {
            get
            {
                return (this.ArticleRedirect != null) ? this.ArticleRedirect.RedirectToArticleSlug : null;
            }
        }

        public bool HasArticle
        {
            get
            {
                return (this.Article != null);
            }
        }

        public bool HasRedirect
        {
            get
            {
                return (this.ArticleRedirect != null);
            }
        }

        public bool IsArticleRevisionActiveRevision
        {
            get
            {
                return (this.Article != null) && (this.ArticleRevision != null)
                       && (this.Article.ActiveRevisionId == this.ArticleRevision.Id);
            }
        }
    }
}