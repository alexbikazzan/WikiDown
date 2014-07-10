using System;
using System.Linq;
using System.Security.Principal;

namespace WikiDown.Security
{
    public class ArticleAccessManager
    {
        public const string RootAccountName = "_root";

        private readonly Repository repository;

        public ArticleAccessManager(Repository repository)
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            this.repository = repository;
        }

        public bool GetCanRead(ArticleAccess articleAccess, IPrincipal principal)
        {
            return (articleAccess == null) || GetIsInRole(articleAccess.CanRead, principal);
        }

        public bool GetCanEdit(ArticleAccess articleAccess, IPrincipal principal)
        {
            return (articleAccess == null) || GetIsInRole(articleAccess.CanEdit, principal);
        }

        public bool GetCanAdmin(ArticleAccess articleAccess, IPrincipal principal)
        {
            return (articleAccess == null) || GetIsInRole(articleAccess.CanAdmin, principal);
        }

        public bool GetCanRead(ArticleId articleId, IPrincipal principal)
        {
            if (articleId == null)
            {
                throw new ArgumentNullException("articleId");
            }

            var articleAccess = repository.GetArticleAccess(articleId);
            return this.GetCanRead(articleAccess, principal);
        }

        public bool GetCanEdit(ArticleId articleId, IPrincipal principal)
        {
            if (articleId == null)
            {
                throw new ArgumentNullException("articleId");
            }

            var articleAccess = repository.GetArticleAccess(articleId);
            return this.GetCanEdit(articleAccess, principal);
        }

        public bool GetCanAdmin(ArticleId articleId, IPrincipal principal)
        {
            if (articleId == null)
            {
                throw new ArgumentNullException("articleId");
            }

            var articleAccess = repository.GetArticleAccess(articleId);
            return this.GetCanAdmin(articleAccess, principal);
        }

        internal static bool GetIsInRole(ArticleAccessRole? acccesLevel, IPrincipal principal)
        {
            var roles = ArticleAccessHelper.GetRoles(acccesLevel);

            var rolesList = (roles != null) ? roles.ToList() : null;
            if (rolesList == null || !rolesList.Any())
            {
                return true;
            }

            return rolesList.Any(principal.IsInRole);
        }
    }
}