using System;

using WikiDown.Security;

namespace WikiDown.Website.Areas.WikiEdit.Models
{
    public class ArticleAdminApiModel
    {
        public ArticleAdminApiModel()
        {
        }

        public ArticleAdminApiModel(ArticleId articleId, Repository repository)
        {
            var article = repository.GetArticle(articleId);

            var articleAccess = (article != null) ? article.ArticleAccess : ArticleAccess.Default();

            this.CanRead = (int)articleAccess.CanRead;
            this.CanEdit = (int)articleAccess.CanEdit;
            this.CanAdmin = (int)articleAccess.CanAdmin;
        }

        public int CanAdmin { get; set; }

        public int CanEdit { get; set; }

        public int CanRead { get; set; }

        public void Save(ArticleId articleId, Repository repository)
        {
            var article = repository.GetArticle(articleId) ?? new Article(articleId);

            var canAdmin = TryGetArticleAccess(this.CanAdmin);
            var canEdit = TryGetArticleAccess(this.CanEdit);
            var canRead = TryGetArticleAccess(this.CanRead);

            article.ArticleAccess.CanAdmin = canAdmin;
            article.ArticleAccess.CanEdit = canEdit;
            article.ArticleAccess.CanRead = canRead;

            repository.SaveArticle(article);
        }

        private static ArticleAccessLevel TryGetArticleAccess(int intValue)
        {
            return Enum.IsDefined(typeof(ArticleAccessLevel), intValue)
                       ? (ArticleAccessLevel)intValue
                       : ArticleAccessLevel.Anonymous;
        }
    }
}