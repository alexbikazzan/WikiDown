using System.Threading.Tasks;

using Raven.Client;
using WikiDown.Security;

namespace WikiDown.Website.Security
{
    public static class RootUserUtility
    {
        private const string UserName = ArticleAccessManager.RootAccountName;

        public static async Task EnsureRootAccount(IDocumentStore documentStore)
        {
            var userManager = UserManagerHelper.GetLazy(documentStore).Value;
            var rootUser = await userManager.FindByNameAsync(ArticleAccessManager.RootAccountName);

            if (rootUser != null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(AppSetting.RootPassword))
            {
                return;
            }

            var user = new WikiDownUser(UserName, "root@root.com") { Roles = ArticleAccessLevel.RootRoles };

            await userManager.CreateAsync(user, AppSetting.RootPassword);
        }
    }
}