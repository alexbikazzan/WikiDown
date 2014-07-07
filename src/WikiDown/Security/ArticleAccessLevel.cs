using System.Collections.Generic;
using System.Linq;

namespace WikiDown.Security
{
    public static class ArticleAccessLevel
    {
        public const string Editor = "Editor";

        public const string SuperUser = "SuperUser";

        public const string Admin = "Admin";

        public const string Root = "Root";

        public static readonly IReadOnlyCollection<string> EditorRoles = new[] { Editor }.ToList();

        public static readonly IReadOnlyCollection<string> SuperUserRoles = new[] { Editor, SuperUser }.ToList();

        public static readonly IReadOnlyCollection<string> AdminRoles = new[] { Editor, SuperUser, Admin }.ToList();

        public static readonly IReadOnlyCollection<string> RootRoles = new[] { Editor, SuperUser, Admin, Root }.ToList();
    }
}