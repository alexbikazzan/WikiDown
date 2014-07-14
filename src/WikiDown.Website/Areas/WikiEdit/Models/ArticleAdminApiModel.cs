using System;

using WikiDown.Security;

namespace WikiDown.Website.Areas.WikiEdit.Models
{
    public class ArticleAdminApiModel
    {
        public ArticleAdminApiModel()
        {
        }

        public ArticleAdminApiModel(Article article)
        {
            if (article == null)
            {
                return;
            }

            this.CanRead = (int)article.ArticleAccess.CanRead;
            this.CanEdit = (int)article.ArticleAccess.CanEdit;
            this.CanAdmin = (int)article.ArticleAccess.CanAdmin;
        }

        public int CanAdmin { get; set; }

        public int CanEdit { get; set; }

        public int CanRead { get; set; }

        public void Save(ArticleId articleId, Repository repository)
        {
            var article = repository.GetArticle(articleId);

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