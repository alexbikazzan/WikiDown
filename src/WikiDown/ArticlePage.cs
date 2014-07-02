namespace WikiDown
{
    public class ArticlePage
    {
        private ArticlePage()
        {
        }

        public Article Article { get; private set; }

        public ArticleId ArticleId { get; private set; }

        public bool IsRedirect
        {
            get
            {
                return (this.Article != null) && (this.Article.Id == this.ArticleId.Id);
            }
        }

        public bool HasArticle
        {
            get
            {
                return (this.Article != null);
            }
        }

        public ArticleRevision Revision { get; private set; }

        public string RedirectArticleSlug { get; private set; }

        public static ArticlePage ForArticle(ArticleId articleId, Article article, ArticleRevision revision = null)
        {
            return new ArticlePage { ArticleId = articleId, Article = article, Revision = revision };
        }

        public static ArticlePage ForNotFound()
        {
            return new ArticlePage();
        }

        public static ArticlePage ForRedirect(string redirectArticleSlug)
        {
            return new ArticlePage { RedirectArticleSlug = redirectArticleSlug };
        }
    }
}