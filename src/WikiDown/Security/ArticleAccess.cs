using System.Security.Principal;

namespace WikiDown.Security
{
    public class ArticleAccess
    {
        public string ArticleId { get; set; }

        public ArticleAccessRole CanRead { get; set; }

        public ArticleAccessRole CanEdit { get; set; }

        public ArticleAccessRole CanAdmin { get; set; }

        public string Id { get; set; }

        //public bool GetCanRead(IPrincipal principal)
        //{
        //    return ArticleAccessManager.GetIsInRole(this.CanRead, principal);
        //}

        //public bool GetCanEdit(IPrincipal principal)
        //{
        //    return ArticleAccessManager.GetIsInRole(this.CanEdit, principal);
        //}

        //public bool GetCanAdmin(IPrincipal principal)
        //{
        //    return ArticleAccessManager.GetIsInRole(this.CanAdmin, principal);
        //}

        public static ArticleAccess Default(ArticleId articleId)
        {
            return new ArticleAccess
                       {
                           ArticleId = articleId.Id,
                           CanAdmin = ArticleAccessRole.Admin,
                           CanEdit = ArticleAccessRole.Editor,
                           CanRead = ArticleAccessRole.Anonymous
                       };
        }
    }
}