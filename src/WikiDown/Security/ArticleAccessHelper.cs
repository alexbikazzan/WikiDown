using System;
using System.Collections.Generic;
using System.Linq;

namespace WikiDown.Security
{
    public static class ArticleAccessHelper
    {
        public const string RootAccountName = "_root";

        public const string Anonymous = "Anonymous";

        public const string LoggedIn = "LoggedIn";

        public const string Editor = "Editor";

        public const string SuperUser = "SuperUser";

        public const string Admin = "Admin";

        public const string Root = "Root";

        public static readonly IReadOnlyCollection<string> LoggedInRoles =
            new[] { ArticleAccessLevel.LoggedIn.ToString() }.ToList();

        public static readonly IReadOnlyCollection<string> EditorRoles =
            LoggedInRoles.Concat(new[] { ArticleAccessLevel.Editor.ToString() }).ToList();

        public static readonly IReadOnlyCollection<string> SuperUserRoles =
            EditorRoles.Concat(new[] { ArticleAccessLevel.SuperUser.ToString() }).ToList();

        public static readonly IReadOnlyCollection<string> AdminRoles =
            SuperUserRoles.Concat(new[] { ArticleAccessLevel.Admin.ToString() }).ToList();

        public static readonly IReadOnlyCollection<string> RootRoles =
            AdminRoles.Concat(new[] { ArticleAccessLevel.Root.ToString() }).ToList();


        public static ArticleAccessLevel GetAccessLevel(int accessLevel)
        {
            return Enum.IsDefined(typeof(ArticleAccessLevel), accessLevel)
                       ? (ArticleAccessLevel)accessLevel
                       : ArticleAccessLevel.Anonymous;
        }

        public static ArticleAccessLevel GetAccessLevel(string accessLevelName)
        {
            ArticleAccessLevel result;
            return Enum.TryParse(accessLevelName ?? string.Empty, out result) ? result : ArticleAccessLevel.Anonymous;
        }

        public static ArticleAccessLevel GetAccessLevel(IEnumerable<string> roles)
        {
            var roleList = (roles ?? Enumerable.Empty<string>()).ToList();

            if (roleList.Contains(Root))
            {
                return ArticleAccessLevel.Root;
            }
            if (roleList.Contains(Admin))
            {
                return ArticleAccessLevel.Admin;
            }
            if (roleList.Contains(SuperUser))
            {
                return ArticleAccessLevel.SuperUser;
            }
            if (roleList.Contains(Editor))
            {
                return ArticleAccessLevel.Editor;
            }

            return ArticleAccessLevel.Anonymous;
        }

        public static IReadOnlyCollection<string> GetRoles(int accessLevel)
        {
            var articleAccessRole = Enum.IsDefined(typeof(ArticleAccessLevel), accessLevel)
                                        ? (ArticleAccessLevel)accessLevel
                                        : ArticleAccessLevel.Anonymous;

            return GetRoles(articleAccessRole);
        }

        public static IReadOnlyCollection<string> GetRoles(ArticleAccessLevel? accessLevel)
        {
            switch (accessLevel)
            {
                case null:
                case ArticleAccessLevel.Anonymous:
                    return Enumerable.Empty<string>().ToList();
                case ArticleAccessLevel.LoggedIn:
                    return LoggedInRoles;
                case ArticleAccessLevel.Editor:
                    return EditorRoles;
                case ArticleAccessLevel.SuperUser:
                    return SuperUserRoles;
                case ArticleAccessLevel.Admin:
                    return AdminRoles;
                case ArticleAccessLevel.Root:
                    return RootRoles;
                default:
                    throw new ArgumentOutOfRangeException("accessLevel");
            }
        }
    }
}