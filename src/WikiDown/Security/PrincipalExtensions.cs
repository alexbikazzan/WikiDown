using System.Security.Principal;

namespace WikiDown.Security
{
    public static class PrincipalExtensions
    {
        public static ArticleAccessLevel GetAccessLevel(this IPrincipal principal)
        {
            if (principal == null)
            {
                return ArticleAccessLevel.Anonymous;
            }

            if (principal.IsInRole(ArticleAccessHelper.Root))
            {
                return ArticleAccessLevel.Root;
            }
            if (principal.IsInRole(ArticleAccessHelper.Admin))
            {
                return ArticleAccessLevel.Admin;
            }
            if (principal.IsInRole(ArticleAccessHelper.SuperUser))
            {
                return ArticleAccessLevel.SuperUser;
            }
            if (principal.IsInRole(ArticleAccessHelper.Editor))
            {
                return ArticleAccessLevel.Editor;
            }
            if (principal.IsInRole(ArticleAccessHelper.LoggedIn))
            {
                return ArticleAccessLevel.LoggedIn;
            }

            return ArticleAccessLevel.Anonymous;
        }
    }
}