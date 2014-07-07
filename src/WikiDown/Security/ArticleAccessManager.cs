using System;
using System.Collections.Generic;
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

        public bool GetCanRead(ArticleId articleId, IPrincipal principal)
        {
            if (articleId == null)
            {
                throw new ArgumentNullException("articleId");
            }

            var articleAccess = repository.GetArticleAccess(articleId);
            return (articleAccess == null) || GetIsInRole(articleAccess.CanRead, principal);
        }

        public bool GetCanEdit(ArticleId articleId, IPrincipal principal)
        {
            if (articleId == null)
            {
                throw new ArgumentNullException("articleId");
            }

            var articleAccess = repository.GetArticleAccess(articleId);
            return (articleAccess == null) || GetIsInRole(articleAccess.CanEdit, principal);
        }

        public bool GetCanAdmin(ArticleId articleId, IPrincipal principal)
        {
            if (articleId == null)
            {
                throw new ArgumentNullException("articleId");
            }

            var articleAccess = repository.GetArticleAccess(articleId);
            return (articleAccess == null) || GetIsInRole(articleAccess.CanAdmin, principal);
        }

        private static bool GetIsInRole(IEnumerable<string> roles, IPrincipal principal)
        {
            var roleList = (roles != null) ? roles.ToList() : null;
            if (roleList == null || !roleList.Any())
            {
                return true;
            }

            return (principal != null) && roleList.Any(principal.IsInRole);
        }
    }
}