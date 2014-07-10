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

            this.CanRead = (int)articleAccess.CanRead;
            this.CanEdit = (int)articleAccess.CanEdit;
            this.CanAdmin = (int)articleAccess.CanAdmin;
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

        private static ArticleAccessRole TryGetArticleAccess(int intValue)
        {
            return Enum.IsDefined(typeof(ArticleAccessRole), intValue)
                       ? (ArticleAccessRole)intValue
                       : ArticleAccessRole.Anonymous;
        }
    }
}