using System;
using System.Collections.Generic;
using System.Linq;

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
            new[] { ArticleAccessLevel.LoggedIn.ToString() }.ToList();

        public static readonly IReadOnlyCollection<string> EditorRoles =
            LoggedInRoles.Concat(new[] { ArticleAccessLevel.Editor.ToString() }).ToList();

        public static readonly IReadOnlyCollection<string> SuperUserRoles =
            EditorRoles.Concat(new[] { ArticleAccessLevel.SuperUser.ToString() }).ToList();

        public static readonly IReadOnlyCollection<string> AdminRoles =
            SuperUserRoles.Concat(new[] { ArticleAccessLevel.Admin.ToString() }).ToList();

        public static readonly IReadOnlyCollection<string> RootRoles =
            AdminRoles.Concat(new[] { ArticleAccessLevel.Root.ToString() }).ToList();

        public static IReadOnlyCollection<string> GetRoles(string accessLevel)
        {
            ArticleAccessLevel articleAccessLevel;
            if (!Enum.TryParse(accessLevel ?? string.Empty, out articleAccessLevel))
            {
                articleAccessLevel = ArticleAccessLevel.Anonymous;
            }

            return GetRoles(articleAccessLevel);
        }


        public static IReadOnlyCollection<string> GetRoles(int accessLevel)
        {
            var articleAccessLevel = Enum.IsDefined(typeof(ArticleAccessLevel), accessLevel)
                                         ? (ArticleAccessLevel)accessLevel
                                         : ArticleAccessLevel.Anonymous;

            return GetRoles(articleAccessLevel);
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