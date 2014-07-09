using System;

using WikiDown.Security;

namespace WikiDown.Website.ApiModels
{
    public class ArticlesAdminApiModel
    {
        public ArticlesAdminApiModel()
        {
        }

        public ArticlesAdminApiModel(ArticleAccess articleAccess)
        {
            if (articleAccess == null)
            {
                return;
            }

            this.CanRead = articleAccess.CanRead.HasValue ? (int)articleAccess.CanRead : -1;
            this.CanEdit = articleAccess.CanEdit.HasValue ? (int)articleAccess.CanEdit : -1;
            this.CanAdmin = articleAccess.CanAdmin.HasValue ? (int)articleAccess.CanAdmin : -1;
        }

        public int CanAdmin { get; set; }

        public int CanEdit { get; set; }

        public int CanRead { get; set; }

        public void Save(ArticleId articleId, Repository repository)
        {
            var canAdmin = TryGetArticleAccess(this.CanAdmin);
            var canEdit = TryGetArticleAccess(this.CanEdit);
            var canRead = TryGetArticleAccess(this.CanRead);

            var articleAccess = repository.GetArticleAccess(articleId) ?? ArticleAccess.Default(articleId);
            articleAccess.CanAdmin = canAdmin;
            articleAccess.CanEdit = canEdit;
            articleAccess.CanRead = canRead;

            repository.SaveArticleAccess(articleAccess);
        }

        private static ArticleAccessLevel TryGetArticleAccess(int intValue)
        {
            return Enum.IsDefined(typeof(ArticleAccessLevel), intValue)
                       ? (ArticleAccessLevel)intValue
                       : ArticleAccessLevel.Anonymous;
        }
    }
}