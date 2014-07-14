using System;
using System.Security.Principal;

namespace WikiDown.Security
{
    public static class ArticleExtensions
    {
        public static void EnsureCanAccess(this Article article, IPrincipal principal, ArticleAccessType accessType)
        {
            if (article == null)
            {
                throw new ArgumentNullException("article");
            }

            bool canAccess = article.CanAccess(principal, accessType);
            if (!canAccess)
            {
                throw new ArticleAccessException(article.Id);
            }
        }

        public static bool CanAccess(this Article article, IPrincipal principal, ArticleAccessType accessType)
        {
            if (article == null)
            {
                throw new ArgumentNullException("article");
            }

            switch (accessType)
            {
                case ArticleAccessType.CanRead:
                    return article.CanRead(principal);
                case ArticleAccessType.CanEdit:
                    return article.CanEdit(principal);
                case ArticleAccessType.CanAdmin:
                    return article.CanAdmin(principal);
                default:
                    throw new ArgumentOutOfRangeException("accessType");
            }
        }

        public static bool CanRead(this Article article, IPrincipal principal)
        {
            if (article == null)
            {
                throw new ArgumentNullException("article");
            }

            var accessLevel = principal.GetAccessLevel();
            return accessLevel >= article.ArticleAccess.CanRead;
        }

        public static bool CanEdit(this Article article, IPrincipal principal)
        {
            if (article == null)
            {
                throw new ArgumentNullException("article");
            }

            var accessLevel = principal.GetAccessLevel();
            return accessLevel >= article.ArticleAccess.CanEdit;
        }

        public static bool CanAdmin(this Article article, IPrincipal principal)
        {
            if (article == null)
            {
                throw new ArgumentNullException("article");
            }

            var accessLevel = principal.GetAccessLevel();
            return accessLevel >= article.ArticleAccess.CanAdmin;
        }
    }
}