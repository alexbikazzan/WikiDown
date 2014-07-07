using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace WikiDown.Security
{
    public class ArticleAccess
    {
        public string ArticleId { get; set; }

        public IEnumerable<string> CanRead { get; set; }

        public IEnumerable<string> CanEdit { get; set; }

        public IEnumerable<string> CanAdmin { get; set; }

        public string Id { get; set; }

        public bool GetCanRead(IPrincipal principal)
        {
            return GetIsPrincipalInRole(this.CanRead, principal);
        }

        public bool GetCanEdit(IPrincipal principal)
        {
            return GetIsPrincipalInRole(this.CanEdit, principal);
        }

        public bool GetCanAdmin(IPrincipal principal)
        {
            return GetIsPrincipalInRole(this.CanAdmin, principal);
        }

        public static ArticleAccess Empty(string articleId)
        {
            return new ArticleAccess
                       {
                           ArticleId = articleId,
                           CanRead = Enumerable.Empty<string>(),
                           CanEdit = Enumerable.Empty<string>(),
                           CanAdmin = Enumerable.Empty<string>()
                       };
        }

        private static bool GetIsPrincipalInRole(IEnumerable<string> roles, IPrincipal principal)
        {
            var rolesList = (roles != null) ? roles.ToList() : null;
            if (rolesList == null || !rolesList.Any())
            {
                return true;
            }

            return rolesList.Any(principal.IsInRole);
        }
    }
}