using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;
using WikiDown.Security;
using WikiDown.Website.Security;

namespace WikiDown.Website.ApiModels
{
    public class AccountUserApiModel
    {
        public AccountUserApiModel()
        {
        }

        public AccountUserApiModel(WikiDownUser user)
        {
            this.Email = user.Email;
            this.UserName = user.UserName;

            this.SetRole(user);
        }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }

        public string UserName { get; set; }

        public async Task Save(UserManager<WikiDownUser> userManager)
        {
            var roles = this.GetRoles();

            var user = await userManager.FindByNameAsync(this.UserName);
            if (user != null)
            {
                user.Roles = roles;
                user.SetEmail(this.Email);

                if (!string.IsNullOrWhiteSpace(this.Password))
                {
                    userManager.ChangePasswordAsync(user.Id, user.PasswordHash)
                    user.SetPasswordHash(this.Password);
                }

                await userManager.UpdateAsync(user);
            }
            else
            {
                user = new WikiDownUser(this.UserName) { Roles = roles };
                user.SetEmail(this.Email);

                await userManager.CreateAsync(user, this.Password);
            }
        }

        public IEnumerable<string> GetRoles()
        {
            if (this.UserName == ArticleAccessManager.RootAccountName)
            {
                return ArticleAccessLevel.RootRoles;
            }

            string role = this.Role ?? string.Empty;
            switch (role)
            {
                case ArticleAccessLevel.Root:
                    return ArticleAccessLevel.RootRoles;
                case ArticleAccessLevel.Admin:
                    return ArticleAccessLevel.AdminRoles;
                case ArticleAccessLevel.SuperUser:
                    return ArticleAccessLevel.SuperUserRoles;
                case ArticleAccessLevel.Editor:
                    return ArticleAccessLevel.EditorRoles;
            }

            return Enumerable.Empty<string>();
        }

        private void SetRole(WikiDownUser user)
        {
            if (user.Roles.Contains(ArticleAccessLevel.Root))
            {
                this.Role = ArticleAccessLevel.Root;
            }
            else if (user.Roles.Contains(ArticleAccessLevel.Admin))
            {
                this.Role = ArticleAccessLevel.Admin;
            }
            else if (user.Roles.Contains(ArticleAccessLevel.SuperUser))
            {
                this.Role = ArticleAccessLevel.SuperUser;
            }
            else if (user.Roles.Contains(ArticleAccessLevel.Editor))
            {
                this.Role = ArticleAccessLevel.Editor;
            }
        }
    }
}