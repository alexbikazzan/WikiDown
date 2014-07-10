using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace WikiDown.Security
{
    public static class ArticleAccessHelper
    {
        public const string Anonymous = "Anonymous";

        public const string LoggedIn = "LoggedIn";

        public const string Editor = "Editor";

        public const string SuperUser = "SuperUser";

        public const string Admin = "Admin";

        public const string Root = "Root";

        public static readonly IReadOnlyCollection<string> LoggedInRoles =
            new[] { ArticleAccessRole.LoggedIn.ToString() }.ToList();

        public static readonly IReadOnlyCollection<string> EditorRoles =
            LoggedInRoles.Concat(new[] { ArticleAccessRole.Editor.ToString() }).ToList();

        public static readonly IReadOnlyCollection<string> SuperUserRoles =
            EditorRoles.Concat(new[] { ArticleAccessRole.SuperUser.ToString() }).ToList();

        public static readonly IReadOnlyCollection<string> AdminRoles =
            SuperUserRoles.Concat(new[] { ArticleAccessRole.Admin.ToString() }).ToList();

        public static readonly IReadOnlyCollection<string> RootRoles =
            AdminRoles.Concat(new[] { ArticleAccessRole.Root.ToString() }).ToList();

        public static ArticleAccessRole GetRole(IPrincipal principal)
        {
            if (principal == null)
            {
                return ArticleAccessRole.Anonymous;
            }

            if (principal.IsInRole(Root))
            {
                return ArticleAccessRole.Root;
            }
            if (principal.IsInRole(Admin))
            {
                return ArticleAccessRole.Admin;
            }
            if (principal.IsInRole(SuperUser))
            {
                return ArticleAccessRole.SuperUser;
            }
            if (principal.IsInRole(Editor))
            {
                return ArticleAccessRole.Editor;
            }
            if (principal.IsInRole(LoggedIn))
            {
                return ArticleAccessRole.LoggedIn;
            }

            return ArticleAccessRole.Anonymous;
        }

        public static IReadOnlyCollection<string> GetRoles(string accessLevel)
        {
            ArticleAccessRole articleAccessRole;
            if (!Enum.TryParse(accessLevel ?? string.Empty, out articleAccessRole))
            {
                articleAccessRole = ArticleAccessRole.Anonymous;
            }

            return GetRoles(articleAccessRole);
        }

        public static IReadOnlyCollection<string> GetRoles(int accessLevel)
        {
            var articleAccessRole = Enum.IsDefined(typeof(ArticleAccessRole), accessLevel)
                                         ? (ArticleAccessRole)accessLevel
                                         : ArticleAccessRole.Anonymous;

            return GetRoles(articleAccessRole);
        }

        public static IReadOnlyCollection<string> GetRoles(ArticleAccessRole? accessLevel)
        {
            switch (accessLevel)
            {
                case null:
                case ArticleAccessRole.Anonymous:
                    return Enumerable.Empty<string>().ToList();
                case ArticleAccessRole.LoggedIn:
                    return LoggedInRoles;
                case ArticleAccessRole.Editor:
                    return EditorRoles;
                case ArticleAccessRole.SuperUser:
                    return SuperUserRoles;
                case ArticleAccessRole.Admin:
                    return AdminRoles;
                case ArticleAccessRole.Root:
                    return RootRoles;
                default:
                    throw new ArgumentOutOfRangeException("accessLevel");
            }
        }
    }
}